namespace Carter.App.MetaData.Export
{
    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class PostExport : RouteMetaData
    {
        public override string Description { get; } = "Start exporting a session. "
            + "This can be called multiple times, including in case of error. "
            + "Also, multiple exports can happen simultaneously. ";

        public override string Tag { get; } = "Export";

        public override string OperationId { get; } = "Export_PostExport";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "The session is not found." },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class GetExport : RouteMetaData
    {
        public override string Description { get; } = "Get a session's export. ";

        public override string Tag { get; } = "Export";

        public override string OperationId { get; } = "Export_GetExport";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "The export will be stream as a zip file." },
            new RouteMetaDataResponse { Code = Status404NotFound, Description = "The session is not found." },
            new RouteMetaDataResponse { Code = Status202Accepted, Description = "The export for the session is pending." },
            new RouteMetaDataResponse { Code = Status500InternalServerError, Description = "Exporting for this session failed." },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }
    #pragma warning restore CS1591
}