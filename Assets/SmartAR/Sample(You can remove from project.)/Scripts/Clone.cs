using UnityEngine;
using System.Collections;

public class Clone : MonoBehaviour
{
    public enum MoveMode
    {
        POSITION,
        TRANSRATE,
    }

    public enum Direction
    {
        X,
        Y,
        Z,
    }

    public GameObject cloneTarget;
    public MoveMode moveMode;
    public Direction direction;
    public float distance;

    // Use this for initialization
    void Start()
    {
        var clone = Instantiate(cloneTarget) as GameObject;
        clone.transform.parent = transform;
        clone.GetComponent<Renderer>().material.color = Color.blue;
        clone.transform.localScale = cloneTarget.transform.localScale;
        clone.transform.position = cloneTarget.transform.position;

        switch (moveMode)
        {
            case MoveMode.POSITION:
                switch (direction)
                {
                    case Direction.X:
                        clone.transform.position = new Vector3(distance, 0, 0);
                        break;
                        
                    case Direction.Y:
                        clone.transform.position = new Vector3(0, distance, 0);
                        break;
                        
                    case Direction.Z:
                        clone.transform.position = new Vector3(0, 0, distance);
                        break;
                }
                break;

            case MoveMode.TRANSRATE:
                switch (direction)
                {
                    case Direction.X:
                        clone.transform.Translate(distance, 0, 0);
                        break;

                    case Direction.Y:
                        clone.transform.Translate(0, distance, 0);
                        break;

                    case Direction.Z:
                        clone.transform.Translate(0, 0, distance);
                        break;
                }
                break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
    }
}
