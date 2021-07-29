using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Qserver.Webserver.HTTP.API
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PkgEntry
    {
        //public uint unk01;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x30)]
        public string Filename;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x50)]
        public byte[] gap;
        public uint StartLocation;
        public uint EntrySize;
    };

    public struct PkgUnpackAPI
    {
        public string[] Entries;
        public string[] Contents;
    }
}
