# Configure

Information about configure option of the `SetupCapture` game object in the `SetupEC` scene.
Options are in order.

## Capture Rate 

Basically how often the `GetCapture()` function is called, respecting frame rate.
A value of 1 means `GetCapture()` is called every frame. A value of 2 means `GetCapture()`
is called every other frame.

## Scene To Load

The name of the scene to load when pressing the "Start Session" button.

## Game Version

Version of your game, useful for matching up captured data with specific games versions.

## Client Version

Readonly, the version of the client installed.

## Default Url

The URL that the client opens with. Recommended to be https://expcap.xyz.

## Offline Mode

When checked, skip connecting to the data capture server.

## Print Additional Capture Info

When checked, prints debugging info every frame.

## Find Objects In Each Frame

When checked, the client will look for `ICapturable` objects every frame instead of only at the start of a scene.
Useful when a game creates/destroys objects dynamically.

## Do Not Print To Console

When checked, nothing will be printed to console.

## Do Not Throw Not Found

When checked, objects/keys not found by the Limit list will be ignored. 
Useful when a game creates/destroys objects dynamically.

## Limit Output To Specified

This allows you to ignore every property exported under `GetCapture()` except those specified. When list size is zero, nothing happend.

Separate object names from key using a colon, `:`, for example:

```
Player:positionX
```

This entry would ignore every key except `positionX` on the `Player` game object.

# API