using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

    public Vector3 DefaultPosition;
    public GameObject ThrowP;
    public List<GameObject> objBlocks = new List<GameObject>();
    public Text scoreLog;
    public Text scoreVal;
    public Text totalScore;
    public float RefreshTime;

    int count = 0;


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        count++;

        if (count >= RefreshTime*60)
        { 
            ScoreObjectDisper(scoreLog,scoreVal,totalScore);
            
            count = 0;
        }

    }


    public void AddBlockObject(GameObject block)
    {
        objBlocks.Add(block);
    }

    public List<GameObject> GetBlockList(){
        return objBlocks;
    }

    public void ScoreObjectDisper(Text log, Text Val,Text Sum)
    {
        float totalScore = 0;
        int itemCount = 0;
        log.text = null;
        foreach (var cube in objBlocks)
        {
            var objScore = ScoreCalculation(cube);
            itemCount ++;
            log.text += cube.name + " = " + objScore + "\n";
            totalScore += objScore;
        }
        Val.text = "Block数 = " + itemCount;
        Sum.text = "Total Score = " + totalScore;
        
    }

    void ExceptObjKiller(){
        var TP = ThrowP.GetComponent<ThrowPicker>();
        var TpObj = TP.CurrentThrowThing();

    }

   float ScoreCalculation(GameObject block)
    {
        var SoSed =  block.GetComponent<ScoreSender>();
        if(SoSed != null)
        {
            var objectPosition = block.transform.position;
            //var distance = (objectPosition.x - DefaultPosition.x) * (objectPosition.y - DefaultPosition.y);
            var heightPoint = ScoreProcess(DefaultPosition.y, SoSed.DefaulYSender(), objectPosition.y);
            var score =  SoSed.ObjectScore * heightPoint;
            return score;
        }
        else
        {
            return 0;
        }

    }

    float ScoreProcess(float baseY, float objDefY, float posY)
    {
        var score = ((posY - baseY) / objDefY) - 1;
        if ((1000f > score) && (score > 0f) )
        {
            return score;
        }
        else
        {
            return 0f;
        }

    }

}
