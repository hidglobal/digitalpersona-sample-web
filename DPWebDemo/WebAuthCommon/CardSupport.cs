/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: CardSupport.cs     

Card support. Loads the DP's Card Engine library DPSCApi.dll and provides support for Smart, Contactless and Proximity cards.
--*/
using System;
using System.Runtime.InteropServices;

using DigitalPersona.Application.Native;

namespace DigitalPersona.Application.CardSupport
{
    // DPSCApi structures
    public class DPSCApi : IDisposable
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

        public class DPCard
        {
            public string CardName;
            public uint nType;
            public int hr;
            public byte[] UID;
        }
        // DPSCApi functions
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPSCInit(IntPtr pSubscriber);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DPSCTerm();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPSCEnumReaders(ref int pnReadersCount, ref IntPtr pszReaders);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPSCGetCardInfo([MarshalAs(UnmanagedType.LPWStr)] string szReaderName, ref IntPtr pCardName, IntPtr CardInfo);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DPSCGetData([MarshalAs(UnmanagedType.LPWStr)] string szDataName, [MarshalAs(UnmanagedType.LPWStr)] string szReaderName, [MarshalAs(UnmanagedType.LPWStr)] string szUserPIN, uint dwOptions, ref byte ppOutData, ref uint pdwDataSize);

        private IntPtr m_hModule = IntPtr.Zero;
        private DPSCInit _pDPSCInit = null;
        private DPSCTerm _pDPSCTerm = null;
        private DPSCEnumReaders _pDPSCEnumReaders = null;
        private DPSCGetCardInfo _pDPSCGetCardInfo = null;
        private DPSCGetData _pDPSCGetData = null;

        private bool m_bInitialized = false;

        public DPSCApi()
        {
            if (!load())
                throw new ApplicationException("Cannot load DPSCApi!");
            int hr = _pDPSCInit(IntPtr.Zero);
            if ( hr < 0 )
                throw new ApplicationException(String.Format("Cannot initialize DPSCApi, error 0x{0,1:X}.", hr));
            m_bInitialized = true;
        }

        ~DPSCApi()
        {
            Dispose();
        }

        private bool getFunctions()
        {
            IntPtr pDPSCInit = NativeMethods.GetProcAddress(m_hModule, "DPSCInit");
            if (pDPSCInit != IntPtr.Zero)
                _pDPSCInit = (DPSCInit)Marshal.GetDelegateForFunctionPointer(pDPSCInit, typeof(DPSCInit));
            IntPtr pDPSCTerm = NativeMethods.GetProcAddress(m_hModule, "DPSCTerm");
            if (pDPSCTerm != IntPtr.Zero)
                _pDPSCTerm = (DPSCTerm)Marshal.GetDelegateForFunctionPointer(pDPSCTerm, typeof(DPSCTerm));
            IntPtr pDPSCEnumReaders = NativeMethods.GetProcAddress(m_hModule, "DPSCEnumReaders");
            if (pDPSCEnumReaders != IntPtr.Zero)
                _pDPSCEnumReaders = (DPSCEnumReaders)Marshal.GetDelegateForFunctionPointer(pDPSCEnumReaders, typeof(DPSCEnumReaders));
            IntPtr pDPSCGetCardInfo = NativeMethods.GetProcAddress(m_hModule, "DPSCGetCardInfo");
            if (pDPSCGetCardInfo != IntPtr.Zero)
                _pDPSCGetCardInfo = (DPSCGetCardInfo)Marshal.GetDelegateForFunctionPointer(pDPSCGetCardInfo, typeof(DPSCGetCardInfo));
            IntPtr pDPSCGetData = NativeMethods.GetProcAddress(m_hModule, "DPSCGetData");
            if (pDPSCGetData != IntPtr.Zero)
                _pDPSCGetData = (DPSCGetData)Marshal.GetDelegateForFunctionPointer(pDPSCGetData, typeof(DPSCGetData));
            return (_pDPSCInit != null) &&
                   (_pDPSCTerm != null) &&
                   (_pDPSCEnumReaders != null) &&
                   (_pDPSCGetData != null);
        }

        private bool load()
        {
            m_hModule = NativeMethods.LoadLibrary("DPSCApi.dll");
            if (m_hModule != IntPtr.Zero)
                return getFunctions();
            return false;
        }

        public void Dispose()
        {
            if ( _pDPSCTerm != null )
                _pDPSCTerm();
            if ( m_hModule != IntPtr.Zero )
                NativeMethods.FreeLibrary( m_hModule );
        }
//---------------------------------------------------------
        public string[] GetReaders()
        {
            int nReaders = 0;
            string[] sReaders = null;
            if ( m_bInitialized )
            {
                var pszReaders = new IntPtr();
                int hr = _pDPSCEnumReaders(ref nReaders, ref pszReaders);
                if ( (nReaders > 0) && (hr >= 0) )
                {
                    sReaders = new string[nReaders];
                    int nLength = 0;
                    for (int i = 0; i < nReaders; i++)
                    {
                        pszReaders += nLength;
                        string str = Marshal.PtrToStringAuto(pszReaders); 
                        if (str.Length == 0)
                            break;
                        nLength = (str.Length + 1) * 2;
                        sReaders[i] = str;
                    }
                }
            }
            return sReaders;
        }

        public DPCard GetCard(string ReaderName)
        {
            IntPtr pCardInfo = IntPtr.Zero;
            DPCard Card= new DPCard();
            try
            {
                DPCE_GETCARDINFO CardInfo = new DPCE_GETCARDINFO();
                CardInfo.ulVersion = 1;
                pCardInfo = Marshal.AllocHGlobal(Marshal.SizeOf(CardInfo));
                Marshal.StructureToPtr(CardInfo, pCardInfo, true);
                IntPtr pCardName = IntPtr.Zero;
                Card.hr = _pDPSCGetCardInfo(ReaderName, ref pCardName, pCardInfo);
                if ((Card.hr >= 0) && (pCardInfo != IntPtr.Zero))
                {
                    DPCE_GETCARDINFO CardInfo2 = (DPCE_GETCARDINFO)Marshal.PtrToStructure(pCardInfo, typeof(DPCE_GETCARDINFO));
                    Card.nType = CardInfo2.Type;
                    Card.UID = CardInfo2.UID;
                    Card.CardName = Marshal.PtrToStringUni(pCardName);
                }
            }
            finally
            {
                if (pCardInfo != IntPtr.Zero)
                    Marshal.FreeHGlobal(pCardInfo);
            }
            return Card;
        }
    }
}