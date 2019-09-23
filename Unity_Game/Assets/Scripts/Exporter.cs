using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exporter : MonoBehaviour
{
    private string url;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SetUrl(string u)
    {
        url = u;
    }
}
