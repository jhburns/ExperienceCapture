namespace Carter.App.MetaData.Tags
{
    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class PostTags : RouteMetaData
    {
        public override string Description { get; } = "Add a tag to a session. "
            + "Adding a duplicate tag does nothing, and returns a successful status code. "
            + "View tags on a session through the GET /sessions/{id} route.";

        public override string Tag { get; } = "Session Tags";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "Session is not found." },
        };

        public override string OperationId { get; } = "SessionTags_PostTags";

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class DeleteTags : RouteMetaData
    {
        public override string Description { get; } = "Delete a tag from a session. "
            + "Deleting a non-existant tag does nothing, and returns a successful status code."
            + "View tags on a session through the GET /sessions/{id} route.";

        public override string Tag { get; } = "Session Tags";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "Session is not found." },
        };

        public override string OperationId { get; } = "SessionTags_DeleteTags";

        public override string SecuritySchema { get; set; } = "apiKey";
    }
    #pragma warning restore CS1591
}