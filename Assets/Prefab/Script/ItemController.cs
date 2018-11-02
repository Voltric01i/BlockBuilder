using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    GameObject KeepItem;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool ControlledItemKeeper(GameObject item)
    {

        if (item == null)
        {
            KeepItem = null;
            Debug.Log("アイテム効力消失");
            return false;
        }
        else if (KeepItem == null)
        {
            KeepItem = item;
            Debug.Log("アイテム新規登録");
            return true;

        }else if (KeepItem == item)
        {
            return false;
        }
         else if (RndPercent(50))
        {
            KeepItem = item;
            Debug.Log("アイテム上書き登録");
            return true;
        }
        else {
            Debug.Log("登録されず");
            return false;
        }
    }

    bool RndPercent(int percent)
    {
        double GeneratePer;
        System.Random r = new System.Random();
        GeneratePer = r.NextDouble();

        GeneratePer *= 100;

        if (GeneratePer < percent)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
