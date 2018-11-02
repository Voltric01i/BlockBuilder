using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {

    public Vector3 localGravity;
    Quaternion thisRot;
    public bool yZeroStop;
    bool once = false;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
	
    void FixedUpdate() {

        if ((yZeroStop == true)&&(this.transform.position.y <=0f) && (once == false))
        {
            rb.velocity = Vector3.zero;
            thisRot =this.transform.rotation;
            once = true;
        }

        if(once == false)
        {
            SetLocalGravity();
        }
        if (once)
        {
            this.transform.position = new Vector3(this.transform.position.x,0,this.transform.position.z);
            this.transform.rotation = thisRot;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}


    void SetLocalGravity()
    {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }
}
