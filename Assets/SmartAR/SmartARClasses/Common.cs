using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace smartar {
    // Note that the constants below must match the native-code constants. 
	public class Error {
	    public const int OK                        =           0;
	    public const int ERROR_UNINITIALIZED       = -2138308608;
	    public const int ERROR_ALREADY_INITIALIZED = -2138308607;
	    public const int ERROR_OUT_OF_MEMORY       = -2138308606;
	    public const int ERROR_NOT_STOPPED         = -2138308605;
	    public const int ERROR_NOT_EMPTY           = -2138308604;
	    public const int ERROR_INVALID_VALUE       = -2138308603;
	    public const int ERROR_INVALID_POINTER     = -2138308602;
	    public const int ERROR_ALREADY_REGISTERED  = -2138308601;
	    public const int ERROR_NOT_REGISTERED      = -2138308600;
	    public const int ERROR_ALREADY_STARTED     = -2138308599;
	    public const int ERROR_NOT_STARTED         = -2138308598;
	    public const int ERROR_NOT_REQUIRED        = -2138308597;
	    public const int ERROR_VERSION_MISSMATCH   = -2138308596;
	    public const int ERROR_NO_DICTIONARY       = -2138308595;
	    public const int ERROR_BUSY                = -2138308594;
		public const int ERROR_EXPIRED_LICENSE     = -2138308591;
		public const int ERROR_INVALID_DICTIONARY  = -2138308589;
	}
	
    // * Note that the constants below must match the native-code constants. 
    public enum Facing {
        FACING_BACK,
        FACING_FRONT,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum Rotation {
        ROTATION_0   =   0,
        ROTATION_90  =  90,
        ROTATION_180 = 180,
        ROTATION_270 = 270,
    };
    
    // * Note that the constants below must match the native-code constants. 
    public enum ImageFormat {
        IMAGE_FORMAT_L8,
        IMAGE_FORMAT_YCRCB420,
        IMAGE_FORMAT_YCBCR420,
        IMAGE_FORMAT_RGBA8888,
        IMAGE_FORMAT_JPEG,
    };
	
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
	{
	    public float x_;
	    public float y_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
	{
	    public float x_;
	    public float y_;
	    public float z_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion
	{
	    public float w_;
	    public float x_;
	    public float y_;
	    public float z_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Matrix44
    {
	    public float v00_;
	    public float v01_;
	    public float v02_;
	    public float v03_;
		
	    public float v10_;
	    public float v11_;
	    public float v12_;
	    public float v13_;
		
	    public float v20_;
	    public float v21_;
	    public float v22_;
	    public float v23_;
		
	    public float v30_;
	    public float v31_;
	    public float v32_;
	    public float v33_;
		
		public static smartar.Matrix44 operator*(smartar.Matrix44 lhs, smartar.Matrix44 rhs)
		{
			return sarSmartar_SarMatrix44_sarMulM(ref lhs, ref rhs);
		}
		
#if UNITY_IOS
		[DllImport("__Internal")]
	    private static extern smartar.Matrix44 sarSmartar_SarMatrix44_sarMulM(ref smartar.Matrix44 lhs, ref smartar.Matrix44 rhs);
#else
		[DllImport("smartar")]
	    private static extern smartar.Matrix44 sarSmartar_SarMatrix44_sarMulM(ref smartar.Matrix44 lhs, ref smartar.Matrix44 rhs);
#endif
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Triangle2
    {
		public Vector2 p0_;
		public Vector2 p1_;
		public Vector2 p2_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Triangle3
    {
		public Vector3 p0_;
		public Vector3 p1_;
		public Vector3 p2_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
		public Size(int width, int height) {
			width_ = width;
			height_ = height;
		}
		public int width_;
		public int height_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
		public int left_;
		public int top_;
		public int right_;
		public int bottom_;
	}
	
	[StructLayout(LayoutKind.Sequential)]
    public struct Image
    {
        public IntPtr pixels_;
        public int width_;
        public int height_;
        public int stride_; // 0 means stride == width
        public ImageFormat format_;
	}

    public class SarImage
    {
        public SarImage(Smart smart) {
            self_ = sarSmartar_SarImage_SarImage(smart.self_);
		}
		
		~SarImage() {
			Dispose();
		}

        public void Dispose()
        {
            if (self_ != IntPtr.Zero)
            {
                sarSmartar_SarImage_delete(self_);
                self_ = IntPtr.Zero;
            }
        }

        public void setData(IntPtr pixels)
        {
            sarSmartar_SarImage_setData(self_, pixels);
        }

        public IntPtr getData()
        {
            return sarSmartar_SarImage_getData(self_);
        }

        public void setWidth(int width)
        {
            sarSmartar_SarImage_setWidth(self_, width);
        }

        public int getWidth()
        {
            return sarSmartar_SarImage_getWidth(self_);
        }

        public void setHeight(int height)
        {
            sarSmartar_SarImage_setHeight(self_, height);
        }

        public int getHeight()
        {
            return sarSmartar_SarImage_getHeight(self_);
        }

        public void setStride(int stride)
        {
            sarSmartar_SarImage_setStride(self_, stride); // 0 means stride == width
        }

        public int getStride()
        {
            return sarSmartar_SarImage_getStride(self_);
        }

        public void setImageFormat(ImageFormat format)
        {
            sarSmartar_SarImage_setImageFormat(self_, format);
        }

        public ImageFormat getImageFormat()
        {
            return sarSmartar_SarImage_getImageFormat(self_);
        }

        public IntPtr self_;

#if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarImage_SarImage(IntPtr smart);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_delete(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_setData(IntPtr self, IntPtr pixels);
        [DllImport("__Internal")]
        private static extern IntPtr sarSmartar_SarImage_getData(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_setWidth(IntPtr self, int width);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarImage_getWidth(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_setHeight(IntPtr self, int height);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarImage_getHeight(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_setStride(IntPtr self, int stride);
        [DllImport("__Internal")]
        private static extern int sarSmartar_SarImage_getStride(IntPtr self);
        [DllImport("__Internal")]
        private static extern void sarSmartar_SarImage_setImageFormat(IntPtr self, ImageFormat format);
        [DllImport("__Internal")]
        private static extern ImageFormat sarSmartar_SarImage_getImageFormat(IntPtr self);
#else
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarImage_SarImage(IntPtr smart);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_delete(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_setData(IntPtr self, IntPtr pixels);
        [DllImport("smartar")]
        private static extern IntPtr sarSmartar_SarImage_getData(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_setWidth(IntPtr self, int width);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarImage_getWidth(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_setHeight(IntPtr self, int height);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarImage_getHeight(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_setStride(IntPtr self, int stride);
        [DllImport("smartar")]
        private static extern int sarSmartar_SarImage_getStride(IntPtr self);
        [DllImport("smartar")]
        private static extern void sarSmartar_SarImage_setImageFormat(IntPtr self, ImageFormat format);
        [DllImport("smartar")]
        private static extern ImageFormat sarSmartar_SarImage_getImageFormat(IntPtr self);
#endif
	}
	
    public abstract class StreamIn
    {
        public abstract uint Read(byte[] buf, uint size);
		public abstract void Close();
		
		public IntPtr self_;
	}
	
    public abstract class StreamOut
    {
        public abstract uint Write(byte[] buf, uint size);
		public abstract void Close();
		
		public IntPtr self_;
	}
	
    public class FileStreamIn : StreamIn, IDisposable
    {
        public FileStreamIn(Smart smart, string filePath) {
			self_ = sarSmartar_SarFileStreamIn_SarFileStreamIn(smart.self_, filePath);
		}
		
		~FileStreamIn() {
			Dispose();
		}
		
		public void Dispose() {
			if (self_ != IntPtr.Zero) {
				sarSmartar_SarFileStreamIn_sarDelete(self_);
				self_ = IntPtr.Zero;
			}
		}
		
        public override uint Read(byte[] buf, uint size) {
			return sarSmartar_SarFileStreamIn_sarRead(self_, buf, size);
		}
		
		public override void Close() {
			Dispose();
		}
		
#if UNITY_IOS
		[DllImport("__Internal")]
	    private static extern IntPtr sarSmartar_SarFileStreamIn_SarFileStreamIn(IntPtr smart, string filePath);
		[DllImport("__Internal")]
	    private static extern uint sarSmartar_SarFileStreamIn_sarRead(IntPtr self, byte[] buf, uint size);
		[DllImport("__Internal")]
	    private static extern void sarSmartar_SarFileStreamIn_sarDelete(IntPtr self);
#else
		[DllImport("smartar")]
	    private static extern IntPtr sarSmartar_SarFileStreamIn_SarFileStreamIn(IntPtr smart, string filePath);
		[DllImport("smartar")]
	    private static extern uint sarSmartar_SarFileStreamIn_sarRead(IntPtr self, byte[] buf, uint size);
		[DllImport("smartar")]
	    private static extern void sarSmartar_SarFileStreamIn_sarDelete(IntPtr self);
#endif
	}
	
    public class AssetStreamIn : StreamIn, IDisposable
    {
        public AssetStreamIn(Smart smart, string filePath) {
			self_ = sarSmartar_SarAssetStreamIn_SarAssetStreamIn(smart.self_, filePath);
		}
		
		~AssetStreamIn() {
			Dispose();
		}
		
		public void Dispose() {
			if (self_ != IntPtr.Zero) {
				sarSmartar_SarAssetStreamIn_sarDelete(self_);
				self_ = IntPtr.Zero;
			}
		}
		
        public override uint Read(byte[] buf, uint size) {
			return sarSmartar_SarAssetStreamIn_sarRead(self_, buf, size);
		}
		
		public override void Close() {
			Dispose();
		}
		
#if UNITY_IOS
		[DllImport("__Internal")]
	    private static extern IntPtr sarSmartar_SarAssetStreamIn_SarAssetStreamIn(IntPtr smart, string filePath);
		[DllImport("__Internal")]
	    private static extern uint sarSmartar_SarAssetStreamIn_sarRead(IntPtr self, byte[] buf, uint size);
		[DllImport("__Internal")]
	    private static extern void sarSmartar_SarAssetStreamIn_sarDelete(IntPtr self);
#else
		[DllImport("smartar")]
	    private static extern IntPtr sarSmartar_SarAssetStreamIn_SarAssetStreamIn(IntPtr smart, string filePath);
		[DllImport("smartar")]
	    private static extern uint sarSmartar_SarAssetStreamIn_sarRead(IntPtr self, byte[] buf, uint size);
		[DllImport("smartar")]
	    private static extern void sarSmartar_SarAssetStreamIn_sarDelete(IntPtr self);
#endif
	}
	
    public class FileStreamOut : StreamOut, IDisposable
    {
        public FileStreamOut(Smart smart, string filePath) {
			self_ = sarSmartar_SarFileStreamOut_SarFileStreamOut(smart.self_, filePath);
		}
		
		~FileStreamOut() {
			Dispose();
		}
		
		public void Dispose() {
			if (self_ != IntPtr.Zero) {
				sarSmartar_SarFileStreamOut_sarDelete(self_);
				self_ = IntPtr.Zero;
			}
		}
		
        public override uint Write(byte[] buf, uint size) {
			return sarSmartar_SarFileStreamOut_sarWrite(self_, buf, size);
		}
		
		public override void Close() {
			Dispose();
		}
		
#if UNITY_IOS
		[DllImport("__Internal")]
	    private static extern IntPtr sarSmartar_SarFileStreamOut_SarFileStreamOut(IntPtr smart, string filePath);
		[DllImport("__Internal")]
	    private static extern uint sarSmartar_SarFileStreamOut_sarWrite(IntPtr self, byte[] buf, uint size);
		[DllImport("__Internal")]
	    private static extern void sarSmartar_SarFileStreamOut_sarDelete(IntPtr self);
#else
		[DllImport("smartar")]
	    private static extern IntPtr sarSmartar_SarFileStreamOut_SarFileStreamOut(IntPtr smart, string filePath);
		[DllImport("smartar")]
	    private static extern uint sarSmartar_SarFileStreamOut_sarWrite(IntPtr self, byte[] buf, uint size);
		[DllImport("smartar")]
	    private static extern void sarSmartar_SarFileStreamOut_sarDelete(IntPtr self);
#endif
	}
}
