using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public abstract class LandmarkEffectorBase : MonoBehaviour
{
    public GameObject sphere_;
    public bool showLandmarks = false;

    protected IntPtr landmarkBuffer_ = IntPtr.Zero;
    protected IntPtr nodePointBuffer_ = IntPtr.Zero;
    protected smartar.RecognitionResult result_;
    protected GameObject[] landmarkObjects_ = new GameObject[smartar.Recognizer.MAX_NUM_LANDMARKS];
    protected GameObject[] nodePointObjects_ = new GameObject[smartar.Recognizer.MAX_NUM_NODE_POINTS];

    void Awake()
    {
        landmarkBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.Landmark)) * smartar.Recognizer.MAX_NUM_LANDMARKS);
        nodePointBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.NodePoint)) * smartar.Recognizer.MAX_NUM_NODE_POINTS);
        for (int i = 0; i < smartar.Recognizer.MAX_NUM_LANDMARKS; i++)
        {
            landmarkObjects_[i] = (GameObject)Instantiate(sphere_, new Vector3(), Quaternion.identity);
            landmarkObjects_[i].transform.localScale = new Vector3(20f, 20f, 20f);
            landmarkObjects_[i].SetActive(false);
            landmarkObjects_[i].transform.parent = transform;
        }
        for (int i = 0; i < smartar.Recognizer.MAX_NUM_NODE_POINTS; i++)
        {
            nodePointObjects_[i] = (GameObject)Instantiate(sphere_, new Vector3(), Quaternion.identity);
            nodePointObjects_[i].transform.localScale = new Vector3(18f, 18f, 18f);
            nodePointObjects_[i].SetActive(false);
            nodePointObjects_[i].transform.parent = transform;
        }
    }

    protected virtual void Start()
    {
        result_ = new smartar.RecognitionResult();
        result_.maxLandmarks_ = smartar.Recognizer.MAX_NUM_LANDMARKS;
        result_.landmarks_ = landmarkBuffer_;
        result_.maxNodePoints_ = smartar.Recognizer.MAX_NUM_NODE_POINTS;
        result_.nodePoints_ = nodePointBuffer_;
    }

    protected virtual void OnGUI()
    {
    }

    protected abstract void GetResult(ref smartar.RecognitionResult result);
    protected abstract smartar.Rotation GetScreenRotation();
    protected abstract smartar.Rotation GetCameraRotation();
    protected abstract bool IsFlipX();
    protected abstract bool IsFlipY();
    protected abstract void SetPose(Transform transformObject, smartar.RecognitionResult result, smartar.Vector3 rotPosition);

    protected virtual void Update()
    {

        if (!showLandmarks)
        {
            disableLandmarkAndNodes();
            return;
        }

        // Get recognition result
        GetResult(ref result_);

        // Set pose
        if (result_.isRecognized_)
        {
            smartar.Vector3 rotPosition;
            smartar.Quaternion rotRotation;
            smartar.Rotation screenRotation = GetScreenRotation();
            smartar.Rotation cameraRotation = GetCameraRotation();
            SmartARController.adjustPose(
                cameraRotation, screenRotation, IsFlipX(), IsFlipY(),
                result_.position_, result_.rotation_, out rotPosition, out rotRotation);

            // Draw nodePoints in unity
            if (result_.numNodePoints_ > 0)
            {
                IntPtr nodePointPtr = result_.nodePoints_;

                // for drawing
                Color nodePointColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                for (int i = 0; i < result_.maxNodePoints_; i++)
                {
                    // get a current nodePoint
                    smartar.NodePoint curNodePoint = (smartar.NodePoint)Marshal.PtrToStructure(nodePointPtr, typeof(smartar.NodePoint));

                    if (i < result_.numNodePoints_)
                    {
                        // set position and rotation for nodePoint
                        smartar.Vector3 rotNodePosition;
                        smartar.Quaternion rotNodeRotation;
                        SmartARController.adjustPose(
                            GetCameraRotation(), screenRotation, IsFlipX(), IsFlipY(),
                            curNodePoint.position_, result_.rotation_, out rotNodePosition, out rotNodeRotation);
                        SetPose(nodePointObjects_[i].transform, result_, rotNodePosition);
                        nodePointObjects_[i].GetComponent<Renderer>().material.SetColor("_Color", nodePointColor);
                        nodePointObjects_[i].SetActive(true);
                        //Debug.Log ("nodePointObjects_[" + i + "].transform.position = " + nodePointObjects_[i].transform.position);
                    }
                    else
                    {
                        nodePointObjects_[i].SetActive(false);
                    }

                    // go to a next ptr
                    nodePointPtr = new IntPtr(nodePointPtr.ToInt64() + (Int64)Marshal.SizeOf(curNodePoint));
                }
            }

            // Draw landmarks in unity
            if (result_.numLandmarks_ > 0)
            {
                IntPtr landmarkPtr = result_.landmarks_;

                // for drawing
                Color landmarkColor;

                for (int i = 0; i < result_.maxLandmarks_; i++)
                {
                    // get a current landmark
                    smartar.Landmark curLandmark = (smartar.Landmark)Marshal.PtrToStructure(landmarkPtr, typeof(smartar.Landmark));

                    if (i < result_.numLandmarks_)
                    {
                        // set position and rotation for landmarks
                        smartar.Vector3 rotLandmarkPosition;
                        smartar.Quaternion rotLandmarkRotation;
                        SmartARController.adjustPose(
                            GetCameraRotation(), screenRotation, IsFlipX(), IsFlipY(),
                            curLandmark.position_, result_.rotation_, out rotLandmarkPosition, out rotLandmarkRotation);
                        SetPose(landmarkObjects_[i].transform, result_, rotLandmarkPosition);

                        // set color
                        switch (curLandmark.state_)
                        {
                            case smartar.LandmarkState.LANDMARK_STATE_TRACKED:
                                landmarkColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                                break;
                            case smartar.LandmarkState.LANDMARK_STATE_LOST:
                                landmarkColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                                break;
                            case smartar.LandmarkState.LANDMARK_STATE_SUSPENDED:
                                landmarkColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                                break;
                            case smartar.LandmarkState.LANDMARK_STATE_MASKED:
                                landmarkColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                                break;
                            default:
                                landmarkColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                                break;
                        }
                        landmarkObjects_[i].GetComponent<Renderer>().material.SetColor("_Color", landmarkColor);
                        landmarkObjects_[i].SetActive(true);
                        //Debug.Log ("landmarkObjects_[" + i + "].transform.position = " + landmarkObjects_[i].transform.position);
                    }
                    else
                    {
                        landmarkObjects_[i].SetActive(false);
                    }

                    // go to a next ptr
                    landmarkPtr = new IntPtr(landmarkPtr.ToInt64() + (Int64)Marshal.SizeOf(curLandmark));
                }
            }

        }
        else
        {
            disableLandmarkAndNodes();
        }
    }
    private void disableLandmarkAndNodes()
    {
        for (int i = 0; i < smartar.Recognizer.MAX_NUM_LANDMARKS; i++)
        {
            landmarkObjects_[i].SetActive(false);
        }
        for (int i = 0; i < smartar.Recognizer.MAX_NUM_NODE_POINTS; i++)
        {
            nodePointObjects_[i].SetActive(false);
        }
    }
}
