using System.Runtime.InteropServices;
using System.Text;

namespace EAGSS.DataPackage
{
    internal class StructDescription
    {
        #region Nested type: ContentInfo

        internal struct ContentInfo
        {
            public int Length;
            public int Offset;
            public string Path;
            public int RealLength;

            public ContentInfo(string path, int offset, int length, int realLength)
            {
                Path = path;
                Offset = offset;
                Length = length;
                RealLength = realLength;
            }
        }

        #endregion

        #region Nested type: EntryInfo

        /// <summary>
        /// EntryInfo
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct EntryInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)] private readonly byte[] _name;

            public int Start;
            public int Length;
            public int RealLength;

            /// <summary>
            /// EntryInfo
            /// </summary>
            /// <param name="name">Entry name (as content folder)</param>
            /// <param name="start">Start offset</param>
            /// <param name="length">Entry length</param>
            /// <param name="realLength">Real entry length</param>
            public EntryInfo(string name, int start, int length, int realLength)
            {
                _name = BytesHelper.ExpandBytes(Encoding.UTF8.GetBytes(name), 52);
                Start = start;
                Length = length;
                RealLength = realLength;
            }

            public string Name
            {
                get { return Encoding.UTF8.GetString(_name).TrimEnd('\x00'); }
            }
        }

        #endregion

        #region Nested type: PackageInfo

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct PackageInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public char[] Magic;

            public int EntryCount;

            /// <summary>
            /// PackageInfo
            /// </summary>
            /// <param name="entryCount">File count in this package</param>
            public PackageInfo(int entryCount)
            {
                Magic = new[] {'P', 'K', 'G', 'I', 'N', 'F', 'O', '\x00'};
                EntryCount = entryCount;
            }
        }

        #endregion
    }
}