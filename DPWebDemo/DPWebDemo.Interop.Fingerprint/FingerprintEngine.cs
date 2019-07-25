using System;
using System.Runtime.InteropServices;

namespace DPWebDemo.Interop.Fingerprint
{
    /// <summary>
    /// .Net wrapper for unmanaged figerprint engine.
    /// </summary>
    public sealed class FingerprintEngine : IDisposable //Blocking
    {
        #region Interop data

        /// <summary>
        /// Binary lage object. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct DATA_BLOB
        {
            /// <summary>
            /// Blob length.
            /// </summary>
            public uint cbData;
            /// <summary>
            /// Pointer to blob.
            /// </summary>
            public IntPtr pbData;
        }

        /// <summary>
        /// Image format version structure. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct FD_IMAGE_FORMAT_VERSION
        {
            public uint uMajor;
            public uint uMinor;
            public uint uBuild;
        }

        /// <summary>
        /// Fingerprint data information. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct FD_DATA_HEADER
        {
            public uint uDataType;
            public Int64 DeviceId;
            public Int64 DeviceType;
            public int DataAcquisitionProgress;
        }

        /// <summary>
        /// Fingerprint image type. Black and white, gray or color images.
        /// Only for interaction with unmanaged code.
        /// </summary>
        private enum FD_IMAGE_TYPE : uint
        {
            FD_IMAGE_UNKNOWN_TYPE = 0,
            FD_IMAGE_BLACK_WHITE = 1,
            FD_IMAGE_GRAY_SCALE = 2,
            FD_IMAGE_COLOR = 3
        }

        /// <summary>
        /// Image padding. Right or left padding. 
        /// Only for interaction with unmanaged code.
        /// </summary>
        private enum FD_IMAGE_PADDING : uint
        {
            FD_IMAGE_NO_PADDING = 0,
            FD_IMAGE_LEFT_PADDING = 1,
            FD_IMAGE_RIGHT_PADDING = 2
        };

        /// <summary>
        /// Positive=black print on white background. Only for interaction with unmanaged code.
        /// </summary>
        private enum FD_IMAGE_POLARITY : uint
        {
            FD_IMAGE_UNKNOWN_POLARITY = 0,
            FD_IMAGE_NEGATIVE_POLARITY = 1,
            FD_IMAGE_POSITIVE_POLARITY = 2
        };

        /// <summary>
        /// Only for interaction with unmanaged code.
        /// </summary>
        private enum FD_IMAGE_COLOR_REPRESENTATION : uint
        {
            FD_IMAGE_NO_COLOR_REPRESENTATION = 0,
            FD_IMAGE_PLANAR_COLOR_REPRESENTATION = 1,
            FD_IMAGE_INTERLEAVED_COLOR_REPRESENTATION = 2
        };

        /// <summary>
        /// Image format information. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct FD_IMAGE_FORMAT
        {
            /// <summary>
            /// is 1 which represents 2D image
            /// </summary>
            public uint uDataType;

            /// <summary>
            /// Black and white, gray or color images
            /// </summary>
            public FD_IMAGE_TYPE uImageType;

            /// <summary>
            /// Image width in pixels.
            /// </summary>
            public int iWidth;

            /// <summary>
            /// Image height in pixels.
            /// </summary>
            public int iHeight;

            /// <summary>
            /// X resolution in DPI.
            /// </summary>
            public int iXdpi;

            /// <summary>
            /// Y resolution in DPI.
            /// </summary>
            public int iYdpi;

            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            public uint uBPP;

            /// <summary>
            /// Right or left padding
            /// </summary>
            public FD_IMAGE_PADDING uPadding;

            /// <summary>
            /// Number of significant bits per pixel
            /// </summary>
            public uint uSignificantBpp;

            /// <summary>
            /// Positive=black print on white background
            /// </summary>
            public FD_IMAGE_POLARITY uPolarity;

            /// <summary>
            /// 
            /// </summary>
            public FD_IMAGE_COLOR_REPRESENTATION uRGBcolorRepresentation;

            /// <summary>
            /// Color planes.
            /// </summary>
            public uint uPlanes;
        }

        /// <summary>
        /// Complete image information. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct FD_IMAGE_HEADER
        {
            /// <summary>
            /// Version of the image format.
            /// </summary>
            public FD_IMAGE_FORMAT_VERSION version;

            /// <summary>
            /// Image format of the current data.
            /// </summary>
            public FD_IMAGE_FORMAT ImageFormat;
        }

        /// <summary>
        /// Finger scan information. Only for interaction with unmanaged code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct FD_IMAGE
        {
            /// <summary>
            /// Size of the blob
            /// </summary>
            public uint uSize;

            /// <summary>
            /// Data information
            /// </summary>
            public FD_DATA_HEADER DataHeader;

            /// <summary>
            /// Offset to the beginning of image
            /// </summary>
            public uint uOffsetToImage;

            /// <summary>
            /// Current image format information
            /// </summary>
            public FD_IMAGE_HEADER Header;

            /// <summary>
            /// Additional header info starts right after
            /// </summary>
            public uint uExtensionSize;
        }

