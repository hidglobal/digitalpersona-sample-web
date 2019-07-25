using System;
using System.Runtime.InteropServices;

namespace DPWebDemo.Interop.Fingerprint
{
    public sealed class FingerprintImage : IDisposable
    {
        public byte[] ImageData { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int BitPerPixel { get; private set; }

        public int DpiX { get; private set; }

        public int DpiY { get; private set; }

        public IntPtr OriginalFingerprintImage { get; private set; }

        public int OriginalFingerprintImageSize { get; private set; }

        private bool isDisposed;

        internal FingerprintImage(byte[] imageData, int width, int height, int bitPerPixel, int dpiX, int dpiY,
            IntPtr originalFingerprintImage, int originalFingerprintImageSize)
        {
            ImageData = imageData;
            Width = width;
            Height = height;
            BitPerPixel = bitPerPixel;
            DpiX = dpiX;
            DpiY = dpiY;
            OriginalFingerprintImage = originalFingerprintImage;
            OriginalFingerprintImageSize = originalFingerprintImageSize;
        }

        #region IDisposable

        /// <summary>
        /// Release unmanaged memory resources.
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
            Marshal.FreeHGlobal(OriginalFingerprintImage);
        }

        ~FingerprintImage()
        {
            Dispose(false);
        }

        #endregion
    }
}