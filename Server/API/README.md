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

This is the bulk of the logic in the application. Also, this will start the exporter from an api call.

## Folder Breakdown

- `LibraryScripts/` a collection of common logic.
- `Routes/` the endpoints of the API, which is comprised of logic to be preformed based on the on the calling path.
- `UsageHelpers/` an [Insomnia](https://insomnia.rest/) export file that makes it easier to develop the API. It also comes with a binary BSON file, that can be loaded as a request body through Insomnia.
- `Validators/` bindings for data received for each applicable route.
- `Export/` the session exporter, formerly a separate component.