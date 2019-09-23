using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public InputField urlInput;
    public DataExporter exporter;

    void Start()
    {
        
    }

    public void getUrl()
    {
        exporter.export(urlInput.text);
    }
}
