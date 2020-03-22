[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Exporter/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Exporter%22)

# Exporter 

Written in .Net Core (C#) and has the following dependencies:
- [MongoDB client](http://mongodb.github.io/mongo-csharp-driver/)
- [Minio Driver](https://github.com/minio/minio-dotnet)
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [Handlebars.Net](https://github.com/rexm/Handlebars.Net)
- [Json.Net](https://www.newtonsoft.com/json)

Gets a game session's data from MongoDB, processes it, outputs it files, then uploads a
zip of all the data to Minio.

## How this works

 It was observed that exporting a large enough session would result in either the process running out of memory and getting killed in the best case or using up all of the memory for a server and crashing it. As a solution this uses something I'm calling a *moving block*, which is designed to lower the memory stress this can have on a server. The way the *moving block* algorithm works is to only export a fraction of the total data at a time, so memory usage is mostly constant. The *moving block* algorithm follows this outline:

 1. Determine the number of documents in a session.
 1. Break up the work into blocks, based on a maximum count. For example a document number of 28 and a maximum count of 10 results in this workload: `[10, 10, 8]`.
 1. For the first *block*, fetch that many documents and export them to disk.
 1. Move to the next *block*, and then repeat from step 3.

 The reason this works is because all session documents are ordered, and can be progressively outputted to disk. Keep in mind that since the CSVs files are broken down by scene, scene information has to be remembered in-between each *block*.