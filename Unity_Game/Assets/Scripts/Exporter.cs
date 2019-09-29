using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Saver;
using System.Linq;

using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Exporter : MonoBehaviour
{
    private string url;
    public string path;

    public string username;

    private List<ISaveable> allSaveable;

    public int captureRate;
    private int frameCount;

    private bool sendToConsole;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Debug.Log(url);
        frameCount = 0;
    }

    void Update()
    {
        CollectSaves();
    }

    public void setUrl(string u)
    {
        url = u;
    }

    public void setUsername(string n)
    {
        username = n;
    }

    public void setRate(int c) {
        captureRate = c;
    }

    public void setToConsole(bool c)
    {
        sendToConsole = c;
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
        frameCount++;
        frameCount %= captureRate;

        if (frameCount == 0) {
            Debug.Log("Ok");
            foreach (ISaveable a in allSaveable)
            {
                string save = JsonConvert.SerializeObject(a.getSave());
                JObject withTime = JObject.Parse(save);
                float timestamp = Time.timeSinceLevelLoad;
                withTime.Add("timeSinceSceneLoad", timestamp);
                withTime.Add("username", username);
                string json = withTime.ToString();
                sendSaves(json);
            }
        }
    }

    private void sendSaves(string data)
    {
        if (sendToConsole)
        {
            Debug.Log(data);
        }
        else
        {
            StartCoroutine(postSaves(data));
        }
    }

    private IEnumerator postSaves(string data)
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
            //Debug.Log("Sent Save");
        }
    }

    private void findSaveable()
    {
        var saveableQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        allSaveable = saveableQuery.Cast<ISaveable>().ToList();
    }
}
