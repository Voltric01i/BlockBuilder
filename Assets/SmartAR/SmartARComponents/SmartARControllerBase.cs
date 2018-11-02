using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

public abstract class SmartARControllerBase : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IOS
    public string licenseFileName;
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
    public string licenseKeyString;
#endif

    [System.Serializable]
    public class TargetEntry
    {
        public string fileName;
        public string id;
    }

    [System.Serializable]
    public class RecognizerSettings
    {
        public smartar.RecognitionMode recognitionMode = smartar.RecognitionMode.RECOGNITION_MODE_TARGET_TRACKING;
        public smartar.SceneMappingInitMode sceneMappingInitMode = smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET;
        public TargetEntry[] targets = { };
        public int maxTargetsPerFrame = 1;
        public smartar.SearchPolicy searchPolicy = smartar.SearchPolicy.SEARCH_POLICY_PRECISIVE;
        public int maxTriangulateMasks = 0;
        public smartar.DenseMapMode denseMapMode = smartar.DenseMapMode.DENSE_MAP_DISABLE;

        public bool useWorkerThread = true;
        public bool useInstantTarget = false;
    }

    // Fields configurable on inspector
    [SerializeField]
    protected RecognizerSettings recognizerSettings = new RecognizerSettings();
    protected RecognizerSettings orgRecognizerSettings = new RecognizerSettings();   // for save and load SceneMap

    // Public properties for runtime configuration
    public RecognizerSettings recognizerSettings_
    {
        get
        {
            return recognizerSettings;
        }
        set
        {
            ConfigRecognizer(value, false);
        }
    }

	// Public properties for orginal configuration
	public RecognizerSettings orgRecognizerSettings_
	{
		get
		{
			return orgRecognizerSettings;
		}
	}

    protected bool isRecognized_ = false;
    public bool isLastRecognized()
    {
        return isRecognized_;
    }

    public abstract smartar.Triangle2[] triangulateMasks_
    {
        set;
    }

    // Fields
    [HideInInspector]
    public IntPtr self_;
    [HideInInspector]
    public smartar.Smart smart_;
    [HideInInspector]
    public smartar.LandmarkDrawer landmarkDrawer_;
    [HideInInspector]
    public smartar.Recognizer recognizer_;
    [HideInInspector]
    public smartar.Target[] targets_;
    [HideInInspector]
    public smartar.Target[] onlineTargets_;

    protected int numWorkerThreads_;

    protected Thread[] workerThreads_;

    protected bool started_ = false;
    protected bool smartInitFailed_ = false;
    [HideInInspector]
    public bool enabled_ = false;

    [HideInInspector]
    public bool isLoadSceneMap_ = false;
    [HideInInspector]
    protected string sceneMapFilePath_;

    // Fields for LandmarkDrawer
    protected IntPtr landmarkBuffer_ = IntPtr.Zero;
    protected IntPtr nodePointBuffer_ = IntPtr.Zero;
    protected IntPtr initPointBuffer_ = IntPtr.Zero;

    // Fields for development
    protected ulong[] recogCountBuf_;
    protected ulong[] recogTimeBuf_;

    public abstract ulong[] recogCount_
    {
        get;
    }

    public abstract ulong[] recogTime_
    {
        get;
    }


    protected bool existsStreamingAsset(string fileName)
    {
        var path = Path.Combine(Application.streamingAssetsPath, fileName);
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var www = new WWW(path))
        {
            while (!www.isDone) {}
            return www.size > 0;
        }
#else
        return File.Exists(path);
