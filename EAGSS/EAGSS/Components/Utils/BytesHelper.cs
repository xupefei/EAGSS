using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace EAGSS
{
    public class BytesHelper
    {
        public static int FindByte(byte[] bytesInput, byte pattern, int intStart)
        {
            for (int i = intStart; i < bytesInput.Length; i++)
            {
                if (bytesInput[i] == pattern)
                    return i;
            }
            return -1;
        }

        public static int FindBytes(byte[] bytesInput, byte[] bytesFind, int intStart, bool isBackward)
        {
            if (isBackward)
            {
                for (int index = intStart - bytesFind.Length; index >= 0; --index)
                {
                    if (IsBytesEqual(CopyBlock(bytesInput, index, bytesFind.Length), bytesFind))
                        return index;
                }
                return -1;
            }

            for (int index = intStart; index <= (bytesInput.Length - bytesFind.Length); ++index)
            {
                if (IsBytesEqual(CopyBlock(bytesInput, index, bytesFind.Length), bytesFind))
                    return index;
            }
            return -1;
        }

        public static bool IsBytesEqual(byte[] byte1, byte[] byte2)
        {
            if (byte1.Length != byte2.Length)
                return false;

            for (int i = 0; i < byte1.Length; i++)
            {
                if (byte1[i] != byte2[i])
                    return false;
            }
            return true;
        }

        public static byte[] ExpandBytes(byte[] org, int length)
        {
            if (org.Length > length)
                throw new Exception("src is lager than length");

            var b = new byte[length];
            FillByte(ref b, 0);

            SetBytes(ref b, org, 0);

            return b;
        }

        public static void ExpandBytes(ref byte[] org, int length)
        {
            if (org.Length > length)
                throw new Exception("src is lager than length");

            var b = new byte[length];
            FillByte(ref b, 0);

            SetBytes(ref b, org, 0);

            org = b;
        }

        public static void FillByte(ref byte[] bytesInput, byte byteValue)
        {
            ArrayList.Repeat(byteValue, bytesInput.Length).CopyTo(bytesInput);
        }

        public static void AppendBytes(ref byte[] bytesInput, byte[] bytesValue)
        {
            if (bytesInput == null || bytesInput.Length == 0)
                bytesInput = bytesValue;

            var newBytes = new byte[bytesInput.Length + bytesValue.Length];

            SetBytes(ref newBytes, bytesInput, 0);
            SetBytes(ref newBytes, bytesValue, bytesInput.Length);

            bytesInput = newBytes;
        }

        public static void SetBytes(ref byte[] bytesInput, byte[] bytesValue, int startPos)
        {
            if (bytesInput.Length < bytesValue.Length + startPos)
                throw new Exception("dest too small");

            Array.Copy(bytesValue, 0, bytesInput, startPos, bytesValue.Length);
        }

        public static byte[] CopyBlock(byte[] bytesOrg, int intStart, int intLength)
        {
            if (intStart + intLength > bytesOrg.Length)
                throw new Exception("src too small");

            var buffer = new byte[intLength];
            Array.Copy(bytesOrg, intStart, buffer, 0, intLength);
            return buffer;
        }

        public static T CopyStruct<T>(object from)
        {
            byte[] bytes = StructToBytes(from);
            return BytesToStruct<T>(bytes);
        }

        public static T BytesToStruct<T>(byte[] bytes)
        {
            int size = Marshal.SizeOf(typeof (T));
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return (T) Marshal.PtrToStructure(buffer, typeof (T));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, true);
                var bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static byte[] BitStringToBytes(string bitStr)
        {
            if (bitStr.IndexOf('-') == -1 && bitStr.Length == 2)
            {
                var byteTemp2 = new byte[1];
                byteTemp2[0] = byte.Parse(bitStr, NumberStyles.AllowHexSpecifier);
                return byteTemp2;
            }

            string[] arrSplit = bitStr.Split('-');
            var byteTemp = new byte[arrSplit.Length];
            for (int i = 0; i < byteTemp.Length; i++)
            {
                byteTemp[i] = byte.Parse(arrSplit[i], NumberStyles.AllowHexSpecifier);
            }
            return byteTemp;
        }
    }
}