using System;
using System.Collections.Generic;
using System.Linq;

namespace TNL.Types
{
    using Data;
    using Entities;
    using Utils;

    public abstract class NetClassRep
    {
        public static readonly List<NetClassRep> ClassList = new();

        public static uint[][] NetClassBitSize { get; private set; }
        public static List<NetClassRep>[][] ClassTable { get; private set; }
        public static bool Initialized { get; private set; }
        public static uint[] ClassCRC { get; private set; }

        public uint ClassGroupMask { get; protected set; }
        public int ClassVersion { get; protected set; }
        public NetClassType ClassType { get; protected set; }
        public uint[] ClassId { get; protected set; }
        public string ClassName { get; protected set; }

        public uint InitialUpdateBitsUsed { get; protected set; }
        public uint PartialUpdateBitsUsed { get; protected set; }
        public uint InitialUpdateCount { get; protected set; }
        public uint PartialUpdateCount { get; protected set; }

        protected NetClassRep()
        {
            InitialUpdateCount = 0;
            InitialUpdateBitsUsed = 0;
            PartialUpdateCount = 0;
            PartialUpdateBitsUsed = 0;

            ClassId = new uint[(int) NetClassGroup.NetClassGroupCount];
        }

        public uint GetClassId(NetClassGroup classGroup)
        {
            return ClassId[(int) classGroup];
        }

        public void AddInitialUpdate(uint bitCount)
        {
            ++InitialUpdateCount;
            InitialUpdateBitsUsed += bitCount;
        }

        public void AddPartialUpdate(uint bitCount)
        {
            ++PartialUpdateCount;
            PartialUpdateBitsUsed += bitCount;
        }

        public abstract BaseObject Create();

        #region Static Functions

        static NetClassRep()
        {
            Initialized = false;

            ClassCRC = new uint[(int) NetClassGroup.NetClassGroupCount];
            for (var i = 0; i < ClassCRC.Length; ++i)
                ClassCRC[i] = 0xFFFFFFFFU;

            NetClassBitSize = new uint[(int) NetClassGroup.NetClassGroupCount][];

            ClassTable = new List<NetClassRep>[(int) NetClassGroup.NetClassGroupCount][];
            for (var i = 0; i < ClassTable.Length; ++i)
            {
                NetClassBitSize[i] = new uint[(int) NetClassType.NetClassTypeCount];

                ClassTable[i] = new List<NetClassRep>[(int) NetClassType.NetClassTypeCount];
                for (var j = 0; j < ClassTable[i].Length; ++j)
                    ClassTable[i][j] = new List<NetClassRep>();
            }
        }

        public static BaseObject Create(string className)
        {
            return (from walk in ClassList where walk.ClassName == className select walk.Create()).FirstOrDefault();
        }

        public static BaseObject Create(uint groupId, uint typeId, int classId)
        {
            return ClassTable[groupId][typeId].Count > classId ? ClassTable[groupId][typeId][classId].Create() : null;
        }

        public static uint GetNetClassCount(uint classGroup, uint classType)
        {
            return (uint) ClassTable[classGroup][classType].Count;
        }

        public static uint GetNetClassBitSize(uint classGroup, uint classType)
        {
            return NetClassBitSize[classGroup][classType];
        }

        public static bool IsVersionBorderCount(uint classGroup, uint classType, uint count)
        {
            return count == GetNetClassCount(classGroup, classType) || (count > 0 && ClassTable[classGroup][classType][(int) count].ClassVersion != ClassTable[classGroup][classType][(int) count - 1].ClassVersion);
        }

        public static NetClassRep GetClass(uint classGroup, uint classType, uint index)
        {
            return ClassTable[classGroup][classType][(int) index];
        }

        public static uint GetClassGroupCRC(NetClassGroup classGroup)
        {
            return ClassCRC[(int) classGroup];
        }

        public static void Initialize()
        {
            if (Initialized)
                return;

            for (var group = 0; group < ClassTable.Length; ++group)
            {
                var groupMask = 1U << group;

                for (var type = 0; type < ClassTable[group].Length; ++type)
                {
                    var dynamicTable = new List<NetClassRep>();

                    dynamicTable.AddRange(ClassList.Where(walk => (int) walk.ClassType == type && (walk.ClassGroupMask & groupMask) != 0));
                    if (dynamicTable.Count == 0)
                        continue;

                    dynamicTable.Sort(new NetClassRepComparer());

                    ClassTable[group][type] = dynamicTable;

                    for (var i = 0; i < ClassTable[group][type].Count; ++i)
                        ClassTable[group][type][i].ClassId[group] = (uint) i;

                    NetClassBitSize[group][type] = Utils.GetBinLog2(Utils.GetNextPow2((uint) ClassTable[group][type].Count + 1U));
                }
            }

            Initialized = true;
        }

        public static void LogBitUsage()
        {
            Console.WriteLine("Net Class Bit Usage:");

            foreach (var walk in ClassList)
            {
                if (walk.InitialUpdateCount > 0U)
                    Console.WriteLine("{0} (Initial) - Count: {1}   Avg Size: {2}", walk.ClassName, walk.InitialUpdateCount, walk.InitialUpdateBitsUsed / (Single)walk.InitialUpdateCount);

                if (walk.PartialUpdateCount > 0U)
                    Console.WriteLine("{0} (Partial) - Count: {1}   Avg Size: {2}", walk.ClassName, walk.PartialUpdateCount, walk.PartialUpdateBitsUsed / (Single)walk.PartialUpdateCount);
            }
        }

        #endregion

        public class NetClassRepComparer : IComparer<NetClassRep>
        {
            public int Compare(NetClassRep a, NetClassRep b)
            {
                if (a.ClassVersion != b.ClassVersion)
                    return a.ClassVersion - b.ClassVersion;

                return string.Compare(a.ClassName, b.ClassName, StringComparison.Ordinal);
            }
        }
    }
}
