using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class OutputCatcher : MonoBehaviour {

    public Text output;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string newOutput = logString + "\n" + output.text;
        string[] byNewline = newOutput.Split('\n');
        var firstLines = byNewline.Take(12).ToList();

        string total = string.Join("\n", firstLines.ToArray<string>());

        output.text = total;
    }

}
