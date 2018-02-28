using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SetCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CameraDevice cam = CameraDevice.Instance;
        cam.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
