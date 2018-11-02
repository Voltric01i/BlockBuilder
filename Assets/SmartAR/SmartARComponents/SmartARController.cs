using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

public class SmartARController : SmartARControllerBase {


    [System.Serializable]
	public class CameraDeviceSettings {
		public int cameraId = 0;
		public Vector2 videoImageSize = new Vector2(0, 0);
		public smartar.FocusMode focusMode = smartar.FocusMode.FOCUS_MODE_CONTINUOUS_AUTO_VIDEO;
	}

	[System.Serializable]
    public class MiscSettings
    {
        public bool showCameraPreview = true;
        public bool showLandmarks = false;
	}

    public void ShowLandmarksButton(bool value)
    {
        if (value)
        {
            miscSettings.showLandmarks = true;
            PlayerPrefs.SetInt("showLandmarks", 1);
        }
        else
        {
            miscSettings.showLandmarks = false;
            PlayerPrefs.SetInt("showLandmarks", 0);
        }
    }


    // Fields configurable on inspector
    [SerializeField]
	private CameraDeviceSettings cameraDeviceSettings = new CameraDeviceSettings();
	[SerializeField]
	private MiscSettings miscSettings = new MiscSettings();

	public CameraDeviceSettings cameraDeviceSettings_ {
		get {
			return cameraDeviceSettings;
		}
		set {
			ConfigCameraDevice(value, false);
		}
	}
    public MiscSettings miscSettings_
    {
        get
        {
			return miscSettings;
		}
        set
        {
			ConfigMisc(value, false);
		}
	}

    public override smartar.Triangle2[] triangulateMasks_ {
		set {
			if (self_ != IntPtr.Zero) { 
	            sarSmartar_SarSmartARController_sarSetTriangulateMasks(self_, value, value.Length);
			}
		}
	}

	// Camera2 API in Android
	private bool forceOldAndroidCameraAPI_ = false;
#if UNITY_ANDROID && !UNITY_EDITOR
	public class AndroidCameraFeature {
		public int androidCameraApiLevel_ = 0;
		public int androidCameraHwFeature_ = -1;
		public bool androidCanUseNewAPI_ = false;
	}

	private AndroidCameraFeature androidCameraFeature = new AndroidCameraFeature();

	public AndroidCameraFeature androidCameraFeature_ {
		get {
			return androidCameraFeature;
		}
	}
#endif

    // Listeners
    public class StillImageListener : smartar.CameraImageListener
    {
        // for Capture Image
        private string m_CaptureImageName;
        private string m_CaptureImagePath;
        private smartar.Smart m_Smart;

        public StillImageListener(string captureImagePath, smartar.Smart smart)
        {
            m_CaptureImagePath = captureImagePath;
            m_Smart = smart;

        }

