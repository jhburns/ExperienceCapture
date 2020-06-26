using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using Capture.Internal.Client;
using Capture.Internal.InputStructure;

public class ClientTests
{
    [Test]
    public void ParsesPairsCorrectly()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            "left:right"
        };

        var result = setupComponent.parseSpecific();

        Assert.AreEqual(result.Length, 1, "Parsing pairs does not have the same length");
        Assert.AreEqual(result[0].key, "left", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[0].value, "right", "Parsing pairs does not have the correct right value");
    }

    [Test]
    public void MultiplePairsParseCorrectly()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            "1:2",
            "3:4",
            "5:6",
            "7:8",
            "9:10",
            "11:12",
        };

        var result = setupComponent.parseSpecific();

        Assert.AreEqual(result.Length, 6, "Parsing pairs does not have the same length");

        Assert.AreEqual(result[0].key, "1", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[0].value, "2", "Parsing pairs does not have the correct right value");

        Assert.AreEqual(result[1].key, "3", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[1].value, "4", "Parsing pairs does not have the correct right value");

        Assert.AreEqual(result[2].key, "5", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[2].value, "6", "Parsing pairs does not have the correct right value");

        Assert.AreEqual(result[3].key, "7", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[3].value, "8", "Parsing pairs does not have the correct right value");

        Assert.AreEqual(result[4].key, "9", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[4].value, "10", "Parsing pairs does not have the correct right value");

        Assert.AreEqual(result[5].key, "11", "Parsing pairs does not have the correct left value");
        Assert.AreEqual(result[5].value, "12", "Parsing pairs does not have the correct right value");
    }

    [Test]
    public void NoColonThrowsWhenParsingPairs()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            "leftright"
        };

        Assert.Throws<SpecificPairsParsingException>(
              delegate () { setupComponent.parseSpecific(); });
    }

    [Test]
    public void NoNameThrowsWhenParsingPairs()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            "left:"
        };

        Assert.Throws<SpecificPairsParsingException>(
              delegate () { setupComponent.parseSpecific(); });
    }

    [Test]
    public void NoKeyThrowsWhenParsingPairs()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            ":right"
        };

        Assert.Throws<SpecificPairsParsingException>(
              delegate () { setupComponent.parseSpecific(); });
    }

    [Test]
    public void MoreThanOneColonThrowsWhenParsingPairs()
    {
        var setup = new GameObject("Setup");
        setup.AddComponent<CaptureSetup>();

        var setupComponent = setup.GetComponent<CaptureSetup>();
        setupComponent.limitOutputToSpecified = new string[]
        {
            "left::right"
        };

        Assert.Throws<SpecificPairsParsingException>(
              delegate () { setupComponent.parseSpecific(); });

        setupComponent.limitOutputToSpecified = new string[]
        {
            "left:::::::::::::::::::::::::right"
        };

        Assert.Throws<SpecificPairsParsingException>(
              delegate () { setupComponent.parseSpecific(); });
    }
}
