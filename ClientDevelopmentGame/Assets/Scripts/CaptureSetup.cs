using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using Newtonsoft.Json;

public class CaptureSetup : MonoBehaviour
{
    public HandleCapturing handler;

    public int captureRate;

    public string sceneToLoad;

    private void Start()
    {
        createHandler("", "client-only", "McClient");
    }

    private IEnumerator getRequest(string uri, System.Action<string> callback)
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
            }
            else
            {
                callback(webRequest.downloadHandler.text);
            }
        }
    }

    private void createHandler(string url, string id, string username)
    {
        HandleCapturing newHandler = Instantiate(handler);

        newHandler.setUrl(url);
        newHandler.setUsername(username);
        newHandler.setID(id);
        newHandler.setRate(captureRate);
        newHandler.setToConsole(true);
        newHandler.setCapturability(false);

        SceneManager.LoadScene(sceneToLoad);
    }

}