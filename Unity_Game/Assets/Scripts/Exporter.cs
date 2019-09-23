using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Saver;
using System.Linq;

using Newtonsoft.Json;
using UnityEngine.Networking;

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
            string save = JsonConvert.SerializeObject(a.getSave());
            StartCoroutine(sendSave(save));
        }
    }

    private IEnumerator sendSave(string data)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(data));

        UnityWebRequest www = UnityWebRequest.Post(url + path, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Sent Save");
        }
    }

    private void findSaveable()
    {
        var saveableQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        allSaveable = saveableQuery.Cast<ISaveable>().ToList();
    }
}
