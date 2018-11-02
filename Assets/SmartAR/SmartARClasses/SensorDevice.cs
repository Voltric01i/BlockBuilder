using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace smartar {
    public class SensorState
    {
        public SensorState(IntPtr self) {
            self_ = self;
        }
        
        ~SensorState() {
            Release();
        }
        
        public void Release() {
            self_ = IntPtr.Zero;
        }
        
        public IntPtr self_;
		
		public void getData(byte[] buffer, int offset) {
			Marshal.Copy(self_, buffer, offset, getDataSize());
		}
		
		public static int getDataSize() {
			return sarSmartar_SarSensorState_sarGetDataSize();
		}
		
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorState_sarGetDataSize();
#else
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorState_sarGetDataSize();
#endif
    };
    
    public class SensorDeviceInfo : IDisposable
    {
        public SensorDeviceInfo() {
            self_ = sarSmartar_SarSensorDeviceInfo_SarSensorDeviceInfo();
        }
        
        ~SensorDeviceInfo() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarSensorDeviceInfo_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public IntPtr self_;
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarSensorDeviceInfo_SarSensorDeviceInfo();
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSensorDeviceInfo_sarDelete(IntPtr self);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSensorDeviceInfo_SarSensorDeviceInfo();
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSensorDeviceInfo_sarDelete(IntPtr self);
#endif
    };
    
    public interface SensorListener
    {
        void OnSensor(SensorState state);
    };
    
    public class SensorDevice : IDisposable
    {
        public SensorDevice(Smart smart)
		: this(smart, IntPtr.Zero) {
        }
        
        public SensorDevice(Smart smart, IntPtr nativeDevice) {
            self_ = sarSmartar_SarSensorDevice_SarSensorDevice(smart.self_, nativeDevice);
			
			//---------------------------------------------------------------
			//=================================================================
			thisObj_ = this;
			//=================================================================
            var sensorListenerDelegate = new SensorListenerDelegate(OnSensor);
            proxyListenerDelegates_.sensorListenerDelegate_ = Marshal.GetFunctionPointerForDelegate(sensorListenerDelegate);
			sarSmartar_SarSensorDeviceProxyListeners_sarCreate(ref proxyListenerDelegates_, out proxyListeners_);
			//---------------------------------------------------------------
        }
        
        ~SensorDevice() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarSensorDevice_sarDelete(self_);
                self_ = IntPtr.Zero;
				
				//---------------------------------------------------------------
				sarSmartar_SarSensorDeviceProxyListeners_sarDelete(ref proxyListeners_);
				//=================================================================
				thisObj_ = null;
				//=================================================================
				//---------------------------------------------------------------
            }
        }
        
        public IntPtr self_;
        
		//---------------------------------------------------------------
        private delegate void SensorListenerDelegate(IntPtr state);
			
		//=================================================================
		private static SensorDevice thisObj_ = null;
		
		private class MonoPInvokeCallbackAttribute : System.Attribute
		{
		    protected Type type;
		    public MonoPInvokeCallbackAttribute( Type t ) { type = t; }
		}
		
		[MonoPInvokeCallback (typeof (SensorListenerDelegate))]
        private static void OnSensor(IntPtr state) {
			thisObj_.sensorState_.self_ = state;
        	thisObj_.sensorListener_.OnSensor(thisObj_.sensorState_);
			thisObj_.sensorState_.self_ = IntPtr.Zero;
        }
		//=================================================================
        
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListeners
	    {
	        public IntPtr sensorListener_;
	    };
			
	    [StructLayout(LayoutKind.Sequential)]
	    private struct ProxyListenerDelegates
	    {
            public IntPtr/*SensorListenerDelegate*/ sensorListenerDelegate_;
	    };
			
#if UNITY_IOS
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarSensorDeviceProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("__Internal")]
	    private static extern void sarSmartar_SarSensorDeviceProxyListeners_sarDelete(ref ProxyListeners listeners);
#else
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarSensorDeviceProxyListeners_sarCreate(ref ProxyListenerDelegates delegates, out ProxyListeners listeners);
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarSensorDeviceProxyListeners_sarDelete(ref ProxyListeners listeners);
#endif
		
		private ProxyListenerDelegates proxyListenerDelegates_ = new ProxyListenerDelegates();
		private ProxyListeners proxyListeners_ = new ProxyListeners();
		
	    private SensorListener sensorListener_ = null;
		
		private SensorState sensorState_ = new SensorState(IntPtr.Zero);
		//---------------------------------------------------------------
        
        
        // setting
        public int SetSensorListener(SensorListener listener) {
			//---------------------------------------------------------------
            sensorListener_ = listener;
            return sarSmartar_SarSensorDevice_sarSetSensorListener(self_, listener != null ? proxyListeners_.sensorListener_ : IntPtr.Zero);
			//---------------------------------------------------------------
        }
        
        public int SetOwningNativeDevice(bool isOwning) {
            return sarSmartar_SarSensorDevice_sarSetOwningNativeDevice(self_, isOwning);
        }
        
        
        // get info
        public int GetDeviceInfo(SensorDeviceInfo info) {
            return sarSmartar_SarSensorDevice_sarGetDeviceInfo(self_, info.self_);
        }
        
        public int GetNativeDevice(out IntPtr nativeDevice) {
            return sarSmartar_SarSensorDevice_sarGetNativeDevice(self_, out nativeDevice);
        }
        

        // start and stop
        public int Start() {
            return sarSmartar_SarSensorDevice_sarStart(self_);
        }
        
        public int Stop() {
            return sarSmartar_SarSensorDevice_sarStop(self_);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarSensorDevice_SarSensorDevice(IntPtr smart, IntPtr nativeDevice);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarSensorDevice_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarSetSensorListener(IntPtr self, IntPtr listener);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarSetOwningNativeDevice(IntPtr self, bool isOwning);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarGetDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarGetNativeDevice(IntPtr self, out IntPtr nativeDevice);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarStart(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSensorDevice_sarStop(IntPtr self);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSensorDevice_SarSensorDevice(IntPtr smart, IntPtr nativeDevice);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarSensorDevice_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarSetSensorListener(IntPtr self, IntPtr listener);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarSetOwningNativeDevice(IntPtr self, bool isOwning);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarGetDeviceInfo(IntPtr self, IntPtr info);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarGetNativeDevice(IntPtr self, out IntPtr nativeDevice);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarStart(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSensorDevice_sarStop(IntPtr self);
#endif
    };
}
