[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/API/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22API%22)

# API

Written in .Net Core (C#) and has the following dependencies:

- [Carter framework](https://github.com/CarterCommunity/Carter)
- [Kestrel web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.0)
- [MongoDB client](http://mongodb.github.io/mongo-csharp-driver/)
- [Minio Driver](https://github.com/minio/minio-dotnet)
- [FluentValidation](https://fluentvalidation.net/)
- [Google APIs client](https://developers.google.com/api-client-library/dotnet)
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [Handlebars.Net](https://github.com/rexm/Handlebars.Net)
- [Json.Net](https://www.newtonsoft.com/json)
- [xUnit](https://xunit.net/)

This is the bulk of the logic in the application. Also, this will start the exporter from an api call.

## Documentation

API documentation is available in [OpenAPI format](https://swagger.io/docs/specification/about/) at `http://localhost:8090/api/v1/openapi/ui` after starting the server.

However, there are some limitations to using OpenAPI right now:

- Response schema in many cases can not be given. Instead, make a request to get the schema in the response informally.
- The `?ugly=true` query parameter does work, but Swagger UI displays all json pretty anyway.

## Local Authentication

To start using the API locally, open the OpenAPI documentation as described above and follow these steps:

- Create a user, if there isn't one yet, with the `POST /authentication/admins/` route. The response should look like:

```text
{
  "signUpToken": "NDi2lOoY7rlxxH7iKlqKinOa3yOc5PCotZXWZAX9UaA=", // <- Copy this value
  "expiration": {
    "$date": 1594784700406
  }
}
```

- With the `POST /users/` route, copy the sign up token into the `signUpToken` property, like so:

```plain
{
  "idToken": "string",
  "signUpToken": "NDi2lOoY7rlxxH7iKlqKinOa3yOc5PCotZXWZAX9UaA="
}
```

- With the `POST /users/{id}/tokens` route, change the id to: `123456789109876543210`, and delete the `claimToken` property in the request body. Next, copy the `accessToken` value.

- At the top of the page, click on 'Authorize' and paste the token into the 'Value' field. Then select 'Authorize' and close.


## Folder Breakdown

- `source/LibraryScripts/` a collection of common logic.
- `source/Routes/` the endpoints of the API, which is comprised of logic to be preformed based on the on the calling path.
- `source/UsageHelpers/` an [Insomnia](https://insomnia.rest/) export file that makes it easier to develop the API. It also comes with a binary BSON file, that can be loaded as a request body through Insomnia.
- `source/Validators/` bindings for data received for each applicable route.
- `source/Export/` the session exporter, formerly a separate component.
- `test` another project for tests.

## Tests

Run by building the integration Dockerfile, `docker build -f integration.Dockerfile .` or with `dotnet test`. The non-docker version will run tests with in parallel to make them faster, but that makes things flaky so integration does things sequentially.