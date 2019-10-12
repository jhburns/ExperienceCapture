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

    public int initialSpawn;
    public int scaleUp;
    public int varience;
    private List<Tester> testers;
    public Tester t;

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

    void Update()
    {
        if (scaleUp > 0)
        {
            createTesters(1, "scale");

            scaleUp--;
        }
        else if (varience > 0)
        {
            int change = UnityEngine.Random.Range(-varience + 1, varience);
            Debug.Log(change);
            
            if (change > 0)
            {
                createTesters(change, "variant");
            }
            else if (change < 0)
            {
                for (int i = testers.Count + change; i < testers.Count; i++)
                {
                    Destroy(testers[i]);
                    testers.RemoveAt(i);
                }
            }
        }
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
        testers = FindTesters();

        if (testers.Count > 0)
        {
            foreach (Tester t in testers)
            {
                t.setText(filler);
            }
        }

        createTesters(initialSpawn, "init");
    }

    private List<Tester> FindTesters()
    {
        var testers = FindObjectsOfType<Tester>();
        return testers.Cast<Tester>().ToList();
    }

    private void createTesters(int count, string type)
    {
        for (int i = 0; i < count; i++)
        {
            Tester test = Instantiate(t);
            testers.Add(test);
            test.setText(filler);
            test.name = test.name + "-" + type + "-" + (testers.Count - 1);
        }
    }
}
