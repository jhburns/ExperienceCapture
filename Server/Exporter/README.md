[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Exporter/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Exporter%22)

# Exporter 

Written in .Net Core (C#) and has the following dependencies:
- [MongoDB client](http://mongodb.github.io/mongo-csharp-driver/)
- [Minio Driver](https://github.com/minio/minio-dotnet)
- [CsvHelper](https://joshclose.github.io/CsvHelper/)

Gets a game session's data from MongoDB, processes it, outputs it to file, then uploads a
zip of all the data to Minio.