#endif
    }

    private void ConfigRecognizer(RecognizerSettings newSettings, bool creating)
    {
        if (creating || newSettings.maxTargetsPerFrame != recognizerSettings.maxTargetsPerFrame)
        {
            recognizer_.SetMaxTargetsPerFrame(newSettings.maxTargetsPerFrame);
        }
        if (creating || newSettings.searchPolicy != recognizerSettings.searchPolicy)
        {
            recognizer_.SetSearchPolicy(newSettings.searchPolicy);
        }
        if (creating || newSettings.maxTriangulateMasks != recognizerSettings.maxTriangulateMasks)
        {
            recognizer_.SetMaxTriangulateMasks(newSettings.maxTriangulateMasks);
        }
        if (creating || newSettings.denseMapMode != recognizerSettings.denseMapMode)
        {
            recognizer_.SetDenseMapMode(newSettings.denseMapMode);
        }
        if (creating || !Enumerable.SequenceEqual(newSettings.targets, recognizerSettings.targets))
        {
            var oldTargets = targets_;

            targets_ = new smartar.Target[newSettings.targets.Length];
            for (int i = 0, num = newSettings.targets.Length; i < num; ++i)
            {
                string name = newSettings.targets[i].fileName;
                if (isLoadSceneMap_)
                {
                    var stream = new smartar.FileStreamIn(smart_, sceneMapFilePath_);
                    targets_[i] = new smartar.SceneMapTarget(smart_, stream);
                    stream.Close();
                }
                else
                {
                    if (name.EndsWith(".v9"))
                    {
                        name += ".dic";
                    }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    name = Application.streamingAssetsPath + "/" + name;
#endif

                    if (!existsStreamingAsset(name))
                    {
                        targets_ = null;
                        if (newSettings.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_TARGET_TRACKING
                            || (newSettings.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_SCENE_MAPPING
                                && newSettings.sceneMappingInitMode == smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET))
                        {
                            // TODO: show message
                        }

                        return;
                    }

                    smartar.AssetStreamIn stream = new smartar.AssetStreamIn(smart_, name);
                    if (name.EndsWith(".map", StringComparison.OrdinalIgnoreCase))
                    {
                        targets_[i] = new smartar.SceneMapTarget(smart_, stream);
                    }
                    else
                    {
                        targets_[i] = new smartar.LearnedImageTarget(smart_, stream);
                    }
                    stream.Close();
                }
            }

            recognizer_.SetTargets(targets_);

            if (oldTargets != null)
            {
                for (int i = 0, num = oldTargets.Length; i < num; ++i)
                {
                    ((IDisposable)oldTargets[i]).Dispose();
                }
            }
        }

        if (recognizerSettings.useInstantTarget)
        {
            var size = recognizerSettings.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_TARGET_TRACKING ? 2 : 1;
            onlineTargets_ = new smartar.Target[size];
        }

        recognizerSettings = newSettings;
    }

    protected virtual void DoCreate()
    {
        if (started_)
        {
            return;
        }

        // Create Smart
#if UNITY_ANDROID || UNITY_IOS
        smart_ = new smartar.Smart(licenseFileName);
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        smart_ = new smartar.Smart(licenseKeyString, (uint)licenseKeyString.Length);
#else
        smart_ = new smartar.Smart(null);
#endif
        smartInitFailed_ = smart_.isConstructorFailed();

        if (smartInitFailed_) { return; }

        // Create LandmarkDrawer
        landmarkDrawer_ = new smartar.LandmarkDrawer(smart_);

        // Create Recognizer
        recognizer_ = new smartar.Recognizer(smart_, recognizerSettings.recognitionMode, recognizerSettings.sceneMappingInitMode);
        ConfigRecognizer(recognizerSettings, true);

        // Create native instance
        if (smartar.Utility.isMultiCore() && recognizerSettings.useWorkerThread)
        {
            bool isTargetTracking = recognizerSettings.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_TARGET_TRACKING;
            numWorkerThreads_ = isTargetTracking ? 2 : 1;
        }
        else
        {
            numWorkerThreads_ = 0;
        }

        // Init fields for LandmarkDrawer
        landmarkBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.Landmark)) * smartar.Recognizer.MAX_NUM_LANDMARKS);
        nodePointBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.NodePoint)) * smartar.Recognizer.MAX_NUM_NODE_POINTS);
        initPointBuffer_ = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(smartar.InitPoint)) * smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS);

        // Init fields for development
        recogCountBuf_ = new ulong[numWorkerThreads_];
        recogTimeBuf_ = new ulong[numWorkerThreads_];

    }

    protected abstract void DoEnable();

    protected virtual void DoDisable()
    {
        if (!enabled_)
        {
            return;
        }
        enabled_ = false;

        if (smartInitFailed_) { return; }

        // Stop components
        landmarkDrawer_.Stop();
        recognizer_.Reset();

    }

    protected virtual void DoDestroy()
    {
        if (!started_)
        {
            return;
        }
        started_ = false;

        if (smartInitFailed_) { return; }

        // clear dics
        recognizer_.SetTargets(null);

        // Free fields for LandmarkDrawer
        Marshal.FreeCoTaskMem(landmarkBuffer_);
        landmarkBuffer_ = IntPtr.Zero;
        Marshal.FreeCoTaskMem(nodePointBuffer_);
        nodePointBuffer_ = IntPtr.Zero;
        Marshal.FreeCoTaskMem(initPointBuffer_);
        initPointBuffer_ = IntPtr.Zero;

        recognizer_.Dispose();

        if (targets_ != null)
        {
            for (int i = 0, num = targets_.Length; i < num; ++i)
            {
                ((IDisposable)targets_[i]).Dispose();
            }
        }

        landmarkDrawer_.Dispose();

        smart_.Dispose();
    }

    public virtual void resetController()
    {
        if (recognizerSettings.useInstantTarget)
        {
            for (int i = 0; i < onlineTargets_.Length; i++)
            {
                onlineTargets_[i] = null;
            }
        }
        recognizer_.Reset();
    }
		
    public void restartController()
    {
        DoDisable();
        DoDestroy();
        DoCreate();
    }

    public void reCreateController(
        smartar.RecognitionMode recognitionMode,
        smartar.SceneMappingInitMode sceneMappingInitMode = smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET)
    {
        DoDisable();
        DoDestroy();
        if (isLoadSceneMap_)
        {
            recognizerSettings.recognitionMode = orgRecognizerSettings.recognitionMode;
			recognizerSettings.sceneMappingInitMode = orgRecognizerSettings.sceneMappingInitMode;
            recognizerSettings.targets = new TargetEntry[orgRecognizerSettings.targets.Length];
            for (int i = 0; i < orgRecognizerSettings.targets.Length; i++)
            {
                recognizerSettings.targets[i] = new TargetEntry();
                recognizerSettings.targets[i].fileName = orgRecognizerSettings.targets[i].fileName;
                recognizerSettings.targets[i].id = orgRecognizerSettings.targets[i].id;
			}
			isLoadSceneMap_ = false;
		} 
        recognizerSettings.recognitionMode = recognitionMode;
        recognizerSettings.sceneMappingInitMode = sceneMappingInitMode;
        DoCreate();
    }

    public abstract void saveSceneMap(smartar.StreamOut stream);

    public void loadSceneMap(string filePath)
    {
        if (!File.Exists(filePath)) { return; }

        // stop
		DoDisable();
		DoDestroy();

        recognizerSettings.recognitionMode = smartar.RecognitionMode.RECOGNITION_MODE_SCENE_MAPPING;
        recognizerSettings.sceneMappingInitMode = smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET;
        recognizerSettings.targets = new TargetEntry[1] { recognizerSettings.targets[0] };
        sceneMapFilePath_ = filePath;
        isLoadSceneMap_ = true;

		// start
		DoCreate();
    }

    void Awake()
    {
        DoCreate();
        // keep default value
    	orgRecognizerSettings.recognitionMode = recognizerSettings.recognitionMode;
    	orgRecognizerSettings.sceneMappingInitMode = recognizerSettings.sceneMappingInitMode;
        orgRecognizerSettings.targets = new TargetEntry[recognizerSettings.targets.Length];
        for (int i = 0; i < recognizerSettings.targets.Length; i++)
        {
            orgRecognizerSettings.targets[i] = new TargetEntry();
            orgRecognizerSettings.targets[i].fileName = recognizerSettings.targets[i].fileName;
            orgRecognizerSettings.targets[i].id = recognizerSettings.targets[i].id;
		}
    	orgRecognizerSettings.maxTargetsPerFrame = recognizerSettings.maxTargetsPerFrame;
    	orgRecognizerSettings.searchPolicy = recognizerSettings.searchPolicy;
    	orgRecognizerSettings.maxTriangulateMasks = recognizerSettings.maxTriangulateMasks;
    	orgRecognizerSettings.denseMapMode = recognizerSettings.denseMapMode;
        orgRecognizerSettings.useWorkerThread = recognizerSettings.useWorkerThread;
    	orgRecognizerSettings.useInstantTarget = recognizerSettings.useInstantTarget;
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        DoDisable();
    }

    void OnDestroy()
    {
        DoDisable();
        DoDestroy();
    }

    void OnApplicationFocus(bool focus)
    {
    }

    void OnApplicationPause(bool pause)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		DoDisable();
