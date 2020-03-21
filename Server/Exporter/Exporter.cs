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

        public static void Main(string[] args)
        {
            // Stopwatch is used to track total time
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
                string tempFolder = $".{Seperator}temp{Seperator}CSVs{Seperator}";

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

                bool isLast = i == workloads.Count - 1;
                ToJson(sessionSorted, "raw", isLast, ws);

                //var (about, scenes) = ProcessScenes(sessionSorted);
                //ToJson(about, "sessionInfo");
                //ToJson(scenes, "onlyCaptures");

                // ToCsv(scenes, "sceneName");
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

            long groupCount = 100;
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

        private static void ToJson(List<BsonDocument> sessionDocs, string about, bool isLast = false, JsonWriterSettings ws = null)
        {
            StringBuilder docsTotal = new StringBuilder();

            foreach (BsonDocument d in sessionDocs)
            {
                docsTotal.AppendFormat("{0}{1}", d.ToJson(ws ?? JsonWriterSettings.Defaults), ",");
            }

            if (isLast)
            {
                docsTotal.Length--;
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

        private static void ToJson(BsonDocument sessionDocs, string about)
        {
            string filename = $"{SessionId}.{about}.json";
            AppendToFile(sessionDocs.ToJson(JsonWriterSettings.Defaults), filename);
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

        private static (List<BsonDocument> otherCaptures, List<SceneBlock> scenes) ProcessScenes(List<BsonDocument> sessionDocs)
        {
            var sceneDocs = sessionDocs.FindAll(d => d.Contains("sceneName"));

            List<SceneBlock> sceneMap = sceneDocs.Select((scene) =>
            {
                return new SceneBlock()
                {
                    StartTime = scene["frameInfo"]["realtimeSinceStartup"].AsDouble,
                    Name = scene["sceneName"].AsString,
                    Docs = new List<BsonDocument>(),
                };
            }).ToList();

            var normalCaptures = sessionDocs.FindAll(d => d.Contains("gameObjects"));
            var otherCaptures = sessionDocs.FindAll(d => !d.Contains("gameObjects"));

            int sceneIndex = 0;
            var currentScene = sceneMap[sceneIndex];

            for (int i = 0; i < normalCaptures.Count; i++)
            {
                var currentDoc = normalCaptures[i];
                var currentTimestamp = currentDoc["frameInfo"]["realtimeSinceStartup"].AsDouble;
                var tempIndex = sceneIndex + 1;

                if (tempIndex < sceneMap.Count
                    && currentTimestamp > sceneMap[tempIndex].StartTime)
                {
                    sceneIndex++;
                    currentScene = sceneMap[sceneIndex];
                }

                currentScene.Docs.Add(currentDoc);
            }

            return (otherCaptures, sceneMap);
        }

        private static void ToJson(List<SceneBlock> scenes, string about)
        {
            var demapped = new List<BsonDocument>();
            foreach (var s in scenes)
            {
                foreach (var d in s.Docs)
                {
                    demapped.Add(d);
                }
            }

            ToJson(demapped, about);
        }

        private static void ToCsv(List<SceneBlock> scenes, string about)
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];

                string json = ToFlatJson(scene.Docs);
                string csv = JsonToCsv(json, ",");

                string path = $".{Seperator}exported{Seperator}CSVs{Seperator}";
                AppendToFile(csv, $"{SessionId}.{about}.{scene.Name}.{i}.csv", path);
            }
        }

        // https://stackoverflow.com/a/36348017
        private static DataTable JsonToTable(string jsonContent)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonContent);
        }

        private static string JsonToCsv(string jsonContent, string delimiter)
        {
            StringWriter csvString = new StringWriter();
            var config = new Configuration
            {
                ShouldSkipRecord = (record) => record.All(string.IsNullOrEmpty),
                MissingFieldFound = (record, index, context) => record.All(string.IsNullOrEmpty),
                Delimiter = delimiter,
            };

            using (var csv = new CsvWriter(csvString, config))
            {
                using (var dt = JsonToTable(jsonContent))
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }

                    csv.NextRecord();

                    foreach (DataRow row in dt.Rows)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }

                        csv.NextRecord();
                    }
                }
            }

            return csvString.ToString();
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
                sw.WriteLine(content);
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
        public List<BsonDocument> Docs { get; set; }
        #pragma warning restore SA151, SA1300
    }
}