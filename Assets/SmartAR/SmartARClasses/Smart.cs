using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace smartar {
	public class Smart : IDisposable {
#if UNITY_ANDROID || UNITY_IOS
        public Smart(string filePath) {
			self_ = sarSmartar_SarSmart_SarSmart(filePath);
		}
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        public Smart(string keyString, uint length)
        {
            self_ = sarSmartar_SarSmart_SarSmart2(keyString, length);
        }
#else
        public Smart(string filePath) {
			self_ = sarSmartar_SarSmart_SarSmart(filePath);
		}
#endif

        ~Smart() {
			Dispose();
		}
		
		public void Dispose() {
			if (self_ != IntPtr.Zero) {
				sarSmartar_SarSmart_sarDelete(self_);
				self_ = IntPtr.Zero;
			}
		}

        public int getInitResultCode() {
            return sarSmartar_SarSmart_sarGetInitResultCode(self_);
        }

        public bool isConstructorFailed() {
            return sarSmartar_SarSmart_sarIsConstructorFailed(self_);
        }

        public IntPtr self_;

#if UNITY_IOS
	    [DllImport("__Internal")]
	    private static extern IntPtr sarSmartar_SarSmart_SarSmart(string filePath);
    	[DllImport("__Internal")]
	    private static extern void sarSmartar_SarSmart_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarSmart_sarGetInitResultCode(IntPtr self);
        [DllImport("__Internal")]
        private static extern bool sarSmartar_SarSmart_sarIsConstructorFailed(IntPtr self);
#else
#   if UNITY_ANDROID
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSmart_SarSmart(string filePath);
#   else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarSmart_SarSmart2(string keyString, uint length);
#   endif
        [DllImport("smartar")]
	    private static extern void sarSmartar_SarSmart_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarSmart_sarGetInitResultCode(IntPtr self);
        [DllImport("smartar")]
        private static extern bool sarSmartar_SarSmart_sarIsConstructorFailed(IntPtr self);
#endif
    }
}
