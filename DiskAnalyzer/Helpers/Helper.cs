using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiskAnalyzer
{
    static class Helper
    {
        public static string FormatSize(long bytes)
        {
            string[] suffixes =
                    { "B", "KB", "MB", "GB", "TB", "PB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }

        public static int FormatSizeInt(long bytes)
        {
            int counter = 0;
            int number = (int)bytes;
            while (number / 1024 >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return number;
        }

        public static int takeNDigits(long number, int N)
        {
            number = Math.Abs(number);
            if (number == 0)
                return (int)number;
            int numberOfDigits = (int)Math.Floor(Math.Log10(number) + 1);
            if (numberOfDigits >= N)
                return (int)Math.Truncate((number / Math.Pow(10, numberOfDigits - N)));
            else
                return (int)number;
        }


        public static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            public static class FILE_ATTRIBUTE
            {
                public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
            }

            public static class SHGFI
            {
                public const uint SHGFI_TYPENAME = 0x000000400;
                public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            }

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }


        public static string GetFilePurposeDescription(string filename)
        {
            Helper.NativeMethods.SHFILEINFO info = new Helper.NativeMethods.SHFILEINFO();

            string fileName = filename;
            uint dwFileAttributes = Helper.NativeMethods.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = (uint)(Helper.NativeMethods.SHGFI.SHGFI_TYPENAME | Helper.NativeMethods.SHGFI.SHGFI_USEFILEATTRIBUTES);

            Helper.NativeMethods.SHGetFileInfo(fileName, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
            return info.szTypeName;
        }

        public static NativeMethods.SHFILEINFO GetFileInfo(string filename)
        {
            NativeMethods.SHFILEINFO info = new Helper.NativeMethods.SHFILEINFO();

            string fileName = filename;
            uint dwFileAttributes = NativeMethods.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = (uint)(NativeMethods.SHGFI.SHGFI_TYPENAME | NativeMethods.SHGFI.SHGFI_USEFILEATTRIBUTES);

            NativeMethods.SHGetFileInfo(fileName, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
            return info;
        }
    }
}
