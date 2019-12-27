# Coding

This section will cover how to add data capture logic to c# scripts in your scenes.

## Setup Script

1. Add the following namespace:

```csharp
using Capture;
```

1. Have the class extend the `ICapturable` interface in addition to MonoBehavior.

```csharp
using Capture;

public class Example : MonoBehaviour, ICapturable
{
	// Class stuff
}
```

2. Add the GetCapture() function to satisfy the ICapturable [interface](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface).

```csharp
using Capture;

public class Example : MonoBehaviour, ICapturable
{

    // Class stuff
	
    public object GetCapture() 
    {
        return new 
        {
            
        };
    }

}
```

**Check:** There shouldn't be any compile or runtime errors.

## Add properties 

With everything setup, data to be captured can now be added. The basic format is:

```csharp
    public object GetCapture()
    {
        return new
        {
            positionX = transform.position.x, // <- separate properties with a comma 
            propertyName = gameObject.value
        };
    }
```
This is using an [Anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types).
It lets you define implicitly (meaning without having to declare `int`, `string`, etc) typed objects simply.

Now run the Setup scene, and you should see a [JSON](https://www.newtonsoft.com/json) object
being printed to console. It should have an info key, and a key called the same name as the
object this script is attached too.

![Example console](images/console.png)

##### Example of console output, although with different names and values. 

Notice how the key only has one property, 'positionX' with some value. 
The way Experience Captures works is by translating the key and value to a JSON string 
for transmission. 

## How Does It Capture Data?

Experience Capture works by running the 'GetCapture()' function on each `ICapturable`
game object over specific intervals. The capture rate can be set as often or little 
as wanted through the prefab, and is based on frame-rate. Additional information 
about the frame is also included automatically, like timestamps. This can be called 
an 'eventless' data capture system, which is designed to be easier to use than
an event based one like Unity Analytics. 

## Common Data Examples

Here is how to capture various common properties.

#### If a GameObject is active

```csharp
    public object GetCapture()
    {
        return new
        {
            objectIsActive = objectName.IsActive()
        };
    }
```

#### Rotation


```csharp
    public object GetCapture()
    {
        return new
        {
            rotation = transform.eulerAngles.z
        };
    }
```

#### Any Vector3

Vector3s, like position in this example can't be directly serialized to JSON.
Instead, use a nested anonymous typed object to store each value clearly like below.  

```csharp
    public object GetCapture()
    {
        return new
        {
            position = new
            {
                transform.position.x,
                transform.position.y,
                transform.position.z,
            }
        };
    }
```

## Next Part

If all the checks are fine, everything should be setup. Next Section: [How Your Data is Captured.](AboutCapture.md)