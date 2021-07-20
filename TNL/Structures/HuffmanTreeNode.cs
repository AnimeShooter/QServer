namespace TNL.Structures
{
    public class HuffmanTreeNode
    {
        public byte Symbol { get; set; }
        public uint Frequency { get; set; }
        public HuffmanTreeNode Right { get; set; }
        public HuffmanTreeNode Left { get; set; }

        public uint NumBits { get; set; }
        public uint Code { get; set; }

        public bool IsLeaf()
        {
            return Left == null && Right == null;
        }
    }
}
