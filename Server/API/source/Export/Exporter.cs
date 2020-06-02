namespace Carter.App.Export.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using System.Text;
    using System.Threading.Tasks;

    using Carter.App.Export.JsonHelper;
    using Carter.App.Hosting;
    using Carter.App.Route.Sessions;

    using CsvHelper;
    using CsvHelper.Configuration;

    using HandlebarsDotNet;

    using Minio;
    using Minio.Exceptions;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;

    public class ExportHandler
    {
        private static readonly string Seperator = Path.DirectorySeparatorChar.ToString();

        private static IMongoDatabase db;

        // TODO: Replace MinioClient with IMinioClient
        private static MinioClient os;

        private static string sessionId;
        private static string prefix;

        private static int currentSceneIndex;
        private static string currentSceneName;
        private static List<string> currentHeader;

        public static void Entry(object id)
        {
            _ = MainAsync((ExporterConfiguration)id);
        }

        protected static async Task MainAsync(ExporterConfiguration config)
        {
            Setup(config);

            try
            {
                var jobId = Guid.NewGuid();
                prefix = $"./{Seperator}exporter_temporary_files{Seperator}{jobId}{Seperator}";
                string outFolder = $"{prefix}exported{Seperator}";
                string zipFolder = $"{prefix}zipped{Seperator}";
                string tempFolder = $"{prefix}temporary{Seperator}CSVs{Seperator}";

                Directory.CreateDirectory(outFolder);
                Directory.CreateDirectory($"{outFolder}CSVs{Seperator}");

                Directory.CreateDirectory(zipFolder);
                Directory.CreateDirectory(tempFolder);

                ConfigureJsonWriter();
                await ExportSession();

                string outLocation = zipFolder + $"{sessionId}_session_exported.zip";
                ZipFolder(outFolder, outLocation);

                await Upload(outLocation);
                await UpdateDoc(ExportOptions.Done);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                await UpdateDoc(ExportOptions.Error);
            }

            Directory.Delete(prefix, true);
        }

        protected static void Setup(ExporterConfiguration config)
        {
            sessionId = config.Id;

            string mongoUrl = $"mongodb://{config.Mongo.ConnectionString}:{config.Mongo.Port}";
            db = new MongoClient(mongoUrl).GetDatabase("ec");

            string minioHost = $"{AppConfiguration.Minio.ConnectionString}:{AppConfiguration.Minio.Port}";
            os = new MinioClient(minioHost, "minio", "minio123");
        }

        protected static async Task ExportSession()
        {
            currentHeader = new List<string>();
            currentSceneIndex = -1; // To play nice with the proccessing code

            var workloads = await GetWorkloads();

            var ws = new JsonWriterSettings()
            {
                Indent = false,
                OutputMode = JsonOutputMode.Strict,
            };

            ToJsonStart("raw");
            ToJsonStart("sessionInfo");
            ToJsonStart("onlyCaptures");

            for (int i = 0; i < workloads.Count; i++)
            {
                // Step should be any of the workloads, except the last
                // First used because if it isn't a step then offset is zero anyway
                int offset = (int)(workloads[0] * i);
                List<BsonDocument> sessionSorted = await SortSession((int)workloads[i], offset);

                bool isFirst = i == 0;
                ToJson(sessionSorted, "raw", isFirst, ws);

                var (otherCaptures, sceneBlocks) = ProcessScenes(sessionSorted);
                if (otherCaptures.Count > 0)
                {
                    ToJson(otherCaptures, "sessionInfo", isFirst);
                }

                foreach (var block in sceneBlocks)
                {
                    ToJson(block.Docs, "onlyCaptures", isFirst);
                    isFirst = false;

                    // First scene shouldn't reqult in a previous scene being copied
                    if (currentSceneIndex == -1)
                    {
                        currentSceneIndex = block.Index;
                        currentSceneName = block.Name;

                        ToCsv(block, "sceneName");
                    }

                    // If at a new scene > 0, then copy the previous
                    else if (block.Index != currentSceneIndex)
                    {
                        CopyCsv(
                            new SceneBlock()
                            {
                                Name = currentSceneName,
                                Index = currentSceneIndex,
                            }, "sceneName");

                        currentSceneIndex = block.Index;
                        currentSceneName = block.Name;

                        ToCsv(block, "sceneName");
                    }
                    else
                    {
                        ToCsv(block, "sceneName");
                    }
                }

                // Copy the last block
                if (i == workloads.Count - 1)
                {
                    CopyCsv(sceneBlocks[sceneBlocks.Count - 1], "sceneName");
                }
            }

            ToJsonEnd("raw");
            ToJsonEnd("sessionInfo");
            ToJsonEnd("onlyCaptures");

            ToJsonStart("database");
            ToJson(await GetSessionInfo(), "database");
            ToJsonEnd("database");

            CreateReadme();
        }

        /*
         * Breaks the session collection into blocks, of a constant size or less
         * Returns: Task<List<long>> of a broken down count of all the documents in a session
         */
        protected static async Task<List<long>> GetWorkloads()
        {
            var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{sessionId}");
            var filter = Builders<BsonDocument>.Filter.Empty;
            long docCount = await sessionCollection.CountDocumentsAsync(filter);

            long groupCount = 1800; // 20 seconds worth of frames from a game running @90fps
            int workloadCount = (int)(docCount / groupCount);
            var workloads = new List<long>();

            for (int i = 0; i < workloadCount; i++)
            {
                workloads.Add(groupCount);
            }

            // Add leftover, if it exists
            // Leftover is always less than groupCount to prevent high memory usage
            if (docCount % groupCount != 0)
            {
                workloads.Add(docCount - (groupCount * workloadCount));
            }

            return workloads;
        }

        protected static async Task<List<BsonDocument>> SortSession(int workload, int offset)
        {
            var sessionCollection = db.GetCollection<BsonDocument>($"sessions.{sessionId}");

            var filter = Builders<BsonDocument>.Filter.Empty;
            var sorter = Builders<BsonDocument>.Sort
                .Ascending(d => d["frameInfo"]["realtimeSinceStartup"]);
            var projection = Builders<BsonDocument>.Projection
                .Exclude("_id");

            return await sessionCollection
                .Find(filter)
                .Skip(offset)
                .Limit(workload)
                .Sort(sorter)
                .Project(projection)
                .ToListAsync();
        }

        protected static async Task<SessionSchema> GetSessionInfo()
        {
            var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

            var filter = Builders<SessionSchema>.Filter
                .Where(s => s.Id == sessionId);

            return await sessions
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        protected static void ToJson(List<BsonDocument> sessionDocs, string about, bool isFirst, JsonWriterSettings ws = null)
        {
            StringBuilder docsTotal = new StringBuilder();

            foreach (BsonDocument d in sessionDocs)
            {
                docsTotal.AppendFormat("{0}{1}", ",", d.ToJson(ws ?? JsonWriterSettings.Defaults));
            }

            // Remove leading comma
            if (isFirst)
            {
                docsTotal.Remove(0, 1);
            }

            string filename = $"{sessionId}.{about}.json";
            AppendToFile(docsTotal.ToString(), filename);
        }

        protected static void ToJsonStart(string about)
        {
            AppendToFile("[", $"{sessionId}.{about}.json");
        }

        protected static void ToJsonEnd(string about)
        {
            AppendToFile("]", $"{sessionId}.{about}.json");
        }

        protected static void ToJson(BsonDocument sessionDoc, string about)
        {
            string filename = $"{sessionId}.{about}.json";
            AppendToFile(sessionDoc.ToJson(JsonWriterSettings.Defaults), filename);
        }

        protected static void ToJson(SessionSchema sessionDoc, string about)
        {
            string filename = $"{sessionId}.{about}.json";
            AppendToFile(sessionDoc.ToJson(JsonWriterSettings.Defaults), filename);
        }

        protected static string ToFlatJson(List<BsonDocument> sessionDocs)
        {
            StringBuilder docsTotal = new StringBuilder();
            docsTotal.Append("[");

            if (sessionDocs.Count > 0)
            {
                foreach (BsonDocument d in sessionDocs)
                {
                    string json = d.ToJson(JsonWriterSettings.Defaults);
                    var dict = JsonHelper.DeserializeAndFlatten(json);
                    string flat = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

                    docsTotal.AppendFormat("{0}{1}", flat, ",");
                }

                docsTotal.Length--; // Remove trailing comma
            }

            docsTotal.Append("]");

            return docsTotal.ToString();
        }

        protected static void ConfigureJsonWriter()
        {
            JsonWriterSettings.Defaults = new JsonWriterSettings()
            {
                Indent = true,
                OutputMode = JsonOutputMode.Strict,
            };
        }

        protected static Configuration GetConfiguration()
        {
            return new Configuration
            {
                ShouldSkipRecord = (record) => record.All(string.IsNullOrEmpty),
                MissingFieldFound = (record, index, context) => record.All(string.IsNullOrEmpty),
                Delimiter = ",",
            };
        }

        protected static (List<BsonDocument> otherCaptures, List<SceneBlock> scenes) ProcessScenes(List<BsonDocument> sessionDocs)
        {
            var sceneDocs = sessionDocs.FindAll(d => d.Contains("sceneName"));
            int sceneIndex = currentSceneIndex;

            List<SceneBlock> sceneMap = sceneDocs.Select((scene) =>
            {
                sceneIndex++;
                return new SceneBlock()
                {
                    StartTime = scene["frameInfo"]["realtimeSinceStartup"].AsDouble,
                    Name = scene["sceneName"].AsString,
                    Index = sceneIndex,
                    Docs = new List<BsonDocument>(),
                };
            }).ToList();

            var otherCaptures = sessionDocs.FindAll(d => !d.Contains("gameObjects"));
            var normalCaptures = sessionDocs.FindAll(d => d.Contains("gameObjects"));

            // If there are no scene captures, return a default block
            if (sceneMap.Count == 0)
            {
                return (
                    otherCaptures,
                    new List<SceneBlock>()
                    {
                        new SceneBlock()
                        {
                            Name = currentSceneName,
                            Index = sceneIndex,
                            Docs = normalCaptures,
                        },
                    });
            }

            // Otherwise, process each block by scene
            int index = 0;
            var currentScene = sceneMap[index];

            for (int i = 0; i < normalCaptures.Count; i++)
            {
                var currentDoc = normalCaptures[i];
                var currentTimestamp = currentDoc["frameInfo"]["realtimeSinceStartup"].AsDouble;
                int tempIndex = sceneIndex + 1;

                if (tempIndex < sceneMap.Count
                    && currentTimestamp > sceneMap[tempIndex].StartTime)
                {
                    index++;
                    currentScene = sceneMap[index];
                }

                currentScene.Docs.Add(currentDoc);
            }

            return (otherCaptures, sceneMap);
        }

        protected static void ToCsv(SceneBlock block, string about)
        {
            string json = ToFlatJson(block.Docs);
            string csv = JsonToCsv(json);

            string path = $"temporary{Seperator}CSVs{Seperator}";
            AppendToFile(csv, $"{sessionId}.{about}.{block.Name}.{block.Index}.csv", path);
        }

        protected static string JsonToCsv(string jsonContent)
        {
            StringWriter csvString = new StringWriter();

            using (var csv = new CsvWriter(csvString, GetConfiguration()))
            {
                var dt = JsonHelper.JsonToTable(jsonContent);
                dt = AlignHeaders(dt);

                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }

                    csv.NextRecord();
                }
            }

            return csvString.ToString();
        }

        protected static void CopyCsv(SceneBlock block, string about)
        {
            StringWriter csvString = new StringWriter();

            using (var csv = new CsvWriter(csvString, GetConfiguration()))
            {
                foreach (string key in currentHeader)
                {
                    csv.WriteField(key);
                }

                csv.NextRecord();
            }

            string path = $"exported{Seperator}CSVs{Seperator}";
            string filename = $"{sessionId}.{about}.{block.Name}.{block.Index}.csv";
            AppendToFile(csvString.ToString(), filename, path);

            string temporaryLocation = $"{prefix}temporary{Seperator}CSVs{Seperator}{filename}";
            File.AppendAllText($"{prefix}{path}{filename}", File.ReadAllText(temporaryLocation));

            // Reset the headers, because a new scene will likely have differerent headers
            currentHeader = new List<string>();
        }

        protected static DataTable AlignHeaders(DataTable dt)
        {
            var header = dt.Columns.Cast<DataColumn>()
                        .Select(col => col.ColumnName)
                        .ToList();

            if (!header.SequenceEqual(currentHeader))
            {
                // Store all new headers in the global header var
                foreach (string h in header)
                {
                    if (!currentHeader.Contains(h))
                    {
                        currentHeader.Add(h);
                    }
                }

                // Add all headers the current block lacks into it
                foreach (string h in currentHeader)
                {
                    if (!header.Contains(h))
                    {
                        dt.Columns.Add(h);
                    }
                }
            }

            // Order headers for the current block
            for (int i = 0; i < currentHeader.Count; i++)
            {
                dt.Columns[currentHeader[i]].SetOrdinal(i);
            }

            return dt;
        }

        protected static void CreateReadme()
        {
            string source = System.IO.File.ReadAllText($".{Seperator}Templates{Seperator}README.txt.handlebars");
            var template = Handlebars.Compile(source);

            var data = new
            {
                id = sessionId,
            };

            AppendToFile(template(data), "README.txt");
        }

        protected static void AppendToFile(string content, string filename, string path = "")
        {
            string fullpath;
            if (path == string.Empty)
            {
                fullpath = $"{prefix}exported{Seperator}{filename}";
            }
            else
            {
                fullpath = $"{prefix}{path}{filename}";
            }

            using (var sw = File.AppendText(fullpath))
            {
                sw.Write(content);
            }
        }

        protected static void ZipFolder(string location, string outName)
        {
            ZipFile.CreateFromDirectory(location, outName);
        }

        // From: https://docs.min.io/docs/dotnet-client-quickstart-guide.html
        protected static async Task Upload(string fileLocation)
        {
            var bucketName = "sessions.exported";

            // Regions shouldn't really matter because the zip is uploaded to local minio anyway
            var location = "us-west-1";
            var objectName = $"{sessionId}_session_exported.zip";
            var filePath = fileLocation;
            var contentType = "application/zip";

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await os.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await os.MakeBucketAsync(bucketName, location);
                }

                // Upload a file to bucket.
                await os.PutObjectAsync(bucketName, objectName, filePath, contentType);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }

        protected static async Task UpdateDoc(ExportOptions status)
        {
            var sessions = db.GetCollection<SessionSchema>(SessionSchema.CollectionName);

            var filter = Builders<SessionSchema>.Filter
                .Where(s => s.Id == sessionId);

            var update = Builders<SessionSchema>.Update
                .Set(s => s.ExportState, status);

            await sessions.UpdateOneAsync(filter, update);
        }
    }

    // Used to group together captures based on timestamps
    public class SceneBlock
    {
        #pragma warning disable SA1516
        public string Name { get; set; }
        public double StartTime { get; set; }
        public int Index { get; set; }
        public List<BsonDocument> Docs { get; set; }
        #pragma warning restore SA151
    }
}