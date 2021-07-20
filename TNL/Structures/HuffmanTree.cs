using System.Collections.Generic;

namespace TNL.Structures
{
    public static class HuffmanTree
    {
        public static readonly Dictionary<byte, HuffmanTreeNode> Leaves = new(); 

        public static HuffmanTreeNode Root { get; set; }

        public static bool TablesBuilt { get; private set; }

        public static void Build()
        {
            if (TablesBuilt)
                return;

            TablesBuilt = true;

            var nodes = new HuffmanTreeNode[256];
            var currCount = 256;

            for (var i = 0; i < 256; ++i)
            {
                nodes[i] = new HuffmanTreeNode
                {
                    Symbol = (byte) i,
                    Frequency = CharFreqs[i] + 1,
                    NumBits = 0,
                    Code = 0,
                    Left = null,
                    Right = null
                };

                Leaves.Add((byte) i, nodes[i]);
            }

            while (currCount != 1)
            {
                uint min1 = 0xFFFFFFFEU, min2 = 0xFFFFFFFFU;
                int index1 = -1, index2 = -1;

                for (var i = 0; i < currCount; ++i)
                {
                    if (nodes[i].Frequency < min1)
                    {
                        min2 = min1;
                        index2 = index1;

                        min1 = nodes[i].Frequency;
                        index1 = i;
                    }
                    else if (nodes[i].Frequency < min2)
                    {
                        min2 = nodes[i].Frequency;
                        index2 = i;
                    }
                }
                
                if (index1 != -1 && index2 != -1 && index1 != index2)
                {
                    var node1 = nodes[index1];
                    var node2 = nodes[index2];

                    var parent = new HuffmanTreeNode
                    {
                        Symbol = (byte) '*',
                        Frequency = node1.Frequency + node2.Frequency,
                        Left = node1,
                        Right = node2
                    };

                    nodes[index1 > index2 ? index2 : index1] = parent;
                    
                    if (index2 != (currCount - 1))
                        nodes[index1 > index2 ? index1 : index2] = nodes[currCount - 1];

                    --currCount;
                }

                Root = nodes[0];
            }

            GenerateCodes(Root, 0, 0);
        }

        private static void GenerateCodes(HuffmanTreeNode node, uint code, uint depth)
        {
            if (node.IsLeaf())
            {
                node.NumBits = depth;
                node.Code = code;
            }
            else
            {
                var leftCode = code & ~(1U << (int) depth);
                GenerateCodes(node.Left, leftCode, depth + 1);

                var rightCode = code | (1U << (int) depth);
                GenerateCodes(node.Right, rightCode, depth + 1);
            }
        }

        public static bool IsLeaf(HuffmanTreeNode node)
        {
            return node.IsLeaf();
        }

        public readonly static uint[] CharFreqs =
        {
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            329   ,
            21    ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            2809  ,
            68    ,
            0     ,
            27    ,
            0     ,
            58    ,
            3     ,
            62    ,
            4     ,
            7     ,
            0     ,
            0     ,
            15    ,
            65    ,
            554   ,
            3     ,
            394   ,
            404   ,
            189   ,
            117   ,
            30    ,
            51    ,
            27    ,
            15    ,
            34    ,
            32    ,
            80    ,
            1     ,
            142   ,
            3     ,
            142   ,
            39    ,
            0     ,
            144   ,
            125   ,
            44    ,
            122   ,
            275   ,
            70    ,
            135   ,
            61    ,
            127   ,
            8     ,
            12    ,
            113   ,
            246   ,
            122   ,
            36    ,
            185   ,
            1     ,
            149   ,
            309   ,
            335   ,
            12    ,
            11    ,
            14    ,
            54    ,
            151   ,
            0     ,
            0     ,
            2     ,
            0     ,
            0     ,
            211   ,
            0     ,
            2090  ,
            344   ,
            736   ,
            993   ,
            2872  ,
            701   ,
            605   ,
            646   ,
            1552  ,
            328   ,
            305   ,
            1240  ,
            735   ,
            1533  ,
            1713  ,
            562   ,
            3     ,
            1775  ,
            1149  ,
            1469  ,
            979   ,
            407   ,
            553   ,
            59    ,
            279   ,
            31    ,
            0     ,
            0     ,
            0     ,
            68    ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0     ,
            0
        };
    }
}
