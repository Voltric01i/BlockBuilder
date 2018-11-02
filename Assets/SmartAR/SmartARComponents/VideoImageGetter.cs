using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

public class VideoImageGetter : MonoBehaviour {

	private Texture2D texture_;
	private Color32[] pixels_;
	private GCHandle pixels_handle_;
	private IntPtr image;
	private SmartARController smartARController_;
#pragma warning disable 414
	private ulong timestamp_;
#pragma warning restore 414

	void DoEnable() {
		// Find SmartARController
		var controllers = (SmartARController[]) FindObjectsOfType(typeof(SmartARController));
		if (controllers != null && controllers.Length > 0) {
			smartARController_ = controllers[0];
		}
	}

	void OnGUI() {
		DoEnable();
	}

	// Use this for initialization
	void Start() {
		//Debug.Log("VideoImageGetter.Start()");
		DoEnable();
		texture_ = new Texture2D(640, 480, TextureFormat.RGBA32, false);
		// need to flip
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		pixels_ = texture_.GetPixels32();
		pixels_handle_ = GCHandle.Alloc(pixels_, GCHandleType.Pinned);
		image = pixels_handle_.AddrOfPinnedObject();
		GetComponent<Renderer>().material.mainTexture = texture_;	
	}
	
	// Update is called once per frame
	void Update () {
        if (smartARController_.smart_.isConstructorFailed())
        {
            gameObject.SetActive(false);
            return;
        }

		if (smartARController_ != null && image != IntPtr.Zero && smartARController_.enabled) {
			smartARController_.getImage(image, out timestamp_);
			if (image == IntPtr.Zero) {
				return;
			}

			if (texture_ != null) {
				texture_.SetPixels32(pixels_);
				texture_.Apply();
				transform.Rotate(0, 10*Time.deltaTime, 0);
			}
		}
	}

	void OnApplicationPause (bool pause) {
		if (pause) {
			//Debug.Log ("VideoImageGetter.OnApplicationPause(true)");
			pixels_handle_.Free ();
		} else {
			pixels_handle_ = GCHandle.Alloc(pixels_, GCHandleType.Pinned);
			image = pixels_handle_.AddrOfPinnedObject();
		}
	}

	void OnApplicationQuit()
	{
		//Debug.Log("VideoImageGetter.OnApplicationQuit(true)");
	}
}
