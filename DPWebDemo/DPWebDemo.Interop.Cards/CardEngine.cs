using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Interop.Cards
{
    /// <summary>
    /// .Net wrapper for unmanaged card engine.
    /// </summary>
    public class CardEngine : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DPCE_GETCARDINFO
        {
            public uint ulVersion;
            public uint Type;
            public uint ulSubType;
            public uint ulAttributes;
            public uint ulOptions;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] UID;
        };

        /// <summary>
        /// Engine is initialized.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initialize card reader engine.
        /// </summary>
        public void Initialize()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (isInitialized)
                return;

            InitializeCore(IntPtr.Zero);

            isInitialized = true;
        }


        /// <summary>
        /// Get connected readers collection.
        /// </summary>
        /// <returns>Connected readers.</returns>
        public IEnumerable<string> GetReaders()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!isInitialized)
                throw new InvalidOperationException("Initialize card reader first.");

            int readersCount = 0;
            string[] readerNames = null;

            var readersPointer = new IntPtr();
            GetReadersCore(ref readersCount, ref readersPointer);
            if (readersCount > 0)
            {
                readerNames = new string[readersCount];
                int offset = 0;
                for (int i = 0; i < readersCount; i++)
                {
                    readersPointer += offset;
                    string str = Marshal.PtrToStringAuto(readersPointer);
                    if (str.Length == 0)
                        break;
                    offset = (str.Length + 1) * 2;
                    readerNames[i] = str;
                }
            }

            return readerNames;
        }

        /// <summary>
        /// Get card iinserted into specific reader.
        /// </summary>
        /// <param name="readerName">Reader name.</param>
        /// <returns>Inserter card.</returns>
        public Card GetCard(string readerName)
        {
            var pCardInfo = IntPtr.Zero;
            var card = new Card();
            try
            {
                var cardInfo = new DPCE_GETCARDINFO();
                cardInfo.ulVersion = 1;

                pCardInfo = Marshal.AllocHGlobal(Marshal.SizeOf(cardInfo));
                Marshal.StructureToPtr(cardInfo, pCardInfo, true);

                var pCardName = IntPtr.Zero;

                GetCardInfoCore(readerName, ref pCardName, pCardInfo);

                if (pCardInfo != IntPtr.Zero)
                {
                    var cardInfo2 = (DPCE_GETCARDINFO)Marshal.PtrToStructure(pCardInfo, typeof(DPCE_GETCARDINFO));
                    card.CardType = (CardType)cardInfo2.Type;
                    card.Id = cardInfo2.UID;
                    card.Name = Marshal.PtrToStringUni(pCardName);
                }
                else
                    return null;
            }
            finally
            {
                if (pCardInfo != IntPtr.Zero)
                    Marshal.FreeHGlobal(pCardInfo);
            }
            return card;
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
            TerminateCore();
        }

        ~CardEngine()
        {
            Dispose(false);
        }

        #endregion

        #region External Functions

        [DllImport("DPSCApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPSCInit", PreserveSig = true)]
        private static extern void InitializeCore(IntPtr subscriber);

        [DllImport("DPSCApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPSCTerm", PreserveSig = true)]
        private static extern void TerminateCore();

        [DllImport("DPSCApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPSCEnumReaders", PreserveSig = true)]
        private static extern void GetReadersCore(ref int readersCount, ref IntPtr pszReaders);

        [DllImport("DPSCApi.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "DPSCGetCardInfo", PreserveSig = true)]
        private static extern void GetCardInfoCore([MarshalAs(UnmanagedType.LPWStr)] string szReaderName, ref IntPtr pCardName, IntPtr cardInfo);

        #endregion
    }
}
