# Architecture Overview 

Experience Capture uses a [client-server model](https://en.wikipedia.org/wiki/Client%E2%80%93server_model) and is a little special because there is both a client for in-game and the web. The main goals of this architecture are to make the service as simple as possible, reduce costs, and allow it to be maintainable and extendible in the future.

## Dataflow

![Dataflow diagram](images/data_flow.png)