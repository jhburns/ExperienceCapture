using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class CaptureSetup : MonoBehaviour
{
    public int captureRate;

    public string sceneToLoad;

    public bool printAdditionalCaptureInfo;
    public bool findCapturableEachFrame;

    public HandleCapturing handler;

    private void Start()
    {
        createHandler("", "client-only", "McClient");
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
        newHandler.setVerbose(printAdditionalCaptureInfo);

        SceneManager.LoadScene(sceneToLoad);
    }

}