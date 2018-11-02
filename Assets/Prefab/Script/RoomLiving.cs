using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//public class RoomLiving : MonoBehaviour
public class RoomLiving : RobotMove
{
    //移動のための変数
    GameObject Robot;
    Rigidbody RobotRb;
    public AnimationCurve SpeedCurve;
    public AnimationCurve IntervalTime;
    public AnimationCurve AriaSize;
    public bool ARmode;
    public bool DebugView;
    public float scale = 1;
    public float rotateSpeed;

    //初期座標用の変数
    Vector3 DefaultPos;

    //連続動作させるための変数
    public int startWaitTime;
    double intervalWaitTime;
    public int MoveDeadTime;
    int startTimeCount = 0;
    bool startTimeSwitch = false;
    int intervalTimeCount = 0;
    bool intervalTimeSwitch = true;

    bool RobotRun = false;

    //当たり判定用
    int groundLayer = 8;
    bool CollisionSwitch = false;
    int collisionCount = 0;
    float colliderDistance;

    //タッチで反応を示すようにするための変数

    bool robotTouch = false;
    int robotWait = 0;
    public GameObject View;
    //public GameObject realRobot;


    //継承用の変数
    RobotMove Move;

    //学習レベル保持用変数
    int RobotLevel = 0;

    Vector3 MovePoint;



    // Use this for initialization
    void Start()
    {
        Robot = this.gameObject;
        RobotRb = this.GetComponent<Rigidbody>();
        DefaultPos = Robot.transform.position;
        Move = gameObject.AddComponent<RobotMove>();
        Move.Propaty(Robot,scale,rotateSpeed,MoveDeadTime,true,SpeedCurve);
        RobotLevel = PlayerPrefs.GetInt("Robot_Level", 2);
        Debug.Log("Robot_Level=" + RobotLevel);
        colliderDistance = 0.6f * 5f * Robot.transform.localScale.x;

    }


