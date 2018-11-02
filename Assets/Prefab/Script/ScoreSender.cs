using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSender : MonoBehaviour
{
    GameObject ScCo;
    ScoreController Sc;
    public float ObjectScore;
    public float ObjectDefaultY;
    // Use this for initialization

    void Start()
    {
        ScCo = GameObject.Find("ScoreController");
        Sc = ScCo.GetComponent<ScoreController>();

        Sc.AddBlockObject(this.gameObject);

    }
    // Update is called once per frame
    void Update()
    {

    }

    public float DefaulYSender()
    {
        return ObjectDefaultY;
    }

    public float ObjectScoreSender()
    {
        return ObjectScore;
    }
}
