using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Capture;
using System.Linq;

using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class HandleCapturing : MonoBehaviour
{
    private string url;
    public string sessionPath;

    public string username;

    private List<ICapturable> allCapturable;

    public int captureRate;
    private int frameCount;

    private bool sendToConsole;

    private string id;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        frameCount = 0;
    }

    void Update()
    {
        collectCaptures();
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
        findCapturable();
    }

    private void collectCaptures()
    {
        frameCount++;
        frameCount %= captureRate;

        if (frameCount == 0) {
            foreach (ICapturable cap in allCapturable)
            {
                string capture = JsonConvert.SerializeObject(cap.getCapture());
                JObject withTime = JObject.Parse(capture);
                float timestamp = Time.timeSinceLevelLoad;
                withTime.Add("timeSinceSceneLoad", timestamp);
                withTime.Add("username", username);
                string json = withTime.ToString();
                sendCaptures(json);
            }
        }
    }

    private void sendCaptures(string data)
    {
        if (sendToConsole)
        {
            Debug.Log(data);
        }
        else
        {
            StartCoroutine(postCaptures(data));
        }
    }

    private IEnumerator postCaptures(string data)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(url + sessionPath + id, data))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                //Debug.Log("Posted capture to server");
            }
        }
    }

    private void findCapturable()
    {
        var capturableQuery = FindObjectsOfType<MonoBehaviour>().OfType<ICapturable>();
        allCapturable = capturableQuery.Cast<ICapturable>().ToList();
    }

    public void setUrl(string u)
    {
        url = u;
    }

    public void setID(string i)
    {
        id = i;
    }

    public void setUsername(string n)
    {
        username = n;
    }

    public void setRate(int c)
    {
        captureRate = c;
    }

    public void setToConsole(bool c)
    {
        sendToConsole = c;
    }
}
