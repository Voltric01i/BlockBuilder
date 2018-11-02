using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public abstract class SmartAREffectorBase : MonoBehaviour
{
    [HideInInspector]
    public float nearClipPlane_;

    [SerializeField]
    protected float _transformScaleFactor = 100f;
	protected static Transform lastRecognizedTransform_ = null;
    protected static GameObject lastRecognizedObject_ = null;

	public static Transform GetLastRecognizedTransform()
	{
		return lastRecognizedTransform_;
	}

    public static void ClearLastRecognized()
    {
        lastRecognizedTransform_ = null;
        lastRecognizedObject_ = null;
    }

    public static bool IsLastRecognizedGameObject(GameObject gameObject)
    {
        if (lastRecognizedObject_ == null || gameObject == null)
        {
            return false;
        }
        return (lastRecognizedObject_.GetInstanceID() == gameObject.GetInstanceID());
    }

    protected virtual void Awake()
    {
        nearClipPlane_ = 20 * _transformScaleFactor;
    }

	protected void setPose(Transform transformObject, smartar.RecognitionResult result)
    {
        smartar.Vector3 zeroPos;
        zeroPos.x_ = 0f;
        zeroPos.y_ = 0f;
        zeroPos.z_ = 0f;
        setPose(transformObject, result, zeroPos);
    }

    public void setPose(Transform transformObject, smartar.RecognitionResult result, smartar.Vector3 landmarkPos)
    {
        smartar.Vector3 rotPosition;
        smartar.Quaternion rotRotation;
        callAdjustPose(result.position_, result.rotation_, out rotPosition, out rotRotation);

        transformObject.rotation = Quaternion.identity;
        transformObject.RotateAround(Vector3.zero, Vector3.right, -90);

        var q = new Quaternion(rotRotation.x_, rotRotation.z_, rotRotation.y_, rotRotation.w_);
        float angle;
        Vector3 axis;
        q.ToAngleAxis(out angle, out axis);
        transformObject.RotateAround(Vector3.zero, axis, angle);

        transformObject.position = new Vector3(
            (rotPosition.x_ + landmarkPos.x_) * nearClipPlane_,
            (rotPosition.z_ + landmarkPos.z_) * nearClipPlane_,
            (rotPosition.y_ + landmarkPos.y_) * nearClipPlane_);
    }
    
    protected abstract void callAdjustPose(smartar.Vector3 srcPosition, smartar.Quaternion srcRotation, out smartar.Vector3 rotPosition, out smartar.Quaternion rotRotation);
    protected abstract void Update();
    public abstract smartar.Rotation GetCameraRotation();
    public abstract smartar.Rotation GetScreenRotation();
    public abstract smartar.Rotation GetImageSensorRotation();
    public abstract bool IsFlipX();
    public abstract bool IsFlipY();
    public abstract bool UseFrontCamera();
}
