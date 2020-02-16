# Documentation

(For client/server versions 1.x.x)

## Unity Client Setup

1. [Download and setup the client asset package.](Setup.md)
1. [Learn how to add data capturing code to the Unity game.](Coding.md)
1. [How your data is captured (optional).](About-Capture.md)
1. Additionally see the [FAQ](FAQ.md) and if that doesn't help join the `#experience-capture` channel and ask.

### Updating Unity Client

- [In general.](Updating.md)
- [Migrating breaking changes for versions 1.0.0 => 1.1.0.](Updating-To-1.1.0.md)

### Compatibility

Version 1.0.0 of the Experience Capture client is designed
to be compatible with [Unity 2018.2.11](https://unity3d.com/get-unity/download/archive) on Windows 10. Docker/Docker-compose versions
are defined in the Ansible playbook located in `Deploy/Packer/` folder and supports Linux/Windows 10.
The WebUI supports Chrome version 79. Other platforms may work, but haven't been tested.

## Data Analysis

[Explore some example analyzers, in Python and Rlang.](ExampleAnalyzers/README.md)

[In-depth information on how the data is formatted.](Export-Format.md)

## Cloud Deploy

Cloud Deploys can happen at two levels, a full setup or a partial redeploy. You'll likely want
a partial redeploy, so [follow this guide](Partial-Deploy.md).

For someone who is setting up the cloud service for the first time, [follow this one instead.](Full-Deploy.md)

## Contributing

[Information on contributing.](Contributing.md)
