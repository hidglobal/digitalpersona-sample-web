/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: DPFPApiSupport.cs     

Fingerprint support - collecting the fingerprint image. Loads the DP's Fingerprint library DPFPApi.dll and provides support for FP readers.
--*/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using DigitalPersona.Application.Native;

namespace DigitalPersona.Application.FingerprintSupport
{
    // DPFPApi structures
    public class DPFPApiSupport : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DATA_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }
        //----------------------
        // FD_IMAGE structs and defines
        [StructLayout(LayoutKind.Sequential, Pack=8)]
        public struct FD_IMAGE_FORMAT_VERSION
        {
            public uint uMajor;
            public uint	uMinor;
            public uint	uBuild;
        }
        [StructLayout(LayoutKind.Sequential, Pack=8)]
        public struct FD_DATA_HEADER
        {
            public uint	uDataType;
            public Int64 DeviceId;
            public Int64 DeviceType;
            public int   DataAcquisitionProgress;
        }

        private enum FD_IMAGE_TYPE
        {
            FD_IMAGE_UNKNOWN_TYPE = 0,
            FD_IMAGE_BLACK_WHITE = 1,
            FD_IMAGE_GRAY_SCALE = 2,
            FD_IMAGE_COLOR = 3
        }
        private enum FD_IMAGE_PADDING
        {
            FD_IMAGE_NO_PADDING = 0,
            FD_IMAGE_LEFT_PADDING = 1,
            FD_IMAGE_RIGHT_PADDING = 2
        };
        private enum FD_IMAGE_POLARITY
        {
            FD_IMAGE_UNKNOWN_POLARITY = 0,
            FD_IMAGE_NEGATIVE_POLARITY = 1,
            FD_IMAGE_POSITIVE_POLARITY = 2
        };
        private enum FD_IMAGE_COLOR_REPRESENTATION
        {
            FD_IMAGE_NO_COLOR_REPRESENTATION = 0,
            FD_IMAGE_PLANAR_COLOR_REPRESENTATION = 1,
            FD_IMAGE_INTERLEAVED_COLOR_REPRESENTATION = 2
        };

        [StructLayout(LayoutKind.Sequential, Pack=8)]
        public struct FD_IMAGE_FORMAT
        {
            public uint uDataType;             /* is 1 which represents 2D image		    */
            public uint uImageType;            /* B&W, gray or color images                 */
            public int iWidth;                 /* image width [pixel]                       */
            public int iHeight;                /* image height [pixel]                      */
            public int iXdpi;                  /* X resolution [DPI]                        */
            public int iYdpi;                  /* Y resolution [DPI]                        */
            public uint uBPP;                   /* number of bits per pixel                 */
            public uint uPadding;               /* right or left padding                    */
            public uint uSignificantBpp;        /* number of significant bits per pixel     */
            public uint uPolarity;              /* positive=black print on white background */
            public uint uRGBcolorRepresentation;
            public uint uPlanes;                /* color planes.                             */
        }
        [StructLayout(LayoutKind.Sequential, Pack=8)]
        public struct FD_IMAGE_HEADER
        {
            public FD_IMAGE_FORMAT_VERSION	version;      /* version of the image format               */
            public FD_IMAGE_FORMAT ImageFormat;  /* image format of the current data          */
        }
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct FD_IMAGE
        {
            public uint uSize;                /* size of the blob                          */
            public FD_DATA_HEADER DataHeader; /* data information                          */
            public uint uOffsetToImage;       /* offset to the beginning of image          */
            public FD_IMAGE_HEADER Header;    /* current image format information          */
            public uint uExtensionSize;       /* additional header info starts right after */
        }