        /// <summary>
        /// Acquisition priority. Only for interaction with unmanaged code.
        /// </summary>
        private enum DP_ACQUISITION_PRIORITY
        {
            /// <summary>
            /// High priority.
            /// </summary>
            DP_PRIORITY_HIGH = 1,

            /// <summary>
            /// Normal priority.
            /// </summary>
            DP_PRIORITY_NORMAL = 2,

            /// <summary>
            /// Low priority.
            /// </summary>
            DP_PRIORITY_LOW = 3
        }

        /// <summary>
        /// Activision type.
        /// </summary>
        private enum DP_SAMPLE_TYPE
        {
            DP_SAMPLE_TYPE_REG = 1,
            DP_SAMPLE_TYPE_VER = 2,

            /// <summary>
            /// Feature extraction
            /// </summary>
            DP_SAMPLE_TYPE_EXTRACT = 4
        }

        /// <summary>
        /// Win32 messages, sent from fingerprint core.
        /// </summary>
        private enum DP_FP_MESSAGES
        {
            /// <summary>
            /// A supplied fingerprint credential was acquired. lParam contains the handle to a parameter of type HDPCREDENTIAL.
            /// </summary>
            WN_COMPLETED = 0,

            /// <summary>
            /// An error occurred. lParam contains the error code that is returned.
            /// </summary>
            WN_ERROR = 1,

            /// <summary>
            /// The fingerprint reader was disconnected. lParam contains a pointer to the device UID. See the structure of type DP_DEVICE_INFO.
            /// </summary>
            WN_DISCONNECT = 2,

            /// <summary>
            /// The fingerprint reader was reconnected. lParam contains a pointer to the device UID. See the structure of type DP_DEVICE_INFO.
            /// </summary>
            WN_RECONNECT = 3,

            /// <summary>
            /// Provides information about the quality of the fingerprint image. lParam contains the fingerprint image quality listed in the enum of type DP_SAMPLE_QUALITY.
            /// </summary>
            WN_SAMPLE_QUALITY = 4,

            /// <summary>
            /// The fingerprint reader was touched.
            /// </summary>
            WN_FINGER_TOUCHED = 5,

            /// <summary>
            /// The finger was removed from the fingerprint reader.
            /// </summary>
            WN_FINGER_GONE = 6,

            /// <summary>
            /// A fingerprint image is ready for processing.
            /// </summary>
            WN_IMAGE_READY = 7,

            /// <summary>
            /// A supplied fingerprint credential was added to the stored fingerprint credential.
            /// </summary>
            WN_FEATURE_SET_ADDED = 8,

            /// <summary>
            /// A fingerprint enrollment template was created.
            /// </summary>
            WN_REGISTRATION_COMPLETED = 9,

            /// <summary>
            /// The operation was stopped by calling either the DPFPStopAcquisition function or the DPFPStopRegistration function.
            /// </summary>
            WN_OPERATION_STOPPED = 10
        }

        #endregion

        /// <summary>
        /// Fingerprint sample (image) collected.
        /// </summary>
        public event EventHandler<ImageCapturedEventArgs> SampleCollected;

        private const uint wmUser = 0x0400;

        /// <summary>
        ///  Message id received from DPFPApi for fingerprint user actions
        /// </summary>
        private const uint eventMsg = wmUser + 205;

        /// <summary>
        /// Window handler, that can receive win32 messages.
        /// </summary>
        private IntPtr window;

        private uint operationHandle;

        /// <summary>
        /// Engine is initialized.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initialize fingerprint reader engine.
        /// </summary>
        /// <param name="window">Window handler, that can receive win32 messages.</param>
        public void Initialize(IntPtr window)
        {
            if (window == IntPtr.Zero)
                throw new ArgumentNullException("window");

            this.window = window;

            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (isInitialized)
                return;

            InitializeCore();

            isInitialized = true;
        }

