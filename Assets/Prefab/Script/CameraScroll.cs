using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CameraScroll : RobotMove {

    public GameObject View;
    public Button CameraButton;
    public Text CameraText;
    public GameObject Robot;
    public Material[] _ButtonMaterial;

    //移動のための変数
    public AnimationCurve SpeedCurve;
    public int MoveDeadTime;
    public float scale = 1;
    public float rotateSpeed;
    int groundLayer = 8;

    public float scrollSpeed = 0.01f;
    bool TouchMove = false;
    bool ButtonTouch = false;
    bool CameraMove = false;
    bool RotateAuto = false;

    Touch t;

    //継承用の変数
    RobotMove Trans;

    private void Start()
    {
        Trans = gameObject.AddComponent<RobotMove>();
        Trans.Propaty(View, scale, rotateSpeed, MoveDeadTime, false, SpeedCurve);
    }

    public void CameraButtonPressed()
    {
         RotateAuto = true;
    }

    private void FixedUpdate()
    {

        //毎フレームの移動関数呼び出し
        string Mess = Trans.Run();

        //Runの返り値ステータスによって動作を指定
        if (Mess == "Stop")
        {
            CameraMove = false;
        }
        else if (Mess == "Turn")
        {

        }
        else if (Mess == "Run")
        {
            CameraMove = true;
        }
        else if (Mess == "ErrorStop")
        {
            CameraMove = false;
        }

    }

    // Update is called once per frame
    void Update() {
        AutoCameraScroll();

        float xAngle = View.transform.eulerAngles.x;
        float yAngle = View.transform.eulerAngles.y;

        int touchCount = Input.touches.
         Count(t => t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled);
        if (touchCount == 1)
        {
            t = Input.touches.First();
            switch (t.phase)
            {

                case TouchPhase.Moved:
                    TouchMove = true;
                    ButtonTouch = false;
                    RotateAuto = false;
                    //移動量に応じて角度計算
                    float x = -t.deltaPosition.x * scrollSpeed;
                    float y = t.deltaPosition.y * scrollSpeed;

                    //回転
                    //View.transform.Rotate(y, x, 0);
                    View.transform.eulerAngles = new Vector3(y + xAngle, x + yAngle, 0);

                    //Debug.Log(xAngle);
                    //Debug.Log(yAngle);
                    break;

                case TouchPhase.Began:
                break;
                case TouchPhase.Stationary:
                    if (TouchMove == false)
                    {
                        ButtonTouch = true;
                    }
                    break;
            }

        }
        else if(touchCount == 2)
        {
            Debug.Log("二本");
        }
        else if (touchCount == 0)
        {
            if ((ButtonTouch == true)&&(TouchMove == false))
            {
                    //タッチしていた位置からRayを飛ばす
                    Ray ray = Camera.main.ScreenPointToRay(t.position);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit))
                    {
                        //Rayを飛ばしてあたったオブジェクトが地面だったら
                        if ((hit.collider.gameObject.layer == groundLayer) && (CameraMove == false))
                        {
                            RotateAuto = false;
                            var runPoint = new Vector3(hit.point.x, View.transform.position.y, hit.point.z);
                            Trans.RunStart(runPoint);
                            CameraMove = true;
                            //Debug.Log(hit.point);

                        }


                    }
                ButtonTouch = false;
            }
            else if (TouchMove)
            { 
                  TouchMove = false;
            }
        }
   
    

    }

    void AutoCameraScroll()
    {
        if (RotateAuto == true)
        {
            BtnStateColorChange(CameraButton, CameraButton.colors.disabledColor, 1);
            BtnStateColorChange(CameraButton, CameraButton.colors.disabledColor, 0);
            CameraText.text = "Camera\nAutoMode";
            View.transform.LookAt(Robot.transform);

        }
        else
        {
            CameraText.text = "Camera\nTouchMode";
            BtnStateColorChange(CameraButton, ColorMake("#FFFFFFFF"), 0);
            BtnStateColorChange(CameraButton, ColorMake("#B1F5F5FF"), 1);

        }
    }

    public Color ColorMake(string hexColor)
    {
        Color color = default(Color);
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            // 成功
        }
        else
        {
            //失敗
            ColorUtility.TryParseHtmlString("#FFFFFFFF", out color);
       }
       return color;
    }

    public static void BtnStateColorChange(Button btn, Color color, int changeState)
    {
        ColorBlock cbBtn = btn.colors;
        switch (changeState)
        {
            case 0://normalColor
                cbBtn.normalColor = color;
                break;
            case 1://highlightedColor
                cbBtn.highlightedColor = color;
                break;
            case 2://pressedColor
                cbBtn.pressedColor = color;
                break;
            case 3://disabledColor
                cbBtn.disabledColor = color;
                break;
        }
        btn.colors = cbBtn;
    }

}