//----------------------
        private enum DP_ACQUISITION_PRIORITY
        {
            DP_PRIORITY_HIGH = 1,	// High priority.
            DP_PRIORITY_NORMAL = 2, // Normal priority.
            DP_PRIORITY_LOW = 3		// Low priority.
        };

        private enum DP_SAMPLE_TYPE
        {
            DP_SAMPLE_TYPE_REG = 1,	
            DP_SAMPLE_TYPE_VER = 2,
            DP_SAMPLE_TYPE_EXTRACT = 4 // Feature extraction
        };

        private enum DP_FP_MESSAGES
        {
            WN_COMPLETED = 0,			// A supplied fingerprint credential was acquired. lParam contains the handle to a parameter of type HDPCREDENTIAL.
            WN_ERROR = 1,				// An error occurred. lParam contains the error code that is returned.
            WN_DISCONNECT = 2,			// The fingerprint reader was disconnected. lParam contains a pointer to the device UID. See the structure of type DP_DEVICE_INFO.
            WN_RECONNECT = 3,			// The fingerprint reader was reconnected. lParam contains a pointer to the device UID. See the structure of type DP_DEVICE_INFO.
            WN_SAMPLE_QUALITY = 4,		// Provides information about the quality of the fingerprint image. lParam contains the fingerprint image quality listed in the enum of type DP_SAMPLE_QUALITY.
            WN_FINGER_TOUCHED = 5,		// The fingerprint reader was touched.
            WN_FINGER_GONE = 6, 		// The finger was removed from the fingerprint reader.
            WN_IMAGE_READY = 7,			// A fingerprint image is ready for processing.
            WN_FEATURE_SET_ADDED = 8,	// A supplied fingerprint credential was added to the stored fingerprint credential.
            WN_REGISTRATION_COMPLETED = 9, // A fingerprint enrollment template was created.
            WN_OPERATION_STOPPED = 10,	// The operation was stopped by calling either the DPFPStopAcquisition function or the DPFPStopRegistration function.
        };
        // DPFPApi functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPFPInit();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DPFPTerm();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPFPCreateAcquisition(DP_ACQUISITION_PRIORITY eAcquisitionPriority, ref Guid DevUID, DP_SAMPLE_TYPE uSampleType, IntPtr hWnd, uint uMsg, ref uint phOperation);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPFPDestroyAcquisition(uint hOperation);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPFPStartAcquisition(uint hOperation);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPFPStopAcquisition(uint hOperation);

        private DPFPInit _pDPFPInit = null;
        private DPFPTerm _pDPFPTerm = null;
        private DPFPCreateAcquisition _pDPFPCreateAcquisition = null;
        private DPFPDestroyAcquisition _pDPFPDestroyAcquisition = null;
        private DPFPStartAcquisition _pDPFPStartAcquisition = null;
        private DPFPStopAcquisition _pDPFPStopAcquisition = null;

        private IntPtr m_hModule = IntPtr.Zero;
        private IntPtr hWndOwner = IntPtr.Zero;
        private IntPtr FPSample = IntPtr.Zero;

        private uint fpOpHandle = 0;
//-----------------------------------------------------------------------------------
// public stuff
        public event EventHandler SampleCollected;
        static public uint fpEventMsg = NativeMethods.WM_USER + 205; // Message received from DPFPApi for fingerprint user actions
        public IntPtr fd_image = IntPtr.Zero;
        public int fd_imageSize = 0;

        public DPFPApiSupport( IntPtr Owner )
        {
            if (!load())
                throw new ApplicationException("Cannot load DPFPApi!");
            int hr = _pDPFPInit();
            if ( hr < 0 )
                throw new ApplicationException(String.Format("Cannot initialize DPFPApi, error 0x{0,1:X}.", hr));
            hWndOwner = Owner;
        }

        ~DPFPApiSupport()
        {
            Dispose();
        }

        public void ProcessSample(Message message)
        {
            try
            {
                if (fpOpHandle != 0)
                {
                    switch (message.WParam.ToInt32())
                    {
                        case (Int32)DP_FP_MESSAGES.WN_COMPLETED:
                        { //  The only message we need - a supplied fingerprint credential was acquired successfully.
                            Reset();
                            DATA_BLOB Blob = (DATA_BLOB)Marshal.PtrToStructure(message.LParam, typeof(DATA_BLOB));
                            if (Blob.cbData > 0)
                            {
                                fd_image = Marshal.AllocHGlobal((int)Blob.cbData);
                                if (fd_image != null)
                                {
                                    fd_imageSize = (int)Blob.cbData;
                                    NativeMethods.CopyMemory(fd_image, Blob.pbData, Blob.cbData);
                                    // fingerprint collected - notify the owner form
                                    OnSampleCollected(EventArgs.Empty);
                                }
                            }
                            break;
                        }
                        case (Int32)DP_FP_MESSAGES.WN_ERROR:
                        { // An error occurred. lParam contains the error code that is returned.
                            Reset();
                            throw new ApplicationException(String.Format("Cannot get FP sample, error 0x{0,1:X}.", message.LParam.ToInt32()));
                        }
                        case (Int32)DP_FP_MESSAGES.WN_DISCONNECT:
                        case (Int32)DP_FP_MESSAGES.WN_RECONNECT:
                        case (Int32)DP_FP_MESSAGES.WN_FINGER_TOUCHED:
                        case (Int32)DP_FP_MESSAGES.WN_FINGER_GONE:
                        case (Int32)DP_FP_MESSAGES.WN_SAMPLE_QUALITY:
                        case (Int32)DP_FP_MESSAGES.WN_IMAGE_READY:       // A fingerprint image is ready for processing.
                        case (Int32)DP_FP_MESSAGES.WN_OPERATION_STOPPED: // The operation was stopped by calling the DPFPStopAcquisition function
                            break;
                    } // switch()
                }
            }
            catch
            { }
        }

        public int StartSample()
        {   // Start collecting fingerprint
            int hr = 0;
            try
            {
                Reset();
                Guid device_id = Guid.Empty;
                hr = _pDPFPCreateAcquisition(DP_ACQUISITION_PRIORITY.DP_PRIORITY_NORMAL, ref device_id, DP_SAMPLE_TYPE.DP_SAMPLE_TYPE_EXTRACT, hWndOwner, fpEventMsg, ref fpOpHandle);
                if ((hr >= 0) && (fpOpHandle != 0))
                    hr = _pDPFPStartAcquisition(fpOpHandle);
            }
            catch
            {
                hr = -1;
            }
            if (hr < 0)
                throw new ApplicationException(String.Format("Cannot get FP sample, error 0x{0,1:X}.", hr));
            return hr;
        }

        public void Reset()
        {   // Clean up, delete collecting fingerprint
            if (fpOpHandle != 0)
            {
                if (_pDPFPStopAcquisition != null)
                    _pDPFPStopAcquisition(fpOpHandle);
                if (_pDPFPDestroyAcquisition != null)
                    _pDPFPDestroyAcquisition(fpOpHandle);
                fpOpHandle = 0;
            }
            if (fd_image != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(fd_image);
                fd_image = IntPtr.Zero;
                fd_imageSize = 0;
            }
        }

        public static uint widthBytes(uint bits) { return ((bits + 31) / 32 * 4); }
