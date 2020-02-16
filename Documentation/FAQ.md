# Frequently Asked Questions

### What types of values can be captured?

JSON has a limited number of data types, mainly:
- Boolean
- Number (int or float)
- String

See [this](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Data_structures) for
more information about types. It is not recommend to use arrays, because they may get
exported weirdly. Nested objects are fine.

### How can I change the client?

See the properties on the CaptureSetup prefab in the Unity Editor. There isn't documentation
on it yet, but hovering over properties has tool-tips that should help explain. 

### What happens if the game crashes/etc?

The reality is there are a number of ways as session could break, including but not limited to:
- The game crashing.
- The client in the game crashing.
- The network between the game and the server cutting out.
- The server crashing or becoming overwhelmed and timing out.
- AWS, the hosting platform, having an outage that effects the server.

The main strategy Experience Capture uses to deal with all of there is to transmit the
data to the back-end as soon as it is produced.

Here is a scenario were the game crashes. Let's say the game is currently 5 frames behind in terms of transmitting
data. That means 5 frames worth of data is still in memory in the game, and/or is currently being sent over the network. When the crash happens those frames are lost, but all previous captures have safely made it to the server.
They represent about 83 milliseconds, assuming the game is running at 60 fps, so very little data is lost in this case. After the crash, the server waits for two minutes just in case the game sends more data which it won't. After those two minutes, that session is marked as closed unexpectedly and can be exported/analyzed like a properly closed session.

## How can I delete sessions?

It is impossible to delete session, or any data for that matter. However, sessions can be archived so hide them in the normal UI. Near full data retentions is a policy designed to protect crucial data from human accidents.