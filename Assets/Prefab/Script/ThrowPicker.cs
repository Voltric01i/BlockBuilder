using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPicker : ThrowSystem {

    public float intervalTime;
    public GameObject[] ItemList;
    public float[] ItemRandPer;
    public GameObject Robot;
    //public GameObject RoomUI;
    //public GameObject PickerUI;
    public GameObject View;
    GameObject ThrowThing;
    Rigidbody ThrRb;
    GravityController GC;
    ScoreSender SC;
    Collider BC;

    float scale = 0f;

    ThrowSystem throw1;
    //bool Pickered = true;
    string throwInfo = null;
    int ReleaseCount = 0;
    bool Release = false;


    // Use this for initialization
    void Start () {
        throw1 = gameObject.AddComponent<ThrowSystem>();
        scale = Robot.transform.localScale.x * 5f;
        throw1.ThrowStart(View,scale);
        ItemGenerate(ItemRand());
    }

    private void Update()
    {
        if(ThrowThing != null && ThrRb != null){
            throwInfo = throw1.Move(ThrowThing, ThrRb, GC,SC,BC);           
        }

    
        //Debug.Log(throwInfo);
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (throwInfo == "Release")
        {
            Release =  true;
        }

        if (Release ==  true)
        {
            ReleaseCount++;
        }

        //Debug.Log(ReleaseCount);
        if (ReleaseCount >= intervalTime * 60)
        {
            Release = false;
            ReleaseCount = 0;
            ItemGenerate(ItemRand());
            //Debug.Log(" Generate");
        }
	}

    public void Back()
    {
        Destroy(ThrowThing);
        ThrowThing = null;
        ThrRb = null;
        throwInfo = "Release";
    }

    void Generate(GameObject tT)
    {
        //throw1.DefaultPos(tT);
        tT.transform.localScale = new Vector3(scale * tT.transform.localScale.x, scale * tT.transform.localScale.y, scale * tT.transform.localScale.z);
        throw1.MoveEnable();
    }

    public void BlueCube()
    {
        ThrowThing = Instantiate(ItemList[0]) as GameObject;
        throw1.DefaultPos(ThrowThing);
        ThrRb = ThrowThing.GetComponent<Rigidbody>();
        GC = ThrowThing.GetComponent<GravityController>();
        SC = ThrowThing.GetComponent<ScoreSender>();
        BC = ThrowThing.GetComponent<BoxCollider>();
        Generate(ThrowThing);
    }
    public void RedCube()
    {
        ThrowThing = Instantiate(ItemList[1]) as GameObject;
        throw1.DefaultPos(ThrowThing);
        ThrRb = ThrowThing.GetComponent<Rigidbody>();
        GC = ThrowThing.GetComponent<GravityController>();
        SC = ThrowThing.GetComponent<ScoreSender>();
        BC = ThrowThing.GetComponent<BoxCollider>();
        Generate(ThrowThing);
    }
    public void YellowCube()
    {
        ThrowThing = Instantiate(ItemList[2]) as GameObject;
        throw1.DefaultPos(ThrowThing);
        ThrRb = ThrowThing.GetComponent<Rigidbody>();
        GC = ThrowThing.GetComponent<GravityController>();
        SC = ThrowThing.GetComponent<ScoreSender>();
        BC = ThrowThing.GetComponent<BoxCollider>();
        Generate(ThrowThing);
    }

    public GameObject CurrentThrowThing(){
        return ThrowThing;
    }


    int ItemRand(){
        System.Random r = new System.Random();
        float[] RandPer = new float[ItemRandPer.Length];
        float sum = 0f;
        float previouslyVal = 0f;
        int num = 0;
        int cnt;
        for(cnt = 0;cnt < RandPer.Length;cnt++){
            RandPer[cnt] = ItemRandPer[cnt] + sum;
            sum += ItemRandPer[cnt];
        }
        var RandVal =  Random.value * 100f;
        for(cnt = 0;cnt < RandPer.Length;cnt++){
            if(previouslyVal <= RandVal && RandVal <= RandPer[cnt]){
                num = cnt;
            }
            previouslyVal = RandPer[cnt];
        }
        return num;
        
    }

    public void RestertItemCreate(){
        StartCoroutine(Create());
    }
    IEnumerator Create(){
        yield return new WaitForSeconds(1);
        ItemGenerate(ItemRand());
    }

    void ItemGenerate(int pic)
    {
        switch (pic) {

            case 0:
                BlueCube();
                break;
            case 1:
                RedCube();
                break;
            case 2:
                YellowCube();
            break;
            default:
                break;
        }
 

    }
}
