using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBackButton : MonoBehaviour {
    public GameObject view;
    public AnimationCurve curve;
    public Vector3 homePosition;
    public Quaternion homeRotation;
    Quaternion homeRot;
    Vector3 startPosition;
    bool cameraMove = false;

    // Use this for initialization
    void Start()
    {
        startPosition = view.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (cameraMove == true)
        {
            startPosition = view.transform.position;
            var curvePos = curve.Evaluate(0);
            view.transform.position = Vector3.Lerp(startPosition, homePosition, curvePos);
            view.transform.rotation = Quaternion.Slerp(view.transform.rotation, homeRotation, 0.05f);
            //view.transform.rotation = Quaternion.Lerp(homeRot, rot, curvePos);

            if ((view.transform.position == homePosition) && (view.transform.rotation.eulerAngles == homeRotation.eulerAngles))
            {
                cameraMove = false;
            }
        }

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
                                homeRot = view.transform.rotation;
                                cameraMove = true;
                        }
                    }
                }
            }
        }

    }
}
