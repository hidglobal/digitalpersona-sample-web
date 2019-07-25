/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: DPFPApiSupport.cs     

Fingerprint support - feature extraction. Loads the DP's Fingerprint Engine dpHFtrEx.dll.
--*/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using DigitalPersona.Application.Native;

namespace DigitalPersona.Application.FingerprintSupport
{
    // dpHFtrEx enums
    public class DPFeatureExtractor : IDisposable
    {
        private enum FT_FTR_TYPE
        {
            FT_PRE_REG_FTR = 0, // fingerprint feature set to be used for enrollment
            FT_REG_FTR = 1,     // fingerprint template
            FT_VER_FTR = 2,     // fingerprint feature set to be used for verification
            FD_IMAGE_COLOR = 3
        }
        private enum FT_IMG_QUALITY
        {
            FT_GOOD_IMG =       0, // The fingerprint image quality is good
            FT_IMG_TOO_LIGHT =  1, // The fingerprint image is too light
            FT_IMG_TOO_DARK =   2, // The fingerprint image is too dark
            FT_IMG_TOO_NOISY =  3, // The fingerprint image is too blurred
            FT_LOW_CONTRAST =   4, // The fingerprint image contrast is too low
            FT_UNKNOWN_IMG_QUALITY = 5  // The fingerprint image quality is  undetermined
        }
        private enum FT_FTR_QUALITY
        {
            FT_GOOD_FTR             = 0, // The fingerprint features quality is good
            FT_NOT_ENOUGH_FTR       = 1, // There are not enough fingerprint features
            FT_NO_CENTRAL_REGION    = 2, // The fingerprint image does not contain the central portion of the finger
            FT_UNKNOWN_FTR_QUALITY  = 3, // The fingerprint features quality is undetermined
            FT_AREA_TOO_SMALL       = 4 // The fingerprint image area is too small
        }
        // dpHFtrEx Feature extraction functions
        // Initializes the fingerprint feature extraction module. This function must be called before any other functions in the module are called. 
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_init();
        // Terminates the fingerprint feature extraction module and releases the resources associated with it. 
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_terminate();
        // Creates a context for the fingerprint feature extraction module. If this function succeeds, it returns the 
        // handle to the context that is created. All of the operations in this context require this handle. 
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_createContext(ref IntPtr fxContext);
        // Destroys a feature extraction context and releases the resources associated with it. 
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_closeContext(IntPtr fxContext);
        // Retrieves the size of the buffer for the fingerprint feature set: the minimum or recommended size that provides the best recognition accuracy, or both.
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_getFeaturesLen(FT_FTR_TYPE FeatureSetPurpose, ref Int32 FeatureSetSizeRecommended, ref Int32 FeatureSetSizeMinimum);
        // Retrieves the size of the buffer for the fingerprint feature set: the minimum or recommended size that provides the best recognition accuracy, or both.
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int FX_extractFeatures(IntPtr fxContext, int ImageSize, IntPtr pImage, 
                                                int FeatureSetPurpose, int FeatureSetSize, IntPtr FeatureSet,
                                                ref int pImageQuality, ref int pFeaturesQualit, ref int FeatureSetCreated);

        private FX_init _pFX_init = null;
        private FX_terminate _pFX_terminate = null;
        private FX_createContext _pFX_createContext = null;
        private FX_closeContext _pFX_closeContext = null;
        private FX_getFeaturesLen _pFX_getFeaturesLen = null;
        private FX_extractFeatures _pFX_extractFeatures = null;

        private IntPtr m_hModule = IntPtr.Zero;
        private IntPtr fd_Features = IntPtr.Zero;
        private IntPtr fxContextHandle = IntPtr.Zero;
        private int hLasterror = 0;
//-----------------------------------------------------------------------------------
// public stuff
        public DPFeatureExtractor()
        {
            if (!load())
            {
                hLasterror = unchecked((int)0x80029C4A); // Error loading type library/DLL.
                throw new ApplicationException("Cannot load dpHFtrEx!");
            }
            hLasterror = _pFX_init();
            if (hLasterror < 0)
                throw new ApplicationException(String.Format("Cannot initialize dpHFtrEx, error 0x{0,1:X}.", hLasterror));
            else
            {
                hLasterror = _pFX_createContext(ref fxContextHandle);
                if (hLasterror < 0)
                    throw new ApplicationException(String.Format("Cannot create dpHFtrEx context, error 0x{0,1:X}.", hLasterror));
            }
        }

        ~DPFeatureExtractor()
        {
            Dispose();
        }

