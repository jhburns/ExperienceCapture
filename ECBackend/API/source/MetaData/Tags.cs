namespace Carter.App.MetaData.Tags
{
    using System;

    using System.Collections.Generic;

    using Carter.App.Route.Sessions;

    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class PostTags : RouteMetaData
    {
        public override string Description { get; } = "Add a tag to a session. "
            + "Adding a duplicate tag does nothing, and returns a successful status code. "
            + "View tags on a session through the GET /sessions/{id} route.";

        public override string Tag { get; } = "Sessions";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "OK" },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "Session is NOT FOUND." },
        };

        public override string OperationId { get; } = "Sessions_PostTags";

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class DeleteTags : RouteMetaData
    {
        public override string Description { get; } = "Delete a tag from a session. "
            + "Deleting a non-existant tag does nothing, and returns a successful status code."
            + "View tags on a session through the GET /sessions/{id} route.";

        public override string Tag { get; } = "Sessions";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "OK" },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "Session is NOT FOUND." },
        };

        public override string OperationId { get; } = "Sessions_DeleteTags";

        public override string SecuritySchema { get; set; } = "apiKey";
    }
    #pragma warning restore CS1591
}