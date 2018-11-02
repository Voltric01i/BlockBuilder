using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeftDistraction : MonoBehaviour {

	// Use this for initialization
	GameObject ScCo;
	GameObject ThrP;
	ThrowPicker Tp;
	ScoreController Sc;
	List<GameObject> objBlocks = new List<GameObject>();
	GameObject CurentObj;

	void Start () {
		ScCo = GameObject.Find("ScoreController");
        Sc = ScCo.GetComponent<ScoreController>();
		ThrP = GameObject.Find("ThrowController");
        Tp = ThrP.GetComponent<ThrowPicker>();
	}
	
	// Update is called once per frame
	void Update () {
		bool flag = false;
		objBlocks = Sc.GetBlockList();
		CurentObj = Tp.CurrentThrowThing();

		if(CurentObj != this.gameObject){
			foreach(var cnt in objBlocks){
				if(cnt == this.gameObject){
					flag = true;
				}
			}
		}else{
			flag = true;
		}
		if(flag != true){
			Destroy(this.gameObject);
		}

	}
}
