using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

using Network;

using InputStructure;

public class CaptureSetup : MonoBehaviour
{
    [Tooltip("How faster data is collect, in frames. For example 60 = every 1 second @60fps.")]
    public int captureRate;

    [Tooltip("First scene to load.")]
    public string sceneToLoad;

    [Tooltip("Label the game version before releasing.")]
    public string gameVersion;
    public const string clientVersionLocked = "1.1.2";
    [Tooltip("Don't edit, is readonly and only informational.")]
    public string clientVersion;

    [Tooltip("If checked, print data to console and don't attempt to connect to a server.")]
    public bool offlineMode;

    [Tooltip("Extra debugging data.")]
    public bool printAdditionalCaptureInfo;
    [Tooltip("Handles game objects being instantiated and destroyed.")]
    public bool findObjectsInEachFrame;
    [Tooltip("Still capture data, but don't print it.")]
    public bool doNotPrintToConsole;

    [Tooltip("Uses Windows docker default host IP, instead of localhost.")]
    public bool useWindowsDefault;

    [Tooltip("Prevents Exceptions when Specified game objects/keys aren't found. Useful when dynamically created objects.")]
    public bool doNotThrowNotFound;
    public string[] limitOutputToSpecified;

    public HandleCapturing handler;

    public InputField nameInput;

    public Text urlTitle;
    public InputField urlInput;

    public Text sessionInfo;
    private string sessionInfoSave;
    public Image sessionBackground;
    public Button newSession;
    public Button start;
    public GameObject eventSystem;

    private string sessionID;
    private string url;

    private void Start()
    {
        setupDefaults();
    }

    private void setupDefaults()
    {
        // In case it doesn't exist in client
        if (GameObject.Find("EventSystem") == null)
        {
            Instantiate(eventSystem);
        }

        if (offlineMode)
        {
            urlTitle.gameObject.SetActive(false);
            urlInput.gameObject.SetActive(false);
            newSession.gameObject.SetActive(false);
        }
        else
        {
            start.gameObject.SetActive(false);
        }

        if (useWindowsDefault)
        {
            urlInput.text = "http://192.168.99.100:4321/";
        }
        else
        {
            urlInput.text = "http://0.0.0.0:4321/";
        }

        nameInput.text = "Boyd";

        sessionInfo.gameObject.SetActive(false);
        sessionInfoSave = sessionInfo.text;
        sessionBackground.gameObject.SetActive(false);

        newSession.onClick.AddListener(delegate () { onNewSessionClick(); });

        start.onClick.AddListener(delegate () { onStartClick(); });

        clientVersion = clientVersionLocked;
    }

    private void onNewSessionClick()
    {
        newSession.gameObject.SetActive(false);

        byte[] bson = Serial.toBSON(new
        {
            create = 1
        });

        StartCoroutine(HTTPHelpers.post(urlInput.text + "sessions/", bson, (data) =>
        {
            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);

            try
            {
                MemoryStream memStream = new MemoryStream(data);
                SessionData responce = Serial.fromBSON<SessionData>(memStream);

                sessionInfo.text = sessionInfoSave + responce.id;
                url = urlInput.text;
                sessionID = responce.id;

                start.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                sessionInfo.text = "Error deserializing JSON response: " + e;
                Debug.Log(e);
                newSession.gameObject.SetActive(true);
            }
        }, 
        (error) =>
        {
            sessionInfo.text = "Error creating new session: " + error;

            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);
            newSession.gameObject.SetActive(true);

            Debug.Log(error);
        })
        );
    }

    private void onStartClick()
    {
        InputStructure.SpecificPair[] pairs;
        try 
        {
            pairs = parseSpecific();
        } 
        catch (Exception e) 
        {
            Debug.Log(e);

            sessionInfo.text = e.ToString();
            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);
            start.gameObject.SetActive(true);

            return;
        }   

        if (offlineMode)
        {
            createHandler("no URL", "NoSessionID", nameInput.text, pairs);
        }
        else
        {
            createHandler(url, sessionID, nameInput.text, pairs);
        }
    }

    private InputStructure.SpecificPair[] parseSpecific() {
        InputStructure.SpecificPair[] tempPairs = new InputStructure.SpecificPair[limitOutputToSpecified.Length];

        for (int i = 0; i < limitOutputToSpecified.Length; i++)
        {
            string pairInput = limitOutputToSpecified[i];
            string[] pairSplit = pairInput.Split(':');

            if (pairSplit.Length != 2) {
                throw new InputStructure.SpecificPairsParsingException("Takes colon separated pair", pairInput);
            }

            if (pairSplit[0] == String.Empty) {
                throw new InputStructure.SpecificPairsParsingException("GameObject must have name", pairInput);
            }

            if (pairSplit[1] == String.Empty) {
                throw new InputStructure.SpecificPairsParsingException("Key must have name", pairInput);
            }
            
            tempPairs[i] = new InputStructure.SpecificPair(pairSplit[0], pairSplit[1]);
        }

        return tempPairs;
    }

    private void createHandler(string url, string id, string username, InputStructure.SpecificPair[] pairs)
    {
        HandleCapturing newHandler = Instantiate(handler);

        newHandler.url = url;
        newHandler.username = username;
        newHandler.id = id;

        newHandler.captureRate = captureRate;
        newHandler.sendToConsole = offlineMode;
        newHandler.isCapturing = false;
        newHandler.isFindingOften = findObjectsInEachFrame;

        newHandler.isVerbose = printAdditionalCaptureInfo;
        newHandler.isSilent = doNotPrintToConsole;

        newHandler.extraInfo = new
        {
            clientVersion = clientVersionLocked,
            gameVersion = gameVersion,
        };

        newHandler.isIgnoringNotFound = doNotThrowNotFound;
        newHandler.pairs = pairs;

        SceneManager.LoadScene(sceneToLoad);
    }

}

internal class SessionData
{
    public string id;
    public bool isOpen;

    public SessionData(string i, bool o)
    {
        id = i;
        isOpen = o;
    }
}