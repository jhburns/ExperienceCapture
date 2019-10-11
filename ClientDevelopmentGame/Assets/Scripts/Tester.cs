using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Capture;

public class Tester : MonoBehaviour, ICapturable
{

    string dummyText;

    private void setText(string t)
    {
        dummyText = t;
    }

    public object getCapture()
    {
        return new
        {
            text = dummyText
        };
    }
}
