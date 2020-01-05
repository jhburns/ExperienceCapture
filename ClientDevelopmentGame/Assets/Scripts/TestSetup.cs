using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/*
 * Quick and dirty way to setup testing objects.
 */
public class TestSetup : MonoBehaviour {

    private string placeholder;

    private string filler;

    public int initialSpawn;
    public int initialFillerLength;

    public Text countDisplay;
    public InputField inputLength;
    public Toggle nullProperty;
    public Toggle nullValue;

    private List<Tester> testers;
    public Tester t;

    void Awake()
    {
        placeholder = "all work and no play makes jack a dull boy";
        inputLength.text = initialFillerLength.ToString();
        calculateFiller(initialFillerLength);

        testers = new List<Tester>();

        foreach (int i in Enumerable.Range(0, initialSpawn))
        {
            createTester("initial");
        }
    }

    private void calculateFiller(int fillerLength)
    {
        string repeated = "";
        for (int i = 0; i < (fillerLength / placeholder.Length) + 1; i++)
        {
            repeated += placeholder + " ";
        }

        filler = repeated.Substring(0, fillerLength);
    }

    public void onAdd()
    {
        createTester("byUser");
    }

    public void onRemove()
    {
        destroyTester();
    }

    public void onUpdateFiller()
    {
        string userInput = inputLength.text;
        int userInt = -1;
        int.TryParse(userInput, out userInt);

        if (userInt < 0)
        {
            throw new Exception("Filler length should be a positive integer.");
        }

        calculateFiller(userInt);
        foreach (Tester t in testers)
        {
            t.setText(filler);
        }
    }

    public void onNullValue()
    {
        foreach (Tester t in testers)
        {
            t.isNullValue = nullValue.isOn;
        }
    }

    public void onNullProperty()
    {
        foreach (Tester t in testers)
        {
            t.isNullProperty = nullProperty.isOn;
        }
    }

    private void createTester(string type)
    {
        Tester tester = Instantiate(t);
        testers.Add(tester);
        tester.setText(filler);
        tester.name = tester.name + "-" + type + "-" + (testers.Count - 1);
        countDisplay.text = testers.Count.ToString();
    }

    private void destroyTester()
    {
        if (testers.Count > 0)
        {
            GameObject t = testers[testers.Count - 1].gameObject;
            DestroyImmediate(t);
            t = null;
            testers.RemoveAt(testers.Count - 1);
            countDisplay.text = testers.Count.ToString();
        }
    }
}
