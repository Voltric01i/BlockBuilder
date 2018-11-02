using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public abstract class InitPointEffectorBase : MonoBehaviour
{
    public Texture image;
    public bool showInitPoint = false;
    public bool showInitPointId = false;

    private IntPtr initPointBuffer_ = IntPtr.Zero;
    private smartar.RecognitionResult result_;
    private GUIStyle style_;
    private GUIStyleState styleState_;

    private struct initPointPos
    {
        public uint id_;
        public Vector2 adjustedScreenPos_;
    }

    private initPointPos[] initPointIDs_ = new initPointPos[smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS];

    void Awake()
    {
        initPointBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.InitPoint)) * smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS);
    }

    protected virtual void Start()
    {
        result_ = new smartar.RecognitionResult();
        result_.maxInitPoints_ = smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS;
        result_.initPoints_ = initPointBuffer_;

        style_ = new GUIStyle();
        style_.fontSize = 30;

        styleState_ = new GUIStyleState();
        styleState_.textColor = Color.yellow;
    }

    protected virtual void OnGUI()
    {
        // show initpoint id
        for (int i = 0; i < result_.numInitPoints_; i++) {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            if (showInitPointId)
            {
                Rect rect = new Rect(initPointIDs_[i].adjustedScreenPos_.x, screenSize.y - initPointIDs_[i].adjustedScreenPos_.y, 100, 50);
                style_.normal = styleState_;
                GUI.Label(rect, initPointIDs_[i].id_.ToString(), style_);
            }

            if (showInitPoint)
            {
                Rect rect2 = new Rect(initPointIDs_[i].adjustedScreenPos_.x - 5, screenSize.y - initPointIDs_[i].adjustedScreenPos_.y - 5, 10, 10);
                GUI.DrawTexture(rect2, image);
            }
        }
    }

    protected abstract void GetResult(ref smartar.RecognitionResult result);
    protected abstract Vector2 GetVideoSize();
	protected abstract bool UseFrontCamera();
	protected abstract smartar.Rotation GetImageSensorRotation();

    protected virtual void Update()
    {
		if (!showInitPoint && !showInitPointId)
        {
            return;
        }

        // Get recognition result
        GetResult(ref result_);

        // Draw initPoints in unity
        if (result_.numInitPoints_ > 0)
        {
            IntPtr initPointPtr = result_.initPoints_;

            // for drawing
            for (int i = 0; i < result_.numInitPoints_; i++)
            {
                // get a current initPoint
                smartar.InitPoint curInitPoint = (smartar.InitPoint)Marshal.PtrToStructure(initPointPtr, typeof(smartar.InitPoint));
                initPointIDs_[i].id_ = curInitPoint.id_;

                // Scaling
                Vector2 videoSize = GetVideoSize();
                Vector2 adjustedScreenSize;
                if (Screen.width < Screen.height)
                {
                    adjustedScreenSize = new Vector2(Screen.height, Screen.width);
                }
                else
                {
                    adjustedScreenSize = new Vector2(Screen.width, Screen.height);
                }
                float adjustRatio = (float)adjustedScreenSize.x / (float)videoSize.x;
                float adjustHeight = ((float)videoSize.y * (float)adjustRatio - (float)adjustedScreenSize.y) / 2.0f;
                initPointIDs_[i].adjustedScreenPos_.x = curInitPoint.position_.x_ * adjustRatio;
                initPointIDs_[i].adjustedScreenPos_.y = curInitPoint.position_.y_ * adjustRatio - adjustHeight;

                if (UseFrontCamera())
                {
#if UNITY_ANDROID && !UNITY_EDITOR
					initPointIDs_[i].adjustedScreenPos_.x = adjustedScreenSize.x - initPointIDs_[i].adjustedScreenPos_.x;
					if (GetImageSensorRotation() == smartar.Rotation.ROTATION_90)
					{
						// rorate 180 degree for Nexus 6P, etc
						initPointIDs_[i].adjustedScreenPos_.x = adjustedScreenSize.x - initPointIDs_[i].adjustedScreenPos_.x;
						initPointIDs_[i].adjustedScreenPos_.y = adjustedScreenSize.y - initPointIDs_[i].adjustedScreenPos_.y;
					}
#endif
#if UNITY_IOS && !UNITY_EDITOR
					initPointIDs_[i].adjustedScreenPos_.y = adjustedScreenSize.y - initPointIDs_[i].adjustedScreenPos_.y;
#endif
                }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
				// Adjust screen orientation
				float xTemp = initPointIDs_[i].adjustedScreenPos_.x;
				float yTemp = initPointIDs_[i].adjustedScreenPos_.y;
                switch (Screen.orientation)
				{
				case ScreenOrientation.LandscapeLeft:
					break;
				case ScreenOrientation.Portrait:
					xTemp = initPointIDs_[i].adjustedScreenPos_.y;
					yTemp = -initPointIDs_[i].adjustedScreenPos_.x;
					initPointIDs_[i].adjustedScreenPos_.x = xTemp;
					initPointIDs_[i].adjustedScreenPos_.y = adjustedScreenSize.x + yTemp;
					break;
				case ScreenOrientation.LandscapeRight:
					xTemp = initPointIDs_[i].adjustedScreenPos_.x;
					yTemp = initPointIDs_[i].adjustedScreenPos_.y;
					initPointIDs_[i].adjustedScreenPos_.x = adjustedScreenSize.x - xTemp;
					initPointIDs_[i].adjustedScreenPos_.y = adjustedScreenSize.y - yTemp;
					break;
				case ScreenOrientation.PortraitUpsideDown:
					xTemp = -initPointIDs_[i].adjustedScreenPos_.y;
					yTemp = initPointIDs_[i].adjustedScreenPos_.x;
					initPointIDs_[i].adjustedScreenPos_.x = adjustedScreenSize.y + xTemp;
					initPointIDs_[i].adjustedScreenPos_.y = yTemp;
					break;
				}
#endif

                // go to a next ptr
                initPointPtr = new IntPtr(initPointPtr.ToInt64() + (Int64)Marshal.SizeOf(curInitPoint));
            }
        }
    }
}
