using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace smartar {
	public struct Utility {
	   	public static int convertPose2Matrix(smartar.Vector3 position, smartar.Quaternion rotation, out smartar.Matrix44 matrix)
		{
			return sarSmartar_sarConvertPose2Matrix(ref position, ref rotation, out matrix);
		}
		
    	public static smartar.Matrix44 setPerspectiveM(float fovy, float aspect, float near, float far)
		{
			return sarSmartar_sarSetPerspectiveM(fovy, aspect, near, far);
		}
		
	    public static void memcpy(IntPtr dst, IntPtr src, int length)
		{
			sarSmartar_sarMemcpy(dst, src, length);
		}
		
		public static bool isMultiCore()
		{
			return sarSmartar_sarIsMultiCore();
		}
		
#if UNITY_IOS
		[DllImport("__Internal")]
	   	private static extern int sarSmartar_sarConvertPose2Matrix(ref smartar.Vector3 position, ref smartar.Quaternion rotation, out smartar.Matrix44 matrix);
		[DllImport("__Internal")]
	    private static extern smartar.Matrix44 sarSmartar_sarSetPerspectiveM(float fovy, float aspect, float near, float far);
		[DllImport("__Internal")]
	    private static extern void sarSmartar_sarMemcpy(IntPtr dst, IntPtr src, int length);
		[DllImport("__Internal")]
	    private static extern bool sarSmartar_sarIsMultiCore();
#else
		[DllImport("smartar")]
		private static extern int sarSmartar_sarConvertPose2Matrix(ref smartar.Vector3 position, ref smartar.Quaternion rotation, out smartar.Matrix44 matrix);
		[DllImport("smartar")]
	    private static extern smartar.Matrix44 sarSmartar_sarSetPerspectiveM(float fovy, float aspect, float near, float far);
		[DllImport("smartar")]
	    private static extern void sarSmartar_sarMemcpy(IntPtr dst, IntPtr src, int length);
		[DllImport("smartar")]
	    private static extern bool sarSmartar_sarIsMultiCore();
#endif
	}
	
    public class LandmarkDrawer : IDisposable
    {
        private enum RenderEventID
        {
            Start = 2001,
        }

        public LandmarkDrawer(Smart smart) {
            self_ = sarSmartar_SarLandmarkDrawer_SarLandmarkDrawer(smart.self_);
        }
        
        ~LandmarkDrawer() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarLandmarkDrawer_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public IntPtr self_;
        
        public int Start() {
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.Start);
            GL.InvalidateState();
            return 0;
        }
        
        public int Stop() {
            return sarSmartar_SarLandmarkDrawer_sarStop(self_);
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarLandmarkDrawer_SarLandmarkDrawer(IntPtr smart);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarLandmarkDrawer_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarStart(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarStop(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarDrawLandmarks(IntPtr self, ref smartar.Matrix44 pmvMatrix, IntPtr landmarks, int numLandmarks);
        [DllImport("__Internal")]
		private static extern int sarSmartar_SarLandmarkDrawer_sarDrawNodePoints(IntPtr self, ref smartar.Matrix44 pmvMatrix, IntPtr nodePoints, int numNodePoints);
		[DllImport("__Internal")]
		private static extern int sarSmartar_SarLandmarkDrawer_sarDrawInitPoints(IntPtr self, ref smartar.Matrix44 imageMatrix, IntPtr initPoints, int numInitPoints);
        // for Unity5.4 and later
        [DllImport("__Internal")]
        private static extern IntPtr GetRenderEventFunc();
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarLandmarkDrawer_SarLandmarkDrawer(IntPtr smart);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarLandmarkDrawer_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarStart(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarStop(IntPtr self);
        [DllImport("smartar")]
		private static extern int sarSmartar_SarLandmarkDrawer_sarDrawLandmarks(IntPtr self, ref smartar.Matrix44 pmvMatrix, IntPtr landmarks, int numLandmarks);
		[DllImport("smartar")]
		private static extern int sarSmartar_SarLandmarkDrawer_sarDrawNodePoints(IntPtr self, ref smartar.Matrix44 pmvMatrix, IntPtr nodePoints, int numNodePoints);
		[DllImport("smartar")]
        private static extern int sarSmartar_SarLandmarkDrawer_sarDrawInitPoints(IntPtr self, ref smartar.Matrix44 imageMatrix, IntPtr initPoints, int numInitPoints);

        // for Unity5.4 and later
        [DllImport("smartar")]
        private static extern IntPtr GetRenderEventFunc();
#endif
    };
}
