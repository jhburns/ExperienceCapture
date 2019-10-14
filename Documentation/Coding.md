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

**Check: ** There shouldn't be any compile or runtime errors.

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

Notice how the key only has one property, 'positionX' with some value. 
The way Expereince Captures works is by translating the key and value to a JSON string 
for transmition. 

## Common Data Table

Here are some ways to extract commonly wanted data from an object:

| Types  | |Value |
| ------------- | ------------- |
| If Game Object is active  | objectName.IsActive()  |
| rotation | transform.eulerAngles.z |