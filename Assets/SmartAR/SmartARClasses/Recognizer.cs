using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace smartar {
    // * Note that the constants below must match the native-code constants. 
    public enum RecognitionMode {
        RECOGNITION_MODE_TARGET_TRACKING,
        RECOGNITION_MODE_SCENE_MAPPING,
        //RECOGNITION_MODE_TARGET_TRACKING_AND_SCENE_MAPPING,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum SearchPolicy {
        SEARCH_POLICY_FAST,
        SEARCH_POLICY_PRECISIVE,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum SceneMappingInitMode {
        SCENE_MAPPING_INIT_MODE_TARGET,
        SCENE_MAPPING_INIT_MODE_HFG,
        SCENE_MAPPING_INIT_MODE_VFG,
        SCENE_MAPPING_INIT_MODE_SFM,
        SCENE_MAPPING_INIT_MODE_DRY_RUN,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum TargetTrackingState {
        TARGET_TRACKING_STATE_IDLE,
        TARGET_TRACKING_STATE_SEARCH,
        TARGET_TRACKING_STATE_TRACKING,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum SceneMappingState {
        SCENE_MAPPING_STATE_IDLE,
        SCENE_MAPPING_STATE_SEARCH,
        SCENE_MAPPING_STATE_TRACKING,
        SCENE_MAPPING_STATE_LOCALIZE,
        SCENE_MAPPING_STATE_LOCALIZE_IMPOSSIBLE,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum LandmarkState {
        LANDMARK_STATE_TRACKED,
        LANDMARK_STATE_LOST,
        LANDMARK_STATE_SUSPENDED,
        LANDMARK_STATE_MASKED,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum DenseMapMode {
        DENSE_MAP_DISABLE,
        DENSE_MAP_SEMI_DENSE,
    };
    
    public abstract class Target
    {
        public abstract int GetPhysicalSize(out Vector2 size);
        
        public IntPtr self_;
    };
    
    public class LearnedImageTarget : Target, IDisposable
    {
        public LearnedImageTarget(Smart smart, StreamIn stream, string customerID = null, string customerKey = null) {
            self_ = sarSmartar_SarLearnedImageTarget_SarLearnedImageTarget(smart.self_, stream.self_, customerID, customerKey);
        }
        
        ~LearnedImageTarget() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarLearnedImageTarget_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public override int GetPhysicalSize(out Vector2 size) {
            return sarSmartar_SarLearnedImageTarget_sarGetPhysicalSize(self_, out size);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarLearnedImageTarget_SarLearnedImageTarget(IntPtr smart, IntPtr stream, string customerID, string customerKey);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarLearnedImageTarget_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarLearnedImageTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarLearnedImageTarget_SarLearnedImageTarget(IntPtr smart, IntPtr stream, string customerID, string customerKey);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarLearnedImageTarget_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarLearnedImageTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#endif
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ChildTargetInfo {
        public Vector3 position_;
        public Quaternion rotation_;
    };
    
    public class CompoundTarget : Target, IDisposable
    {
        public CompoundTarget(Smart smart, Target[] childTargets, ChildTargetInfo[] childTargetInfos) {
            IntPtr[] childTargetSelfs = new IntPtr[childTargets.Length];
            for (int i = 0; i < childTargets.Length; ++i) {
                childTargetSelfs[i] = childTargets[i].self_;
            }
			
            self_ = sarSmartar_SarCompoundTarget_SarCompoundTarget(smart.self_, childTargetSelfs, childTargetInfos, childTargets.Length);
        }
        
        ~CompoundTarget() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarCompoundTarget_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public override int GetPhysicalSize(out Vector2 size) {
            return sarSmartar_SarCompoundTarget_sarGetPhysicalSize(self_, out size);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarCompoundTarget_SarCompoundTarget(IntPtr smart, IntPtr[] childTargets, ChildTargetInfo[] childTargetInfos, int numChildTargets);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarCompoundTarget_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCompoundTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarCompoundTarget_SarCompoundTarget(IntPtr smart, IntPtr[] childTargets, ChildTargetInfo[] childTargetInfos, int numChildTargets);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarCompoundTarget_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCompoundTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#endif
    };
    
    public class SceneMapTarget : Target, IDisposable
    {
        public SceneMapTarget(Smart smart, StreamIn stream) {
            self_ = sarSmartar_SarSceneMapTarget_SarSceneMapTarget(smart.self_, stream.self_);
        }
        
        ~SceneMapTarget() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarSceneMapTarget_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public override int GetPhysicalSize(out Vector2 size) {
            return sarSmartar_SarSceneMapTarget_sarGetPhysicalSize(self_, out size);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarSceneMapTarget_SarSceneMapTarget(IntPtr smart, IntPtr stream);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSceneMapTarget_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSceneMapTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSceneMapTarget_SarSceneMapTarget(IntPtr smart, IntPtr stream);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSceneMapTarget_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSceneMapTarget_sarGetPhysicalSize(IntPtr self, out Vector2 size);
#endif
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct RecognitionRequest {
        //public RequestMode requestMode_;
        public IntPtr image_;
        public ulong timestamp_;
        public int numSensorStates_;
        public IntPtr/*SensorState[]*/ sensorStates_;
        
        public int numTriangulateMasks_;
        public IntPtr/*Triangle2[]*/ triangulateMasks_;
        
		// * request.targets_ is removed
        //public int numTargets_;
        //public IntPtr/*Target[]*/ targets_;
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Landmark
    {
        public uint id_;
        public LandmarkState state_;
        public Vector3 position_;
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NodePoint
    {
        public uint id_;
        public Vector3 position_;
    };
    
	[StructLayout(LayoutKind.Sequential)]
	public struct InitPoint
	{
		public uint id_;
		public Vector2 position_;
	};
    
	[StructLayout(LayoutKind.Sequential)]
    public struct RecognitionResult
    {
        //public RecognitionMode mode_;
        public IntPtr/*Target*/ target_;
        public bool isRecognized_;
        public Vector3 position_;
        public Quaternion rotation_;
        
        public ulong timestamp_;
        
        public Vector3 velocity_;
        public Vector3 angularVelocity_;
        
        // for target tracking
        public TargetTrackingState targetTrackingState_;
        
        // for scene mapping
        public SceneMappingState sceneMappingState_;
        
        public int numLandmarks_;
        public int maxLandmarks_;
        public IntPtr/*Landmark[]*/ landmarks_;
        
        public int numNodePoints_;
        public int maxNodePoints_;
        public IntPtr/*NodePoint[]*/ nodePoints_;
	
		public int numInitPoints_;
		public int maxInitPoints_;
		public IntPtr/*InitPoint[]*/ initPoints_;
	};
        
    public interface WorkDispatchedListener
    {
        void OnWorkDispatched();
    };
    
    public class RecognitionResultHolder
    {
        public RecognitionResultHolder(IntPtr self) {
            self_ = self;
        }
        
        ~RecognitionResultHolder() {
            Release();
        }
        
        public void Release() {
            self_ = IntPtr.Zero;
        }
        
        public IntPtr self_;
        
        public int GetNumResults() {
            return sarSmartar_SarRecognitionResultHolder_sarGetNumResults(self_);
        }
        
        public int GetResults(RecognitionResult[] results) {
			GCHandle gch = GCHandle.Alloc(results, GCHandleType.Pinned);
			IntPtr addr = gch.AddrOfPinnedObject();
            int res = sarSmartar_SarRecognitionResultHolder_sarGetResults(self_, addr, results.Length);
			gch.Free();
			return res;
        }
        
        public int GetResult(Target target, out RecognitionResult result) {
            return sarSmartar_SarRecognitionResultHolder_sarGetResult(self_, target.self_, out result);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetNumResults(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetResults(IntPtr self, IntPtr results, int maxResults);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetResult(IntPtr self, IntPtr target, out RecognitionResult result);
#else
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetNumResults(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetResults(IntPtr self, IntPtr results, int maxResults);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognitionResultHolder_sarGetResult(IntPtr self, IntPtr target, out RecognitionResult result);
#endif
    };
    
    public interface RecognizedListener
    {
        void OnRecognized(RecognitionResultHolder resultHolder);
    };
    
    public class Recognizer : IDisposable
    {
        // * Note that the constants below must match the native-code constants. 
        public static readonly int MAX_NUM_LANDMARKS = 512;
        public static readonly int MAX_NUM_NODE_POINTS = 2048;
		public static readonly int MAX_NUM_INITIALIZATION_POINTS = 64;
		public static readonly int MAX_PROPAGATION_DURATION = 3000000; //usec
    
        public Recognizer(Smart smart, RecognitionMode recogMode = RecognitionMode.RECOGNITION_MODE_TARGET_TRACKING, SceneMappingInitMode initMode = SceneMappingInitMode.SCENE_MAPPING_INIT_MODE_TARGET) {
            self_ = sarSmartar_SarRecognizer_SarRecognizer(smart.self_, recogMode, initMode);
			
			//---------------------------------------------------------------
			//=================================================================
			thisObj_ = this;
			//=================================================================
            var workDispatchedListenerDelegate = new WorkDispatchedListenerDelegate(OnWorkDispatched);
            proxyListenerDelegates_.workDispatchedListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(workDispatchedListenerDelegate);
            var recognizedListenerDelegate = new RecognizedListenerDelegate(OnRecognized);
            proxyListenerDelegates_.recognizedListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(recognizedListenerDelegate);
			sarSmartar_SarRecognizerProxyListeners_sarCreate(ref proxyListenerDelegates_, out proxyListeners_);
			//---------------------------------------------------------------
        }
        
        ~Recognizer() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarRecognizer_sarDelete(self_);
                self_ = IntPtr.Zero;
				
				//---------------------------------------------------------------
				sarSmartar_SarRecognizerProxyListeners_sarDelete(ref proxyListeners_);
				//=================================================================
				thisObj_ = null;
				//=================================================================
				//---------------------------------------------------------------
            }
        }
        
        public IntPtr self_;
        
		//---------------------------------------------------------------
        private delegate void WorkDispatchedListenerDelegate();
        private delegate void RecognizedListenerDelegate(IntPtr resultHolder);
		
		//=================================================================
		private static Recognizer thisObj_ = null;
		
		private class MonoPInvokeCallbackAttribute : System.Attribute
		{
		    protected Type type;
		    public MonoPInvokeCallbackAttribute( Type t ) { type = t; }
		}
		
		[MonoPInvokeCallback (typeof (WorkDispatchedListenerDelegate))]
        private static void OnWorkDispatched() {
        	thisObj_.workDispatchedListener_.OnWorkDispatched();
        }
        
		[MonoPInvokeCallback (typeof (RecognizedListenerDelegate))]
        private static void OnRecognized(IntPtr resultHolder) {
			thisObj_.resultHolder_.self_ = resultHolder;
        	thisObj_.recognizedListener_.OnRecognized(thisObj_.resultHolder_);
			thisObj_.resultHolder_.self_ = IntPtr.Zero;
        }
		//=================================================================
        
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListeners
	    {
	        public IntPtr workDispatchedListener_;
	        public IntPtr recognizedListener_;
	    };
		
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListenerDelegates
	    {
            public IntPtr/*WorkDispatchedListenerDelegate*/ workDispatchedListenerDelegate_;
            public IntPtr/*RecognizedListenerDelegate*/ recognizedListenerDelegate_;
	    };
		
#if UNITY_IOS
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarRecognizerProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarRecognizerProxyListeners_sarDelete(ref ProxyListeners listeners);
#else
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarRecognizerProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarRecognizerProxyListeners_sarDelete(ref ProxyListeners listeners);
#endif
		
		private ProxyListenerDelegates proxyListenerDelegates_ = new ProxyListenerDelegates();
		private ProxyListeners proxyListeners_ = new ProxyListeners();
		
	    private WorkDispatchedListener workDispatchedListener_ = null;
	    private RecognizedListener recognizedListener_ = null;
		
		private RecognitionResultHolder resultHolder_ = new RecognitionResultHolder(IntPtr.Zero);
		//---------------------------------------------------------------
        
        
        // setting
        public int SetCameraDeviceInfo(CameraDeviceInfo info) {
            return sarSmartar_SarRecognizer_sarSetCameraDeviceInfo(self_, info.self_);
        }
        
        public int SetSensorDeviceInfo(SensorDeviceInfo info) {
            return sarSmartar_SarRecognizer_sarSetSensorDeviceInfo(self_, info.self_);
        }
        
		public int SetTargets(Target[] targets/*, RequestMode mode = REQUEST_MODE_DEFAULT*/) {
			if (targets != null) { 
				IntPtr[] targetSelfs = new IntPtr[targets.Length];
				for (int i = 0; i < targets.Length; ++i) {
                    if (targets[i] == null) { break; }
					targetSelfs[i] = targets[i].self_;
				}
				
				return sarSmartar_SarRecognizer_sarSetTargets(self_, targetSelfs, targets.Length/*, RequestMode mode = REQUEST_MODE_DEFAULT*/);
			}
			else {
				return sarSmartar_SarRecognizer_sarSetTargets(self_, null, 0);
			}
		}
        
        // start and stop
        public int Reset() {
            return sarSmartar_SarRecognizer_sarReset(self_);
        }
        
        
        // run
        public int Run(RecognitionRequest request) {
            return sarSmartar_SarRecognizer_sarRun(self_, ref request);
        }
        
        public int Dispatch(RecognitionRequest request) {
            return sarSmartar_SarRecognizer_sarDispatch(self_, ref request);
        }
        
        public int RunWorker() {
            return sarSmartar_SarRecognizer_sarRunWorker(self_);
        }
        
        public int SetWorkDispatchedListener(WorkDispatchedListener listener) {
			//---------------------------------------------------------------
            workDispatchedListener_ = listener;
            return sarSmartar_SarRecognizer_sarSetWorkDispatchedListener(self_, listener != null ? proxyListeners_.workDispatchedListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        
        // get results
        public int GetNumResults() {
            return sarSmartar_SarRecognizer_sarGetNumResults(self_);
        }
        
        public int GetResults(RecognitionResult[] results) {
			GCHandle gch = GCHandle.Alloc(results, GCHandleType.Pinned);
			IntPtr addr = gch.AddrOfPinnedObject();
            int res = sarSmartar_SarRecognizer_sarGetResults(self_, addr, results.Length);
			gch.Free();
			return res;
        }
        
        public int GetResult(Target target, out RecognitionResult result) {
            return sarSmartar_SarRecognizer_sarGetResult(self_, target.self_, out result);
        }
        
        public int SetRecognizedListener(RecognizedListener listener) {
			//---------------------------------------------------------------
            recognizedListener_ = listener;
            return sarSmartar_SarRecognizer_sarSetRecognizedListener(self_, listener != null ? proxyListeners_.recognizedListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        
        public int SetMaxTargetsPerFrame(int maxTargets) {
            return sarSmartar_SarRecognizer_sarSetMaxTargetsPerFrame(self_, maxTargets);
        }
        
        public int SetSearchPolicy(SearchPolicy policy) {
            return sarSmartar_SarRecognizer_sarSetSearchPolicy(self_, policy);
        }
        
        
        public int PropagateResult(RecognitionResult fromResult, out RecognitionResult toResult, ulong timestamp, bool useVelocity = true) {
            return sarSmartar_SarRecognizer_sarPropagateResult(self_, ref fromResult, out toResult, timestamp, useVelocity);
        }
        
        public int SetMaxTriangulateMasks(int maxMasks) {
            return sarSmartar_SarRecognizer_sarSetMaxTriangulateMasks(self_, maxMasks);
        }
        
        
        // for scene mapping
        public int SaveSceneMap(StreamOut stream) {
            return sarSmartar_SarRecognizer_sarSaveSceneMap(self_, stream.self_);
        }
        
        public int FixSceneMap(bool isFix) {
            return sarSmartar_SarRecognizer_sarFixSceneMap(self_, isFix);
        }
        
        public int ForceLocalize() {
            return sarSmartar_SarRecognizer_sarForceLocalize(self_);
        }
        
        public int RemoveLandmark(Landmark landmark) {
            return sarSmartar_SarRecognizer_sarRemoveLandmark(self_, ref landmark);
        }
        
        public int SetDenseMapMode(DenseMapMode mode) {
            return sarSmartar_SarRecognizer_sarSetDenseMapMode(self_, mode);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarRecognizer_SarRecognizer(IntPtr smart, RecognitionMode recogMode, SceneMappingInitMode initMode);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarRecognizer_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetCameraDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetSensorDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetTargets(IntPtr self, IntPtr[] targets, int numTargets/*, RequestMode mode = REQUEST_MODE_DEFAULT*/);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarReset(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarRun(IntPtr self, ref RecognitionRequest request);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarDispatch(IntPtr self, ref RecognitionRequest request);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarRunWorker(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetWorkDispatchedListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarGetNumResults(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarGetResults(IntPtr self, IntPtr results, int maxResults);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarGetResult(IntPtr self, IntPtr target, out RecognitionResult result);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetRecognizedListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetMaxTargetsPerFrame(IntPtr self, int maxTargets);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetSearchPolicy(IntPtr self, SearchPolicy policy);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarPropagateResult(IntPtr self, ref RecognitionResult fromResult, out RecognitionResult toResult, ulong timestamp, bool useVelocity);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetMaxTriangulateMasks(IntPtr self, int maxMasks);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSaveSceneMap(IntPtr self, IntPtr stream);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarFixSceneMap(IntPtr self, bool isFix);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarForceLocalize(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarRemoveLandmark(IntPtr self, ref Landmark landmark);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarRecognizer_sarSetDenseMapMode(IntPtr self, DenseMapMode mode);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarRecognizer_SarRecognizer(IntPtr smart, RecognitionMode recogMode, SceneMappingInitMode initMode);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarRecognizer_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetCameraDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetSensorDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetTargets(IntPtr self, IntPtr[] targets, int numTargets/*, RequestMode mode = REQUEST_MODE_DEFAULT*/);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarReset(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarRun(IntPtr self, ref RecognitionRequest request);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarDispatch(IntPtr self, ref RecognitionRequest request);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarRunWorker(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetWorkDispatchedListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarGetNumResults(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarGetResults(IntPtr self, IntPtr results, int maxResults);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarGetResult(IntPtr self, IntPtr target, out RecognitionResult result);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetRecognizedListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetMaxTargetsPerFrame(IntPtr self, int maxTargets);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetSearchPolicy(IntPtr self, SearchPolicy policy);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarPropagateResult(IntPtr self, ref RecognitionResult fromResult, out RecognitionResult toResult, ulong timestamp, bool useVelocity);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetMaxTriangulateMasks(IntPtr self, int maxMasks);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSaveSceneMap(IntPtr self, IntPtr stream);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarFixSceneMap(IntPtr self, bool isFix);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarForceLocalize(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarRemoveLandmark(IntPtr self, ref Landmark landmark);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarRecognizer_sarSetDenseMapMode(IntPtr self, DenseMapMode mode);
#endif
    };
}
