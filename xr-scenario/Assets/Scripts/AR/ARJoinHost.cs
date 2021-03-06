﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;

public class ARJoinHost : MonoBehaviour
{
	//TempScript for 
	public string hostIP;
	public NetworkManager netManager;
	// Use this for initialization
	void Start ()
	{
		if (VRSettings.loadedDeviceName == "HoloLens")
		{
			netManager.networkAddress = hostIP;
			netManager.StartClient();
		}

		if (VRSettings.loadedDeviceName == "GearVR")
		{
			netManager.networkAddress = hostIP;
			netManager.StartClient();
		}
	}
}
