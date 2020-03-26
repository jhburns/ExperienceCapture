[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/API/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22API%22)

# API

Written in .Net Core (C#) and has the following dependencies:
- [Carter framework](https://github.com/CarterCommunity/Carter)
- [Kestrel web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.0)
- [MongoDB client](http://mongodb.github.io/mongo-csharp-driver/)
- [Minio Driver](https://github.com/minio/minio-dotnet)
- [Docker.DotNet](https://github.com/microsoft/Docker.DotNet)
- [FluentValidation](https://fluentvalidation.net/)
- [Google APIs client](https://developers.google.com/api-client-library/dotnet)

This is the bulk of the logic in the application. Also, this will start the exporter from an api call.

## Folder Breakdown

- `LibraryScripts/` a collection of common logic.
- `Routes/` the endpoints of the API, which is comprised of logic to be preformed based on the on the calling path.
- `UsageHelpers/` an [Insomnia](https://insomnia.rest/) file that makes it easy to develope the API.
- `Validators/` bindings for data received for each applicable route.