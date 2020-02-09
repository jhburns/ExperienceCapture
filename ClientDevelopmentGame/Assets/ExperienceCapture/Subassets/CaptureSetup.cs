using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

using UnityEngine.UI;

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
    public const string clientVersionLocked = "1.1.4";
    [Tooltip("Don't edit, is readonly and only informational.")]
    public string clientVersion;

    [Tooltip("Url to fill in automatically on the login page. Examples: 'http://192.168.99.100:3003', 'https://expcap.xyz'")]
    public string defaultUrl;

    [Tooltip("If checked, print data to console and don't attempt to connect to a server.")]
    public bool offlineMode;

    [Tooltip("Extra debugging data.")]
    public bool printAdditionalCaptureInfo;
    [Tooltip("Handles game objects being instantiated and destroyed.")]
    public bool findObjectsInEachFrame;
    [Tooltip("Still capture data, but don't print it.")]
    public bool doNotPrintToConsole;

    [Tooltip("Prevents Exceptions when Specified game objects/keys aren't found. Useful when dynamically created objects.")]
    public bool doNotThrowNotFound;
    public string[] limitOutputToSpecified;

    public HandleCapturing handler;

    public Text nameTitle;
    public InputField nameInput;

    public Text urlTitle;
    public InputField urlInput;
    public Text openingInfo;
    public Text connectionInfo;

    public Text sessionInfo;
    private string sessionInfoSave;
    public Image sessionBackground;
    public Button newSession;
    public Button start;
    public GameObject eventSystem;

    private string sessionID;
    private string url;
    private SecretStorage store;

    private void Start()
    {
        setupDefaults();
    }

    private void setupDefaults()
    {
        // In case an event system doesn't exist in client
        if (GameObject.Find("EventSystem") == null)
        {
            Instantiate(eventSystem);
        }

        if (offlineMode)
        {
            newSession.gameObject.SetActive(false);
            urlTitle.gameObject.SetActive(false);
            urlInput.gameObject.SetActive(false);
        }
        else
        {
            nameTitle.gameObject.SetActive(false);
            nameInput.gameObject.SetActive(false);
            start.gameObject.SetActive(false);
        }

        urlInput.text = defaultUrl;

        // Just too coincidental 
        nameInput.text = "Boyd";

        sessionInfo.gameObject.SetActive(false);
        sessionInfoSave = sessionInfo.text;
        sessionBackground.gameObject.SetActive(false);

        openingInfo.gameObject.SetActive(false);
        connectionInfo.gameObject.SetActive(false);

        // Function wrappers since otherwise they would be trigged immediately
        newSession.onClick.AddListener(delegate () { onLoginClick(); });
        start.onClick.AddListener(delegate () { onStartClick(); });

        clientVersion = clientVersionLocked;
    }

    private void onLoginClick()
    {
        urlTitle.gameObject.SetActive(false);
        urlInput.gameObject.SetActive(false);
        newSession.gameObject.SetActive(false);

        connectionInfo.gameObject.SetActive(true);

        // Content of the body is ignored
        string emptyBody = new {}.ToString();

        StartCoroutine(HTTPHelpers.post(urlInput.text + "/api/v1/users/claims/", emptyBody,
            (responce) => {
                openingInfo.gameObject.SetActive(true);
                connectionInfo.gameObject.SetActive(false);

                string claimSanitized = UnityWebRequest.EscapeURL(responce);
                string url = urlInput.text + "/signInFor?claimToken=" + claimSanitized;

                Application.OpenURL(url);

                pollClaim(responce);
            }, (error) => {
                Debug.Log(error);

                sessionInfo.text = error;

                connectionInfo.gameObject.SetActive(false);

                sessionInfo.gameObject.SetActive(true);
                sessionBackground.gameObject.SetActive(true);

                urlTitle.gameObject.SetActive(true);
                urlInput.gameObject.SetActive(true);

                newSession.gameObject.SetActive(true);
            })
        );
    }

    private void pollClaim(string claimToken)
    {
        StartCoroutine(HTTPHelpers.pollGet(urlInput.text + "/api/v1/users/claims/", claimToken, 
            (responce) => {
                store = new SecretStorage(responce);
                createSession();
            }, (error) => {
                Debug.Log(error);
            })
        );
    }

    private void createSession()
    {
        byte[] emptyBody = Serial.toBSON(new {});

        StartCoroutine(HTTPHelpers.post(urlInput.text + "/api/v1/sessions?bson=true", emptyBody, store.accessToken,
            (data) => {
                sessionInfo.gameObject.SetActive(true);
                sessionBackground.gameObject.SetActive(true);
                openingInfo.gameObject.SetActive(false);

                nameTitle.gameObject.SetActive(true);
                nameInput.gameObject.SetActive(true);

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
            }, (error) => {
                sessionInfo.text = error;

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

    // All of this is since Unity only supports 1D arrays in the editor
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

    private void createHandler(string url, string id, string playerName, InputStructure.SpecificPair[] pairs)
    {
        HandleCapturing newHandler = Instantiate(handler);

        newHandler.url = url;
        newHandler.playerName = playerName;
        newHandler.id = id;

        newHandler.captureRate = captureRate;
        newHandler.sendToConsole = offlineMode;
        newHandler.isCapturing = false;
        newHandler.isFindingOften = findObjectsInEachFrame;

        newHandler.isVerbose = printAdditionalCaptureInfo;
        newHandler.isSilent = doNotPrintToConsole;

        newHandler.store = store;

        // extraInfo can be used to capture arbitrary data
        newHandler.extraInfo = new
        {
            clientVersion = clientVersionLocked,
            gameVersion,
        };

        newHandler.isIgnoringNotFound = doNotThrowNotFound;
        newHandler.pairs = pairs;

        SceneManager.LoadScene(sceneToLoad);
    }

}

// Minimally validated,
// More data is returned from the server but is never used
internal class SessionData
{
    public string id;

    public SessionData(string i, bool o)
    {
        id = i;
    }
}