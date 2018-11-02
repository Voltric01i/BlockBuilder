using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackControl : MonoBehaviour {

    public GameObject menuTint;
    public GameObject menuList;
    public GameObject Touch;

    // Use this for initialization
    void Start () { 

    }
	
	// Update is called once per frame
	void Update () {

        if(OnMenuTouch() == "OtherTouch")
        {
            menuTint.SetActive(false);
            menuList.SetActive(false);
            Touch.SetActive(true);
        }

    }



    string OnMenuTouch()
    {
        // タッチされているとき
        if (0 < Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タッチ情報をコピー
                Touch t = Input.GetTouch(i);
                // タッチしたときかどうか
                if (t.phase == TouchPhase.Began)
                {
                    //タッチした位置からRayを飛ばす
                    Ray ray = Camera.main.ScreenPointToRay(t.position);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit))
                    {
                        //Rayを飛ばしてあたったオブジェクトが自分自身だったら
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            Debug.Log("MenuTouch");
                            return "MenuTouch";
                            
                        }
                        else
                        {
                            Debug.Log("OtherTouch");
                            return "OtherTouch";

                        }
                    }
                }
            }
        }
        Debug.Log("NotTouch");
        return "NotTouch"; //タッチされてなかったらfalse
    }
}
