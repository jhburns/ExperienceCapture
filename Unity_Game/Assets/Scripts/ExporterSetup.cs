using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExporterSetup : MonoBehaviour
{
    public Exporter ex;

    public void checkStatus(string url)
    {
        StartCoroutine(GetRequest(url, (data) =>
            {
                if (data != "OK")
                {
                    Debug.Log("Error, server responded with error of: " + data);
                }
                else
                {
                    CreateExporter(url);
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

    private void CreateExporter(string url)
    {
        Exporter newExporter = Instantiate(ex);
        newExporter.setUrl(url);
        SceneManager.LoadScene("Game");
    }

}