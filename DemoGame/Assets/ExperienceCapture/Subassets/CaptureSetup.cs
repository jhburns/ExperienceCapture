﻿namespace Capture.Internal.Client
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.SceneManagement;
    using UnityEngine.Networking;
    using System;

    using UnityEngine.UI;

    using Capture.Internal.Network;

    public class CaptureSetup : MonoBehaviour
    {
        [Tooltip("How faster data is collect, in frames. For example 60 = every 1 second @60fps. Should be greater than 0.")]
        public int captureRate;

        [Tooltip("First scene to load. Required.")]
        public string sceneToLoad;

        [Tooltip("Label the game version before releasing.")]
        public string gameVersion;
        public const string clientVersionLocked = "1.3.0";
        [Tooltip("Don't edit, is read-only and only informational.")]
        public string clientVersion;

        [Tooltip("URL to default to on the login page. Example: 'https://expcap.xyz'")]
        public string defaultUrl;

        [Tooltip("Only capture certain properties. Increase the size and add an entry like: [GameObject]:[Property]. Disabled outside the Editor.")]
        public string[] limitOutputToSpecified;

        [Tooltip("If checked, print data to console and don't attempt to connect to a server. Disabled in builds.")]
        public bool offlineMode;

        [Tooltip("Extra debugging data.")]
        public bool printDebuggingData;

        [Tooltip("Still capture data, but don't print it to the console.")]
        public bool doNotPrintCaptures;

        public HandleCapturing handler;

        public Text nameTitle;
        public InputField nameInput;
        public Text dataInfo;

        public Text urlTitle;
        public InputField urlInput;
        public Text openingInfo;
        public Text connectionInfo;
        public Image loadingCircle;
        public Text warningInfo;

        public Text sessionInfo;
        private string sessionInfoSave;
        public Image sessionBackground;
        public Button newSession;
        public Button start;

        private string sessionID;
        private string url;
        private SecretStorage store;

        public Image errorModal;
        public Text errorBody;

        private void Start()
        {
            // Don't allow the game to run in offline mode in production
            #if !UNITY_EDITOR
                offlineMode = false;
            #endif

            setup();

            validateArguments();
        }

        private void setup()
        {
            newSession.gameObject.SetActive(!offlineMode);
            urlTitle.gameObject.SetActive(!offlineMode);
            urlInput.gameObject.SetActive(!offlineMode);
            warningInfo.gameObject.SetActive(!offlineMode);

            nameTitle.gameObject.SetActive(offlineMode);
            nameInput.gameObject.SetActive(offlineMode);
            start.gameObject.SetActive(offlineMode);
            dataInfo.gameObject.SetActive(offlineMode);
            sessionInfo.gameObject.SetActive(offlineMode);
            sessionBackground.gameObject.SetActive(offlineMode);

            if (offlineMode)
            {
                sessionInfo.text = "Running offline.";
            }

            urlInput.text = defaultUrl;

            // Save the text of session so it can be appended
            // In front of each session id
            sessionInfoSave = sessionInfo.text;
            openingInfo.gameObject.SetActive(false);
            connectionInfo.gameObject.SetActive(false);

            // Function wrappers since otherwise they would be trigged immediately
            newSession.onClick.AddListener(delegate () { onLoginClick(); });
            start.onClick.AddListener(delegate () { onStartClick(); });

            clientVersion = clientVersionLocked;
        }

        private void validateArguments()
        {
            if (captureRate <= 0)
            {
                throw new ArgumentException("Capture Rate needs to be a positive, non-zero number.");
            }

            if (sceneToLoad == string.Empty)
            {
                throw new ArgumentException("Scene to load needs to not empty.");
            }
        }

        private void onLoginClick()
        {
            urlTitle.gameObject.SetActive(false);
            urlInput.gameObject.SetActive(false);
            warningInfo.gameObject.SetActive(false);

            newSession.gameObject.SetActive(false);

            connectionInfo.gameObject.SetActive(true);
            loadingCircle.gameObject.SetActive(true);

            // Content of the body is ignored
            byte[] emptyBody = Serial.toBSON(new { });
            url = urlInput.text.Trim('/');

            StartCoroutine(HTTPHelpers.post(url + "/api/v1/authentication/claims?bson=true",
                emptyBody,
                string.Empty,
                (data) =>
                {
                    openingInfo.gameObject.SetActive(true);
                    connectionInfo.gameObject.SetActive(false);

                    try
                    {
                        ClaimData responce = Serial.fromBSON<ClaimData>(data);

                        StartCoroutine(WaitThenOpen(responce));
                    }
                    catch (Exception e)
                    {
                        sessionInfo.text = "Error deserializing BSON response: " + e;
                        Debug.Log(e);
                        newSession.gameObject.SetActive(true);
                    }
                }, (error) =>
                {
                    showError(error);
                })
            );
        }

        private IEnumerator WaitThenOpen(ClaimData responce)
        {
            yield return new WaitForSeconds(0.5f);

            string claimSanitized = UnityWebRequest.EscapeURL(responce.claimToken);
            string openUrl = url + "/externalSignIn?claimToken=" + claimSanitized;

            Application.OpenURL(openUrl);

            pollClaim(responce.claimToken);
        }

        private void pollClaim(string claimToken)
        {
            StartCoroutine(HTTPHelpers.pollGet(url + "/api/v1/authentication/claims?bson=true", claimToken,
                (data) =>
                {
                    try
                    {
                        AccessData responce = Serial.fromBSON<AccessData>(data);

                        store = new SecretStorage(responce.accessToken);
                        createSession();
                    }
                    catch (Exception e)
                    {
                        sessionInfo.text = "Error deserializing BSON response: " + e;
                        Debug.Log(e);
                        newSession.gameObject.SetActive(true);
                    }
                }, (error) =>
                {
                    showError(error);
                })
            );
        }

        private void createSession()
        {
            byte[] emptyBody = Serial.toBSON(new { });

            StartCoroutine(HTTPHelpers.post(url + "/api/v1/sessions?bson=true", emptyBody, store.accessToken,
                (data) =>
                {
                    sessionInfo.gameObject.SetActive(true);
                    sessionBackground.gameObject.SetActive(true);
                    openingInfo.gameObject.SetActive(false);
                    loadingCircle.gameObject.SetActive(false);

                    nameTitle.gameObject.SetActive(true);
                    nameInput.gameObject.SetActive(true);

                    try
                    {
                        SessionData responce = Serial.fromBSON<SessionData>(data);

                        sessionInfo.text = sessionInfoSave + responce.id;
                        sessionID = responce.id;

                        start.gameObject.SetActive(true);
                    }
                    catch (Exception e)
                    {
                        showError(e.ToString());
                    }
                }, (error) =>
                {
                    showError(error);
                })
            );
        }

        private void onStartClick()
        {
            InputStructure.SpecificPair[] pairs;

            try
            {
                pairs = parseSpecific();

                if (offlineMode)
                {
                    createHandler("no URL", "NoSessionID", nameInput.text, pairs);
                }
                else
                {
                    createHandler(url, sessionID, nameInput.text, pairs);
                }
            }
            catch (Exception e)
            {
                showError(e.ToString());
            }
        }
 
        private void showError(string error)
        {
            Debug.Log(error);

            errorBody.text = error;
            errorModal.gameObject.SetActive(true);

            connectionInfo.gameObject.SetActive(false);
            loadingCircle.gameObject.SetActive(false);
            sessionInfo.gameObject.SetActive(false);
            sessionBackground.gameObject.SetActive(false);
            openingInfo.gameObject.SetActive(false);

            urlTitle.gameObject.SetActive(true);
            urlInput.gameObject.SetActive(true);
            warningInfo.gameObject.SetActive(true);
            newSession.gameObject.SetActive(true);

            nameTitle.gameObject.SetActive(false);
            nameInput.gameObject.SetActive(false);
            start.gameObject.SetActive(false);
            dataInfo.gameObject.SetActive(false);

            sessionInfo.gameObject.SetActive(false);
            sessionBackground.gameObject.SetActive(false);
        }

        public void OnCloseError()
        {
            errorModal.gameObject.SetActive(false);
        }

        // All of this is since Unity only supports 1D arrays in the editor
        public InputStructure.SpecificPair[] parseSpecific()
        {
            InputStructure.SpecificPair[] tempPairs = new InputStructure.SpecificPair[limitOutputToSpecified.Length];

            for (int i = 0; i < limitOutputToSpecified.Length; i++)
            {
                string pairInput = limitOutputToSpecified[i];
                string[] pairSplit = pairInput.Split(':');

                if (pairSplit.Length != 2)
                {
                    throw new InputStructure.SpecificPairsParsingException("Takes colon separated pair", pairInput);
                }

                if (pairSplit[0] == String.Empty)
                {
                    throw new InputStructure.SpecificPairsParsingException("GameObject must have name", pairInput);
                }

                if (pairSplit[1] == String.Empty)
                {
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

            newHandler.isVerbose = printDebuggingData;
            newHandler.isSilent = doNotPrintCaptures;

            newHandler.store = store;

            // extraInfo can be used to capture arbitrary data
            newHandler.extraInfo = new
            {
                clientVersion = clientVersionLocked,
                gameVersion,
            };

            newHandler.pairs = pairs;

            SceneManager.LoadScene(sceneToLoad);
        }

    }

    // Minimally validated,
    // More data is returned from the server but is never used
    internal class SessionData
    {
        public string id;

        public SessionData(string i)
        {
            id = i;
        }
    }

    internal class ClaimData
    {
        public string claimToken;

        public ClaimData(string c)
        {
            claimToken = c;
        }
    }

    internal class AccessData
    {
        public string accessToken;

        public AccessData(string a)
        {
            accessToken = a;
        }
    }
}