#endif
    }

    void OnApplicationQuit()
    {
        DoDisable();
        DoDestroy();
    }

    void OnValidate()
    {
        bool isSceneMapping = recognizerSettings_.recognitionMode == smartar.RecognitionMode.RECOGNITION_MODE_SCENE_MAPPING;
        bool isInitModeTarget = recognizerSettings_.sceneMappingInitMode == smartar.SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET;

        if (isSceneMapping && !isInitModeTarget && 0 < recognizerSettings_.targets.Length)
        {
            recognizerSettings_.targets = new TargetEntry[] { recognizerSettings_.targets[0] };
        }
    }

    public int GetResult(string id, ref smartar.RecognitionResult result)
    {
        int ret = -1;
        if (id == null || string.Equals(id, ""))
        {
            ret = CallNativeGetResult(self_, IntPtr.Zero, ref result);
            isRecognized_ = result.isRecognized_;
            return ret;
        }
        else if (recognizerSettings.useInstantTarget)
        {
            for (int i = 0; i < onlineTargets_.Length; i++)
            {
                if (onlineTargets_[i] == null) { break; }

                if (string.Compare(recognizerSettings.targets[i].id, id) == 0)
                {
                    ret = CallNativeGetResult(self_, onlineTargets_[i].self_, ref result);
                    isRecognized_ = result.isRecognized_;
                    return ret;
                }
            }

            result = new smartar.RecognitionResult();
            isRecognized_ = result.isRecognized_;
            return smartar.Error.ERROR_INVALID_VALUE;
        }
        else
        {
            for (int i = 0, num = recognizerSettings.targets.Length; i < num; ++i)
            {
                if (string.Compare(recognizerSettings.targets[i].id, id) == 0)
                {
                    if (targets_ == null) { break; }
                    ret = CallNativeGetResult(self_, targets_[i].self_, ref result);
                    isRecognized_ = result.isRecognized_;
                    return ret;
                }
            }

            result = new smartar.RecognitionResult();
            isRecognized_ = result.isRecognized_;
            return smartar.Error.ERROR_INVALID_VALUE;
        }
    }

    protected abstract int CallNativeGetResult(IntPtr self, IntPtr target, ref smartar.RecognitionResult result); 

    protected struct CreateParam
    {
        public IntPtr smart_;
        public IntPtr sensorDevice_;
        public IntPtr screenDevice_;
        public IntPtr recognizer_;
        public IntPtr cameraImageDrawer_;
    }
}
