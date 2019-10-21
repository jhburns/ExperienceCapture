using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.UI;

using Newtonsoft.Json;

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
        sessionBackground.gameObject.SetActive(false);

        newSession.onClick.AddListener(delegate () { onNewSessionClick(); });

        start.onClick.AddListener(delegate () { onStartClick(); });

        clientVersion = clientVersionLocked;
    }

    private void onNewSessionClick()
    {
        newSession.gameObject.SetActive(false);


        StartCoroutine(get(urlInput.text + "session/", (data) =>
        {
            sessionInfo.gameObject.SetActive(true);
            sessionBackground.gameObject.SetActive(true);

            try
            {
                SessionData responce = JsonConvert.DeserializeObject<SessionData>(data);

                if (responce.status != "OK")
                {
                    sessionInfo.text = "Error getting new session: " + responce.status;
                }
                else
                {
                    sessionInfo.text = sessionInfo.text + responce.id;
                    url = urlInput.text;
                    sessionID = responce.id;

                    start.gameObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                sessionInfo.text = "Error with deserializing, from JSON response.";
                Debug.Log(e);
            }
        })
);
    }

    private IEnumerator get(string uri, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);

                sessionInfo.gameObject.SetActive(true);
                sessionBackground.gameObject.SetActive(true);
                sessionInfo.text = "Error with networking: " + webRequest.error;
            }
            else
            {
                callback(webRequest.downloadHandler.text);
            }
        }
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