namespace Carter.App.MetaData.Sessions
{
    using Carter.App.MetaData.Extra;

    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class PostSessions : RouteMetaData
    {
        public override string Description { get; } = "Create/start a session. ";

        public override string Tag { get; } = "Sessions";

        public override string OperationId { get; } = "Sessions_PostSessions";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            MetaDataExtra.GetBsonDocumentation(),
            MetaDataExtra.GetUglyDocumentation(),
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }

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
                Description = "Get a specific page of sessions, starting at zero. "
                    + "(Default: 0)",
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
                Type = typeof(string),
            },
            new QueryStringParameter
            {
                Name = "lacksTags",
                Description = "Get only sessions that no not have the given tags.",
                Type = typeof(string),
            },
            new QueryStringParameter
            {
                Name = "sort",
                Description = "The direction to sort sessions. "
                    + "Should only be values 'Alphabetical', 'NewestFirst', or 'OldestFirst'. "
                    + "(Default: NewestFirst)",
                Type = typeof(string),
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

    public class PostSession : RouteMetaData
    {
        public override string Description { get; } = "Add capture data to a session. ";

        public override string Tag { get; } = "Sessions";

        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(ExampleCaptureSchema),
            },
        };

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            new QueryStringParameter
            {
                Name = "bson",
                Description = "When true, the request body is BSON encoded.",
                Type = typeof(bool),
            },
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = Status200OK,
                Description = "Ok.",
            },
            new RouteMetaDataResponse
            {
                Code = Status404NotFound,
                Description = "The session is not found.",
            },
            new RouteMetaDataResponse
            {
                Code = Status400BadRequest,
                Description = "Bad request, meaning the session has already been closed or the request schema is invalid.",
            },
        };

        public override string OperationId { get; } = "Sessions_PostSession";

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class GetSession : RouteMetaData
    {
        public override string Description { get; } = "Get a session. ";

        public override string Tag { get; } = "Sessions";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            MetaDataExtra.GetBsonDocumentation(),
            MetaDataExtra.GetUglyDocumentation(),
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = Status200OK,
                Description = "Ok.",
            },
            new RouteMetaDataResponse
            {
                Code = Status404NotFound,
                Description = "The session is not found.",
            },
        };

        public override string OperationId { get; } = "Sessions_GetSession";

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class DeleteSession : RouteMetaData
    {
        public override string Description { get; } = "Close a session. "
            + "Keep in mind, the session and its data are not removed from the database.";

        public override string Tag { get; } = "Sessions";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            MetaDataExtra.GetBsonDocumentation(),
            MetaDataExtra.GetUglyDocumentation(),
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = Status200OK,
                Description = "Ok.",
            },
            new RouteMetaDataResponse
            {
                Code = Status404NotFound,
                Description = "The session is not found.",
            },
        };

        public override string OperationId { get; } = "Sessions_GetSession";

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    #pragma warning disable SA1300
    internal class ExampleCaptureSchema
    {
        public string exampleAnyOtherDataIsAllowed { get; set; }

        public FrameInfo frameInfo { get; set; }
    }

    internal class FrameInfo
    {
        public double realtimeSinceStartup { get; set; }
    }
    #pragma warning restore SA1300
    #pragma warning restore CS1591
}