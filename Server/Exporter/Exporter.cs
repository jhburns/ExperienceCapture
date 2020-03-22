namespace Export.App.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using System.Text;
    using System.Threading.Tasks;

    using CsvHelper;
    using CsvHelper.Configuration;

    using Exporter.App.CustomExceptions;
    using Exporter.App.JsonHelper;

    using HandlebarsDotNet;

    using Minio;
    using Minio.Exceptions;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;

    public class ExportHandler
    {
        private static readonly IMongoDatabase DB = new MongoClient(@"mongodb://db:27017").GetDatabase("ec");
        private static readonly MinioClient OS = new MinioClient("os:9000", "minio", "minio123");

        private static readonly string SessionId = Environment.GetEnvironmentVariable("exporter_session_id")
            ?? throw new EnviromentVarNotSet("The following is unset", "exporter_session_id");

        private static readonly string Seperator = Path.DirectorySeparatorChar.ToString();

        private static Stopwatch timer;

        private static int currentSceneIndex;
        private static string currentSceneName;
        private static List<string> currentHeader;

        // TODO: use file-path strings more structured
        public static void Main(string[] args)
        {
            // Stopwatch is used to track total time
            // So started first
            timer = new Stopwatch();
            timer.Start();

            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            try
            {
                string outFolder = $".{Seperator}exported{Seperator}";
                string zipFolder = $".{Seperator}zipped{Seperator}";
                string tempFolder = $".{Seperator}temporary{Seperator}CSVs{Seperator}";

                Directory.CreateDirectory(outFolder);
                Directory.CreateDirectory($"{outFolder}CSVs{Seperator}");

                Directory.CreateDirectory(zipFolder);
                Directory.CreateDirectory(tempFolder);

                ConfigureJsonWriter();
                await ExportSession();

                string outLocation = zipFolder + $"{SessionId}_session_exported.zip";
                ZipFolder(outFolder, outLocation);

                await Upload(outLocation);
                await UpdateDoc();

                PrintFinishTime();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Uncomment to make it so the program stays open, for debugging
            // System.Threading.Thread.Sleep(100000000);
        }

        private static void PrintFinishTime()
        {
            timer.Stop();
            var span = timer.Elapsed;
            string elapsed = string.Format(
                "{0:00}:{1:00}:{2:00}.{3:00}",
                span.Hours,
                span.Minutes,
                span.Seconds,
                span.Milliseconds / 10);

            timer.Reset();
            timer.Start();

            Console.WriteLine("Completed in: " + elapsed);
        }

        private static async Task ExportSession()
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

                    if (block.Index != currentSceneIndex)
                    {
                        currentSceneIndex = block.Index;
                        currentSceneName = block.Name;

                        if (currentSceneIndex != 0)
                        {
                            CopyCsv(block, "sceneName");
                        }

                        ToCsv(block, "sceneName");
                    }
                    else
                    {
                        // Write to temp
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
         * Returns: Task<List<long>> of a broken down count of all the documents in a session
         */
        private static async Task<List<long>> GetWorkloads()
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{SessionId}");
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
            if (docCount % groupCount != 0)
            {
                workloads.Add(docCount - (groupCount * workloadCount));
            }

            return workloads;
        }

        private static async Task<List<BsonDocument>> SortSession(int workload, int offset)
        {
            var sessionCollection = DB.GetCollection<BsonDocument>($"sessions.{SessionId}");

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

        private static async Task<BsonDocument> GetSessionInfo()
        {
            var sessions = DB.GetCollection<BsonDocument>("sessions");

            var filter = Builders<BsonDocument>.Filter
                .Eq("id", SessionId);

            return await sessions
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        private static void ToJson(List<BsonDocument> sessionDocs, string about, bool isFirst, JsonWriterSettings ws = null)
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

            string filename = $"{SessionId}.{about}.json";
            AppendToFile(docsTotal.ToString(), filename);
        }

        private static void ToJsonStart(string about)
        {
            AppendToFile("[", $"{SessionId}.{about}.json");
        }

        private static void ToJsonEnd(string about)
        {
            AppendToFile("]", $"{SessionId}.{about}.json");
        }

        private static void ToJson(BsonDocument sessionDoc, string about)
        {
            string filename = $"{SessionId}.{about}.json";
            AppendToFile(sessionDoc.ToJson(JsonWriterSettings.Defaults), filename);
        }

        private static string ToFlatJson(List<BsonDocument> sessionDocs)
        {
            StringBuilder docsTotal = new StringBuilder();
            docsTotal.Append("[");

            foreach (BsonDocument d in sessionDocs)
            {
                string json = d.ToJson(JsonWriterSettings.Defaults);
                var dict = JsonHelper.DeserializeAndFlatten(json);
                string flat = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

                docsTotal.AppendFormat("{0}{1}", flat, ",");
            }

            docsTotal.Length--; // Remove trailing comma
            docsTotal.Append("]");

            return docsTotal.ToString();
        }

        private static void ConfigureJsonWriter()
        {
            JsonWriterSettings.Defaults = new JsonWriterSettings()
            {
                Indent = true,
                OutputMode = JsonOutputMode.Strict,
            };
        }

        private static Configuration GetConfiguration()
        {
            return new Configuration
            {
                ShouldSkipRecord = (record) => record.All(string.IsNullOrEmpty),
                MissingFieldFound = (record, index, context) => record.All(string.IsNullOrEmpty),
                Delimiter = ",",
            };
        }

        private static (List<BsonDocument> otherCaptures, List<SceneBlock> scenes) ProcessScenes(List<BsonDocument> sessionDocs)
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

        private static void ToCsv(SceneBlock block, string about)
        {
            string json = ToFlatJson(block.Docs);
            string csv = JsonToCsv(json);

            string path = $".{Seperator}temporary{Seperator}CSVs{Seperator}";
            AppendToFile(csv, $"{SessionId}.{about}.{block.Name}.{block.Index}.csv", path);
        }

        private static string JsonToCsv(string jsonContent)
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

        private static void CopyCsv(SceneBlock block, string about)
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

            string path = $".{Seperator}exported{Seperator}CSVs{Seperator}";
            string filename = $"{SessionId}.{about}.{block.Name}.{block.Index}.csv";
            AppendToFile(csvString.ToString(), filename, path);

            string temporaryLocation = $".{Seperator}temporary{Seperator}CSVs{Seperator}{filename}";
            File.AppendAllText($"{path}{filename}", File.ReadAllText(temporaryLocation));

            currentHeader = new List<string>();
        }

        // TODO: make less magic
        // TODO: check this works for any data
        private static DataTable AlignHeaders(DataTable dt)
        {
            var header = dt.Columns.Cast<DataColumn>()
                        .Select(x => x.ColumnName)
                        .ToList();

            if (header.Count < currentHeader.Count)
            {
                foreach (string h in header)
                {
                    dt.Columns.Add(h);
                }
            }

            for (int i = 0; i < currentHeader.Count; i++)
            {
                dt.Columns[currentHeader[i]].SetOrdinal(i);
            }

            if (header.Count > currentHeader.Count)
            {
                currentHeader = header;
            }

            return dt;
        }

        private static void CreateReadme()
        {
            string source = System.IO.File.ReadAllText($".{Seperator}Templates{Seperator}README.txt.handlebars");
            var template = Handlebars.Compile(source);

            var data = new
            {
                id = SessionId,
            };

            AppendToFile(template(data), "README.txt");
        }

        private static void AppendToFile(string content, string filename, string path = "")
        {
            string fullpath;
            if (path == string.Empty)
            {
                fullpath = $".{Seperator}exported{Seperator}{filename}";
            }
            else
            {
                fullpath = $"{path}{filename}";
            }

            using (var sw = File.AppendText(fullpath))
            {
                sw.Write(content);
            }
        }

        private static void ZipFolder(string location, string outName)
        {
            ZipFile.CreateFromDirectory(location, outName);
        }

        // From: https://docs.min.io/docs/dotnet-client-quickstart-guide.html
        private static async Task Upload(string fileLocation)
        {
            var bucketName = "sessions.exported";
            var location = "us-west-1";
            var objectName = $"{SessionId}_session_exported.zip";
            var filePath = fileLocation;
            var contentType = "application/zip";

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await OS.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await OS.MakeBucketAsync(bucketName, location);
                }

                // Upload a file to bucket.
                await OS.PutObjectAsync(bucketName, objectName, filePath, contentType);
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }

        private static async Task UpdateDoc()
        {
            var sessions = DB.GetCollection<BsonDocument>("sessions");

            var filter = Builders<BsonDocument>.Filter
                .Eq("id", SessionId);

            var update = Builders<BsonDocument>.Update
                .Set("isExported", true)
                .Set("isPending", false);

            await sessions.UpdateOneAsync(filter, update);
        }
    }

    // Used to group together captures based on timestamps
    internal class SceneBlock
    {
        #pragma warning disable SA1516, SA1300
        public string Name { get; set; }
        public double StartTime { get; set; }
        public int Index { get; set; }
        public List<BsonDocument> Docs { get; set; }
        #pragma warning restore SA151, SA1300
    }
}