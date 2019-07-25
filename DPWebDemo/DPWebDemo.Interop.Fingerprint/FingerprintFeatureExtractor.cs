using System;
using System.Runtime.InteropServices;

namespace DPWebDemo.Interop.Fingerprint
{
    /// <summary>
    /// .Net wrapper for unmanaged feature extraction. 
    /// </summary>
    public sealed class FingerprintFeatureExtractor : IDisposable
    {
        #region Interop

        /// <summary>
        /// Fingerprint image quality.
        /// Only for interaction with unmanaged code.
        /// </summary>
        private enum FT_IMG_QUALITY
        {
            /// <summary>
            /// The fingerprint image quality is good.
            /// </summary>
            FT_GOOD_IMG = 0,

            /// <summary>
            /// The fingerprint image is too light.
            /// </summary>
            FT_IMG_TOO_LIGHT = 1,

            /// <summary>
            /// The fingerprint image is too dark.
            /// </summary>
            FT_IMG_TOO_DARK = 2,

            /// <summary>
            /// The fingerprint image is too blurred.
            /// </summary>
            FT_IMG_TOO_NOISY = 3,

            /// <summary>
            /// The fingerprint image contrast is too low.
            /// </summary>
            FT_LOW_CONTRAST = 4,

            /// <summary>
            /// The fingerprint image quality is  undetermined.
            /// </summary>
            FT_UNKNOWN_IMG_QUALITY = 5
        }

        /// <summary>
        /// Fingerprint feature qulity.
        /// Only for interaction with unmanaged code.
        /// </summary>
        private enum FT_FTR_QUALITY
        {
            /// <summary>
            /// The fingerprint features quality is good.
            /// </summary>
            FT_GOOD_FTR = 0,

            /// <summary>
            /// There are not enough fingerprint features.
            /// </summary>
            FT_NOT_ENOUGH_FTR = 1,

            /// <summary>
            /// The fingerprint image does not contain the central portion of the finger.
            /// </summary>
            FT_NO_CENTRAL_REGION = 2,

            /// <summary>
            /// The fingerprint features quality is undetermined.
            /// </summary>
            FT_UNKNOWN_FTR_QUALITY = 3,

            /// <summary>
            /// The fingerprint image area is too small.
            /// </summary>
            FT_AREA_TOO_SMALL = 4
        }

        #endregion

        private IntPtr fxContextHandle = IntPtr.Zero;

        private bool isInitialized;

        private bool isDisposed;

        /// <summary>
        /// Initialize fingerprint feature extraction core.
        /// </summary>
        public void Initialize()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (isInitialized)
                return;

            InitializeCore();

            CreateContextCore(ref fxContextHandle);
            isInitialized = true;
        }

        /// <summary>
        /// Extract fingerprint featuries.
        /// </summary>
        public byte[] Extract(FingerprintImage fingerprintImage, ExtractionType extractionType)
        {
            if (!isInitialized)
                throw new InvalidOperationException("Initialize FingerprintFeatureExtractor first.");

            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (fingerprintImage == null)
                throw new ArgumentNullException("fingerprintImage");

            var featureSetSizeRecommended = 0;
            var featureSetSizeMinimum = 0;

            CalculateFeatureSizeCore(extractionType, ref featureSetSizeRecommended, ref featureSetSizeMinimum);

            if (featureSetSizeRecommended <= 0)
                throw new OutOfMemoryException();

            var fdFeatures = Marshal.AllocHGlobal(featureSetSizeRecommended);

            var pImageQuality = (int)FT_IMG_QUALITY.FT_UNKNOWN_IMG_QUALITY;
            var pFeaturesQualit = (int)FT_FTR_QUALITY.FT_UNKNOWN_FTR_QUALITY;
            var featureSetCreated = 0;

            ExtractFeaturesCore(fxContextHandle, fingerprintImage.OriginalFingerprintImageSize, fingerprintImage.OriginalFingerprintImage, (int)extractionType,
                featureSetSizeRecommended, fdFeatures, ref pImageQuality, ref pFeaturesQualit, ref featureSetCreated);

            if (featureSetCreated == 0 ||
                pImageQuality != (int)FT_IMG_QUALITY.FT_GOOD_IMG ||
                pFeaturesQualit != (int)FT_FTR_QUALITY.FT_GOOD_FTR)
                throw new InvalidOperationException("Cannot extract features.");

            var featuriesData = new byte[featureSetSizeRecommended];

            Marshal.Copy(fdFeatures, featuriesData, 0, featureSetSizeRecommended);

            Marshal.FreeHGlobal(fdFeatures);

            return featuriesData;

        }

        #region IDisposable

        ~FingerprintFeatureExtractor()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;

            if (!isInitialized)
                return;

            if (fxContextHandle != IntPtr.Zero)
            {
                CloseContextCore(fxContextHandle);
                fxContextHandle = IntPtr.Zero;
            }

            TerminateCore();
        }

        /// <summary>
        /// Release unmanaged memory resources and realise fingerprint feature core.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Initializes the fingerprint feature extraction module. This function must be called before any other functions in the module are called. 
        /// </summary>
        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_init", PreserveSig = true)]
        private static extern void InitializeCore();

        /// <summary>
        /// Terminates the fingerprint feature extraction module and releases the resources associated with it.
        /// </summary>
        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_terminate", PreserveSig = true)]
        private static extern void TerminateCore();

        /// <summary>
        ///  Creates a context for the fingerprint feature extraction module. If this function succeeds, it returns the 
        /// handle to the context that is created. All of the operations in this context require this handle. 
        /// </summary>
        /// <param name="fxContext">Handle to the context.</param>
        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_createContext", PreserveSig = true, CallingConvention = CallingConvention.StdCall)]
        private static extern void CreateContextCore(ref IntPtr fxContext);

        /// <summary>
        /// Destroys a feature extraction context and releases the resources associated with it.
        /// </summary>
        /// <param name="fxContext">Handle to the context.</param>
        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_closeContext", PreserveSig = true)]
        private static extern void CloseContextCore(IntPtr fxContext);

        /// <summary>
        /// Retrieves the size of the buffer for the fingerprint feature set: the minimum or recommended size that provides the best recognition accuracy, or both.
        /// </summary>
        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_getFeaturesLen", PreserveSig = true)]
        private static extern void CalculateFeatureSizeCore(ExtractionType featureSetPurpose, ref Int32 featureSetSizeRecommended, ref Int32 featureSetSizeMinimum);

        [DllImport("dpHFtrEx.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FX_extractFeatures", PreserveSig = true)]
        private static extern void ExtractFeaturesCore(IntPtr fxContext, int imageSize, IntPtr pImage, int featureSetPurpose, int featureSetSize, IntPtr featureSet,
            ref int pImageQuality, ref int pFeaturesQualit, ref int featureSetCreated);


        #endregion

    }
}
