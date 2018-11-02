using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : MonoBehaviour {

    GameObject view;
    Rigidbody rb;
    float scale;
    bool release = true;
    Vector3 itemDef;
    bool OnceTouch = false;
    float MovePar = 1f;

    int moveStartTime = 0;


    // Use this for initialization
    public void ThrowStart (GameObject c,float s) {
        view = c;
        scale = s;
        itemDef = new Vector3(0,scale * -0.5f, 0);

    }

    
    public void DefaultPos(GameObject ob)
    {
        Vector3 screenPos = Input.mousePosition;
        ob.transform.position = view.transform.position + itemDef + view.transform.forward * 0.08f * scale;

    }
    

    public void MoveEnable()
    {
        release = false;
    }

    // Update is called once per frame
    public string Move (GameObject throwIt,Rigidbody throwRb,GravityController throwG,ScoreSender scoreSed,Collider colid) {
        // Debug.Log(worldPos);

        //Debug.Log(moveStartTime);
        //Debug.Log(OnceTouch);

        if (throwIt == null)
        {
            moveStartTime = 0;
        }
        else if ((moveStartTime <= 31) && (throwIt != null))
        {
            moveStartTime++;
        }


        if ((throwIt != null) && (throwRb != null) && (colid != null) )
        {
            if (release == false)
            {
                if (Input.touchCount == 0)
                {
                    if (OnceTouch == true)
                    {
                        OnceTouch = false;
                        if (throwG != null && scoreSed != null)
                        {
                            throwG.enabled = true;
                            scoreSed.enabled = true;

                        }
                        else
                        {
                           // throwRb.useGravity = true;
                        }
                        var itemthrow = new Vector3(0, scale * MovePar * 0.015f, 0);
                        var far = view.transform.forward * scale * MovePar * 0.015f;
                        //Debug.Log(far);
                        //Debug.Log(view.transform.forward);
                        throwRb.AddForce((far + itemthrow), ForceMode.Force);
                        colid.enabled = true;
                        release = true;
                        return "Release";
                    }
                    else
                    {
                        DefaultPos(throwIt);
                        //Debug.Log(view.transform.forward);
                        return "NotTouch";
                    }

                }
                else if (Input.touchCount > 0)
                {
                    if (moveStartTime >= 30)
                    {
                        OnceTouch = true;
                    }
                    var screenPos = Input.mousePosition;
                    screenPos.z = 0.7f * scale;
                    var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                    throwIt.transform.position = worldPos;
                    var TouchDate = Input.GetTouch(0);

                    MovePar = TouchDate.deltaPosition.magnitude / TouchDate.deltaTime;
                    //Debug.Log(MovePar);

                    return "Less";
                }
                else
                {
                    return "Error";
                }
            }
            else
            {
                return "Release";
            }

        }
        else
        {
            return "Error";
        }
    }
}
