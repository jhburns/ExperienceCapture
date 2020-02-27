# Frequently Asked Questions

### What types of values can be captured?

JSON has a limited number of data types, mainly:
- Boolean
- Number (int or float)
- String

See [this](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Data_structures) for
more information about types. It is not recommend to use arrays, because they may get
exported weirdly. Nested objects are fine.

[comment]: <> (TODO: Move this info to Coding.md)

### How can I change the client?

[comment]: <> (TODO: Document CaptureSetup)

See the properties on the CaptureSetup prefab in the Unity Editor. There isn't documentation
on it yet, but hovering over properties has tool-tips that should help explain. 

### What happens if the game crashes/etc?

The reality is there are a number of ways as session could break, including but not limited to:
- The game crashing.
- The client in the game crashing.
- The network between the game and the server cutting out.
- The server crashing or becoming overwhelmed and timing out.
- AWS, the hosting platform, having an outage that effects the server.

The main strategy Experience Capture uses to deal with all of there is to transmit the data to the back-end as soon as it is produced. So in the worst case less than a second of data is ever lost.

## How can I delete sessions?

It is impossible to delete a session, or any data for that matter. However, sessions can be archived to hide them, which is analogous to a delete.

## How to find all capturable objects?

`Ctrl-Shift-F` and then search for `ICapturable` across the whole project.