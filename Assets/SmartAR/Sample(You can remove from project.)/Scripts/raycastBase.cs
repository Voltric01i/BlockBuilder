using UnityEngine;
using System.Collections;

public class raycastBase : MonoBehaviour
{
    public Camera cam;
    public GameObject sp;
    public Transform ARtarget;
    private Ray r;
    private RaycastHit rhit = new RaycastHit();
	private TargetEffector targetEffector_ = null;

	void Start()
	{
		targetEffector_ = FindObjectsOfType<TargetEffector>()[0];
	}

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                if (Input.touches [0].phase == TouchPhase.Began)
                {
                    r = this.cam.ScreenPointToRay(Input.touches [0].position);
                    attachSphere();
                }
            }
        } else
        {
            if (Input.GetMouseButtonDown(0))
            {
                r = this.cam.ScreenPointToRay(Input.mousePosition);
                attachSphere();
            }
        }
    }

    private void attachSphere()
    {
		if (Physics.Raycast(r, out rhit, 10000, 1 << 8) && targetEffector_.result_.isRecognized_)
        {
            GameObject obj = (GameObject)Instantiate(sp);
            obj.transform.parent = ARtarget;
            obj.transform.localScale = sp.transform.localScale;
            obj.transform.position = rhit.point;
        }
    }
}
