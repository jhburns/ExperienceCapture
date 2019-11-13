﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Capture;
using System.Linq;

using Newtonsoft.Json;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Network;

public class HandleCapturing : MonoBehaviour
{
    private string url;
    public string sessionPath;

    private string username;

    private List<ICapturable> allCapturable;

    private int captureRate;
    private int frameCount;

    private bool sendToConsole;
    private string id;
    private int openRequests;
    private bool isCapturing;
    private bool findEveryFrame;

    private bool isFirst;

    private bool isVerbose;
    private bool isSilent;

    private List<string> capturableNames;

    private object extraInfo;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        frameCount = 0;
        openRequests = 0;
        isCapturing = false;
        isFirst = true;
    }

    void Update()
    {
        collectCaptures();

        printExtraInfo();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        if (findEveryFrame)
        {
            for (int i = 0; i < allCapturable.Count; i++)
            {
                if (!ReferenceEquals(allCapturable[i], null))
                {
                    allCapturable.RemoveAt(i);
                }
            }

            findCapturable();
        }

        Dictionary<string, object> captureData = new Dictionary<string, object>();
        Dictionary<string, object> gameObjects = new Dictionary<string, object>();

        if (allCapturable != null)
            {
                for (int i = 0; i < allCapturable.Count; i++)
                {
                    gameObjects.Add(capturableNames[i], allCapturable[i].GetCapture());
                }
        }

        object info = new
        {
            Time.unscaledDeltaTime,
            Time.realtimeSinceStartup,
            Time.timeSinceLevelLoad,
        };

        captureData.Add("gameObjects", gameObjects);
        captureData.Add("frameInfo", info);
        sendCaptures(captureData);
    }

    private void sendCaptures(object data)
    {
        if (sendToConsole && !isSilent)
        {
            Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
        }
        

        if (!sendToConsole)
        {
            byte[] bson = Serial.toBSON(data);

            openRequests++;
            StartCoroutine(HTTPHelpers.post(url + sessionPath + id, bson, 
            (responceData) => 
            {
                openRequests--;
            }, 
            (error) =>
            {
                Debug.Log(error);
            })
            );
        }
    }

    private void printExtraInfo()
    {
        if (isVerbose && !isSilent)
        {
            string extra = "Extra info about the frame.\n";
            if (allCapturable != null)
            {
                extra += "Capturable objects: " + allCapturable.Count + "\n";
                extra += "Open requests: " + openRequests + "\n";
            }
            else
            {
                extra += "Still setting up capture.";
                extra += "Session ID: " + id;
            }

            Debug.Log(extra);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Cleanup")
        {
            isCapturing = true;
            findCapturable();
            sendInitialMessage();
            sendSceneLoadMessage(scene);
        }
        else
        {
            isCapturing = false;
            allCapturable = null;

            if (!sendToConsole)
            {
                Debug.Log("Cleaning up...");
                StartCoroutine(sendDelete());
            }
            else
            {
                quit();
            }
        }
    }

    private void findCapturable()
    {
        var capturableQuery = FindObjectsOfType<MonoBehaviour>().OfType<ICapturable>();
        allCapturable = capturableQuery.Cast<ICapturable>().ToList();

        sanitizeNames();
    }

    private void sanitizeNames()
    {
        capturableNames = new List<string>();
        List<MonoBehaviour> monoCaptures = new List<MonoBehaviour>();
        foreach (ICapturable c in allCapturable)
        {
            MonoBehaviour monoCapture = (MonoBehaviour)c; // All found objects are also MonoBenavior type
            capturableNames.Add(monoCapture.name);
            monoCaptures.Add(monoCapture);
        }

        bool[] repeatNames = new bool[capturableNames.Count];
        for (int i = 0; i < capturableNames.Count; i++)
        {
            for (int j = i + 1; j < capturableNames.Count; j++)
            {
                if (!repeatNames[i] && capturableNames[i] == capturableNames[j])
                {
                    repeatNames[i] = true;
                    repeatNames[j] = true;
                }
            }
        }


        for (int i = 0; i < capturableNames.Count; i++) 
        {
            if (repeatNames[i]) {
                MonoBehaviour monoCapture = monoCaptures[i];
                capturableNames[i] = monoCapture.name + "-" + monoCapture.GetInstanceID();
            }
        }
    }

    private void sendInitialMessage()
    {
        if (isFirst)
        {
            isFirst = false;

            object firstInfo = new
            {
                dateTime = DateTime.Now.ToString("yyyy.MM.dd-hh:mm:ss"),
                description = "Session Started",
                extraInfo,
                frameInfo = new
                {
                    unscaledDeltaTime = -1,
                    realtimeSinceStartup = -1,
                    timeSinceLevelLoad = -1,
                },
                username
            };

            sendCaptures(firstInfo);
        }
    }

    private void sendSceneLoadMessage(Scene scene)
    {
        object sceneLoadInfo = new
        {
            description = "Scene Loaded",
            frameInfo = new
            {
                Time.unscaledDeltaTime,
                Time.realtimeSinceStartup,
                Time.timeSinceLevelLoad,
            },
            sceneName = scene.name
        };

        sendCaptures(sceneLoadInfo);
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

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Finished cleanup, exiting");
                quit();
            }
        }
    }

    private void quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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

    public void setVerbose(bool v)
    {
        isVerbose = v;
    }

    public void setSilence(bool s)
    {
        isSilent = s;
    }

    public void setFindEveryFrame(bool f)
    {
        findEveryFrame = f;
    }

    public void setExtraInfo(object e)
    {
        extraInfo = e;
    }
}