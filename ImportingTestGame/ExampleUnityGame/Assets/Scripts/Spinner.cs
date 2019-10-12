using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Capture;

public class Spinner : MonoBehaviour, ICapturable
{
	
	void Update () {
		transform.Rotate (Vector3.forward * -3);
	}

    public object getCapture() {
        return new 
        {
            rotation = transform.eulerAngles.z
        };
    }

}
