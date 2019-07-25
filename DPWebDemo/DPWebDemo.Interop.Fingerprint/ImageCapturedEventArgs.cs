using System;

namespace DPWebDemo.Interop.Fingerprint
{
    public class ImageCapturedEventArgs : EventArgs
    {
        public FingerprintImage FingerprintImage { get; private set; }

        public ImageCapturedEventArgs(FingerprintImage fingerprintImage)
        {
            FingerprintImage = fingerprintImage;
        }
    }
}
