using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Saver;
using System.Linq;

using Newtonsoft.Json;

public class Exporter : MonoBehaviour
{
    private string url;
    public string path;

    private List<ISaveable> allSaveable;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        CollectSaves();
    }

    public void setUrl(string u)
    {
        url = u;
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
        Debug.Log("Scene Loaded");
        findSaveable();
    }

    private void CollectSaves()
    {
        foreach (ISaveable a in allSaveable)
        {
            Debug.Log(JsonConvert.SerializeObject(a.getSave()));
        }
    }

    private void findSaveable()
    {
        var saveableQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        allSaveable = saveableQuery.Cast<ISaveable>().ToList();
    }
}
