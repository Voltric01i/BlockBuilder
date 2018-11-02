using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SettingController : MonoBehaviour {

    public GameObject LandmarkEnabler;
    public GameObject TargetPointEnabler;
    UnityEngine.UI.Toggle LE;
    UnityEngine.UI.Toggle TE;


    // Use this for initialization
    void Start () {


        LE = LandmarkEnabler.GetComponent<UnityEngine.UI.Toggle>();
        TE = TargetPointEnabler.GetComponent<UnityEngine.UI.Toggle>();

        LE.isOn = (PlayerPrefs.GetInt("showLandmarks",1) != 0) ? true : false;
        TE.isOn = (PlayerPrefs.GetInt("showTargetPoint",0) != 0) ? true : false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
