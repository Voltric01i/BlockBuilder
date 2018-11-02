using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using smartar;

public class InitPointEffector : InitPointEffectorBase
{
    private SmartARController smartARController_;
    private SmartAREffector smartAREffector_;
    private TargetEffector targetEffector_;

    void DoEnable()
    {
        // Find SmartARController
        if (smartARController_ != null)
        {
            return;
        }
        var controllers = (SmartARController[])FindObjectsOfType(typeof(SmartARController));
        if (controllers != null && controllers.Length > 0)
        {
            smartARController_ = controllers[0];
        }

        // Find SmartARController
        if (smartAREffector_ != null)
        {
            return;
        }
        var smartAREffectors = (SmartAREffector[])FindObjectsOfType(typeof(SmartAREffector));
        if (smartAREffectors != null && smartAREffectors.Length > 0)
        {
            smartAREffector_ = smartAREffectors[0];
        }

        if (targetEffector_ != null)
        {
            return;
        }
        var targetEffectors = FindObjectsOfType<TargetEffector>();
        if (targetEffectors != null && targetEffectors.Length > 0)
        {
            targetEffector_ = targetEffectors[0];
        }
    }

    protected override void Start()
    {
        DoEnable();
        base.Start();
    }

    protected override void OnGUI()
    {
        DoEnable();
        base.OnGUI();
    }

    protected override void Update()
    {
        if (smartARController_ == null || smartAREffector_ == null)
        {
            return;
        }

        if (!smartARController_.enabled_)
        {
            return;
        }

        if (smartARController_.smart_.isConstructorFailed()) { return; }

        base.Update();
    }

    protected override void GetResult(ref RecognitionResult result)
        {
        smartARController_.GetResult(targetEffector_.targetID, ref result);
    }

    protected override UnityEngine.Vector2 GetVideoSize()
            {
        return smartARController_.cameraDeviceSettings_.videoImageSize;
                }

    protected override bool UseFrontCamera()
                {
        Facing facing;
        smartARController_.cameraDevice_.GetFacing(out facing);
        return (facing == Facing.FACING_FRONT);
                }

    protected override smartar.Rotation GetImageSensorRotation()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
		smartar.Rotation imageSensorRotation = smartar.Rotation.ROTATION_0;
		smartARController_.cameraDevice_.GetImageSensorRotation(out imageSensorRotation);
		return imageSensorRotation;
#else
        return smartar.Rotation.ROTATION_0;
#endif
    }
}
