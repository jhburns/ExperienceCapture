**⭐ Latest Client Version: 1.2.0 ⭐** [Install](Setup.md) or [Update](Updating.md)

# Documentation

## Unity Client Setup

1. [Download and install the client asset package.](Setup.md)
1. [Learn how to add data capturing code to the Unity game.](Coding.md)
1. [How your data is captured (optional).](About-Capture.md)
1. [Additional ways to configure the client (optional).](Configure.md)
1. Additionally see the [FAQ](FAQ.md) and if that doesn't help join the `#experience-capture` channel and ask.

## Updating Unity Client

- [Update to the latest version (1.2.0).](Updating.md)

## Compatibility Info

The Experience Capture client is designed to be compatible with [Unity version 2018.2.11](https://unity3d.com/get-unity/download/archive) on Windows 10. Docker/Docker-compose versions are defined in the Ansible playbook located in `Deploy/Packer/` folder and supports Linux/Windows 10. The WebUI supports Chrome version 79 on Windows, Linux, and Android (Sorry, I don't have an iPhone). Other platforms may work because these solutions are designed to be cross-platform, but haven't been tested yet.

## Data Analysis

- [Explore some example analyzers, in Python and Rlang.](ExampleAnalyzers/README.md)
- [In-depth information on how the data is exported.](Export-Format.md)

The data is exported to JSON and CSV files, so your preferred language should work for doing analytics.

## Cloud Deploy

Cloud Deploys can happen at two levels, a full setup or a partial redeploy. You'll likely want
a partial redeploy, so [follow this guide](Partial-Deploy.md).

For someone who is setting up the cloud service for the first time, [follow this one instead.](Full-Deploy.md)

Additionally see [the architecture overview](Architecture.md) for more information about how all of the services  interact with each other. There is also a guide on how to restore [backups](Backups.md).

## Contributing

[Information on contributing.](Contributing.md)
