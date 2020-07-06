# Configure

Information about configure option of the `SetupCapture` game object in the `SetupEC` scene.
Options are in order.

## Capture Rate

This has to be a number greater than 0.

The rate is how often the `GetCapture()` function is called, with respect to frame rate.
A value of 1 means `GetCapture()` is called every frame. A value of 2 means `GetCapture()`
is called every other frame. Two games with different frame rates and the same value for this
will result in one capturing more often than the other.

## Scene To Load

This cannot be empty.

The name of the scene to load when pressing the "Start Session" button.

## Game Version

String version of your game, useful for matching up captured data with specific games versions.

## Client Version

Read-only, the version of the client installed.

## Default Url

The URL that the client starts with. Recommended to be https://expcap.xyz.

## Limit Output To Specified

This allows you to ignore every property exported under `GetCapture()` except those specified. When list size is zero, nothing happened.

Separate object names from key using a colon, `:`, for example:

```text
Player:positionX
```

This entry would ignore every key except `positionX` on the `Player` game object.

## Offline Mode

When checked, skip connecting to the data capture server and print all of the capture data to the console instead. The captures printed to the console are the **exact** same as would be sent to the ECBackend.

## Print Additional Capture Info

When checked, prints debugging info every frame.

## Do Not Print To Console

When checked, nothing will be printed to console.

## Configuration API

The capturer exposes an API for additional configuration, but be warned it is experimental.

### Cleanup Key

This will override the default cleanup key of `Q` to the key `W`:

```csharp
CaptureConfig.OverrideCleanupKey(KeyCode.W);
```

This gets the current cleanup key:

```csharp
KeyCode cleanupKey = CaptureConfig.GetCleanupKey();
```