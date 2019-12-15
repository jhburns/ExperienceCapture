[![Integration Status](https://github.com/jhburns/ExperienceCapture/workflows/Dockerfile%20and%20Yaml/badge.svg)](https://github.com/jhburns/ExperienceCapture/actions?query=workflow%3A%22Dockerfile+and+Yaml%22)

# Experience Capture

The project aims to simplify and empower data collection from interactive technologies.

It uses a client-server model to do so, and is broken down into important folders:

- *ExampleUnityGame/* is an example game using the client.
- *ClientDevelopmentGame/* is a game used to develop the client. 
- *SetupTestGame/* is a game for testing how easy/possible it is to integrate into the asset into a new game. 
- *Server/* is a backend service for collection and analyzing data.

## Extra Folders

- *Deploy/* bakes images and manages cloud deploys. 
- *.github/* Contains the workflow files. 

## Setup

All of the games only need Unity 2018.2.11f, link: https://unity3d.com/unity/whatsnew/unity-2018.2.11.

For the [server README](https://github.com/jhburns/ExperienceCapture/tree/master/Server#server) for setup info about that.