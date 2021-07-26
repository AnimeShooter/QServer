using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qserver.External.HTTP.API
{
    public unsafe static class pkg
    {
        public static PkgUnpackAPI UnpackPkg(byte[] buffer)
        {
            uint something = BitConverter.ToUInt32(buffer, 0);

            List<PkgEntry> pkgs = new List<PkgEntry>();

            int i = 0;
            uint byteCount = 0;
            while(true)
            {
                byte[] entry = new byte[0x88];
                Array.Copy(buffer, 0x04+(entry.Length * i), entry, 0, entry.Length); // offset by 4 (file header?)

                byte[] unpackedEntry = PkgDecode(entry);

                //PkgEntry pkg = GetObject<PkgEntry>(PkgDecode(entry));

                PkgEntry pkg = new PkgEntry()
                {
                    Filename = Encoding.ASCII.GetString(unpackedEntry).Split('\x00')[0],
                    StartLocation = BitConverter.ToUInt32(unpackedEntry, 0x80),
                    EntrySize = BitConverter.ToUInt32(unpackedEntry, 0x84),
                };

                byteCount += pkg.EntrySize + 0x88;

                if (byteCount > buffer.Length - 4)
                    break;

                if (pkg.Filename == "")
                    break;

                pkgs.Add(pkg);
                i++;
            }

            // format result
            var result = new PkgUnpackAPI()
            {
                Entries = new string[i],
                Contents = new string[i]
            };

            for(int j = 0; j < i; j++)
            {
                byte[] contentBuf = new byte[pkgs[j].EntrySize];
                Array.Copy(buffer, 4 + pkgs[j].StartLocation, contentBuf, 0, contentBuf.Length); // offset by 4 (entry content header?)
                result.Entries[j] = pkgs[j].Filename;
                result.Contents[j] = Encoding.UTF8.GetString(PkgDecode(contentBuf));
            }

            return result;
        }

        public static byte[] PkgDecode(byte[] bin)
        {
            int size = bin.Length;
            byte[] bout = new byte[size];
            //byte[] bin; // edi
            int index; // ecx
            int v5; // edi
            int index_2; // eax
            int v7; // esi
            int in2;

            if (size / 4 > 0)
            {
                v5 = (size / 4) * 4; // Takes chunks of 4 bbytes
                for (int i = 0; i < v5; i += 4)
                {
                    // Read and NOT
                    uint start = ~BitConverter.ToUInt32(bin, i);

                    // Swap
                    uint right = (start >> 19);
                    right &= 0x00001FFF; // ditch overflow
                    uint left = (start << 13);
                    //left &= 0xFFFFE000;
                    var bytes = BitConverter.GetBytes(left | right);

                    // Store DWORD
                    for (int j = 0; j < 4; j++)
                        bout[i + j] = bytes[j];
                }
            }
            // NOTE: fixed the last few bytes after 4th
            //index_2 = size % 4;
            //for(int i = (size / 4) + (size % 4); i < size; i++)
            //    bout[i] = bin[i]; //*(byte*)index_2 = *((byte*)bin + index_2 - out);

            return bout;
        }
    }
}
