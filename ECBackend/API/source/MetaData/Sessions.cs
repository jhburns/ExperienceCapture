namespace Carter.App.MetaData.Sessions
{
    using System.Collections.Generic;

    using Carter.App.MetaData.Extra;

    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class GetSessions : RouteMetaData
    {
        public override string Description { get; } = "Get sessions, by page. ";

        public override string Tag { get; } = "Sessions";

        public override string OperationId { get; } = "Sessions_GetSessions";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            MetaDataExtra.GetBsonDocumentation(),
            MetaDataExtra.GetUglyDocumentation(),
            new QueryStringParameter
            {
                Name = "page",
                Description = "Get a specific page of sessions, starting at zero.",
                Type = typeof(int),
            },
            new QueryStringParameter
            {
                Name = "isOngoing",
                Description = "When true get only ongoing sessions, and when false only non-ongoing ones. "
                    + "If not included, get all session ongoing or not.",
                Type = typeof(bool),
            },
            new QueryStringParameter
            {
                Name = "hasTags",
                Description = "Get only sessions that have the given tags.",
                Type = typeof(List<string>),
            },
            new QueryStringParameter
            {
                Name = "lacksTags",
                Description = "Get only sessions that no not have the given tags.",
                Type = typeof(List<string>),
            },
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = Status200OK,
                Description = "Ok.",
            },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }
    #pragma warning restore CS1591
}