using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField urlInput;
    public ExporterSetup exporter;

    void Start()
    {
        
    }

    public void getUrl()
    {
        exporter.checkStatus(urlInput.text);
    }
}