        /// <summary>
        /// Handle Win32 message.
        /// </summary>
        /// <param name="hwnd">Window handle of the message.</param>
        /// <param name="msg">ID number for the message.</param>
        /// <param name="wParam">WParam pointer.</param>
        /// <param name="lParam">LParam pointer.</param>
        /// <param name="handled">Value that is returned to Windows in response to handling the message.</param>
        public void ProcessSample(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (operationHandle == 0 || !isInitialized)
                return;

            if (msg != eventMsg)
                return;

            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            switch ((DP_FP_MESSAGES)wParam.ToInt32())
            {
                case DP_FP_MESSAGES.WN_COMPLETED:
                    {
                        if (lParam == IntPtr.Zero)
                            throw new ArgumentNullException("lParam");
                        var imageBlob = (DATA_BLOB)Marshal.PtrToStructure(lParam, typeof(DATA_BLOB));

                        if (imageBlob.cbData > 0)
                            ImageCaptured(imageBlob);

                        handled = true;
                        break;
                    }
                case DP_FP_MESSAGES.WN_ERROR:
                    {
                        Stop();
                        handled = true;
                        throw new InvalidOperationException(String.Format("Cannot get FP sample, error 0x{0,1:X}.",
                            lParam.ToInt32()));
                    }
                case DP_FP_MESSAGES.WN_DISCONNECT:
                case DP_FP_MESSAGES.WN_RECONNECT:
                case DP_FP_MESSAGES.WN_FINGER_TOUCHED:
                case DP_FP_MESSAGES.WN_FINGER_GONE:
                case DP_FP_MESSAGES.WN_SAMPLE_QUALITY:
                case DP_FP_MESSAGES.WN_IMAGE_READY: // A fingerprint image is ready for processing.
                case DP_FP_MESSAGES.WN_OPERATION_STOPPED:
                    // The operation was stopped by calling the DPFPStopAcquisition function
                    handled = true;
                    break;
            }

        }

        /// <summary>
        /// Start collecting fingerprint
        /// </summary>
        public void Start()
        {

            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!isInitialized)
                throw new InvalidOperationException("Initialize fingerprint reader first.");

            Stop();
            var deviceId = Guid.Empty;
            CreateAcquisitionCore(DP_ACQUISITION_PRIORITY.DP_PRIORITY_NORMAL, ref deviceId, DP_SAMPLE_TYPE.DP_SAMPLE_TYPE_EXTRACT, window, eventMsg, ref operationHandle);

            if (operationHandle != 0)
                StartAcquisitionCore(operationHandle);

        }

        /// <summary>
        /// Clean up, delete collecting fingerprint
        /// </summary>
        public void Stop()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!isInitialized)
                throw new InvalidOperationException("Initialize object first.");

            if (operationHandle != 0)
            {
                StopAcquisitionCore(operationHandle);
                DestroyAcquisitionCore(operationHandle);
                operationHandle = 0;
            }
        }

        private void ImageCaptured(DATA_BLOB data)
        {
            var imageInformationPtr = Marshal.AllocHGlobal((int)data.cbData);

            CopyMemory(imageInformationPtr, data.pbData, data.cbData);

            var imageInformation = (FD_IMAGE)Marshal.PtrToStructure(imageInformationPtr, typeof(FD_IMAGE));

            var imagePtr = imageInformationPtr + (int)imageInformation.uOffsetToImage;

            var imageWidth = imageInformation.Header.ImageFormat.iWidth;
            var imageHeigth = imageInformation.Header.ImageFormat.iHeight;
            var imageBpp = (int)imageInformation.Header.ImageFormat.uBPP;

            var imageData = new byte[(imageWidth * imageHeigth * imageBpp + 7) / 8];

            Marshal.Copy(imagePtr, imageData, 0, imageData.Length);

            var fingerprintImage = new FingerprintImage(imageData, imageWidth, imageHeigth, imageBpp,
                imageInformation.Header.ImageFormat.iXdpi, imageInformation.Header.ImageFormat.iYdpi, imageInformationPtr, (int)imageInformation.uSize);

            OnSampleCollected(new ImageCapturedEventArgs(fingerprintImage));

        }

        /// <summary>
        /// Rise SampleCollected event.
        /// </summary>
        /// <param name="e">Event args.</param>
        private void OnSampleCollected(ImageCapturedEventArgs e)
        {
            var handler = SampleCollected;
            if (handler != null)
                handler(this, e);
        }

        #region IDisposable

        /// <summary>
        /// Release unmanaged memory resources and realise fingerprint core.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;
            if (operationHandle != 0)
            {
                StopAcquisitionCore(operationHandle);
                DestroyAcquisitionCore(operationHandle);
            }

            TerminateCore();
        }

        ~FingerprintEngine()
        {
            Dispose(false);
        }

        #endregion

        #region External Functions

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPInit", PreserveSig = true)]
        private static extern void InitializeCore();

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPTerm")]
        private static extern void TerminateCore();

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPCreateAcquisition", PreserveSig = true)]
        private static extern void CreateAcquisitionCore(DP_ACQUISITION_PRIORITY acquisitionPriority, ref Guid uid, DP_SAMPLE_TYPE sampleType, IntPtr hWnd, uint msg, ref uint operation);

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPDestroyAcquisition", PreserveSig = true)]
        private static extern void DestroyAcquisitionCore(uint operation);

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPStartAcquisition", PreserveSig = true)]
        private static extern void StartAcquisitionCore(uint operation);

        [DllImport("DPFPApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPFPStopAcquisition", PreserveSig = true)]
        private static extern void StopAcquisitionCore(uint operation);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

        #endregion
    }
}
