﻿namespace Capture.Internal.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Capture;
    using System.Linq;

    using Newtonsoft.Json;
    using Capture.Internal.Network;

    using Capture.Internal.InputStructure;

    using Capture.Internal.Debug;

    public class HandleCapturing : MonoBehaviour
    {
        public string url { get; set; }
        private string sessionPath = "/api/v1/sessions/";

        public string playerName { get; set; }

        private List<ICapturable> allCapturable;

        public int captureRate { get; set; }
        private int frameCount;

        public bool sendToConsole { get; set; }
        public string id { get; set; }
        public bool isCapturing { get; set; }

        private bool isFirst;

        public bool isVerbose { get; set; }
        public bool isSilent { get; set; }

        private List<string> capturableNames;

        public object extraInfo { get; set; }

        private int openRequests;
        private float averageOpenRequests;
        private MinMax minOpenRequests;
        private MinMax maxOpenRequests;

        private MinMax minResponceTime;
        private float averageResponceTime;
        private MinMax maxResponceTime;
        private int responceCount;

        public SpecificPair[] pairs { get; set; }

        public SecretStorage store { get; set; }

        void Awake()
        {
            // Setting itself to not be destroyed is easier to manage
            // as act more like a property of the object
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            frameCount = 0;

            openRequests = 0;
            minOpenRequests = new MinMax(true);
            averageOpenRequests = 1f;
            maxOpenRequests = new MinMax(false);

            // Start not capturing so the Setup scene isn't captured
            isCapturing = false;
            isFirst = true;
            responceCount = 0;

            // These values mean measurements aren't perfect, but it is good enough
            minResponceTime = new MinMax(true);
            averageResponceTime = 1f;
            maxResponceTime = new MinMax(false);
        }

        void Update()
        {
            collectCaptures();

            printExtraInfo();

            checkCleanup();
        }

        // OnEnable and OnDisable are boilerplate to an OnSceneLoaded event
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

            findCapturable();

            // Dictionaries used for easier manipulation
            Dictionary<string, object> captureData = new Dictionary<string, object>();
            Dictionary<string, object> gameObjects = new Dictionary<string, object>();

            // Loop through and gather captures for every object
            if (allCapturable != null)
            {
                for (int i = 0; i < allCapturable.Count; i++)
                {
                    gameObjects.Add(capturableNames[i], allCapturable[i].GetCapture());
                }
            }

            TimeCapture info = new TimeCapture();

            captureData.Add("frameInfo", info);
            captureData.Add("gameObjects", gameObjects);
            sendCaptures(captureData, gameObjects);
        }

        // sendCaptures overload exits for any data that isn't in-game data
        // Like scene or data about the session
        private void sendCaptures(object data)
        {
            serializeCaptures(data);
        }

        private void sendCaptures(Dictionary<string, object> data, Dictionary<string, object> gameData)
        {
            // Logic to try and find gameObject, key values
            if (pairs.Length != 0)
            {
                Dictionary<string, object> tempData = new Dictionary<string, object>();

                for (int i = 0; i < pairs.Length; i++)
                {
                    string key = pairs[i].key;
                    string value = pairs[i].value;

                    if (!gameData.ContainsKey(key))
                    {
                        continue;
                    }
                    object currentCapture = gameData[name];

                    if (currentCapture.GetType().GetProperty(value) == null)
                    {
                        continue;
                    }

                    // Reflection has to be used here as object type is unknown
                    // But should be safe as it is checked above
                    tempData.Add(value, currentCapture.GetType().GetProperty(value).GetValue(currentCapture, null));
                }

                data = tempData;
            }

            serializeCaptures(data);
        }

        private void serializeCaptures(object data)
        {
            if (sendToConsole && !isSilent)
            {
                if (pairs.Length != 0)
                {
                    Debug.Log(JsonConvert.SerializeObject(data));
                }
                else
                {
                    Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
                }

                return;
            }

            byte[] bson = Serial.toBSON(data);

            openRequests++;
            float start = Time.realtimeSinceStartup;

            string requestPath = url + sessionPath + id + "?bson=true";

            StartCoroutine(HTTPHelpers.post(requestPath, bson, store.accessToken,
                (responceData) =>
                {
                    openRequests--;
                    responceCount++;

                    float responceTime = Time.realtimeSinceStartup - start;
                    averageResponceTime = (averageResponceTime * responceCount + responceTime) / (responceCount + 1);

                    averageOpenRequests = (averageOpenRequests * responceCount + openRequests) / (responceCount + 1);

                    minOpenRequests.Include(openRequests);
                    maxOpenRequests.Include(openRequests);

                    minResponceTime.Include(responceTime);
                    maxResponceTime.Include(responceTime);
                },
                (error) =>
                {
                    openRequests--;
                    Debug.Log(error);
                })
            );
        }

        private void printExtraInfo()
        {
            if (isVerbose && !isSilent)
            {
                string extra = "Extra info about the frame.\n";
                extra += "Session ID: " + id + "\n";

                if (allCapturable != null)
                {
                    extra += "Capturable objects: " + allCapturable.Count + "\n";
                }

                extra += "Open requests: current=" + openRequests;
                extra += " min=" + minOpenRequests.Value;
                extra += " mean=" + averageOpenRequests;
                extra += " max=" + maxOpenRequests.Value + "\n";

                extra += "Request response time: min=" + minResponceTime.Value;
                extra += " mean=" + averageResponceTime;
                extra += " max=" + maxResponceTime.Value + "\n";

                Debug.Log(extra);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "CleanupEC")
            {
                isCapturing = true;
                sendInitialMessage();
                sendSceneLoadMessage(scene);
            }
            else
            {
                isCapturing = false;
                allCapturable = null;

                if (!sendToConsole)
                {
                    Debug.Log("Waiting for all connections to close...");
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
            // Apparently slow, uses FindObjectsOfType, but hasn't been a problem so far
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
                // Safe because found objects are also MonoBenavior type
                MonoBehaviour monoCapture = (MonoBehaviour)c;
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
                if (repeatNames[i])
                {
                    MonoBehaviour monoCapture = monoCaptures[i];
                    // GetInstanceID is guaranteed to be unique
                    // May be different between plays, so be careful
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
                    // Negative numbers used because although there is no meaningful
                    // timestamp associated with this data, it makes exporting easier
                    frameInfo = new TimeCapture(-1f, -1f, -1f),
                    dateTime = DateTime.UtcNow.ToString("o"), // ISO 8601 datetime
                    description = "Session Started",
                    captureRate,
                    extraInfo,
                    special = true,
                    Application.targetFrameRate,
                    playerName,
                };

                sendCaptures(firstInfo);
            }
        }

        private void sendSceneLoadMessage(Scene scene)
        {
            object sceneLoadInfo = new
            {
                frameInfo = new TimeCapture(),
                description = "Scene Loaded",
                sceneName = scene.name,
                special = true,
            };

            sendCaptures(sceneLoadInfo);
        }

        private void checkCleanup()
        {
            if (Input.GetKeyDown(CaptureConfig.GetCleanupKey()))
            {
                SceneManager.LoadScene("CleanupEC");
            }
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
            return HTTPHelpers.delete(url + sessionPath + id, store.accessToken,
                () =>
                {
                    Debug.Log("Finished cleanup, exiting for you.");

                    quit();
                },
                (error) =>
                {
                    Debug.Log(error);
                });
        }

        private void quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    [Serializable]
    internal class TimeCapture
    {
        public float realtimeSinceStartup { get; private set; }
        public float timeSinceLevelLoad { get; private set; }
        public float unscaledDeltaTime { get; private set; }

        public TimeCapture()
        {
            realtimeSinceStartup = Time.realtimeSinceStartup;
            timeSinceLevelLoad = Time.timeSinceLevelLoad;
            unscaledDeltaTime = Time.unscaledDeltaTime;
        }

        public TimeCapture(float rss, float tsll, float udt)
        {
            realtimeSinceStartup = rss;
            timeSinceLevelLoad = tsll;
            unscaledDeltaTime = udt;
        }
    }
}