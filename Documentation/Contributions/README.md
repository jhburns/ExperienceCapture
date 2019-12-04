# README (SVD)

- Name: Jonathan Hirokazu Burns
- ID: 2288851
- email: jburns@chapman.edu
- Course: 353-01
- Assignment: Submission #4

## Files

Due to this project being started before if being
apart of the Networking class (there are a huge number of files), only relevant files
to this submission's (#2) goal's are included:

- /Server/API/LibraryScripts/Network.cs
- /Server/API/Sessions.cs
- /Server/API/Bootstrapper.cs
- /Server/API/Dockerfile
- /Server/docker-compose.yaml
- /Server/Exporter/Exporter.cs
- /Server/Exporter/Dockerfile

New relevant files for submission #4:

- /Deploy/Packer/playbook.yaml
- /Deploy/Packer/build.json (No comments in JSON)
- /Deploy/Pulumi/Program.cs
- /Deploy/Pulumi/EnviromentVarNotSet.cs


(I counted, there were 3811 files in the repo)

## References

- https://docs.unity3d.com/2018.2/Documentation/Manual/
- http://mongodb.github.io/mongo-csharp-driver/
- https://docs.microsoft.com/en-us/dotnet/core/
- https://github.com/NancyFx/Nancy/wiki/Documentation
- https://docs.mongodb.com/
- https://docs.docker.com/compose/ 
- https://docs.docker.com/
- https://stackoverflow.com/questions/14473510/how-to-make-an-image-handler-in-nancyfx/28623873
- https://stackoverflow.com/questions/14473510/how-to-make-an-image-handler-in-nancyfx
- https://docs.microsoft.com/en-us/aspnet/
- https://medium.com/@michaelparkerdev/linting-c-in-2019-stylecop-sonar-resharper-and-roslyn-73e88af57ebd
- https://github.com/DotNetAnalyzers/StyleCopAnalyzers
- https://stackoverflow.com/questions/24189172/get-url-parameters-in-nancyfx
- https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
- https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
- https://codeburst.io/adding-google-sign-in-to-your-webapp-a-react-example-dcec8c73cb9f
- https://docs.ansible.com/
- https://www.packer.io/docs/
- https://www.pulumi.com/docs/
- https://create-react-app.dev/docs/getting-started/
- https://caddyserver.com/v1/docs
- https://docs.aws.amazon.com/
- https://github.com/pulumi/examples/blob/master/aws-cs-webserver/Program.cs

## Compile or Runtime Errors 
- "Timeout error": Client can sometimes not connect to the server when it was recently started. Wait and reconnect.
- Various HTTP errors can occur if the client or server drops a message. 
- Normal http errors can occur if trying to improperly access the api, for example 404. These are on purpose.
- Front-end client not working, always assumes there is mock-data.

## Running

### Client

Download relevant standalone to avoid setting up client:
- Windows: https://drive.google.com/open?id=12b1yRq9q-LpHwPWfK0Y2U_uSG97njwdU
- Mac: https://drive.google.com/open?id=18k-mjayT6e6ZwdGbQUfNxrg6MIhB1AVK

Then extract, and double-click the exe on Windows. Install game on Mac. Use
windowed mode for convenience on startup. 

See root https://github.com/jhburns/ExperienceCapture#experience-capture for manual setup (requires Unity). 

### Server

See Server README: https://github.com/jhburns/ExperienceCapture/tree/master/Server#server

It is only recommended to run the reverse-proxy on Docker Toolbox for Windows. 

Also see 

