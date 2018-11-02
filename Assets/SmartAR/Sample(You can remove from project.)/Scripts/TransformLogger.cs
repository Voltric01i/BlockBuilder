using UnityEngine;
using System.Collections;

public class TransformLogger : MonoBehaviour {

	private int mCount = 0;

	// Use this for initialization
	void Start () {
		// NOP
	}

	// Update is called once per frame
	void Update () {
		mCount++;
		if (mCount % 10 != 0) return;

		Debug.Log("***************************************");
		Debug.Log(transform);
		Debug.Log("***** Global position *****");
		Debug.Log(transform.position);
//		Debug.Log("***** Local position *****");
//		Debug.Log(transform.localPosition);
	}
}
