/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: NativeMethods.cs
        
Abstract: system functions

--*/
using System;
using System.Runtime.InteropServices;

namespace DigitalPersona.Application.Native
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

        public const UInt32 WM_USER = 0x0400;
    }
}
