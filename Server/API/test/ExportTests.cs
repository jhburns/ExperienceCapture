namespace Carter.Tests.Export
{
    using System.Collections.Generic;

    using Carter.App.Export.Main;

    using MongoDB.Bson;

    using Xunit;

    public class ExportTests : ExportHandler
    {
        [Fact]
        public void CheckIsFlattenedExport()
        {
            string json = "{ \"a\": { \"b\": true } }";
            var documents = new List<BsonDocument>()
            {
                BsonDocument.Parse(json),
            };

            string flattened = ExportTests.ToFlatJson(documents);
            var parsed = BsonDocument.Parse($"{{ \"wrapper\": {flattened} }}");

            Assert.True(parsed["wrapper"][0]["a.b"].AsBoolean, "Json structure is not flattened.");

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a"];
            });
        }

        [Fact]
        public void CheckIsFlattenedManyExport()
        {
            string json = "{ \"a\": { \"b\": { \"c\": { \"d\": { \"e\": { \"f\": { \"g\": { \"h\": { \"i\": true } } } } } } } } }";
            var documents = new List<BsonDocument>()
            {
                BsonDocument.Parse(json),
            };

            string flattened = ExportTests.ToFlatJson(documents);
            var parsed = BsonDocument.Parse($"{{ \"wrapper\": {flattened} }}");

            Assert.True(parsed["wrapper"][0]["a.b.c.d.e.f.g.h.i"].AsBoolean, "Json structure is not flattened.");
        }

        [Fact]
        public void CheckIsFlattenedTypesExport()
        {
            string json = "{ \"a\": { \"b\": true, \"c\": 1, \"d\": \"test\", \"e\": null } }";
            var documents = new List<BsonDocument>()
            {
                BsonDocument.Parse(json),
            };

            string flattened = ExportTests.ToFlatJson(documents);
            var parsed = BsonDocument.Parse($"{{ \"wrapper\": {flattened} }}");

            Assert.True(parsed["wrapper"][0]["a.b"].AsBoolean, "Json structure is not flattened properly.");
            Assert.True(parsed["wrapper"][0]["a.c"].AsInt32 == 1, "Json structure is not flattened properly.");
            Assert.True(parsed["wrapper"][0]["a.d"].AsString == "test", "Json structure is not flattened properly.");
            Assert.True(parsed["wrapper"][0]["a.e"].BsonType == BsonType.Null, "Json structure is not flattened properly.");
        }

        [Fact]
        public void CheckIsFlattenedBranchingExport()
        {
            string json = "{ \"a\": { \"b\": true, \"c\": { \"d\": true } } }";
            var documents = new List<BsonDocument>()
            {
                BsonDocument.Parse(json),
            };

            string flattened = ExportTests.ToFlatJson(documents);
            var parsed = BsonDocument.Parse($"{{ \"wrapper\": {flattened} }}");

            Assert.True(parsed["wrapper"][0]["a.b"].AsBoolean, "Json structure is not flattened properly.");
            Assert.True(parsed["wrapper"][0]["a.c.d"].AsBoolean, "Json structure is not flattened properly.");

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a.c"];
            });
        }

        [Fact]
        public void CheckIsFlattenedWeirdNamesExport()
        {
            string json = "{ \"a.\": { \"b \": true, \"c$%^&\": { \"...\": true } } }";
            var documents = new List<BsonDocument>()
            {
                BsonDocument.Parse(json),
            };

            string flattened = ExportTests.ToFlatJson(documents);
            var parsed = BsonDocument.Parse($"{{ \"wrapper\": {flattened} }}");

            Assert.True(parsed["wrapper"][0]["a..b "].AsBoolean, "Json structure is not flattened properly.");
            Assert.True(parsed["wrapper"][0]["a..c$%^&...."].AsBoolean, "Json structure is not flattened properly.");

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a.b"];
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a.b "];
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a.c"];
            });

            Assert.Throws<KeyNotFoundException>(() =>
            {
                _ = parsed["wrapper"][0]["a.cc$%^&"];
            });
        }
    }
}