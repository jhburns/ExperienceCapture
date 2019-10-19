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

2. Add the getCapture() function to satisfy the ICapturable [interface](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface).

```csharp
using Capture;

public class Example : MonoBehaviour, ICapturable
{

	// Class stuff
	
    public object getCapture() {
        return new 
        {
            
        };
    }

}
```

**Check:** There shouldn't be any compile or runtime errors.

## Add properties 

With eveything setup, data to be captured can now be added. The basic format is:

```csharp
    public object getCapture()
    {
        return new
        {
            positionX = transform.position.x
        };
    }
```
This is using an [Anonymous type](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types).
It lets you define implictly (meaning without having to declare `int`, `string`, etc) typed objects simply.

Now run the Setup scene, and you should see a [JSON](https://www.newtonsoft.com/json) object
being printed to console. It should have an info key, and a key called the same name as the
object this script is attached too. 

![Example console](images/console.png)

## How Does It Capture Data?

Experience Capture works by running the 'getCapture()' function on each `ICapturable`
game object over specific intervals. The capture rate can be set as often or little 
as wanted through the prefab, and is based on frame-rate. Additional information 
about the frame is also included automatically, like timestamps. This can be called 
an 'eventless' data capture system, which is designed to be easier to use than
an event based one like Unity Analytics. 

##### Example of console output, although with different names and values. 

Notice how the key only has one property, 'positionX' with some value. 
The way Experience Captures works is by translating the key and value to a JSON string 
for transmission. 

## Common Data Table (WIP)

Here are some ways to extract commonly wanted data from an object:

| Types  | Value |
| ------------- | ------------- |
| if game object is active  | objectName.IsActive()  |
| rotation | transform.eulerAngles.z |