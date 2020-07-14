namespace Carter.App.MetaData.UsersAndAuthentication
{
    using Carter.App.MetaData.Extra;

    using Carter.App.Validation.AccessTokenRequest;
    using Carter.App.Validation.AdminPassword;
    using Carter.App.Validation.Person;

    using Carter.OpenApi;

    using static Microsoft.AspNetCore.Http.StatusCodes;

    #pragma warning disable CS1591
    public class PostUsers : RouteMetaData
    {
        public override string Description { get; } =
            "Create a user. "
            + "When developing locally, a mock user with the id '123456789109876543210' is created. "
            + "When developing locally, validating the idToken is skipped.";

        public override string Tag { get; } = "Users";

        public override string OperationId { get; } = "Users_PostUsers";

        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(PersonRequest),
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
                Code = Status400BadRequest,
                Description = "Bad request, meaning the request schema is invalid.",
            },
            new RouteMetaDataResponse
            {
                Code = Status401Unauthorized,
                Description = "Unauthorized, due to the identification or sign up token being invalid, ",
            },
            new RouteMetaDataResponse
            {
                Code = Status404NotFound,
                Description = "The sign up token has already been used.",
            },
            new RouteMetaDataResponse
            {
                Code = Status409Conflict,
                Description = "The person being created already exists.",
            },
        };
    }

    public class PostAccessToken : RouteMetaData
    {
        public override string Description { get; } =
            "Create an access token which can be used to authenticate with protected routes. "
            + "When developing locally, validating the idToken is skipped. "
            + "The claimToken property for the request schema is optional, and when used "
            + "results in the access token being placed in the given claim token.";

        public override string Tag { get; } = "Users";

        public override string OperationId { get; } = "Users_PostAccessToken";

        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(AccessTokenRequest),
            },
        };

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
                Description = "Either an access token in JSON/BSON format, or an 'OK' meaning the claim was redeemed correctly.",
            },
            new RouteMetaDataResponse
            {
                Code = Status400BadRequest,
                Description = "Bad request, meaning the request schema is invalid.",
            },
            new RouteMetaDataResponse
            {
                Code = Status401Unauthorized,
                Description = "Unauthorized, due to the identification token being invalid, "
                    + " or the claim token is invalid.",
            },
            new RouteMetaDataResponse
            {
                Code = Status409Conflict,
                Description = "Conflict, due the {id} not being the same as the Subject of the request token.",
            },
        };
    }

    public class PostClaims : RouteMetaData
    {
        public override string Description { get; } = "Create a claim token. "
            + "It can be filled by having the user sign in using their browser.";

        public override string Tag { get; } = "Authentication";

        public override string OperationId { get; } = "Authentication_PostClaimToken";

        public override QueryStringParameter[] QueryStringParameter { get; } =
        {
            MetaDataExtra.GetBsonDocumentation(),
            MetaDataExtra.GetUglyDocumentation(),
        };

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse { Code = Status200OK, Description = "Ok." },
        };
    }

    public class GetClaims : RouteMetaData
    {
        public override string Description { get; } = "Get a claim token. "
            + "Responds with an access token after it has been filled using POST /users/{id}/tokens.";

        public override string Tag { get; } = "Authentication";

        public override string OperationId { get; } = "Authentication_GetClaimToken";

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
                Code = Status400BadRequest,
                Description = "A claim token was not placed in the 'ExperienceCapture-Claim-Token' cookie.",
            },
            new RouteMetaDataResponse
            {
                Code = Status202Accepted,
                Description = "An access token has not been granted for the claim yet.",
            },
        };
    }

    public class PostAdmin : RouteMetaData
    {
        public override string Description { get; } = "Create an admin sign up token. "
            + "When developing locally, checking the password is skipped.";

        public override string Tag { get; } = "Authentication";

        public override string OperationId { get; } = "Authentication_PostAdmin";

        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                Request = typeof(AdminPasswordRequest),
            },
        };

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
                Code = Status400BadRequest,
                Description = "Bad request, meaning the request schema is invalid.",
            },
            new RouteMetaDataResponse
            {
                Code = Status401Unauthorized,
                Description = "The admin password is incorrect.",
            },
        };
    }

    public class GetUser : RouteMetaData
    {
        public override string Description { get; } = "Get a user. "
            + "Users with the 'Normal' can only get themselves. "
            + "Users with the 'Admin' role can get any user.";

        public override string Tag { get; } = "Users";

        public override string OperationId { get; } = "Users_GetUser";

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
                Description = "The user is not found.",
            },
            new RouteMetaDataResponse
            {
                Code = Status401Unauthorized,
                Description = "The user requesting is not the same user as asked for, or an admin.",
            },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class DeleteUser : RouteMetaData
    {
        public override string Description { get; } = "Delete a user. "
            + "Keep in mind deleting only means the user is hidden and not removed from the database. "
            + "Deleting a non-existing user does nothing, and returns a successful status."
            + "Users with the 'Normal' can only delete themselves. "
            + "Users with the 'Admin' role can delete any user.";

        public override string Tag { get; } = "Users";

        public override string OperationId { get; } = "Users_DeleteUser";

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
                Description = "The user is not found.",
            },
            new RouteMetaDataResponse
            {
                Code = Status401Unauthorized,
                Description = "The user requesting is not the same user as asked for, or an admin.",
            },
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class PostSignUp : RouteMetaData
    {
        public override string Description { get; } = "Create a sign up token. ";

        public override string Tag { get; } = "Authentication";

        public override string OperationId { get; } = "Authentication_PostSignUpToken";

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
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }

    public class GetUsers : RouteMetaData
    {
        public override string Description { get; } = "Get all users. "
            + "Requires the role 'Admin'. "
            + "On a different path than /users/ because of an issue with Carter.";

        public override string Tag { get; } = "Users";

        public override string OperationId { get; } = "Users_GetUsers";

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
        };

        public override string SecuritySchema { get; set; } = "apiKey";
    }
    #pragma warning restore CS1591
}