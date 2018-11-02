using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using smartar;

public class SmartAREffector : SmartAREffectorBase
{
    [HideInInspector]
    public SmartARController smartARController_;

    protected override void Awake()
    {
        smartARController_ = FindObjectsOfType<SmartARController>()[0];
        base.Awake();
    }

    protected override void Update()
    {
        if (smartARController_ == null) { return; }
        if (!smartARController_.enabled_) { return; }
        if (smartARController_.smart_.isConstructorFailed()) { return; }

        var targetEffectors = FindObjectsOfType<TargetEffector>();
        if (targetEffectors == null || targetEffectors.Length <= 0) { return; }

        for (int i = 0; i < targetEffectors.Length; i++)
        {
            smartARController_.GetResult(targetEffectors[i].targetID, ref targetEffectors[i].result_);

            if (targetEffectors[i].result_.isRecognized_)
            {
                if (lastRecognizedObject_ == null || SmartAREffectorBase.IsLastRecognizedGameObject(targetEffectors[i].gameObject))
                {
                    targetEffectors[i].transform.position = new UnityEngine.Vector3(0, 0, 0);
                    targetEffectors[i].transform.rotation = UnityEngine.Quaternion.identity;
                    setPose(smartARController_.transform, targetEffectors[i].result_);
                    lastRecognizedTransform_ = smartARController_.transform;
                    lastRecognizedObject_ = targetEffectors[i].gameObject;
                }
                else
                {
                    var targetTransform = new GameObject().transform;
                    setPose(targetTransform, targetEffectors[i].result_);
                    targetTransform.rotation = lastRecognizedTransform_.rotation * UnityEngine.Quaternion.Inverse(targetTransform.rotation);
                    targetTransform.position = targetTransform.rotation * targetTransform.position;
                    targetEffectors[i].transform.position = lastRecognizedTransform_.position - targetTransform.position;
                    targetEffectors[i].transform.rotation = targetTransform.rotation;
                    Destroy(targetTransform.gameObject);
                }
            }
            else
            {
                if ( SmartAREffectorBase.IsLastRecognizedGameObject(targetEffectors[i].gameObject))
                {
                    ClearLastRecognized();
                }
            }
        }
    }

	protected override void callAdjustPose(smartar.Vector3 srcPosition, smartar.Quaternion srcRotation, out smartar.Vector3 rotPosition, out smartar.Quaternion rotRotation)
    {
        smartar.Rotation screenRotation = GetScreenRotation();
        smartar.Rotation cameraRotation = GetCameraRotation();
        SmartARController.adjustPose(cameraRotation, screenRotation, IsFlipX(), IsFlipY(), srcPosition, srcRotation, out rotPosition, out rotRotation);
    }

    public override smartar.Rotation GetCameraRotation()
    {
        smartar.Rotation cameraRotation = smartar.Rotation.ROTATION_0;
        if (smartARController_ != null)
        {
            smartARController_.cameraDevice_.GetRotation(out cameraRotation);
#if UNITY_ANDROID && !UNITY_EDITOR
			smartar.Facing facing = smartar.Facing.FACING_BACK;
			smartARController_.cameraDevice_.GetFacing(out facing);
			if (facing == smartar.Facing.FACING_FRONT)
			{
				smartar.Rotation imageSensorRotation = smartar.Rotation.ROTATION_0;
				smartARController_.cameraDevice_.GetImageSensorRotation(out imageSensorRotation);
				if (imageSensorRotation == smartar.Rotation.ROTATION_90)
				{
					cameraRotation = (smartar.Rotation)((360 + (int)cameraRotation - 180) % 360);
				}	
			}
#endif
        }
        return cameraRotation;
    }

    public override smartar.Rotation GetScreenRotation()
    {
		smartar.Rotation screenRotation;
		smartARController_.screenDevice_.GetRotation(out screenRotation);
		return screenRotation;
    }

    public override smartar.Rotation GetImageSensorRotation()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (smartARController_ != null)
		{
			smartar.Rotation imageSensorRotation = smartar.Rotation.ROTATION_0;
			smartARController_.cameraDevice_.GetImageSensorRotation(out imageSensorRotation);
			return imageSensorRotation;
		}
		else 
#endif
        {
            return smartar.Rotation.ROTATION_0;
        }
    }

    public override bool IsFlipX()
    {
        if (smartARController_ != null)
        {
            return smartARController_.isFlipX_;
        }
        return false;
    }

    public override bool IsFlipY()
    {
        if (smartARController_ != null)
        {
            return smartARController_.isFlipY_;
        }
        return false;
    }

    public override bool UseFrontCamera()
    {
        if (smartARController_ != null)
        {
            smartar.Facing facing;
            smartARController_.cameraDevice_.GetFacing(out facing);
            return (facing == smartar.Facing.FACING_FRONT);
        }
        return false;
    }
}
