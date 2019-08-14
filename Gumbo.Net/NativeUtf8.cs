﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Gumbo
{
    public class NativeUtf8
    {
        /// <summary>
        /// Determines the length of the specified string (not including the terminating null character).
        /// </summary>
        /// <param name="nullTerminatedString"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)] static extern int lstrlenA(IntPtr nullTerminatedString);

        /// <summary>
        /// Allocates pointer and puts null terminated UTF-8 string bytes.
        /// </summary>
        /// <param name="value">Original managed string</param>
        /// <returns></returns>
        public static IntPtr NativeUtf8FromString(string value)
        {
            var length = Encoding.UTF8.GetByteCount(value);
            var buffer = new byte[length + 1];
            Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);
            var nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
            return nativeUtf8;
        }

        /// <summary>
        /// Reads all bytes as a null terminated UTF-8 string.
        /// </summary>
        /// <param name="nativeUtf8"></param>
        /// <returns></returns>
        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            if (nativeUtf8 == IntPtr.Zero)
                return null;
            var length = lstrlenA(nativeUtf8);
            return StringFromNativeUtf8(nativeUtf8, length);
        }

        /// <summary>
        /// Reads all bytes as a null terminated UTF-8 string.
        /// </summary>
        /// <param name="nativeUtf8"></param>
        /// <returns></returns>
        public static string StringFromNativeUtf8(IntPtr nativeUtf8, int length)
        {
            if (nativeUtf8 == IntPtr.Zero)
                return null;
            var buffer = new byte[length];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
