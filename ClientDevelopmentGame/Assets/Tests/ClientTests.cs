using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using Capture.Internal.Client;
using Capture.Internal.InputStructure;

public class ClientTests
{

    [Test]
    public void ClientTestsSimplePasses()
    {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator ClientTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
