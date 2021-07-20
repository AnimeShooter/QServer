namespace TNL.Structures
{
    public class BitSet
    {
        public uint BitMask { get; private set; }

        public BitSet()
        {
            BitMask = 0U;
        }

        public BitSet(uint bits)
        {
            BitMask = bits;
        }

        public void Set()
        {
            BitMask = 0xFFFFFFFFU;
        }

        public void Set(uint bits)
        {
            BitMask |= bits;
        }

        public void Set(BitSet s, bool b)
        {
            BitMask = (BitMask & ~(s.BitMask)) | (b ? s.BitMask : 0);
        }

        public void Clear()
        {
            BitMask = 0;
        }

        public void Clear(uint bits)
        {
            BitMask &= ~bits;
        }

        public void Toggle(uint bits)
        {
            BitMask ^= bits;
        }

        public bool Test(uint bits)
        {
            return (BitMask & bits) != 0U;
        }

        public bool TestStrict(uint bits)
        {
            return (BitMask & bits) == bits;
        }

        public static implicit operator uint(BitSet b)
        {
            return b.BitMask;
        }

        public static implicit operator BitSet(uint bits)
        {
            return new(bits);
        }

        public static BitSet operator |(BitSet b, uint bits)
        {
            return new(b.BitMask | bits);
        }

        public static BitSet operator &(BitSet b, uint bits)
        {
            return new(b.BitMask & bits);
        }

        public static BitSet operator ^(BitSet b, uint bits)
        {
            return new(b.BitMask ^ bits);
        }
    }
}
