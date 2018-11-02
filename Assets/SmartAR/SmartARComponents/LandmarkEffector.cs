using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using smartar;

public class LandmarkEffector : LandmarkEffectorBase
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
        smartARController_.GetResult(targetEffector_.targetID, ref result_);
    }

    protected override smartar.Rotation GetScreenRotation()
    {
        smartar.Rotation screenRotation;
        smartARController_.screenDevice_.GetRotation(out screenRotation);
        return screenRotation;
    }

    protected override bool IsFlipX()
    {
        return smartARController_.isFlipX_;
    }

    protected override bool IsFlipY()
    {
        return smartARController_.isFlipY_;
    }

    protected override smartar.Rotation GetCameraRotation()
    {
        return smartARController_.cameraRotation_;
    }

    protected override void SetPose(Transform transformObject, smartar.RecognitionResult result, smartar.Vector3 rotPosition)
    {
		Transform lastRecognizedTransform = SmartAREffectorBase.GetLastRecognizedTransform();
		if (lastRecognizedTransform == null) 
        {
			return;
        }
		if (SmartAREffectorBase.IsLastRecognizedGameObject(targetEffector_.gameObject))
		{
			smartAREffector_.setPose(transformObject, result, rotPosition);
			transformObject.position = transformObject.position - lastRecognizedTransform.position;
        }
        else
        {
			if (smartAREffector_.UseFrontCamera ()) {
				if (smartAREffector_.GetImageSensorRotation() == smartar.Rotation.ROTATION_90) {
					// Nexus 6P,6,5X front camera
					rotPosition.x_ = -rotPosition.x_;
					rotPosition.y_ = -rotPosition.y_;
				}
				result.position_.x_ -= rotPosition.x_;
				result.position_.y_ += rotPosition.y_;
				result.position_.z_ -= rotPosition.z_;
            }
			else
            {
				result.position_.x_ -= rotPosition.x_;
				result.position_.y_ -= rotPosition.y_;
				result.position_.z_ -= rotPosition.z_;
            }
			smartar.Vector3 zeroVec = new smartar.Vector3();
			zeroVec.x_ = zeroVec.y_ = zeroVec.z_ = 0.0f;

			var targetTransform = new GameObject().transform;
			smartAREffector_.setPose(targetTransform, result, zeroVec);
			targetTransform.rotation = lastRecognizedTransform.rotation * UnityEngine.Quaternion.Inverse(targetTransform.rotation);
			targetTransform.position = targetTransform.rotation * targetTransform.position;
			transformObject.position = lastRecognizedTransform.position - targetTransform.position;
			transformObject.rotation = targetTransform.rotation;
			Destroy(targetTransform.gameObject);
        }
    }
}
