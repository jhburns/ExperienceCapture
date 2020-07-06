[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Report/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Report%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Dockerfile/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Dockerfile%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Yaml/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Yaml%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Spellcheck/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Spellcheck%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Markdown/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3AMarkdown)

<p align="center">
  <img src="Documentation/images/logo.png" />
</p>

<p align="center">
  <b>A video game to spreadsheet converter.</b>
</p>

# Experience Capture

This is a video game analytics platform oriented towards capturing as much data from a play-session as possible. Unlike traditional data analytics solutions, Experience Capture lacks the concept of events and instead gathers data from every frame. The benefits of avoiding events are making this easier to integrate a game, higher data output, and analytics can be done after a play-session using whatever language is preferred. For more info see the [documentation](Documentation/README.md).

The major subparts of this project are:

- [Unity asset package](https://docs.unity3d.com/Manual/AssetPackages.html) client that integrates into any Unity game.
- Back-end server that receives, stores, and processes captured data from play-sessions.
- Front-end client for the server that authenticates and allows users to manage play session data.
- Infrastructure setup that builds the server and deploys it to [Amazon Web Services (AWS)](https://aws.amazon.com/).

## Folder Breakdown

- `DemoGame/` is a basic 'test your reaction time' game showing off the technology.
- `ExampleAnalyzers/` is examples in different languages of how to analyze the exported data.
- `ClientDevelopmentGame/` is the game used to develop the client.
- `ECBackend/` is a collection of back-end services for collection and processing data.
- `Deploy/` is an automated system to build and deploy the server to AWS.
- `SetupTestGame/` is a game used for testing the Unity client integration, to maintain backwards compatibility.
- `Documentation/` is documentation.
- `.github/` Contains the workflow files, for CI.

## Setup Games

All of the games need Unity version [2018.2.11f](https://unity3d.com/unity/whatsnew/unity-2018.2.11), which can be installed through Unity Hub.

## Other Setup

The other parts (`ECBackend/`, `Deploy/`) require the following installed:

[comment1]: <> (/* yaspeller ignore:start */)

- [Docker](https://docs.docker.com/install/)
- [Docker Compose](https://docs.docker.com/compose/install/)

[comment2]: <> (/* yaspeller ignore:end */)

In order to deploy having various cloud services is needed, [see here](Documentation/Cloud-Deploy.md).

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details