//-----------------------------------------------------------------------------------
        private bool getFunctions()
        {
            IntPtr pDPFPInit = NativeMethods.GetProcAddress(m_hModule, "DPFPInit");
            if (pDPFPInit != IntPtr.Zero)
                _pDPFPInit = (DPFPInit)Marshal.GetDelegateForFunctionPointer(pDPFPInit, typeof(DPFPInit));
            IntPtr pDPFPTerm = NativeMethods.GetProcAddress(m_hModule, "DPFPTerm");
            if (pDPFPTerm != IntPtr.Zero)
                _pDPFPTerm = (DPFPTerm)Marshal.GetDelegateForFunctionPointer(pDPFPTerm, typeof(DPFPTerm));
            IntPtr pDPFPCreateAcquisition = NativeMethods.GetProcAddress(m_hModule, "DPFPCreateAcquisition");
            if (pDPFPCreateAcquisition != IntPtr.Zero)
                _pDPFPCreateAcquisition = (DPFPCreateAcquisition)Marshal.GetDelegateForFunctionPointer(pDPFPCreateAcquisition, typeof(DPFPCreateAcquisition));
            IntPtr pDPFPDestroyAcquisition = NativeMethods.GetProcAddress(m_hModule, "DPFPDestroyAcquisition");
            if (pDPFPDestroyAcquisition != IntPtr.Zero)
                _pDPFPDestroyAcquisition = (DPFPDestroyAcquisition)Marshal.GetDelegateForFunctionPointer(pDPFPDestroyAcquisition, typeof(DPFPDestroyAcquisition));
            IntPtr pDPFPStartAcquisition = NativeMethods.GetProcAddress(m_hModule, "DPFPStartAcquisition");
            if (pDPFPStartAcquisition != IntPtr.Zero)
                _pDPFPStartAcquisition = (DPFPStartAcquisition)Marshal.GetDelegateForFunctionPointer(pDPFPStartAcquisition, typeof(DPFPStartAcquisition));
            IntPtr pDPFPStopAcquisition = NativeMethods.GetProcAddress(m_hModule, "DPFPStartAcquisition");
            if (pDPFPStopAcquisition != IntPtr.Zero)
                _pDPFPStopAcquisition = (DPFPStopAcquisition)Marshal.GetDelegateForFunctionPointer(pDPFPStopAcquisition, typeof(DPFPStopAcquisition));
            return (_pDPFPInit != null) &&
                   (_pDPFPTerm != null) &&
                   (_pDPFPCreateAcquisition != null) &&
                   (_pDPFPDestroyAcquisition != null) &&
                   (_pDPFPStartAcquisition != null) &&
                   (_pDPFPStopAcquisition != null);
        }

        private bool load()
        {
            m_hModule = NativeMethods.LoadLibrary("DPFPApi.dll");
            if (m_hModule != IntPtr.Zero)
                return getFunctions();
            return false;
        }

        public void Dispose()
        {
            Reset();
            if ( _pDPFPTerm != null )
                _pDPFPTerm();
            if ( m_hModule != IntPtr.Zero )
                NativeMethods.FreeLibrary( m_hModule );
        }

        public virtual void OnSampleCollected(EventArgs e)
        {
            EventHandler handler = SampleCollected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
