using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace smartar {
    public class CameraImageDrawer : IDisposable
    {
        private enum RenderEventID
        {
            Start = 1001,
            Draw,
            Draw2,
        }

        public CameraImageDrawer(Smart smart) {
            self_ = sarSmartar_SarCameraImageDrawer_SarCameraImageDrawer(smart.self_);
        }
        
        ~CameraImageDrawer() {
            Dispose();
        }
        
        public void Dispose() {
            if (self_ != IntPtr.Zero) {
                sarSmartar_SarCameraImageDrawer_sarDelete(self_);
                self_ = IntPtr.Zero;
            }
        }
        
        public IntPtr self_;
        
        public int SetDrawRange(float x1, float y1, float x2, float y2) {
            return sarSmartar_SarCameraImageDrawer_sarSetDrawRange(self_, x1, y1, x2, y2);
        }
        
        public int SetRotation(Rotation rotation) {
            return sarSmartar_SarCameraImageDrawer_sarSetRotation(self_, rotation);
        }
		
        public int SetFlipX(bool flipX) {
            return sarSmartar_SarCameraImageDrawer_sarSetFlipX(self_, flipX);
        }
		
        public int SetFlipY(bool flipY) {
            return sarSmartar_SarCameraImageDrawer_sarSetFlipY(self_, flipY);
        }
		
        public int Start() {
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.Start);
            GL.InvalidateState();
            return 0;
        }
        
        public int Stop() {
            return sarSmartar_SarCameraImageDrawer_sarStop(self_);
        }
        
        public int Draw(Image image) {
            sarSmartar_SarCameraImageDrawer_sarSetDrawData(ref image);
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.Draw);
            GL.InvalidateState();
            return 0;
        }
        
        public int Draw(Image image, Rect rect) {
            sarSmartar_SarCameraImageDrawer_sarSetDrawData2(ref image, ref rect);
            GL.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEventID.Draw2);
            GL.InvalidateState();
            return 0;
        }
        
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarCameraImageDrawer_SarCameraImageDrawer(IntPtr smart);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarCameraImageDrawer_sarDelete(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawRange(IntPtr self, float x1, float y1, float x2, float y2);
        [DllImport("__Internal")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetRotation(IntPtr self, Rotation rotation);
        [DllImport("__Internal")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetFlipX(IntPtr self, bool flipX);
        [DllImport("__Internal")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetFlipY(IntPtr self, bool flipY);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarStart(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarStop(IntPtr self);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarDraw(IntPtr self, IntPtr image);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarDraw2(IntPtr self, IntPtr image, ref Rect rect);

        // for Unity5.4 and later
        [DllImport("__Internal")]
        private static extern IntPtr GetRenderEventFunc();
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawData(ref Image image);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawData2(ref Image image, ref Rect rect);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarCameraImageDrawer_SarCameraImageDrawer(IntPtr smart);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarCameraImageDrawer_sarDelete(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawRange(IntPtr self, float x1, float y1, float x2, float y2);
        [DllImport("smartar")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetRotation(IntPtr self, Rotation rotation);
        [DllImport("smartar")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetFlipX(IntPtr self, bool flipX);
        [DllImport("smartar")]
	    private static extern int sarSmartar_SarCameraImageDrawer_sarSetFlipY(IntPtr self, bool flipY);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarStart(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarStop(IntPtr self);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarDraw(IntPtr self, IntPtr image);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarDraw2(IntPtr self, IntPtr image, ref Rect rect);

        // for Unity5.4 and later
        [DllImport("smartar")]
        private static extern IntPtr GetRenderEventFunc();
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawData(ref Image image);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarCameraImageDrawer_sarSetDrawData2(ref Image image, ref Rect rect);
#endif
    };
}
