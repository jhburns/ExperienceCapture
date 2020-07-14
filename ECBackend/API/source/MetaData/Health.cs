namespace Carter.App.MetaData.Health
{
    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class GetHealth : RouteMetaData
    {
        public override string Description { get; } = "Check if the API Server is healthy.";

        public override string Tag { get; } = "Health";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
        };

        public override string OperationId { get; } = "Health_GetHealth";
    }

    public class GetRoot : RouteMetaData
    {
        public override string Description { get; } = "Check if the API Server is healthy. "
            + "Does the same thing as /health, but with a message.";

        public override string Tag { get; } = "Health";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
        };

        public override string OperationId { get; } = "Health_GetRoot";
    }
    #pragma warning restore CS1591
}