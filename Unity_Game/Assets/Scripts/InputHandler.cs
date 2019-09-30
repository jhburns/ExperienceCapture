using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField urlInput;
    public InputField nameInput;
    public CaptureSetup exporter;

    public bool useWindowsDefault;

    void Start()
    {
        if (useWindowsDefault)
        {
            urlInput.text = "http://192.168.99.100:4321/";
        }
        else
        {
            urlInput.text = "http://0.0.0.0:4321/";
        }

        nameInput.text = "Alice";
    }

    public void getUrl()
    {
        exporter.checkStatus(urlInput.text, nameInput.text);
    }
}
