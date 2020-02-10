# Export Format

After exporting, downloading, and unzipping a sessions here is how the data you receive will be organized.
The explanations are by filename, [Session ID] is the four character identifier for each session. All files
have the data sorted by timestamp by the exporter and timestamps are not unique.

## [Session ID].database.json

This data is a [MongoDB document](https://docs.mongodb.com/manual/core/document/) coming straight from the database used to store all of the session data. It is generated entirely by the API, meaning none of it comes from the game client. In constant, all of the other files mentioned below only have data generated by the game client.

Here is an example of this file type, with annotations:
```json
{
  "_id" : { "$oid" : "5e3350fb774b120001aefa93" }, // Data base key, not important
  "isOpen" : true,
  "isExported" : false,
  "isPending" : true,
  "user" : { // The user who logged into the service, NOT the player
    "fullname" : "Smitty Jensens",
    "firstname" : "Smitty",
    "lastname" : "Jensens",
    "email" : "smitty@jenkins.com",
    "createdAt" : { "$date" : 1580169485875 },
    "id" : "123456789109876543210"
  },
  "createdAt" : { "$date" : 1580421371319 },
  "tags" : ["archived"], // Flexible properties on the data
  "id" : "RUE6" // Unique ID
}
```

In addition, all of the keys starting with a dollar sign, `$`, are [extended JSON properties used by MongoDB](https://docs.mongodb.com/manual/reference/mongodb-extended-json/).

## [Session ID].sessionInfo.json

Only the non-gameObjects frames, annotated example:
```json
[{
  "dateTime" : "2020-01-30T21:56:41.6282480Z", // UTC time generated on the client
  "description" : "Session Started",
  "captureRate" : 1, // How often a capture is taken, 1 = every frame
  "extraInfo" : {
    "clientVersion" : "1.1.2",
    "gameVersion" : ""
  },
  "special" : true,
  "targetFrameRate" : -1, // framerate, -1 means VSYNC
  "username" : "Boyd", // Name of the player
  "frameInfo" : {
    "realtimeSinceStartup" : -1.0,
    "timeSinceLevelLoad" : -1.0,
    "unscaledDeltaTime" : -1.0
  }
},{ // All of the different scenes included here, only one in this example
  "description" : "Scene Loaded",
  "sceneName" : "SampleScene",
  "special" : true,
  "frameInfo" : {
    "realtimeSinceStartup" : 50.620361328125,
    "timeSinceLevelLoad" : 0.0,
    "unscaledDeltaTime" : 0.016727924346923828
  }
}]
```

## [Session ID].onlyCaptures.json

This contains all of the data that was produced by the client calling `GetCapture()`.

Annotated example:
```json
[{
  "gameObjects" : {
    "player": { // Arbitrary data exported by the developer
        "positionX": 45
    }
  },
  "frameInfo" : {
    "realtimeSinceStartup" : 50.644088745117188, // Time since the game was launched
    "timeSinceLevelLoad" : 0.0, // Time relative to the level loading
    "unscaledDeltaTime" : 0.016727924346923828 // time between frames
  }
},{
    ... Rest of the captures
```

## CSV files

Multiple `.csv` files can be created, and each is a only data from one of the scenes. Like the `[Session ID].onlyCaptures.json` file, only data from `GetCapture()` is contained in them. Their general file format is:
```
[Session ID].byScene.[Scene Name].[Index].csv
```
with a more concrete example of:
```
RUE6.byScene.SampleScene.0.csv
```

In this example, SampleScene is the scene name, and 0 the index. That indicates that SampleScene was the first scene loaded (besides the setup Scene). The index is just a count of which number in order the scene was loaded, starting at 0.

For example given the following scenes being loaded/played in a game:
```json
Menu -> Main -> GameOver -> Main
```
The following files would be exported, with the session ID EXEX/:
```json
EXEX.byScene.Menu.0.csv
EXEX.byScene.Main.1.csv
EXEX.byScene.GameOver.2.csv
EXEX.byScene.Main.3.csv
```

Additionally because csv is a table format, all properties are flattened. So, an example header value would be `gameObjects.player.positionX`. And since columns are not flexible like json, `NULL` is used for real null values, missing properties, and missing game objects names.

## [Session ID].raw.json

The data without any processing. Only useful if the data was exported improperly. The only JSON that isn't formatted, to save space.