    void Update()
    {
        if (Robot.transform.position.y <= -5f * 5f * Robot.transform.localScale.x)
        {
            RobotReset();
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
                        if (hit.collider.gameObject == Robot.gameObject)
                        {
                            Debug.Log("tuni");
                            //CameraRun();
                            robotTouch = true;

                        }
                    }
                }
            }
        }


    }

    public void RobotReset()
    {
        Robot.transform.position = DefaultPos;
        RobotRb.velocity = Vector3.zero;
        CollisionSwitch = false;
        Move.RunStart(LearningMovePoint(false));
    }

    private void FixedUpdate()
    {
    
        //毎フレームの移動関数呼び出し
        string Mess = Move.Run();

        //スタートまでの待ち時間計算
        if (startTimeCount >=  startWaitTime*50)
        {
            startTimeSwitch = true;
        }
        if (startTimeSwitch == false)
        {
            startTimeCount++;
        }



        //Runの返り値ステータスによって動作を指定
        if (Mess == "Stop")
        {
            intervalTimeSwitch = true;
            intervalTimeCount++;
            RobotRun =  false;
        }
        else if(Mess == "Turn")
        {
            intervalTimeCount = 0;
            intervalTimeSwitch = false;
            RobotRun = true;
        }
        else if(Mess == "Run")
        {
            RobotRun = true;
        }
        else if(Mess == "ErrorStop")
        {
            IntelligenceMove();
        }

        //移動の間の時間の計算
        if (intervalTimeCount >= intervalWaitTime * 60)
        {
            intervalTimeSwitch = false;
            intervalWaitTime = IntervalTime.Evaluate(Random.value) * 10.0f;
            intervalTimeCount = 0;
        }

        //ロボットが動いておらず、スタートの待ち時間が経ち、間の時間が経過したときに再度うろうろさせる
        if ((RobotRun == false) && (startTimeSwitch == true) && (intervalTimeSwitch == false))
        {
            IntelligenceMove();
        }
                
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Graund")
        {
            CollisionStop(other.ClosestPointOnBounds(this.transform.position));
            if (DebugView)
                Debug.Log(other.ClosestPointOnBounds(this.transform.position));
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Graund")
        {
            CollisionStop(other.ClosestPointOnBounds(this.transform.position));
            if (DebugView)
                Debug.Log(other.ClosestPointOnBounds(this.transform.position));
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Graund")
        {
            CollisionStop(other.ClosestPointOnBounds(this.transform.position));
            if (DebugView)
                Debug.Log(other.ClosestPointOnBounds(this.transform.position));
        }
    }
    */

    //カメラに向かって行く
    void CameraRun()
    {
        intervalTimeSwitch = false;
        var pos = View.transform.position-View.transform.forward;
        Debug.Log(pos);
        Move.RunStart(pos);
    }
 
    //移動用関数
    void IntelligenceMove()
    {

        intervalTimeSwitch = false;

        switch (RobotLevel) {
            case 1:
                //すぐに諦めて次の目的地へ
                CollisionSwitch = false;
                collisionCount = 0;
                MovePoint = LearningMovePoint(false);
                break;
            case 2:
                //指定回数試行してだめなら次の目的地へ
                CollisionSwitch = false;
                MovePoint = LearningMovePoint(false);

                break;
        }
        if (DebugView)
        {
            Debug.Log("MovePoint:"+MovePoint);
        }
        Move.RunStart(MovePoint);

    }

    //座標生成用関数
    public Vector3 LearningMovePoint(bool missingMove)
    {
        Vector3 Pos = new Vector3(0, 0, 0);
        switch (RobotLevel)
        {
            case 1:
                //あくまで指定座標を直線で行き来するだけ
                float EreaSize = 2.5f * 5 * Robot.transform.localScale.x;

                System.Random r = new System.Random();
                float endX = (EreaSize * 2f * (AriaSize.Evaluate(Random.value) - 0.5f));
                float endZ = (EreaSize * 2f * (AriaSize.Evaluate(Random.value) - 0.5f));
                Pos = new Vector3(Robot.transform.position.x + endX, Move.VirtualElevation(), Robot.transform.position.z + endZ);

                break;
            case 2:
                //指定座標に向かうために反射角へ動く
                float OneMoveEreaSize = 3f * 5 * Robot.transform.localScale.x;


                System.Random T = new System.Random();
                float endX2 = (OneMoveEreaSize * 2f * (AriaSize.Evaluate(Random.value) - 0.5f));
                float endZ2 = (OneMoveEreaSize * 2f * (AriaSize.Evaluate(Random.value) - 0.5f));
                if (missingMove)
                {
                    if (DebugView)
                        Debug.Log("再衝突対策処理");

                        Pos = new Vector3(Robot.transform.position.x + endX2, Move.VirtualElevation(), Robot.transform.position.z + endZ2);
                        Pos = OneMoveEreaSize * Pos +  Robot.transform.forward;
                }
                else
                {
                    if(ARmode)
                        Pos = new Vector3(endX2*1.0f, Move.VirtualElevation(),endZ2*1.0f);
                    else
                        Pos = new Vector3(Robot.transform.position.x + endX2, Move.VirtualElevation(), Robot.transform.position.z + endZ2);
                }


                break;
        }

        return Pos;
    }


    public void CollisionStop(Vector3 collitionPoint)
    {
        collisionCount++;
        CollisionSwitch = true;

        //2回以上衝突を繰り返していたら
        if (collisionCount >= 2)
        {
            //目的地を再構築する
            if (DebugView)
                Debug.Log("目的地再構築");

            collisionCount = 0;
            IntelligenceMove();
        }
        else
        {
            if (DebugView)
                Debug.Log("再衝突");

            switch (RobotLevel)
            {
                case 1:
                    //来た道を引き返す
                    Vector3 Pos1 = ((collitionPoint - this.transform.position) * 2f) + this.transform.position;
                    Pos1 = new Vector3(Pos1.x, Move.VirtualElevation(), Pos1.z);
                    Move.RunStart(Pos1);

                    if(DebugView)
                        Debug.Log("引き返し座標 :" + Pos1);
                    break;

                case 2:
                    //壁と並行に動いて引き返す
                    Vector3 Pos2 = ((collitionPoint - this.transform.position) * 2f) + this.transform.position;
                    Pos2 = new Vector3(Pos2.x, Move.VirtualElevation(), Pos2.z);
                    Move.RunStart(Pos2);
                    if(DebugView)
                        Debug.Log("引き返し座標 :" + Pos2);
                    break;
            }
        }
    }

}



