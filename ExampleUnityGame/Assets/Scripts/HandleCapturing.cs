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
    private int openRequests;
    private bool isCapturing;

    //private bool isFirst;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        frameCount = 0;
        openRequests = 0;
        setCapturability(true);
        //isFirst = true;
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
        if (scene.name == "Cleanup")
        {
            setCapturability(false);
            allCapturable = null;

            if (!sendToConsole)
            {
                Debug.Log("Cleaning up...");
                StartCoroutine(sendDelete());
            }
        }
        else
        {
            setCapturability(true);
            findCapturable();
            sendInitialMessage();
        }
    }

    private void collectCaptures()
    {
        if (!isCapturing)
        {
            return;
        }

        frameCount++;
        frameCount %= captureRate;

        if (frameCount != 0)
        {
            return;
        }

        Dictionary<string, object> captureCollection = new Dictionary<string, object>();

        foreach (ICapturable c in allCapturable)
        {
            MonoBehaviour monoCapture = (MonoBehaviour) c; // Needs to fixed and become a safe conversion
            captureCollection.Add(monoCapture.name, c.getCapture());
        }

        object info = new
        {
            timestamp = Time.timeSinceLevelLoad,
        };

        captureCollection.Add("info", info);
        sendCaptures(JsonConvert.SerializeObject(captureCollection));
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

            openRequests++;
            yield return request.SendWebRequest();
            openRequests--;

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

    private void sendInitialMessage()
    {
        //isFirst = false;

        object firstInfo = new
        {
            user = username,
            taken = System.DateTime.Now.ToString("yyyy.MM.dd-hh:mm:ss"),
            info = new {
                timestamp = -1,

            }
        };

        sendCaptures(JsonConvert.SerializeObject(firstInfo));
    }

    private IEnumerator sendDelete()
    {
        if (openRequests > 0)
        {
            yield return null;
        }

        StartCoroutine(deleteSession());
    }

    private IEnumerator deleteSession()
    {
        using (UnityWebRequest request = UnityWebRequest.Delete(url + sessionPath + id))
        {
            request.method = UnityWebRequest.kHttpVerbDELETE;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Finished cleanup, exiting");
            }
        }
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

    public void setCapturability(bool c)
    {
        isCapturing = c;
    }
}
