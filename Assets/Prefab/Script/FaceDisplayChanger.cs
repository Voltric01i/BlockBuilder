using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDisplayChanger : MonoBehaviour {

    public Material[] _FaceMaterial;
    public float IntervalTime;
    public int BlinkTime;
    GameObject Display;
    int blinkTimeCount;
    int blinkCount = 0;
    int blinkRandom = 0;
    bool blinkSwitch = true;

	// Use this for initialization
	void Start () {
        Display = this.gameObject;
        blinkRandom = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log(blinkCount);

        //瞬きの値加算
        if ((blinkCount/60f  >= blinkRandom) && (blinkSwitch == false))
        {
            blinkSwitch = true;
            blinkCount = 0;
            System.Random r = new System.Random();
            blinkRandom = 1+r.Next(BlinkTime);
        }
        else
        {
            blinkCount++;
        }

        //瞬き中の処理
        if (blinkTimeCount/60f >= IntervalTime)
        {
            blinkSwitch = false;
        }
        if (blinkSwitch == true)
        {
            Display.GetComponent<Renderer>().material = _FaceMaterial[0];
            blinkTimeCount++;
        }
        else
        {
            Display.GetComponent<Renderer>().material = _FaceMaterial[1];
            blinkTimeCount = 0;
        }

	}
}
