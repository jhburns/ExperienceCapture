using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Capture;

public class Tester : MonoBehaviour, ICapturable
{
    string dummyText;

    public bool isNullValue { set; get; }
    public bool isNullProperty { set; get; }

    void Awake()
    {
        isNullValue = false;
        isNullProperty = false;
    }

    public void setText(string t)
    {
        dummyText = t;
    }

    public object GetCapture()
    {
        if (isNullValue)
        {
            return new
            {
                dummyText = (string) null,
            };
        }

        if (isNullProperty)
        {
            return new { };
        }

        return new
        {
            dummyText,
        };
    }
}
