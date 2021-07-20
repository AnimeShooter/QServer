using System;
using System.Collections.Generic;
using System.Linq;

namespace TNL.Structures
{
    public static class StringTable
    {
        public const uint InitialHashTableSize = 1237;
        public const uint InitialNodeListSize = 2048;
        public const uint CompactThreshold = 32768;

        private static readonly Dictionary<uint, string> Table = new();
        private static uint _nextIndex = 1;

        public static uint Insert(string str, bool caseSensitive = true)
        {
            if (str.Length == 0)
                return 0U;

            var ind = Lookup(str, caseSensitive);
            if (ind > 0)
                return ind;

            ind = _nextIndex++;

            Table.Add(ind, str);

            return ind;
        }

        public static uint Lookup(string str, bool caseSensitive = true)
        {
            return (from pair in Table where pair.Value == str select pair.Key).FirstOrDefault();
        }

        public static uint HashString(string inString)
        {
            throw new NotImplementedException();
        }

        public static string GetString(uint index)
        {
            return index == 0 ? "" : Table[index];
        }

        public class Node
        {
            public uint MasterIndex { get; set; }
            public uint NextIndex { get; set; }
            public uint Hash { get; set; }
            public ushort StringLen { get; set; }
            public ushort RefCount { get; set; }
            public string StringData { get; set; }
        }
    }
}
