[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Report/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Report%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Dockerfile/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Dockerfile%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Yaml/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Yaml%22)
[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Spellcheck/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Spellcheck%22)

# Experience Capture

<p align="center">
  <img src="Documentation/images/logo.png" />
</p>

This project is a custom video game analytics platform that is designed to support research needs. The four main parts are:
  - [Unity asset package](https://docs.unity3d.com/Manual/AssetPackages.html) client that integrates into any Unity game.
  - Back-end server that receives, stores, and processes captured data from play-sessions.
  - Front-end client that coordinates authentication and allows users to manage data.
  - Deploy system that allows repeatable and automated deploys of the service to [Amazon Web Services (AWS)](https://aws.amazon.com/).

Briefly, the main strength of this game analytics platform is to capture all data each frame, instead of using an event-based model. As a result, it is both easier to capture large amounts of data, hence the name, and perform arbitrary analytics work after a play-session. More on this in the [documentation]().

## Folders

- `ExampleUnityGame/` is an demo game showing off the client.
- `ClientDevelopmentGame/` is the game used to develop the client.
- `Server/` is a collection of back-end services for collection and processing data.
- `Deploy/` is an automated system to deploy the server to AWS in a repeatable way.
- `SetupTestGame/` is a game for testing if it is possible to integrate the client asset into a new game.
- `.github/`` Contains the workflow files, defining [Github Actions](https://github.com/features/actions).
- `Documentation/` has most of the documentation of course.

## Setup Games

All of the games only need Unity [2018.2.11f](https://unity3d.com/unity/whatsnew/unity-2018.2.11), which can be installed through Unity Hub.

## Other Setup

The other services (server, deploy, ...) only have the requirement of having [Docker](https://docs.docker.com/install/) installed locally. For deploys, having various cloud services is needed. See the README in each folder for more information on each service.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details