        public void OnImage(smartar.ImageHolder imageHolder, ulong timestamp)
        {
            // save still image
            m_CaptureImageName = string.Format("{0}/capture_image_{1}.jpg", m_CaptureImagePath, DateTime.Now.ToString("d-MM-yyyy-HH-mm-ss-f"));
            int bufSize = imageHolder.getImageSizeInBytes();
            if (bufSize > 0)
            {
                smartar.Image image = new smartar.Image();
                image.pixels_ = Marshal.AllocHGlobal(bufSize);
				imageHolder.getImage(ref image, bufSize, m_Smart);
#if UNITY_IPHONE && !UNITY_EDITOR
				_SaveJpegToCameraRoll(image.pixels_, bufSize);
#elif UNITY_ANDROID && !UNITY_EDITOR
				byte[] jpegData = new byte[bufSize];
				Marshal.Copy(image.pixels_, jpegData, 0, bufSize);
				File.WriteAllBytes(m_CaptureImageName, jpegData);
				using (AndroidJavaObject utils = new AndroidJavaObject("com.sony.smartar.unsupportedutils.UnsupportedUtils"))
				{
					var scanFilePath = utils.CallStatic<string>("moveToExternalDir", m_CaptureImageName);
					if (string.IsNullOrEmpty(scanFilePath)) { return; }
					utils.CallStatic("scanCaptureImage", scanFilePath);
				}
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                m_CaptureImageName = string.Format("{0}/capture_image_{1}.bmp", m_CaptureImagePath, DateTime.Now.ToString("d-MM-yyyy-HH-mm-ss-f"));
                byte[] bitmapData = new byte[bufSize];
                Marshal.Copy(image.pixels_, bitmapData, 0, bufSize);
                File.WriteAllBytes(m_CaptureImageName, bitmapData);
#endif
                Marshal.FreeHGlobal(image.pixels_);
            }
        }

    }
    public class AutoFocusListener : smartar.CameraAutoAdjustListener
    {
        public void OnAutoAdjust(bool success)
        {
            // TODO if need, please implemant this.
            Debug.Log("AutoFocusListener.OnAutoAdjust() called");
        }

    }
    public class AutoExposureListener : smartar.CameraAutoAdjustListener
    {
        public void OnAutoAdjust(bool success)
        {
            // TODO if need, please implemant this.
            Debug.Log("AutoExposureListener.OnAutoAdjust() called");
        }
    }
    public class AutoWhiteBalanceListener : smartar.CameraAutoAdjustListener
    {
        public void OnAutoAdjust(bool success)
        {
            // TODO if need, please implemant this.
            Debug.Log("AutoWhiteBalanceListener.OnAutoAdjust() called");
        }
    }

	// Fields
	[HideInInspector]
	public smartar.CameraDevice cameraDevice_;
	[HideInInspector]
	public smartar.SensorDevice sensorDevice_;
	[HideInInspector]
	public smartar.ScreenDevice screenDevice_;
	[HideInInspector]
	public smartar.CameraImageDrawer cameraImageDrawer_;
	[HideInInspector]
	public bool isFlipX_ = false;
	[HideInInspector]
	public bool isFlipY_ = false;
	[HideInInspector]
	public smartar.Rotation cameraRotation_;

	private bool isFront_ = false;

	// Fields for development
	private float drawTimeSec_ = 0.0f;
	private float drawStartTimeSec_;

    [HideInInspector]
    public enum RenderEventID
    {
        DoDraw = 3001,
    }

	// Properties for development
	public ulong cameraFrameCount_ {
		get {
			if (self_ != IntPtr.Zero) { 
	            return sarSmartar_SarSmartARController_sarGetCameraFrameCount(self_);
			}
			else {
				return 0;
			}
		}
	}

    public override ulong[] recogCount_ {
		get {
			for (int i = 0; i < recogCountBuf_.Length; ++i) {
				if (self_ != IntPtr.Zero) { 
	                recogCountBuf_[i] = sarSmartar_SarSmartARController_sarGetRecogCount(self_, i);
				}
			}
			return recogCountBuf_;
		}
	}

    public override ulong[] recogTime_ {
		get {
			for (int i = 0; i < recogTimeBuf_.Length; ++i) {
				if (self_ != IntPtr.Zero) { 
	                recogTimeBuf_[i] = sarSmartar_SarSmartARController_sarGetRecogTime(self_, i);
				}
			}
			return recogTimeBuf_;
		}
	}

	public ulong drawCount_ {
		get {
			return (ulong) Time.renderedFrameCount;
		}
	}

