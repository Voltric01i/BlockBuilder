using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace smartar {
    // * Note that the constants below must match the native-code constants. 
    public enum FocusMode {
        FOCUS_MODE_MANUAL,
        FOCUS_MODE_CONTINUOUS_AUTO_PICTURE,
        FOCUS_MODE_CONTINUOUS_AUTO_VIDEO,
        FOCUS_MODE_EDOF,
        FOCUS_MODE_FIXED,
        FOCUS_MODE_INFINITY,
        FOCUS_MODE_MACRO,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum FlashMode {
        FLASH_MODE_AUTO,
        FLASH_MODE_OFF,
        FLASH_MODE_ON,
        FLASH_MODE_RED_EYE,
        FLASH_MODE_TORCH,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum ExposureMode {
        EXPOSURE_MODE_MANUAL,
        EXPOSURE_MODE_CONTINUOUS_AUTO,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum WhiteBalanceMode {
        WHITE_BALANCE_MODE_CONTINUOUS_AUTO,
        WHITE_BALANCE_MODE_CLOUDY_DAYLIGHT,
        WHITE_BALANCE_MODE_DAYLIGHT,
        WHITE_BALANCE_MODE_FLUORESCENT,
        WHITE_BALANCE_MODE_INCANDESCENT,
        WHITE_BALANCE_MODE_SHADE,
        WHITE_BALANCE_MODE_TWILIGHT,
        WHITE_BALANCE_MODE_WARM_FLUORESCENT,
        WHITE_BALANCE_MODE_MANUAL,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum SceneMode {
        SCENE_MODE_ACTION,
        SCENE_MODE_AUTO,
        SCENE_MODE_BARCODE,
        SCENE_MODE_BEACH,
        SCENE_MODE_CANDLELIGHT,
        SCENE_MODE_FIREWORKS,
        SCENE_MODE_LANDSCAPE,
        SCENE_MODE_NIGHT,
        SCENE_MODE_NIGHT_PORTRAIT,
        SCENE_MODE_PARTY,
        SCENE_MODE_PORTRAIT,
        SCENE_MODE_SNOW,
        SCENE_MODE_SPORTS,
        SCENE_MODE_STEADYPHOTO,
        SCENE_MODE_SUNSET,
        SCENE_MODE_THEATRE,
    };
    
    public class CameraDeviceInfo : IDisposable
    {
        public CameraDeviceInfo() {
            self_ = sarSmartar_SarCameraDeviceInfo_SarCameraDeviceInfo();
        }
        
        ~CameraDeviceInfo() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarCameraDeviceInfo_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public IntPtr self_;
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarCameraDeviceInfo_SarCameraDeviceInfo();
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarCameraDeviceInfo_sarDelete(IntPtr self);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarCameraDeviceInfo_SarCameraDeviceInfo();
        [DllImport("smartar")]
        private static extern void sarSmartar_SarCameraDeviceInfo_sarDelete(IntPtr self);
#endif
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraFpsRange
    {
        public float min_;
        public float max_;
    };
    
    public class ImageHolder
    {
        public ImageHolder(IntPtr self) {
            self_ = self;
        }
        
        ~ImageHolder() {
            self_ = IntPtr.Zero;
        }
        
        public int getImageSizeInBytes() {
            return sarSmartar_SarImageHolder_sarGetImageSizeInBytes(self_);
        }
        
        public int getImage(ref Image image, int maxSizeInBytes, Smart smart) {
            return sarSmartar_SarImageHolder_sarGetImage(self_, ref image, maxSizeInBytes, smart.self_);
        }
        
        public IntPtr self_;
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarImageHolder_sarGetImageSizeInBytes(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarImageHolder_sarGetImage(IntPtr self, ref Image image, int maxSizeInBytes, IntPtr smart);
#else
        [DllImport("smartar")]
        private static extern int sarSmartar_SarImageHolder_sarGetImageSizeInBytes(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarImageHolder_sarGetImage(IntPtr self, ref Image image, int maxSizeInBytes, IntPtr smart);
#endif
    };
    
    public interface CameraImageListener
    {
        void OnImage(ImageHolder imageHolder, ulong timestamp);
    };
    
    public interface CameraShutterListener
    {
        void OnShutter();
    };
    
    public interface CameraAutoAdjustListener
    {
        void OnAutoAdjust(bool success);
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraArea
    {
        public float left_;
        public float top_;
        public float right_;
        public float bottom_;
        public float weight_;
    };
    
    public class CameraDevice : IDisposable
    {
	    public const int INVALID_CAMERA_ID = -1;
		
        public CameraDevice(Smart smart, bool forceOldAndroidAPI = false)
		: this() {
            self_ = sarSmartar_SarCameraDevice_SarCameraDevice(smart.self_, forceOldAndroidAPI);
        }
        
        public CameraDevice(Smart smart, int cameraId, bool forceOldAndroidAPI = false)
		: this(smart, cameraId, IntPtr.Zero, forceOldAndroidAPI) {
        }
        
        public CameraDevice(Smart smart, int cameraId, IntPtr nativeDevice, bool forceOldAndroidAPI = false)
		: this() {
            self_ = sarSmartar_SarCameraDevice_SarCameraDevice2(smart.self_, cameraId, nativeDevice, forceOldAndroidAPI);
        }
		
		private CameraDevice() {
			//---------------------------------------------------------------
			//=================================================================
			thisObj_ = this;
			//=================================================================
            var videoImageListenerDelegate = new CameraImageListenerDelegate(OnVideoImage);
            proxyListenerDelegates_.videoImageListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(videoImageListenerDelegate);

            var stillImageListenerDelegate = new CameraImageListenerDelegate(OnStillImage);
            proxyListenerDelegates_.stillImageListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(stillImageListenerDelegate);

            var shutterListenerDelegate = new CameraShutterListenerDelegate(OnShutter);
            proxyListenerDelegates_.shutterListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(shutterListenerDelegate);

            var autoFocusListenerDelegate = new CameraAutoAdjustListenerDelegate(OnAutoFocus);
            proxyListenerDelegates_.autoFocusListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(autoFocusListenerDelegate);

            var autoExposureListenerDelegate = new CameraAutoAdjustListenerDelegate(OnAutoExposure);
            proxyListenerDelegates_.autoExposureListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(autoExposureListenerDelegate);

            var autoWhiteBalanceListenerDelegate = new CameraAutoAdjustListenerDelegate(OnAutoWhiteBalance);
            proxyListenerDelegates_.autoWhiteBalanceListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(autoWhiteBalanceListenerDelegate);
			sarSmartar_SarCameraDeviceProxyListeners_sarCreate(ref proxyListenerDelegates_, out proxyListeners_);
			//---------------------------------------------------------------
        }
        
        ~CameraDevice() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarCameraDevice_sarDelete(self_);
                self_ = IntPtr.Zero;
				
				//---------------------------------------------------------------
				sarSmartar_SarCameraDeviceProxyListeners_sarDelete(ref proxyListeners_);
				//=================================================================
				thisObj_ = null;
				//=================================================================
				//---------------------------------------------------------------
            }
        }
        
        public IntPtr self_;
        
		//---------------------------------------------------------------
        private delegate void CameraImageListenerDelegate(IntPtr imageHolder, ulong timestamp);
        private delegate void CameraShutterListenerDelegate();
        private delegate void CameraAutoAdjustListenerDelegate(bool success);
			
		//=================================================================
		private static CameraDevice thisObj_ = null;
		
		private class MonoPInvokeCallbackAttribute : System.Attribute
		{
		    protected Type type;
		    public MonoPInvokeCallbackAttribute( Type t ) { type = t; }
		}
		
		[MonoPInvokeCallback (typeof (CameraImageListenerDelegate))]
        private static void OnVideoImage(IntPtr imageHolder, ulong timestamp) {
			thisObj_.imageHolder_.self_ = imageHolder;
        	thisObj_.videoImageListener_.OnImage(thisObj_.imageHolder_, timestamp);
			thisObj_.imageHolder_.self_ = IntPtr.Zero;
        }
        
		[MonoPInvokeCallback (typeof (CameraImageListenerDelegate))]
        private static void OnStillImage(IntPtr imageHolder, ulong timestamp) {
			thisObj_.imageHolder_.self_ = imageHolder;
        	thisObj_.stillImageListener_.OnImage(thisObj_.imageHolder_, timestamp);
			thisObj_.imageHolder_.self_ = IntPtr.Zero;
        }
        
		[MonoPInvokeCallback (typeof (CameraShutterListenerDelegate))]
        private static void OnShutter() {
        	thisObj_.shutterListener_.OnShutter();
        }
        
		[MonoPInvokeCallback (typeof (CameraAutoAdjustListenerDelegate))]
        private static void OnAutoFocus(bool success) {
        	thisObj_.autoFocusListener_.OnAutoAdjust(success);
        }
        
		[MonoPInvokeCallback (typeof (CameraAutoAdjustListenerDelegate))]
        private static void OnAutoExposure(bool success) {
        	thisObj_.autoExposureListener_.OnAutoAdjust(success);
        }
        
		[MonoPInvokeCallback (typeof (CameraAutoAdjustListenerDelegate))]
        private static void OnAutoWhiteBalance(bool success) {
        	thisObj_.autoWhiteBalanceListener_.OnAutoAdjust(success);
        }
		//=================================================================
        
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListeners
	    {
	        public IntPtr videoImageListener_;
	        public IntPtr stillImageListener_;
	        public IntPtr shutterListener_;
	        public IntPtr autoFocusListener_;
	        public IntPtr autoExposureListener_;
	        public IntPtr autoWhiteBalanceListener_;
	    };
		
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListenerDelegates
	    {
            public IntPtr/*CameraImageListenerDelegate*/ videoImageListenerDelegate_;
            public IntPtr/*CameraImageListenerDelegate*/ stillImageListenerDelegate_;
            public IntPtr/*CameraShutterListenerDelegate*/ shutterListenerDelegate_;
            public IntPtr/*CameraAutoAdjustListenerDelegate*/ autoFocusListenerDelegate_;
            public IntPtr/*CameraAutoAdjustListenerDelegate*/ autoExposureListenerDelegate_;
            public IntPtr/*CameraAutoAdjustListenerDelegate*/ autoWhiteBalanceListenerDelegate_;
	    };
			
#if UNITY_IOS
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarCameraDeviceProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarCameraDeviceProxyListeners_sarDelete(ref ProxyListeners listeners);
#else
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarCameraDeviceProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarCameraDeviceProxyListeners_sarDelete(ref ProxyListeners listeners);
#endif
		
		private ProxyListenerDelegates proxyListenerDelegates_ = new ProxyListenerDelegates();
		private ProxyListeners proxyListeners_ = new ProxyListeners();
		
	    private CameraImageListener videoImageListener_ = null;
	    private CameraImageListener stillImageListener_ = null;
	    private CameraShutterListener shutterListener_ = null;
		private CameraAutoAdjustListener autoFocusListener_ = null;
		private CameraAutoAdjustListener autoExposureListener_ = null;
		private CameraAutoAdjustListener autoWhiteBalanceListener_ = null;
		
		private ImageHolder imageHolder_ = new ImageHolder(IntPtr.Zero);
        //---------------------------------------------------------------

        // setting
        public int SetNativeVideoOutput(IntPtr nativeVideoOutput) {
            return sarSmartar_SarCameraDevice_sarSetNativeVideoOutput(self_, nativeVideoOutput);
        }
        
        public int SetVideoImageListener(CameraImageListener listener, Smart smart) {
			//---------------------------------------------------------------
            videoImageListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetVideoImageListener(self_, (listener != null ? proxyListeners_.videoImageListener_ : IntPtr.Zero), smart.self_);
			//---------------------------------------------------------------
        }
        
        public int SetVideoImageSize(int width, int height) {
            return sarSmartar_SarCameraDevice_sarSetVideoImageSize(self_, width, height);
        }
        
        public int SetVideoImageFormat(ImageFormat format) {
            return sarSmartar_SarCameraDevice_sarSetVideoImageFormat(self_, format);
        }
        
        public int SetVideoImageFpsRange(float min, float max) {
            return sarSmartar_SarCameraDevice_sarSetVideoImageFpsRange(self_, min, max);
        }
        
        public int SetStillImageListener(CameraImageListener listener) {
			//---------------------------------------------------------------
            stillImageListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetStillImageListener(self_, listener != null ? proxyListeners_.stillImageListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetStillImageSize(int width, int height) {
            return sarSmartar_SarCameraDevice_sarSetStillImageSize(self_, width, height);
        }
        
        public int SetStillImageFormat(ImageFormat format) {
            return sarSmartar_SarCameraDevice_sarSetStillImageFormat(self_, format);
        }
        
        public int SetShutterListener(CameraShutterListener listener) {
			//---------------------------------------------------------------
            shutterListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetShutterListener(self_, listener != null ? proxyListeners_.shutterListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetFocusMode(FocusMode mode) {
            return sarSmartar_SarCameraDevice_sarSetFocusMode(self_, mode);
        }
        
        public int SetFocusAreas(CameraArea[] areas) {
            return sarSmartar_SarCameraDevice_sarSetFocusAreas(self_, areas, areas.Length);
        }
        
        public int SetExposureMode(ExposureMode mode) {
            return sarSmartar_SarCameraDevice_sarSetExposureMode(self_, mode);
        }
        
        public int SetExposureAreas(CameraArea[] areas) {
            return sarSmartar_SarCameraDevice_sarSetExposureAreas(self_, areas, areas.Length);
        }
        
        public int SetFlashMode(FlashMode mode) {
            return sarSmartar_SarCameraDevice_sarSetFlashMode(self_, mode);
        }
        
        public int SetWhiteBalanceMode(WhiteBalanceMode mode) {
            return sarSmartar_SarCameraDevice_sarSetWhiteBalanceMode(self_, mode);
        }
        
        public int SetSceneMode(SceneMode mode) {
            return sarSmartar_SarCameraDevice_sarSetSceneMode(self_, mode);
        }
        
        public int SetAutoFocusListener(CameraAutoAdjustListener listener) {
			//---------------------------------------------------------------
            autoFocusListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetAutoFocusListener(self_, listener != null ? proxyListeners_.autoFocusListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetAutoExposureListener(CameraAutoAdjustListener listener) {
			//---------------------------------------------------------------
            autoExposureListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetAutoExposureListener(self_, listener != null ? proxyListeners_.autoExposureListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetAutoWhiteBalanceListener(CameraAutoAdjustListener listener) {
			//---------------------------------------------------------------
            autoWhiteBalanceListener_ = listener;
            return sarSmartar_SarCameraDevice_sarSetAutoWhiteBalanceListener(self_, listener != null ? proxyListeners_.autoWhiteBalanceListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetOwningNativeDevice(bool isOwning) {
            return sarSmartar_SarCameraDevice_sarSetOwningNativeDevice(self_, isOwning);
        }
        
        
        // get info
        public static int GetDefaultCameraId(Smart smart, Facing facing, out int cameraId, bool forceOldAndroidAPI = false) {
            return sarSmartar_SarCameraDevice_sarGetDefaultCameraId(smart.self_, facing, out cameraId, forceOldAndroidAPI);
        }
        
        public int GetSupportedVideoImageSize(Size[] sizes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedVideoImageSize(self_, sizes, sizes.Length);
        }
        
        public int GetSupportedVideoImageFormat(ImageFormat[] formats) {
            return sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFormat(self_, formats, formats.Length);
        }
        
        public int GetSupportedVideoImageFpsRange(CameraFpsRange[] ranges) {
            return sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFpsRange(self_, ranges, ranges.Length);
        }
        
        public int GetSupportedStillImageSize(Size[] sizes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedStillImageSize(self_, sizes, sizes.Length);
        }
        
        public int GetSupportedStillImageFormat(ImageFormat[] formats) {
            return sarSmartar_SarCameraDevice_sarGetSupportedStillImageFormat(self_, formats, formats.Length);
        }
        
        public int GetSupportedFocusMode(FocusMode[] modes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedFocusMode(self_, modes, modes.Length);
        }
        
        public int GetMaxNumFocusAreas() {
            return sarSmartar_SarCameraDevice_sarGetMaxNumFocusAreas(self_);
        }
        
        public int GetSupportedFlashMode(FlashMode[] modes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedFlashMode(self_, modes, modes.Length);
        }
        
        public int GetSupportedExposureMode(ExposureMode[] modes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedExposureMode(self_, modes, modes.Length);
        }
        
        public int GetMaxNumExposureAreas() {
            return sarSmartar_SarCameraDevice_sarGetMaxNumExposureAreas(self_);
        }
        
        public int GetSupportedWhiteBalanceMode(WhiteBalanceMode[] modes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedWhiteBalanceMode(self_, modes, modes.Length);
        }
        
        public int GetSupportedSceneMode(SceneMode[] modes) {
            return sarSmartar_SarCameraDevice_sarGetSupportedSceneMode(self_, modes, modes.Length);
        }
        
        
        public int GetDeviceInfo(CameraDeviceInfo info) {
            return sarSmartar_SarCameraDevice_sarGetDeviceInfo(self_, info.self_);
        }
        
        public int GetFovY(out float fovy, float heightRatio = 1.0f) {
			bool calibrated;
            return sarSmartar_SarCameraDevice_sarGetFovY(self_, out fovy, heightRatio, out calibrated);
        }
        
        public int GetFovY(out float fovy, float heightRatio, out bool calibrated) {
            return sarSmartar_SarCameraDevice_sarGetFovY(self_, out fovy, heightRatio, out calibrated);
        }
        
        public int GetFacing(out Facing facing) {
            return sarSmartar_SarCameraDevice_sarGetFacing(self_, out facing);
        }
        
        public int GetRotation(out Rotation rotation) {
            return sarSmartar_SarCameraDevice_sarGetRotation(self_, out rotation);
        }
        
        public int GetNativeDevice(out IntPtr nativeDevice) {
            return sarSmartar_SarCameraDevice_sarGetNativeDevice(self_, out nativeDevice);
        }
        
        
        // start and stop
        public int Start() {
            return sarSmartar_SarCameraDevice_sarStart(self_);
        }
        
        public int Stop() {
            return sarSmartar_SarCameraDevice_sarStop(self_);
        }
        
        
        // misc
        public int CaptureStillImage() {
            return sarSmartar_SarCameraDevice_sarCaptureStillImage(self_);
        }
        
        public int RunAutoFocus() {
            return sarSmartar_SarCameraDevice_sarRunAutoFocus(self_);
        }
        
        public int RunAutoExposure() {
            return sarSmartar_SarCameraDevice_sarRunAutoExposure(self_);
        }
        
        public int RunAutoWhiteBalance() {
            return sarSmartar_SarCameraDevice_sarRunAutoWhiteBalance(self_);
        }

        // for camera2 api in Android
        public int GetImageSensorRotation(out Rotation rotation) {
            return sarSmartar_SarCameraDevice_sarGetImageSensorRotation(self_, out rotation);   
        }

        public int GetAndroidCameraAPIFeature(out int apiLevel, out int hwFeature) {
            return sarSmartar_SarCameraDevice_sarGetAndroidCameraAPIFeature(self_, out apiLevel, out hwFeature);
        }

        public bool IsAndroidCamera2Available(Smart smart, bool isFront, out int apiLevel, out int hwFeature) {
            return sarSmartar_SarCameraDevice_sarIsAndroidCamera2Available(smart.self_, isFront, out apiLevel, out hwFeature);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarCameraDevice_SarCameraDevice(IntPtr smart, bool forceOldAndroidAPI = false);
        [DllImport("__Internal")]
		private static extern IntPtr sarSmartar_SarCameraDevice_SarCameraDevice2(IntPtr smart, int cameraId, IntPtr nativeDevice, bool forceOldAndroidAPI = false);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarCameraDevice_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetNativeVideoOutput(IntPtr self, IntPtr nativeVideoOutput);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageListener(IntPtr self, IntPtr listener, IntPtr smart);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageSize(IntPtr self, int width, int height);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageFormat(IntPtr self, ImageFormat format);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageFpsRange(IntPtr self, float min, float max);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageSize(IntPtr self, int width, int height);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageFormat(IntPtr self, ImageFormat format);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetShutterListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFocusMode(IntPtr self, FocusMode mode);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFocusAreas(IntPtr self, CameraArea[] areas, int numAreas);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetExposureMode(IntPtr self, ExposureMode mode);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetExposureAreas(IntPtr self, CameraArea[] areas, int numAreas);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFlashMode(IntPtr self, FlashMode mode);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetWhiteBalanceMode(IntPtr self, WhiteBalanceMode mode);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetSceneMode(IntPtr self, SceneMode mode);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoFocusListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoExposureListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoWhiteBalanceListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarSetOwningNativeDevice(IntPtr self, bool isOwning);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetDefaultCameraId(IntPtr smart, Facing facing, out int cameraId, bool forceOldAndroidAPI = false);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageSize(IntPtr self, Size[] sizes, int maxSizes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFormat(IntPtr self, ImageFormat[] formats, int maxFormats);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFpsRange(IntPtr self, CameraFpsRange[] ranges, int maxRanges);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedStillImageSize(IntPtr self, Size[] sizes, int maxSizes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedStillImageFormat(IntPtr self, ImageFormat[] formats, int maxFormats);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedFocusMode(IntPtr self, FocusMode[] modes, int maxModes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetMaxNumFocusAreas(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedFlashMode(IntPtr self, FlashMode[] modes, int maxModes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedExposureMode(IntPtr self, ExposureMode[] modes, int maxModes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetMaxNumExposureAreas(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedWhiteBalanceMode(IntPtr self, WhiteBalanceMode[] modes, int maxModes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedSceneMode(IntPtr self, SceneMode[] modes, int maxModes);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetFovY(IntPtr self, out float fovy, float heightRatio, out bool calibrated);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetFacing(IntPtr self, out Facing facing);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetRotation(IntPtr self, out Rotation rotation);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetNativeDevice(IntPtr self, out IntPtr nativeDevice);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarStart(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarStop(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarCaptureStillImage(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoFocus(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoExposure(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoWhiteBalance(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraDevice_sarGetImageSensorRotation(IntPtr self, out Rotation rotation);
		[DllImport("__Internal")]
		private static extern int sarSmartar_SarCameraDevice_sarGetAndroidCameraAPIFeature(IntPtr self, out int apiLevel, out int hwFeature);
		[DllImport("__Internal")]
		private static extern bool sarSmartar_SarCameraDevice_sarIsAndroidCamera2Available(IntPtr smart, bool isFront, out int apiLevel, out int hwFeature);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarCameraDevice_SarCameraDevice(IntPtr smart, bool forceOldAndroidAPI = false);
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarCameraDevice_SarCameraDevice2(IntPtr smart, int cameraId, IntPtr nativeDevice, bool forceOldAndroidAPI = false);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarCameraDevice_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetNativeVideoOutput(IntPtr self, IntPtr nativeVideoOutput);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageListener(IntPtr self, IntPtr listener, IntPtr smart);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageSize(IntPtr self, int width, int height);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageFormat(IntPtr self, ImageFormat format);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetVideoImageFpsRange(IntPtr self, float min, float max);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageSize(IntPtr self, int width, int height);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetStillImageFormat(IntPtr self, ImageFormat format);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetShutterListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFocusMode(IntPtr self, FocusMode mode);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFocusAreas(IntPtr self, CameraArea[] areas, int numAreas);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetExposureMode(IntPtr self, ExposureMode mode);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetExposureAreas(IntPtr self, CameraArea[] areas, int numAreas);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetFlashMode(IntPtr self, FlashMode mode);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetWhiteBalanceMode(IntPtr self, WhiteBalanceMode mode);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetSceneMode(IntPtr self, SceneMode mode);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoFocusListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoExposureListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetAutoWhiteBalanceListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarSetOwningNativeDevice(IntPtr self, bool isOwning);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetDefaultCameraId(IntPtr smart, Facing facing, out int cameraId, bool forceOldAndroidAPI = false);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageSize(IntPtr self, Size[] sizes, int maxSizes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFormat(IntPtr self, ImageFormat[] formats, int maxFormats);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedVideoImageFpsRange(IntPtr self, CameraFpsRange[] ranges, int maxRanges);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedStillImageSize(IntPtr self, Size[] sizes, int maxSizes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedStillImageFormat(IntPtr self, ImageFormat[] formats, int maxFormats);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedFocusMode(IntPtr self, FocusMode[] modes, int maxModes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetMaxNumFocusAreas(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedFlashMode(IntPtr self, FlashMode[] modes, int maxModes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedExposureMode(IntPtr self, ExposureMode[] modes, int maxModes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetMaxNumExposureAreas(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedWhiteBalanceMode(IntPtr self, WhiteBalanceMode[] modes, int maxModes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetSupportedSceneMode(IntPtr self, SceneMode[] modes, int maxModes);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetFovY(IntPtr self, out float fovy, float heightRatio, out bool calibrated);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetFacing(IntPtr self, out Facing facing);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetRotation(IntPtr self, out Rotation rotation);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetNativeDevice(IntPtr self, out IntPtr nativeDevice);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarStart(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarStop(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarCaptureStillImage(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoFocus(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoExposure(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarRunAutoWhiteBalance(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetImageSensorRotation(IntPtr self, out Rotation rotation);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraDevice_sarGetAndroidCameraAPIFeature(IntPtr self, out int apiLevel, out int hwFeature);
        [DllImport("smartar")]
        private static extern bool sarSmartar_SarCameraDevice_sarIsAndroidCamera2Available(IntPtr smart, bool isFront, out int apiLevel, out int hwFeature);
#endif
    };
}
