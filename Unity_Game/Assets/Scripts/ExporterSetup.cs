using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExporterSetup : MonoBehaviour
{
    public Exporter ex;

    public int captureRate;

    public void checkStatus(string url, string username)
    {
        StartCoroutine(GetRequest(url, (data) =>
            {
                if (data != "OK")
                {
                    Debug.Log("Error, server responded with error of: " + data);
                }
                else
                {
                    CreateExporter(url, username);
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

    private void CreateExporter(string url, string username)
    {
        Exporter newExporter = Instantiate(ex);
        newExporter.setUrl(url);
        newExporter.setUsername(username);
        newExporter.setRate(captureRate);
        SceneManager.LoadScene("Game");
    }

}