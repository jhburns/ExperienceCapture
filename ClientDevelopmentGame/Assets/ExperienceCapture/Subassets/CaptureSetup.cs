using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class CaptureSetup : MonoBehaviour
{
    [Tooltip("How faster data is collect, in frames. For example 60 = every 1 second @60fps.")]
    public int captureRate;

    [Tooltip("First scene to load.")]
    public string sceneToLoad;

    [Tooltip("Extra debugging data.")]
    public bool printAdditionalCaptureInfo;
    [Tooltip("Handles game objects being instantiated and destroyed.")]
    public bool findObjectsInEachFrame;
    [Tooltip("Still capture data, but don't print it.")]
    public bool doNotPrintToConsole;

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
        newHandler.setFindEveryFrame(findObjectsInEachFrame);

        newHandler.setVerbose(printAdditionalCaptureInfo);
        newHandler.setSilence(doNotPrintToConsole);

        SceneManager.LoadScene(sceneToLoad);
    }

}