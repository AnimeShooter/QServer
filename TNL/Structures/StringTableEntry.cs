namespace TNL.Structures
{
    public class StringTableEntry
    {
        public uint Index { get; private set; }

        public StringTableEntry()
        {
            Index = 0;
        }

        public StringTableEntry(string str, bool caseSensitive = true)
        {
            Index = StringTable.Insert(str, caseSensitive);
        }

        public StringTableEntry(StringTableEntry copy)
        {
            Index = copy.Index;
        }

        public void Set(string str, bool caseSensitive = true)
        {
            Index = StringTable.Insert(str, caseSensitive);
        }

        public bool IsNull()
        {
            return Index == 0U;
        }

        public bool IsNotNull()
        {
            return Index != 0U;
        }

        public bool IsValid()
        {
            return Index != 0U;
        }

        public string GetString()
        {
            return StringTable.GetString(Index);
        }

        public static bool operator ==(StringTableEntry s1, StringTableEntry s2)
        {
            return s1 != null && s2 != null && s1.Index == s2.Index;
        }

        public static bool operator !=(StringTableEntry s1, StringTableEntry s2)
        {
            return !(s1 == s2);
        }

        public static implicit operator bool(StringTableEntry s)
        {
            return s.Index != 0U;
        }

        public bool Equals(StringTableEntry other)
        {
            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as StringTableEntry;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }
    }
}