        public int Extract(Int32 ImageSize, IntPtr pRawImage, ref IntPtr Features, ref int FeaturesSize)
        {   // input - raw image, output - feature collection
            if (hLasterror >= 0)
            {
                try
                {
                    if (fxContextHandle != IntPtr.Zero)
                    {
                        Int32 FeatureSetSizeRecommended = 0;
                        Int32 FeatureSetSizeMinimum = 0;
                        hLasterror = _pFX_getFeaturesLen(FT_FTR_TYPE.FT_VER_FTR, ref FeatureSetSizeRecommended, ref FeatureSetSizeMinimum);
                        if (hLasterror >= 0)
                        {
                            if (FeatureSetSizeRecommended > 0)
                            {
                                FeaturesSize = FeatureSetSizeRecommended;
                                if (fd_Features != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(fd_Features);
                                    fd_Features = IntPtr.Zero;
                                }
                                fd_Features = Marshal.AllocHGlobal(FeatureSetSizeRecommended);
                                if (fd_Features == null)
                                    hLasterror = unchecked((int)0x8007000E); // E_OUTOFMEMORY
                                else
                                {
                                    int pImageQuality = (int)FT_IMG_QUALITY.FT_UNKNOWN_IMG_QUALITY;
                                    int pFeaturesQualit = (int)FT_FTR_QUALITY.FT_UNKNOWN_FTR_QUALITY;
                                    int FeatureSetCreated = 0;
                                    hLasterror = _pFX_extractFeatures(fxContextHandle, ImageSize, pRawImage, (int)FT_FTR_TYPE.FT_VER_FTR, FeatureSetSizeRecommended, fd_Features, ref pImageQuality, ref pFeaturesQualit, ref FeatureSetCreated );
                                    if ((hLasterror < 0) && ((FeatureSetCreated == 0) || (pImageQuality != (int)FT_IMG_QUALITY.FT_GOOD_IMG) || (pFeaturesQualit != (int)FT_FTR_QUALITY.FT_GOOD_FTR)))
                                        hLasterror = -1;
                                }
                            }
                            else
                                hLasterror = unchecked((int)0x8007000E); // E_OUTOFMEMORY
                        }
                    }
                    else
                        hLasterror = -1;
                }
                catch
                {
                    hLasterror = -1;
                }
            }
            if (hLasterror < 0)
                throw new ApplicationException(String.Format("Cannot extract FP features, error 0x{0,1:X}.", hLasterror));
            else
                Features = fd_Features;
            return hLasterror;
        }

        public void Reset()
        {  // clean up, delete extracted features
            if (fxContextHandle != IntPtr.Zero)
            {
                if (_pFX_closeContext != null)
                    _pFX_closeContext(fxContextHandle);
                fxContextHandle = IntPtr.Zero;
            }
            if (fd_Features != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(fd_Features);
                fd_Features = IntPtr.Zero;
            }
            hLasterror = 0;
        }
//-----------------------------------------------------------------------------------
        private bool getFunctions()
        {
            IntPtr pFX_init = NativeMethods.GetProcAddress(m_hModule, "FX_init");
            if (pFX_init != IntPtr.Zero)
                _pFX_init = (FX_init)Marshal.GetDelegateForFunctionPointer(pFX_init, typeof(FX_init));
            IntPtr pFX_terminate = NativeMethods.GetProcAddress(m_hModule, "FX_terminate");
            if (pFX_terminate != IntPtr.Zero)
                _pFX_terminate = (FX_terminate)Marshal.GetDelegateForFunctionPointer(pFX_terminate, typeof(FX_terminate));
            IntPtr pFX_createContext = NativeMethods.GetProcAddress(m_hModule, "FX_createContext");
            if (pFX_createContext != IntPtr.Zero)
                _pFX_createContext = (FX_createContext)Marshal.GetDelegateForFunctionPointer(pFX_createContext, typeof(FX_createContext));
            IntPtr pFX_closeContext = NativeMethods.GetProcAddress(m_hModule, "FX_closeContext");
            if (pFX_closeContext != IntPtr.Zero)
                _pFX_closeContext = (FX_closeContext)Marshal.GetDelegateForFunctionPointer(pFX_closeContext, typeof(FX_closeContext));
            IntPtr pFX_getFeaturesLen = NativeMethods.GetProcAddress(m_hModule, "FX_getFeaturesLen");
            if (pFX_getFeaturesLen != IntPtr.Zero)
                _pFX_getFeaturesLen = (FX_getFeaturesLen)Marshal.GetDelegateForFunctionPointer(pFX_getFeaturesLen, typeof(FX_getFeaturesLen));
            IntPtr pFX_extractFeatures = NativeMethods.GetProcAddress(m_hModule, "FX_extractFeatures");
            if (pFX_extractFeatures != IntPtr.Zero)
                _pFX_extractFeatures = (FX_extractFeatures)Marshal.GetDelegateForFunctionPointer(pFX_extractFeatures, typeof(FX_extractFeatures));
            return (_pFX_init != null) &&
                   (_pFX_terminate != null) &&
                   (_pFX_createContext != null) &&
                   (_pFX_closeContext != null) &&
                   (_pFX_getFeaturesLen != null) &&
                   (_pFX_extractFeatures != null);
        }

        private bool load()
        {
            m_hModule = NativeMethods.LoadLibrary(Environment.SystemDirectory + "\\dpHFtrEx.dll");
            if (m_hModule != IntPtr.Zero)
                return getFunctions();
            return false;
        }

        public void Dispose()
        {
            Reset();
            if (_pFX_terminate != null)
                _pFX_terminate();
            if ( m_hModule != IntPtr.Zero )
                NativeMethods.FreeLibrary( m_hModule );
        }
    }
}
