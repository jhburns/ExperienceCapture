using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using Newtonsoft.Json;

public class ExporterSetup : MonoBehaviour
{
    public Exporter ex;

    public int captureRate;

    public bool sendToConsole;

    public string newSessionPath; 

    public void checkStatus(string url, string username)
    {
        if (sendToConsole)
        {
            createExporter(url, null, username);
            return;
        }

        StartCoroutine(GetRequest(url + newSessionPath, (data) =>
            {
                try
                {
                    SessionData responce = JsonConvert.DeserializeObject<SessionData>(data);

                    if (responce.status != "OK")
                    {
                        Debug.Log("Error, server responded with status of: " + responce.status);
                    }
                    else
                    {
                        createExporter(url, responce.id, username);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            })
        );
    }

    IEnumerator GetRequest(string uri, System.Action<string> callback)
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

    private void createExporter(string url, string id, string username)
    {
        Exporter newExporter = Instantiate(ex);
        newExporter.setUrl(url);
        newExporter.setUsername(username);
        newExporter.setID(id);
        newExporter.setRate(captureRate);
        newExporter.setToConsole(sendToConsole);
        SceneManager.LoadScene("Game");
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