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

    using Carter.App.Export.Hosting;
    using Carter.App.Export.JsonExtra;
    using Carter.App.Hosting;
    using Carter.App.Libs.FileExtra;
    using Carter.App.Libs.MinioExtra;
    using Carter.App.Route.Sessions;

    using CsvHelper;
    using CsvHelper.Configuration;

    using HandlebarsDotNet;

    using Minio.Exceptions;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Driver;

    /// <summary>
    /// Main class for exporting.
    /// </summary>
    public class ExportHandler
    {
        private static readonly string Seperator = Path.DirectorySeparatorChar.ToString();

        private static IMongoDatabase db;

        private static IMinioClient os;

        private static string sessionId;
        private static string prefix;

        private static int currentSceneIndex;
        private static string currentSceneName;
        private static List<string> currentHeader;

        /// <summary>
        /// Used when starting an exporter thread.
        /// </summary>
        /// <param name="config">Should be of type ExporterConfiguration.</param>
        public static void Entry(object config)
        {
            _ = MainAsync((ExporterConfiguration)config);
        }

        /// <summary>
        /// Needed so async methods can be used.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="config">Global configuration for a thread.</param>
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
                Directory.CreateDirectory($"{outFolder}extras{Seperator}");

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

        /// <summary>
        /// Sets up static properties that act as global variables.
        /// </summary>
        /// <param name="config">Contains the session id and connection information.</param>
        protected static void Setup(ExporterConfiguration config)
        {
            sessionId = config.Id;

            string mongoUrl = $"mongodb://{config.Mongo.ConnectionString}:{config.Mongo.Port}";
            db = new MongoClient(mongoUrl).GetDatabase("ec");

            string minioHost = $"{AppConfiguration.Minio.ConnectionString}:{AppConfiguration.Minio.Port}";
            os = new MinioClientExtra(minioHost, "minio", "minio123");
        }

        /// <summary>
        /// Process captures then writes them to files.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
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

            string extrasFolder = $"exported{Seperator}extras{Seperator}";

            AppendToFile("[", $"{sessionId}.raw.json", extrasFolder);
            AppendToFile("[", $"{sessionId}.sessionInfo.json");
            AppendToFile("[", $"{sessionId}.onlyCaptures.json");

            for (int i = 0; i < workloads.Count; i++)
            {
                // Step should be any of the workloads, except the last
                // First used because if it isn't a step then offset is zero anyway
                int offset = (int)(workloads[0] * i);
                List<BsonDocument> sessionSorted = await SortSession((int)workloads[i], offset);

                bool isFirst = i == 0;
                AppendToFile(ToJson(sessionSorted, isFirst, ws), $"{sessionId}.raw.json", extrasFolder);

                var (otherCaptures, sceneBlocks) = ProcessScenes(sessionSorted);
                if (otherCaptures.Count > 0)
                {
                    AppendToFile(ToJson(otherCaptures, isFirst), $"{sessionId}.sessionInfo.json");
                }

                foreach (var block in sceneBlocks)
                {
                    AppendToFile(ToJson(block.Docs, isFirst), $"{sessionId}.onlyCaptures.json");
                    isFirst = false;

                    // First scene shouldn't reqult in a previous scene being copied
                    if (currentSceneIndex == -1)
                    {
                        currentSceneIndex = block.Index;
                        currentSceneName = block.Name;
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
                    }

                    AppendToFile(
                        ToCsv(block),
                        $"{sessionId}.sceneName.{currentSceneName}.{currentSceneIndex}.csv",
                        $"temporary{Seperator}CSVs{Seperator}");
                }

                // Copy the last block
                if (i == workloads.Count - 1)
                {
                    CopyCsv(sceneBlocks[sceneBlocks.Count - 1], "sceneName");
                }
            }

            AppendToFile("]", $"{sessionId}.raw.json", extrasFolder);
            AppendToFile("]", $"{sessionId}.sessionInfo.json");
            AppendToFile("]", $"{sessionId}.onlyCaptures.json");

            AppendToFile("[", $"{sessionId}.database.json", extrasFolder);
            AppendToFile(ToJson(await GetSessionInfo()), $"{sessionId}.database.json", extrasFolder);
            AppendToFile("]", $"{sessionId}.database.json", extrasFolder);

            AppendToFile(CreateReadme(), "README.txt");
        }

        /// <summary>
        /// Breaks the session collection into blocks, of a constant size or less.
        /// Enables chunked processing of captures, so the process doesn't run out of memory.
        /// </summary>
        /// <returns>
        /// A count of all of captures in each block.
        /// </returns>
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

        /// <summary>
        /// Gets captures in a range.
        /// </summary>
        /// <returns>
        /// Captures in order by the 'realtimeSinceStartup' property.
        /// </returns>
        /// <param name="workload">How many captures to get.</param>
        /// <param name="offset">Where to start getting captures from, begining at zero.</param>
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

        /// <summary>
        /// Gets metadata about a session.
        /// </summary>
        /// <returns>
        /// Session metadata.
        /// </returns>
        protected static async Task<SessionSchema> GetSessionInfo()
        {
            var sessions = db.GetCollection<SessionSchema>("sessions");

            var filter = Builders<SessionSchema>.Filter
                .Where(s => s.Id == sessionId);

            return await sessions
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Converts a list of BSON documents into partial JSON.
        /// </summary>
        /// <returns>
        /// JSON objects in an array, but without closing or ending [].
        /// </returns>
        /// <param name="sessionDocs">A group of captures to convert.</param>
        /// <param name="isFirst">Should be true if these captures are the first in a collection.</param>
        /// <param name="ws">Additional serialization options.</param>
        protected static string ToJson(List<BsonDocument> sessionDocs, bool isFirst, JsonWriterSettings ws = null)
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

            return docsTotal.ToString();
        }

        /// <summary>
        /// Converts a BSON document to JSON.
        /// </summary>
        /// <returns>
        /// Serialized JSON, using default serialization settings.
        /// </returns>
        /// <param name="sessionDoc">A document.</param>
        protected static string ToJson(BsonDocument sessionDoc)
        {
            return sessionDoc.ToJson(JsonWriterSettings.Defaults);
        }

        /// <summary>
        /// Converts a session to JSON.
        /// </summary>
        /// <returns>
        /// Serialized JSON, using default serialization settings.
        /// </returns>
        /// <param name="sessionDoc">A session.</param>
        protected static string ToJson(SessionSchema sessionDoc)
        {
            return sessionDoc.ToJson(JsonWriterSettings.Defaults);
        }

        /// <summary>
        /// Converts a list of BSON documents to flat JSON.
        /// </summary>
        /// <returns>
        /// Serialized JSON, flattened into an array.
        /// </returns>
        /// <param name="sessionDocs">Documents.</param>
        protected static string ToFlatJson(List<BsonDocument> sessionDocs)
        {
            StringBuilder docsTotal = new StringBuilder();
            docsTotal.Append("[");

            if (sessionDocs.Count > 0)
            {
                foreach (BsonDocument d in sessionDocs)
                {
                    string json = d.ToJson(JsonWriterSettings.Defaults);
                    var dict = JsonExtra.DeserializeAndFlatten(json);
                    string flat = Newtonsoft.Json.JsonConvert.SerializeObject(dict);

                    docsTotal.AppendFormat("{0}{1}", flat, ",");
                }

                docsTotal.Length--; // Remove trailing comma
            }

            docsTotal.Append("]");

            return docsTotal.ToString();
        }

        /// <summary>
        /// Sets up the JSON writer with custom default settings.
        /// </summary>
        protected static void ConfigureJsonWriter()
        {
            JsonWriterSettings.Defaults = new JsonWriterSettings()
            {
                Indent = true,
                OutputMode = JsonOutputMode.Strict,
            };
        }

        /// <summary>
        /// Builds CSV configuration.
        /// </summary>
        /// <returns>
        /// A base configuration.
        /// </returns>
        protected static Configuration GetConfiguration()
        {
            return new Configuration
            {
                ShouldSkipRecord = (record) => record.All(string.IsNullOrEmpty),
                MissingFieldFound = (record, index, context) => record.All(string.IsNullOrEmpty),
                Delimiter = ",",
            };
        }

        /// <summary>
        /// Takes a group of captures and separates special or normal ones.
        /// </summary>
        /// <returns>
        /// A list of special captures and a list of normal scene blocks.
        /// </returns>
        /// <param name="sessionDocs">Captures.</param>
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
            sceneIndex = 0;
            var currentScene = sceneMap[sceneIndex];

            foreach (var doc in normalCaptures)
            {
                var currentTimestamp = doc["frameInfo"]["realtimeSinceStartup"].AsDouble;

                if (sceneIndex + 1 < sceneMap.Count
                    && currentTimestamp > sceneMap[sceneIndex + 1].StartTime)
                {
                    sceneIndex++;
                    currentScene = sceneMap[sceneIndex];
                }

                currentScene.Docs.Add(doc);
            }

            return (otherCaptures, sceneMap);
        }

        /// <summary>
        /// Converts a scene block to JSON.
        /// </summary>
        /// <returns>
        /// A flattened CSV representation of the sceneblock.
        /// </returns>
        /// <param name="block">A scene block to serialize.</param>
        protected static string ToCsv(SceneBlock block)
        {
            string json = ToFlatJson(block.Docs);
            string csv = JsonToCsv(json);
            return csv;
        }

        /// <summary>
        /// Converts JSON to CSV.
        /// </summary>
        /// <returns>
        /// A CSV block.
        /// </returns>
        /// <param name="jsonContent">A JSON array.</param>
        protected static string JsonToCsv(string jsonContent)
        {
            StringWriter csvString = new StringWriter();

            using (var csv = new CsvWriter(csvString, GetConfiguration()))
            {
                var dt = JsonExtra.JsonToTable(jsonContent);
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

        /// <summary>
        /// Copies temporary CSV files to finished files.
        /// </summary>
        /// <param name="block">Information for the finished files.</param>
        /// <param name="about">Extra information for the finished files.</param>
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

            // Write collected headers
            string path = $"exported{Seperator}CSVs{Seperator}";
            string filename = $"{sessionId}.{about}.{block.Name}.{block.Index}.csv";
            AppendToFile(csvString.ToString(), filename, path);

            // Copy data contents to finalized location
            string temporaryLocation = $"{prefix}temporary{Seperator}CSVs{Seperator}{filename}";
            File.AppendAllText($"{prefix}{path}{filename}", File.ReadAllText(temporaryLocation));

            // Reset the headers, because a new scene should have differerent headers
            currentHeader = new List<string>();
        }

        /// <summary>
        /// Merges past and current headers together.
        /// </summary>
        /// <returns>
        /// A data table with fixed headers.
        /// </returns>
        /// <param name="dt">A data table to merge with past headers.</param>
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

        /// <summary>
        /// Templates the README.
        /// </summary>
        /// <returns>
        /// A README with values substituted in the template.
        /// </returns>
        protected static string CreateReadme()
        {
            string source = FileExtra.GetEmbeddedFile($"Templates{Seperator}README.txt.handlebars");
            var template = Handlebars.Compile(source);

            var data = new
            {
                id = sessionId,
            };

            return template(data);
        }

        /// <summary>
        /// Appends chunks of data to a file.
        /// </summary>
        /// <param name="content">Data to be written.</param>
        /// <param name="filename">Name of the file being appended.</param>
        /// <param name="path">Location of the file.</param>
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

        /// <summary>
        /// Zips a file and outputs it.
        /// </summary>
        /// <param name="location">Where to start zipping from.</param>
        /// <param name="outName">Where to output the zip to.</param>
        protected static void ZipFolder(string location, string outName)
        {
            ZipFile.CreateFromDirectory(location, outName);
        }

        // From: https://docs.min.io/docs/dotnet-client-quickstart-guide.html

        /// <summary>
        /// Uploads a file to Minio.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="fileLocation">Where the file to upload can be found.</param>
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

        /// <summary>
        /// Updates the session document when done exporting.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="status">Information on what caused the end of an export.</param>
        protected static async Task UpdateDoc(ExportOptions status)
        {
            var sessions = db.GetCollection<SessionSchema>("sessions");

            var filter = Builders<SessionSchema>.Filter
                .Where(s => s.Id == sessionId);

            var update = Builders<SessionSchema>.Update
                .Set(s => s.ExportState, status);

            await sessions.UpdateOneAsync(filter, update);
        }
    }

    /// <summary>
    /// Groups captures together based on timestamps.
    /// </summary>
    public class SceneBlock
    {
        #pragma warning disable SA1516

        /// <summary>Scene name, which is non-unique.</summary>
        public string Name { get; set; }

        /// <summary>Timestamp the scene was started.</summary>
        public double StartTime { get; set; }

        /// <summary>Number of times the scene was restarted. Begins at zero.</summary>
        public int Index { get; set; }

        /// <summary>Captures belonging to this block.</summary>
        public List<BsonDocument> Docs { get; set; }
        #pragma warning restore SA151
    }
}