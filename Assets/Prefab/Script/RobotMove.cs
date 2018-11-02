using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMove : MonoBehaviour {

    AnimationCurve SpeedCurve;
    GameObject Robot;
    float scale;
    int deadTime;
    float rotateSpeed;
    bool TurnEnab;


    bool RobotTurn = false;
    bool RobotRun = false;
    Vector3 MoveCheck;
    int deadTimeCount = 0;
    int turnTimeCount = 0;
    int moveTimeCount = 0;
    int runTimeCount = 0;




    //初期位置基準で動かすための変数
    Vector3 originPosition;

    Vector3 startPosition;
    Vector3 endPosition;

    //内部変数への代入
    public void Propaty(GameObject bot, float sp, float rotate, int dead, bool Turn, AnimationCurve Curve)
    {
        scale = sp;
        SpeedCurve = Curve;
        Robot = bot;
        originPosition = Robot.transform.position;
        deadTime = dead;
        rotateSpeed = rotate;
        TurnEnab = Turn;
    }


    //移動開始時に呼び出す関数
    public void RunStart(Vector3 end)
    {
        deadTimeCount = 0;
        endPosition = end;
        startPosition = MoveCheck = Robot.transform.position;
        //Debug.Log(endPosition);
        RobotTurn = true;
        RobotRun = true;
        moveTimeCount = 0;
        runTimeCount = 0;
    }

    //毎フレーム呼び出して欲しい関数
    public string Run()
    {

        //Robotが移動中か
        if (RobotRun == true)
        {

            //ErrorStop検知用の動作時間測定
            moveTimeCount++;

            //乗り上げ対策処理
            //回転オプションが有効か
            if (TurnEnab)
            {          
                //Debug.Log(virtualY);
                endPosition = new Vector3(endPosition.x,VirtualElevation(), endPosition.z);

            }
            //目的地方向のベクトルを再作成
            var relatevePos = endPosition - Robot.transform.position;
            if ((relatevePos == Vector3.zero) &&(TurnEnab)) {
                Debug.Log("Quaternion.LookRotation(Vector.zero)");
                endPosition = Vector3.zero;
            }

            var endRotation = Quaternion.LookRotation(relatevePos);


            //Debug.Log("endRotation.eulerAngles.y =" + endRotation.eulerAngles.y);
            //Debug.Log("Robot.transform.rotation.eulerAngles.y = " + Robot.transform.rotation.eulerAngles.y);
            //Debug.Log(Robot.transform.rotation.eulerAngles);

            //指定座標(に近いところ)まで行ったことを確認し動作を止める
            if ((Robot.transform.position - endPosition).magnitude <= 0.01f)
            {
                Robot.transform.position = endPosition;
                RobotRun = false;
                deadTimeCount = 0;

            }

            //回転オプションが有効か
            if (TurnEnab) {
                //指定座標の方向(に近いところ)まで回転したことを確認し移動へと移行
                //移動中にぶつかって向きが変わった時用に回転は毎回呼び出す↓
                Robot.transform.rotation = Quaternion.Slerp(Robot.transform.rotation, endRotation, rotateSpeed);

                if ((endRotation.eulerAngles.y * endRotation.eulerAngles.y - Robot.transform.rotation.eulerAngles.y * Robot.transform.rotation.eulerAngles.y <= 0.2f)
                     && endRotation.eulerAngles.y * endRotation.eulerAngles.y - Robot.transform.rotation.eulerAngles.y * Robot.transform.rotation.eulerAngles.y >= -0.2f)
                {
                    RobotTurn = false;
                    turnTimeCount = 0;
                }
            }
            else
            {
                RobotTurn = false;
            }

            //カーブの値を横軸は移動完了率、縦軸は加速度として使って移動する
            if (RobotTurn == false)
            {
                runTimeCount++;
                var par = (Robot.transform.position - startPosition).magnitude / (endPosition - startPosition).magnitude;
                var curvePos = SpeedCurve.Evaluate(par);
                var t = runTimeCount / 50f;
                Robot.transform.position = Vector3.MoveTowards(Robot.transform.position, endPosition, curvePos * t * scale);
                //Robot.transform.position = Vector3.Lerp(startPosition, endPosition, speed * 0.005f);
            }



        }



        //deadTime秒の間、ルート全体の20%以下しか進まなかったときにエラーを返すための機構
        if (moveTimeCount % 60 == 0)
        {
            MoveCheck = Robot.transform.position;
        }
        if ((Robot.transform.position - MoveCheck).magnitude / (endPosition - startPosition).magnitude <= 0.20f)
        {
            deadTimeCount++;
        }
        else
        {
            deadTimeCount = 0;
        }
        //Debug.Log("瞬間移動率" + (Robot.transform.position - MoveCheck).magnitude / (endPosition - startPosition).magnitude);
        //Debug.Log("deadTimeCount = "+deadTimeCount);
        //ステータスを返り値として返す
        if (Robot == null)
        {
            return "null";
        }
        else if ((RobotRun == true) && ((deadTimeCount / 50 >= deadTime + 1)||(turnTimeCount /50 >= deadTime+1)) || ((endPosition == Vector3.zero)&&(TurnEnab == true)) )
        {
            Debug.Log("ErrorStop");
            turnTimeCount = 0;
            deadTimeCount = 0;
            runTimeCount = 0;
            moveTimeCount = 0;
            return "ErrorStop";
        }
        else if (RobotTurn)
        {
            turnTimeCount++;
            return "Turn";

        }
        else if (RobotRun)
        {
            return "Run";
        }
        else
        {
            deadTimeCount = 0;
            runTimeCount = 0;
            moveTimeCount = 0;
            return "Stop";
        }

    }

    public float VirtualElevation()
    {
        //目的地方向のベクトルを作成
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(0f, -20f, 0f);
        var togoPos = endPosition - Robot.transform.position;
        var robotEuler = Robot.transform.localEulerAngles.x;
        if (robotEuler > 180)
        {
            robotEuler -= 360;
        }
        if (robotEuler > 25)
        {
            robotEuler = 25;
        }
        else if (robotEuler < -25)
        {
            robotEuler = -25;
        }
        var virtualY = Robot.transform.position.y - (robotEuler * Mathf.Deg2Rad * Mathf.Sqrt(togoPos.x * togoPos.x + togoPos.z * togoPos.z));

        return virtualY;
    }





 }


