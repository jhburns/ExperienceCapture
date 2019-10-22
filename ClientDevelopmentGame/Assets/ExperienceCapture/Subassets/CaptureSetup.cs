using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.UI;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

using Network;

public class CaptureSetup : MonoBehaviour
{
    [Tooltip("How faster data is collect, in frames. For example 60 = every 1 second @60fps.")]
    public int captureRate;

    [Tooltip("First scene to load.")]
    public string sceneToLoad;

    [Tooltip("Label the game version before releasing.")]
    public string gameVersion;
    public const string clientVersionLocked = "1.1.0";
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

    public HandleCapturing handler;

    public InputField nameInput;

    public Text urlTitle;
    public InputField urlInput;

    public Text sessionInfo;
    private string sessionInfoSave;
    public Image sessionBackground;
    public Button newSession;
    public Button start;

    private string sessionID;
    private string url;

    private void Start()
    {
        setupDefaults();
    }

    private void setupDefaults()
    {
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

        byte[] bson = Serializer.toBSON(new
        {
            create = 1
        });

        StartCoroutine(HTTPHelpers.post(urlInput.text + "sessions/", bson, (data) =>
        {
            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);

            try
            {
                SessionData responce = JsonConvert.DeserializeObject<SessionData>(data);

                if (responce.status != "OK")
                {
                    sessionInfo.text = "Error creating new session: " + responce.status;
                    newSession.gameObject.SetActive(true);
                }
                else
                {
                    sessionInfo.text = sessionInfoSave + responce.id;
                    url = urlInput.text;
                    sessionID = responce.id;

                    start.gameObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                sessionInfo.text = "Error deserializing JSON response.";
                Debug.Log(e);
                newSession.gameObject.SetActive(true);
            }
        }, 
        (error) =>
        {
            newSession.gameObject.SetActive(true);

            sessionInfo.text = "Error creating new session: " + error;
            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);

            Debug.Log(error);
        })
        );
    }

    private void onStartClick()
    {
        if (offlineMode)
        {
            createHandler("no URL", "NoSessionID", nameInput.text);
        }
        else
        {
            createHandler(url, sessionID, nameInput.text);
        }
    }

    private void createHandler(string url, string id, string username)
    {
        HandleCapturing newHandler = Instantiate(handler);

        newHandler.setUrl(url);
        newHandler.setUsername(username);
        newHandler.setID(id);

        newHandler.setRate(captureRate);
        newHandler.setToConsole(offlineMode);
        newHandler.setCapturability(false);
        newHandler.setFindEveryFrame(findObjectsInEachFrame);

        newHandler.setVerbose(printAdditionalCaptureInfo);
        newHandler.setSilence(doNotPrintToConsole);

        newHandler.setExtraInfo(new
        {
            gameVersion = gameVersion,
            clientVersion = clientVersionLocked
        });

        SceneManager.LoadScene(sceneToLoad);
    }

}

internal class SessionData
{
    public string id;
    public string status;

    public SessionData(string i, string stat)
    {
        id = i;
        status = stat;
    }
}