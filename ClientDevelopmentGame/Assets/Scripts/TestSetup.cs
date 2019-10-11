using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

/*
 * Quick and dirty way to setup testing objects.
 */
public class TestSetup : MonoBehaviour {

    private string placeholder;

    public int fillerLength;
    private string filler;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        placeholder = "all work and no play makes jack a dull boy";

        string repeated = "";
        for (int i = 0; i < (fillerLength / placeholder.Length) + 1; i++)
        {
            repeated += placeholder + " ";
        }

        filler = repeated.Substring(0, fillerLength);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<Tester> testers = FindTesters();

        if (testers.Count > 0)
        {
            foreach (Tester t in testers)
            {
                t.setText(filler);
            }
        }
    }

    private List<Tester> FindTesters()
    {
        var testers = FindObjectsOfType<Tester>();
        return testers.Cast<Tester>().ToList();
    }
}
