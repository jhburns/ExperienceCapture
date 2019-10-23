using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using Newtonsoft.Json;

public class CaptureSetup : MonoBehaviour
{
    public HandleCapturing ex;

    public int captureRate;

    public bool sendToConsole;

    public string newSessionPath; 

    public string sceneToLoad;

    public void checkStatus(string url, string username)
    {
        if (sendToConsole)
        {
            createExporter(url, null, username);
            return;
        }

        StartCoroutine(postCaptures(url + newSessionPath, (data) =>
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
                        Debug.Log("Session id is: " + responce.id);
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

    private IEnumerator postCaptures(string location)
    {
        using (UnityWebRequest request = UnityWebRequest.Put(location, "{}"))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {

            }
        }
    }

    private void createExporter(string url, string id, string username)
    {
        HandleCapturing newExporter = Instantiate(ex);

        newExporter.setUrl(url);
        newExporter.setUsername(username);
        newExporter.setID(id);
        newExporter.setRate(captureRate);
        newExporter.setToConsole(sendToConsole);

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