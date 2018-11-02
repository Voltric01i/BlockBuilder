using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace smartar {
    public class ScreenDevice : IDisposable
    {
        public ScreenDevice(Smart smart) {
			self_ = sarSmartar_SarScreenDevice_SarScreenDevice(smart.self_);
        }
        
        ~ScreenDevice() {
            Dispose();
        }
        
        public void Dispose() {
			if (self_ != IntPtr.Zero) {
				sarSmartar_SarScreenDevice_sarDelete(self_);
				self_ = IntPtr.Zero;
			}
        }
        
		public IntPtr self_;
		
		public int GetRotation(out Rotation rotation) {
			return sarSmartar_SarScreenDevice_sarGetRotation(self_, out rotation);
		}
        
#if UNITY_IOS
	    [DllImport("__Internal")]
	    private static extern IntPtr sarSmartar_SarScreenDevice_SarScreenDevice(IntPtr self);
    	[DllImport("__Internal")]
	    private static extern void sarSmartar_SarScreenDevice_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarScreenDevice_sarGetRotation(IntPtr self, out Rotation rotation);
#else
	    [DllImport("smartar")]
	    private static extern IntPtr sarSmartar_SarScreenDevice_SarScreenDevice(IntPtr self);
    	[DllImport("smartar")]
	    private static extern void sarSmartar_SarScreenDevice_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarScreenDevice_sarGetRotation(IntPtr self, out Rotation rotation);
#endif
    };
}
