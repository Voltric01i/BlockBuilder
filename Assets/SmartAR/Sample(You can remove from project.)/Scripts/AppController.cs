using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AppController : AppControllerBase
{
    [SerializeField]
    protected SmartARController smartARController;

#if UNITY_ANDROID && !UNITY_EDITOR
    private static string labelText_;
    private static int apiLevel_ = 1;
    private static int hwFeature_ = 2;
    private static bool canChangeNewAPI_ = false;
    private static bool success_ = false;
#endif
    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        // Escape key is application quit.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (smartARController.smart_.isConstructorFailed()) { return; }
        if (smartARController.targets_ == null) { return; }

        // For Frame
        foreach (var target in smartARController.targets_)
        {
            foreach (var effector in effectors_)
            {
                if (effector.result_.isRecognized_)
                {
                    if (effector.targetID == null)
                    {
                        var child = effector.transform.Find("Frame");
                        if (child == null) { return; }
                        child.localScale = new Vector3(300f, 150f, 300f);
                    }
                    else if (effector.result_.target_ == target.self_ && !smartARController.isLoadSceneMap_)
                    {
                        var size = new smartar.Vector2();
                        target.GetPhysicalSize(out size);
                        var child = effector.transform.Find("Frame");
                        var nearClipPlane = Camera.main.nearClipPlane * 1000;
                        if (child == null) { return; }
                        child.localScale = new Vector3(size.x_ * nearClipPlane, 150f, size.y_ * nearClipPlane);
                    }
                }
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        labelText_ = stringBuilder_.ToString();
#endif
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        smartCheckError(smartARController);

        if (smartARController.smart_.isConstructorFailed()) { return; }

        if (GUI.Button(new Rect(0, Screen.height - 100, 200, 100), "Reset"))
        {
            smartARController.resetController();
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if (smartARController.enabled_)
        {
            GUI.Label(new Rect(0, 0, 1000, 1000), labelText_);
            smartARController.getAndroidCameraFeature(out apiLevel_, out hwFeature_, out canChangeNewAPI_);
            updateLabelText(apiLevel_, hwFeature_, canChangeNewAPI_);
            
            if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 100, 200, 100), "API:1<->2"))
            {
                success_ = smartARController.changeAndroidCameraAPI(out apiLevel_, out hwFeature_, out canChangeNewAPI_);
                updateLabelText(apiLevel_, hwFeature_, canChangeNewAPI_);
    }
        }

#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    System.Text.StringBuilder stringBuilder_ = new System.Text.StringBuilder();
    private void updateLabelText(int apiLevel, int hwFeature, bool canChangeNewAPI)
    {
        stringBuilder_.Length = 0;

        // add for camera api 2.0
        stringBuilder_.Append("Android Camera API Level: ");
        switch (apiLevel)
        {
            case 1:
                stringBuilder_.Append("1");
                break;
            case 2:
                stringBuilder_.Append("2");
                break;
        }

        stringBuilder_.Append("\nHW support: ");
        switch (hwFeature)
        {
            case 0:
                stringBuilder_.Append("LIMITED");
                break;
            case 1:
                stringBuilder_.Append("FULL");
                break;
			case 2:
				stringBuilder_.Append("LEGACY");
				break;
			case 3:
				stringBuilder_.Append("LEVEL_3");
				break;
			default:
				stringBuilder_.Append("INVALID");
				break;
        }

        stringBuilder_.Append("\n");
        if (canChangeNewAPI)
        {
            stringBuilder_.Append("can use camera2 api");
        }
        else {
            stringBuilder_.Append("can't use camera2 api");
        }
    }
#endif

}
