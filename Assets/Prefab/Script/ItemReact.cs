using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReact :RobotMove {

    RobotMove Move;

    GameObject robot;
    GameObject ItemKeeper;


    public float scale = 1;
    public float intervalWaitTime;
    public float rotateSpeed;
    public int MoveDeadTime;
    public bool DebugView;
    bool RobotRun = false;

    int intervalTimeCount = 0;
    bool intervalTimeSwitch = true;

    bool MoveSwitch = false;
    int ItemMoveTime = 0;
    int ItemMoveEndTime = 0;
    bool OnetimeSwitch = true;


    public AnimationCurve SpeedCurve;
    public AnimationCurve ReactTime;

    Rigidbody RobotRb;
    RoomLiving RobotRl;
    


    // Use this for initialization
    void Start () {
        robot = GameObject.Find("MainRobot");
        ItemKeeper = GameObject.Find("ItemController");

        //RobotRb = robot.GetComponent<Rigidbody>();
        //RobotRl = robot.GetComponent<RoomLiving>();
        //Move = gameObject.AddComponent<RobotMove>();

        Move.Propaty(robot, scale, rotateSpeed, MoveDeadTime, true, SpeedCurve);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Robot")
        {
            if (OnetimeSwitch == true)
            {
                if (ItemKeeper.GetComponent<ItemController>().ControlledItemKeeper(this.gameObject)){
                    RobotRl.enabled = false;
                    ItemMoveEndTime = RndTime() * 50;
                    Debug.Log(ItemMoveEndTime);

                    MoveSwitch = true;
                    if (DebugView)
                        Debug.Log("ItemReact:Start");
                }

            }
        }
    }

    void FixedUpdate()
    {

        if (MoveSwitch == true) {
            ItemMoveTime++;
            string Mess = Move.Run();

            if (Mess == "Stop")
            {
                intervalTimeSwitch = true;
                intervalTimeCount++;
                RobotRun = false;
            }
            else if (Mess == "Turn")
            {
                intervalTimeCount = 0;
                intervalTimeSwitch = false;
                RobotRun = true;
            }
            else if (Mess == "Run")
            {
                RobotRun = true;
            }
            else if (Mess == "ErrorStop")
            {
                ItemMove();
            }

            //移動の間の時間の計算
            if (intervalTimeCount >= intervalWaitTime * 50)
            {
                intervalTimeSwitch = false;
                intervalTimeCount = 0;
            }

            //ロボットが動いておらず、スタートの待ち時間が経ち、間の時間が経過したときに再度うろうろさせる
            if ((RobotRun == false) && (intervalTimeSwitch == false))
            {
                ItemMove();
            }
            
            if(ItemMoveTime > ItemMoveEndTime)
            {
                OnetimeSwitch = false;
                RobotRl.enabled = true;
                ItemKeeper.GetComponent<ItemController>().ControlledItemKeeper(null);
                this.enabled = false;

            }

        }
    }

    void ItemMove()
    {
        Move.RunStart(this.gameObject.transform.position);
    }

    // Update is called once per frame
    void Update () {
		
	}

    int RndTime()
    {
        double Generate;

        Generate = ReactTime.Evaluate(Random.value) * 10;
        int ret = (int)Generate;

        return ret;
    }

    bool RndPercent(int percent)
    {
        double GeneratePer;
        System.Random r = new System.Random();
        GeneratePer =  r.NextDouble();

        GeneratePer *= 100;

        if(GeneratePer < percent)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
