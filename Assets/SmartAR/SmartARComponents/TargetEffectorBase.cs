using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public abstract class TargetEffectorBase : MonoBehaviour
{
    public string targetID;

    [HideInInspector]
    public smartar.RecognitionResult result_;

    [SerializeField]
    protected int m_LostPermissionCount = 0;

    protected IntPtr landmarkBuffer_ = IntPtr.Zero;
    protected IntPtr initPointBuffer_ = IntPtr.Zero;
    protected int m_LostCount = int.MinValue;

    protected virtual void Awake()
    {
        landmarkBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.Landmark)) * smartar.Recognizer.MAX_NUM_LANDMARKS);
        initPointBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.InitPoint)) * smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS);
    }

    protected virtual void Start()
    {
        result_ = new smartar.RecognitionResult();
        result_.maxLandmarks_ = smartar.Recognizer.MAX_NUM_LANDMARKS;
        result_.landmarks_ = landmarkBuffer_;
        result_.maxInitPoints_ = smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS;
        result_.initPoints_ = initPointBuffer_;

        showOrHideChildrens(false);
    }

    protected virtual void Update()
    {
        if (result_.isRecognized_)
        {
            m_LostCount = 0;
        }
        else
        {
            if (m_LostCount == int.MinValue) { return; }
            ++m_LostCount;
        }
        var isShown = m_LostCount <= m_LostPermissionCount;
        showOrHideChildrens(isShown);
    }

    void OnDestroy()
    {
        if (landmarkBuffer_ == IntPtr.Zero) { return; }
        Marshal.FreeCoTaskMem(landmarkBuffer_);
        landmarkBuffer_ = IntPtr.Zero;
        Marshal.FreeCoTaskMem(initPointBuffer_);
        initPointBuffer_ = IntPtr.Zero;
    }

    void OnValidate()
    {
        m_LostPermissionCount = m_LostPermissionCount < 0 ? 0 : m_LostPermissionCount;
    }

    protected virtual void showOrHideChildrens(bool enabled)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = enabled;
        }
    }
}