	public ulong drawTime_ {
		get {
			return (ulong)(drawTimeSec_ * 1000);
		}
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	public bool changeAndroidCameraAPI(out int apiLevel, out int hwFeature, out bool canChangeNewAPI)
        {
		//// get current status
		//cameraDevice_.GetAndroidCameraAPIFeature(out apiLevel, out hwFeature);
		//int oldApiLevel = apiLevel;
		//int oldHwFeature = hwFeature;

		bool success = false;
		canChangeNewAPI = cameraDevice_.IsAndroidCamera2Available(smart_, isFront_, out apiLevel, out hwFeature);
		if (canChangeNewAPI)
                {
		// change API
			DoDisable();
			forceOldAndroidCameraAPI_ = !forceOldAndroidCameraAPI_;
			DoEnable();
			success = true;
                    }

		// get latest status
		cameraDevice_.GetAndroidCameraAPIFeature(out apiLevel, out hwFeature);
		return success;
            }

	public void getAndroidCameraFeature(out int apiLevel, out int hwFeature, out bool canChangeNewAPI)
                {
		canChangeNewAPI = cameraDevice_.IsAndroidCamera2Available(smart_, isFront_, out apiLevel, out hwFeature);
		cameraDevice_.GetAndroidCameraAPIFeature(out apiLevel, out hwFeature);
	}
#endif

	private void ConfigCameraDevice (CameraDeviceSettings newSettings, bool creating)
	{
		if (!creating && (newSettings.cameraId != cameraDeviceSettings.cameraId
				|| newSettings.videoImageSize != cameraDeviceSettings.videoImageSize)) {
			cameraDeviceSettings = newSettings;
			if (enabled_) {
				DoDisable();
				DoEnable();
			}
		} else {
			if (creating || newSettings.focusMode != cameraDeviceSettings.focusMode) {
				var modes = new smartar.FocusMode[32];
				var numModes = cameraDevice_.GetSupportedFocusMode(modes);
				modes = new ArraySegment<smartar.FocusMode>(modes, 0, numModes).Array;
				if (((System.Collections.Generic.IList<smartar.FocusMode>)modes).Contains(newSettings.focusMode)) {
					cameraDevice_.SetFocusMode(newSettings.focusMode);
				}
			}

			cameraDeviceSettings = newSettings;
		}
	}

	private void ConfigMisc (MiscSettings newSettings, bool creating)
	{
		miscSettings = newSettings;
	}

    protected override void DoCreate()
    {
        base.DoCreate();

		cameraImageDrawer_ = new smartar.CameraImageDrawer(smart_);
		sensorDevice_ = new smartar.SensorDevice(smart_);
        screenDevice_ = new smartar.ScreenDevice(smart_);

		smartar.SensorDeviceInfo sensorInfo = new smartar.SensorDeviceInfo();
		sensorDevice_.GetDeviceInfo(sensorInfo);
		recognizer_.SetSensorDeviceInfo(sensorInfo);

		CreateParam param;
		param.smart_ = smart_.self_;
        param.recognizer_ = recognizer_.self_;
		param.sensorDevice_ = sensorDevice_.self_;
		param.screenDevice_ = screenDevice_.self_;
		param.cameraImageDrawer_ = cameraImageDrawer_.self_;
		self_ = sarSmartar_SarSmartARController_sarDoCreate(ref param, numWorkerThreads_ > 0);

		if (self_ != IntPtr.Zero) { 
		// Start worker threads
	        if (numWorkerThreads_ > 0)
	        {
			workerThreads_ = new Thread[numWorkerThreads_];
	            for (int i = 0; i < numWorkerThreads_; ++i)
	            {
				Thread thread = new Thread(() => {
					sarSmartar_SarSmartARController_sarRunWorkerThread(self_);
				});
				thread.Start();
				workerThreads_[i] = thread;
			}
	        }
	        else
	        {
			workerThreads_ = null;
		}

		started_ = true;
	}
    }

    protected override void DoEnable() {
        if (enabled_) {
			return;
		}

        if (smartInitFailed_) { return; }
		if (self_ == IntPtr.Zero) { return; }

		// Create CameraDevice
        cameraDevice_ = new smartar.CameraDevice(smart_, cameraDeviceSettings.cameraId, forceOldAndroidCameraAPI_);

        cameraDevice_.GetRotation(out cameraRotation_);
        int frontCameraId;
        smartar.CameraDevice.GetDefaultCameraId(smart_, smartar.Facing.FACING_FRONT, out frontCameraId, forceOldAndroidCameraAPI_);
        isFront_ = frontCameraId != smartar.CameraDevice.INVALID_CAMERA_ID && cameraDeviceSettings.cameraId == frontCameraId;

#if UNITY_ANDROID && !UNITY_EDITOR
        // get camera hw support
        int apiLevel = 0;
        int feature = -1;
        androidCameraFeature.androidCanUseNewAPI_ = cameraDevice_.IsAndroidCamera2Available(smart_, isFront_, out apiLevel, out feature);
        cameraDevice_.GetAndroidCameraAPIFeature(out apiLevel, out feature);
        androidCameraFeature.androidCameraApiLevel_ = apiLevel;
        androidCameraFeature.androidCameraHwFeature_ = feature;
#endif

        if (cameraDeviceSettings.videoImageSize.x != 0 && cameraDeviceSettings.videoImageSize.y != 0) {
            cameraDevice_.SetVideoImageSize((int) cameraDeviceSettings.videoImageSize.x, (int) cameraDeviceSettings.videoImageSize.y);
        }
        ConfigCameraDevice(cameraDeviceSettings, true);

        isFlipX_ = false;
        isFlipY_ = false;
        if (isFront_) {
#if UNITY_IOS && !UNITY_EDITOR
            isFlipY_ = true;
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
            // NOOP
#else
            if (cameraRotation_ == smartar.Rotation.ROTATION_0 || cameraRotation_ == smartar.Rotation.ROTATION_90) {
                isFlipX_ = true;
            } else {
                isFlipY_ = true;
            }
#endif
        }
        cameraImageDrawer_.SetFlipX(isFlipX_);
        cameraImageDrawer_.SetFlipY(isFlipY_);

        // Setup Recognizer
        smartar.CameraDeviceInfo cameraInfo = new smartar.CameraDeviceInfo();
        cameraDevice_.GetDeviceInfo(cameraInfo);
        recognizer_.SetCameraDeviceInfo(cameraInfo);

        // Enable native instance
        sarSmartar_SarSmartARController_sarDoEnable(self_, cameraDevice_.self_);

        // set listener 
        cameraDevice_.SetStillImageListener(new StillImageListener(createCaptureImagePath(), smart_));
        cameraDevice_.SetAutoFocusListener(new AutoFocusListener());
        cameraDevice_.SetAutoExposureListener(new AutoExposureListener());
        cameraDevice_.SetAutoWhiteBalanceListener(new AutoWhiteBalanceListener());

        // Start components
		cameraImageDrawer_.Start();
		landmarkDrawer_.Start();
		cameraDevice_.Start();
		sensorDevice_.Start();

		if (GetComponent<Camera>().clearFlags != CameraClearFlags.Depth && GetComponent<Camera>().clearFlags != CameraClearFlags.Nothing) {
			GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		}

		enabled_ = true;
	}

    protected override void DoDisable()
    {
		if (sensorDevice_ != null) {
		sensorDevice_.Stop();
		}
		if (cameraDevice_ != null) {
		cameraDevice_.Stop();
		}
		if (cameraImageDrawer_ != null) {
		cameraImageDrawer_.Stop();
		}
		if (self_ != IntPtr.Zero) { 
	        sarSmartar_SarSmartARController_sarDoDisable(self_);
		}
		if (cameraDevice_ != null) {
		cameraDevice_.Dispose();
			cameraDevice_ = null;
	}

        base.DoDisable();
		}

    protected override void DoDestroy()
    {
		// Finish worker threads
        if (numWorkerThreads_ > 0)
        {
			if (self_ != IntPtr.Zero) { 
	            sarSmartar_SarSmartARController_sarFinishWorkerThread(self_);
			}
            for (int i = 0; i < numWorkerThreads_; ++i)
            {
				workerThreads_[i].Join();
			}
		}
		if (self_ != IntPtr.Zero) { 
	        sarSmartar_SarSmartARController_sarDoDestroy(self_);
		self_ = IntPtr.Zero;
		}
		if (screenDevice_ != null) {
		screenDevice_.Dispose();
			screenDevice_ = null;
            }
		if (sensorDevice_ != null) {
	        sensorDevice_.Dispose();
			sensorDevice_ = null;
        }
		if (cameraImageDrawer_ != null) {
		cameraImageDrawer_.Dispose();
			cameraImageDrawer_ = null;
	}
        base.DoDestroy();
	}

    public override void resetController()
    {
		if (self_ != IntPtr.Zero) { 
	        sarSmartar_SarSmartARController_sarSuspendWorkerThread(self_);
            base.resetController();
	        sarSmartar_SarSmartARController_sarResumeWorkerThread(self_);
	}
	}

    void Start()
    {
        GL.IssuePluginEvent(GetRenderEventFunc(), 0);
	}

	void Update () {
        if (smartInitFailed_) { return; }
		DoEnable();
		if (self_ != IntPtr.Zero) { 
	        GetComponent<Camera>().fieldOfView = sarSmartar_SarSmartARController_sarGetFovy(self_);
		}
	}

	void OnPreRender() {
        if (smartInitFailed_) { return; }
		if (self_ == IntPtr.Zero) { return; } 
			
        drawStartTimeSec_ = Time.realtimeSinceStartup;

#if UNITY_5_0
    #if UNITY_ANDROID && !UNITY_EDITOR
        GL.IssuePluginEvent(0);
#elif UNITY_IOS && !UNITY_EDITOR
        GL.Clear(true, false, Color.green);
    #endif
#else
        GL.IssuePluginEvent(GetRenderEventFunc(), 0);
        GL.InvalidateState();
#endif

		// Draw landmarks
		if (miscSettings.showLandmarks) {
			// Get result
			var result = new smartar.RecognitionResult();
			result.maxLandmarks_ = smartar.Recognizer.MAX_NUM_LANDMARKS;
			result.landmarks_ = landmarkBuffer_;
			result.maxNodePoints_ = smartar.Recognizer.MAX_NUM_NODE_POINTS;
			result.nodePoints_ = nodePointBuffer_;
			result.maxInitPoints_ = smartar.Recognizer.MAX_NUM_INITIALIZATION_POINTS;
			result.initPoints_ = initPointBuffer_;
			GetResult(null, ref result);

			// Draw landmarks
			var adjustedPosition = new smartar.Vector3();
			var adjustedRotation = new smartar.Quaternion();
			sarSmartar_SarSmartARController_sarAdjustPose(self_, ref result.position_, ref result.rotation_, out adjustedPosition, out adjustedRotation);
			var mvMatrix = new smartar.Matrix44();
			smartar.Utility.convertPose2Matrix(adjustedPosition, adjustedRotation, out mvMatrix);
            var projMatrix = getProjMatrix();
            var initPointMatrix = sarSmartar_SarSmartARController_sarGetInitPointMatrix(self_);
            var pmvMatrix = projMatrix * mvMatrix;

            // Draw Preview, landmarks, node points, init points
            sarSmartar_SarSmartARController_sarSetDrawData(Screen.width, Screen.height, miscSettings.showCameraPreview);
            sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawLandmarkData(self_, landmarkDrawer_.self_, ref pmvMatrix, result.landmarks_, result.numLandmarks_);
            sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawNodePointData(self_, landmarkDrawer_.self_, ref pmvMatrix, result.nodePoints_, result.numNodePoints_);
            sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawInitPointData(self_, landmarkDrawer_.self_, ref initPointMatrix, result.initPoints_, result.numInitPoints_);
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.DoDraw);
            GL.InvalidateState();
		}
        else
        {
            sarSmartar_SarSmartARController_sarSetDrawData(Screen.width, Screen.height, miscSettings.showCameraPreview);
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.DoDraw);
            GL.InvalidateState();
        }
	}

	void OnPostRender() {
        if (smartInitFailed_) { return; }
		if (self_ == IntPtr.Zero) { return; }

        sarSmartar_SarSmartARController_sarDoEndFrame(self_);

		drawTimeSec_ += Time.realtimeSinceStartup - drawStartTimeSec_;
	}

    protected override int CallNativeGetResult(IntPtr self, IntPtr target, ref smartar.RecognitionResult result)
            {
		if (self_ != IntPtr.Zero) { 
	        	return sarSmartar_SarSmartARController_sarGetResult(self_, target, ref result);
            }
		else {
			return smartar.Error.ERROR_INVALID_POINTER;
        }
    }

    static public void adjustPose(smartar.Rotation cameraRotation, smartar.Rotation screenRotation, bool isFlipX, bool isFlipY,
        smartar.Vector3 srcPosition, smartar.Quaternion srcRotation, out smartar.Vector3 rotPosition, out smartar.Quaternion rotRotation)
	{
        smartar.Rotation rotation = (smartar.Rotation)((cameraRotation - screenRotation + 360) % 360);

        rotPosition = srcPosition;
        rotRotation = srcRotation;
		if (isFlipX) {
	        rotPosition.x_ = -rotPosition.x_;

	        rotRotation.y_ = -rotRotation.y_;
	        rotRotation.z_ = -rotRotation.z_;
		}
		if (isFlipY) {
	        rotPosition.y_ = -rotPosition.y_;

	        rotRotation.x_ = -rotRotation.x_;
	        rotRotation.z_ = -rotRotation.z_;
		}
        if (rotation != smartar.Rotation.ROTATION_0) {
            float rad;
            switch (rotation) {
            case smartar.Rotation.ROTATION_90: {
                rad = Mathf.PI * 0.5f * 0.5f;
                break;
            }
            case smartar.Rotation.ROTATION_180: {
                rad = Mathf.PI * 1.0f * 0.5f;
                break;
            }
            case smartar.Rotation.ROTATION_270: {
                rad = Mathf.PI * 1.5f * 0.5f;
                break;
            }
            default:
				throw new InvalidOperationException("unexpected value: " + rotation);
            }
            // rotate pose around z axis
			Quaternion quat0 = new Quaternion(rotRotation.x_, rotRotation.z_, rotRotation.y_, rotRotation.w_);
            Quaternion quat1 = new Quaternion(0.0f, Mathf.Sin(rad), 0.0f, Mathf.Cos(rad));
			quat0 = quat0 * quat1;
            rotRotation.x_ = quat0.x;
            rotRotation.y_ = quat0.z;
            rotRotation.z_ = quat0.y;
            rotRotation.w_ = quat0.w;
        }
	}

	// get camera preview
	public void getImage(IntPtr image, out ulong timestamp) {
		if (self_ != IntPtr.Zero && image != IntPtr.Zero) { 
			sarSmartar_SarSmartARController_sarGetImage(self_, image, out timestamp);
		}
		else {
			timestamp = 0;
		}
	}


    public float getFovyFromSmartAR()
    {
		if (self_ != IntPtr.Zero) { 
	        return sarSmartar_SarSmartARController_sarGetFovy(self_);
		}
		else {
			return 0.0f;
		}
    }

    public void getCameraId(smartar.Facing facing, out int cameraId)
    {
		if (smart_ != null) {
	        smartar.CameraDevice.GetDefaultCameraId(smart_, facing, out cameraId, forceOldAndroidCameraAPI_);
		}
		else {
			cameraId = -1;
		}
    }

    public smartar.Matrix44 getProjMatrix()
    {
        return smartar.Utility.setPerspectiveM(GetComponent<Camera>().fieldOfView, (float)Screen.width / (float)Screen.height, 0.01f, 100.0f);
    }

	public override void saveSceneMap(smartar.StreamOut stream)
	{
        sarSmartar_SarSmartARController_sarSuspendWorkerThread(self_);
        recognizer_.SaveSceneMap(stream);
        sarSmartar_SarSmartARController_sarResumeWorkerThread(self_);
	}

    private string createCaptureImagePath()
    {
        string path;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                path = Application.persistentDataPath;
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                path = Application.dataPath;
                break;
            default:
                path = null;
                break;
        }

        return path;
    }

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void _SaveJpegToCameraRoll(IntPtr jpegData, int jpegDataLength);
        
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarSmartARController_sarDoCreate(ref CreateParam param, bool workerThreadEnabled);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarDoDestroy(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarDoEnable(IntPtr self, IntPtr cameraDevice);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarDoDisable(IntPtr self);
		[DllImport("__Internal")]
		private static extern int sarSmartar_SarSmartARController_sarSuspendWorkerThread(IntPtr self);
		[DllImport("__Internal")]
		private static extern int sarSmartar_SarSmartARController_sarResumeWorkerThread(IntPtr self);
		[DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarDoDraw(IntPtr self, int width, int height, bool showCameraImage);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarDoEndFrame(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarGetResult(IntPtr self, IntPtr target, ref smartar.RecognitionResult result);
        [DllImport("__Internal")]
        private static extern float sarSmartar_SarSmartARController_sarGetFovy(IntPtr self);
        [DllImport("__Internal")]
        private static extern smartar.Matrix44 sarSmartar_SarSmartARController_sarGetInitPointMatrix(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarAdjustPose(IntPtr self, ref smartar.Vector3 fromPosition, ref smartar.Quaternion fromRotation, out smartar.Vector3 toPosition, out smartar.Quaternion toRotation);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarRunWorkerThread(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarFinishWorkerThread(IntPtr self);
        [DllImport("__Internal")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetCameraFrameCount(IntPtr self);
        [DllImport("__Internal")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetRecogCount(IntPtr self, int index);
        [DllImport("__Internal")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetRecogTime(IntPtr self, int index);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarSetTriangulateMasks(IntPtr self, smartar.Triangle2[] masks, int numMasks);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSmartARController_sarGetImage(IntPtr self, IntPtr image, out ulong timestamp);
        [DllImport("__Internal")]
        private static extern IntPtr GetRenderEventFunc();
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmartARController_sarSetDrawData(int width, int height, bool showCameraImage);
        [DllImport("__Internal")]
		private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawLandmarkData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 pmvMatrix, IntPtr landmarks, int numLandmarks);
	    [DllImport("__Internal")]
		private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawNodePointData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 pmvMatrix, IntPtr nodePoints, int numNodePoints);
        [DllImport("__Internal")]
		private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawInitPointData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 imageMatrix, IntPtr initPoints, int numInitPoints);
#else
    [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSmartARController_sarDoCreate(ref CreateParam param, bool workerThreadEnabled);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarDoDestroy(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarDoEnable(IntPtr self, IntPtr cameraDevice);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarDoDisable(IntPtr self);
        [DllImport("smartar")]
		private static extern int sarSmartar_SarSmartARController_sarSuspendWorkerThread(IntPtr self);
		[DllImport("smartar")]
		private static extern int sarSmartar_SarSmartARController_sarResumeWorkerThread(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarDoDraw(IntPtr self, int width, int height, bool showCameraImage);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarDoEndFrame(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarGetResult(IntPtr self, IntPtr target, ref smartar.RecognitionResult result);
        [DllImport("smartar")]
        private static extern float sarSmartar_SarSmartARController_sarGetFovy(IntPtr self);
        [DllImport("smartar")]
        private static extern smartar.Matrix44 sarSmartar_SarSmartARController_sarGetInitPointMatrix(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarAdjustPose(IntPtr self, ref smartar.Vector3 fromPosition, ref smartar.Quaternion fromRotation, out smartar.Vector3 toPosition, out smartar.Quaternion toRotation);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarRunWorkerThread(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarFinishWorkerThread(IntPtr self);
        [DllImport("smartar")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetCameraFrameCount(IntPtr self);
        [DllImport("smartar")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetRecogCount(IntPtr self, int index);
        [DllImport("smartar")]
        private static extern ulong sarSmartar_SarSmartARController_sarGetRecogTime(IntPtr self, int index);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarSetTriangulateMasks(IntPtr self, smartar.Triangle2[] masks, int numMasks);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarGetImage(IntPtr self, IntPtr image, out ulong timestamp);
        [DllImport("smartar")]
        private static extern IntPtr GetRenderEventFunc();
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmartARController_sarSetDrawData(int width, int height, bool showCameraImage);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawLandmarkData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 pmvMatrix, IntPtr landmarks, int numLandmarks);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawNodePointData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 pmvMatrix, IntPtr nodePoints, int numNodePoints);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSmartARController_sarSetLandmarkDrawerDrawInitPointData(IntPtr self, IntPtr landmark_self, ref smartar.Matrix44 imageMatrix, IntPtr initPoints, int numInitPoints);
#endif
}
