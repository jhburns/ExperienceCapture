# How Your Data Is Captured 

There are two different options for capturing data, based on the "Find Objects In Each Frame" flag
on the prefab. Keep in mind there is a difference between *capturing* data 
and *sending* data. Meaning that data is captured the **exact same**, but is sent either to the
console locally in "Offline Mode" or to a server normally. In other words, the data printed to the console is the **exact same** as the data that shows up in session's zip.

## Static GameObject Capturing

Lets assume there are two objects in this scene, a player and a ball. 
The 'PlayerObject' Unity GameObject has this script with the following capture logic:

```csharp
using Capture;

public class Player : MonoBehaviour, ICapturable
{

	// Player functionality 
	
    public object GetCapture() {
        return new 
        {
            positionX = transform.position.x;
        };
    }

}
```

and the 'BallObject' Unity GameObject has the following script attached to it:
```csharp
using Capture;

public class Ball : MonoBehaviour, ICapturable
{

	// Ball functionality 
	
    public object GetCapture() {
        return new 
        {
            positionX = transform.position.x,
            isActive = this.IsActive()
        };
    }
}

```

This would produce a capture of an example frame like so [comments added]:

```json
{
  "gameObjects": {
    "PlayerObject": {
      "positionX": 3.0777777
    },
    "BallObject": {
      "positionX": 6.0,
       "isActive": true
    }
  },
  "frameInfo": {
    "unscaledDeltaTime": 0.0171986,
    "realtimeSinceStartup": 4.075656,
    "timeSinceLevelLoad": 0.6566279
  }
}
```

Here are a couple important points to notice:
- All GameObjects are grouped under a `gameObjects` key.
- Data points are grouped together based on the [GameObject name](https://docs.unity3d.com/ScriptReference/Object-name.html). 
This allows multiple scripts to use the same key name, which is recommenced for simplicity.
- The keys (`positionX`, `isActive`) for each value are determined by the left side of the returned
object.
- All of the timestamps are normal [Unity Time](https://docs.unity3d.com/2018.2/Documentation/ScriptReference/Time.html) variables

## Static GameObject Capturing

Now lets show what happens when "Find Objects In Each Frame" flag is checked.
It works similar to the previous example, but now ICapturable GameObjects
won't cause the capture system to error when they are Instantiated/Destroyed. 

We will look at three frame examples, with the following states:
1. PlayerObject exists, BallObject is Instantiated for the next frame.
1. PlayerObject exists, BallObject is Destroyed for the next frame.
1. PlayerObject exists.

Which produces

1.
```json
{
  "gameObjects": {
    "PlayerObject": {
      "positionX": 2.6666666
    }
  },
  "frameInfo": {
    "unscaledDeltaTime": 0.0173622,
    "realtimeSinceStartup": 4.058367,
    "timeSinceLevelLoad": 0.6394293
  }
}
```

2.
```json
{
  "gameObjects": {
    "PlayerObject": {
      "positionX": 3.0777777
    },
    "BallObject": { //Created
      "positionX": 6.0,
      "isActive": true
    }
  },
  "frameInfo": { 
    "unscaledDeltaTime": 0.0171986,
    "realtimeSinceStartup": 4.075656,
    "timeSinceLevelLoad": 0.6566279 
  }
}
```

3.
```json
{
  "gameObjects": {
    "PlayerObject": {
      "positionX": 6.0777777
    } // Ball no longer exists
  },
  "frameInfo": {
    "unscaledDeltaTime": 0.0175118,
    "realtimeSinceStartup": 4.09290743,
    "timeSinceLevelLoad": 0.6741397
  }
}
```

So in summary, if an object doesn't exist it will be ignored by the exporter.
As a result, it is best practice to attach a capture script to an object directly 
instead of trying to check for existence of it from another object. 

## More

Additional information can be found in the [FAQ.](FAQ.md)