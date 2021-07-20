using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace TNL.Structures
{
    using Utils;

    public static class ReflectedSerializer
    {
        private static readonly Dictionary<string, Func<BitStream, Type, object>> ReadLookup = new();
        private static readonly Dictionary<string, Action<BitStream, object>> WriteLookup = new();

        static ReflectedSerializer()
        {
            // Read Functions
            ReadLookup.Add("Byte", (b, t) => { b.Read(out byte v); return v; });
            ReadLookup.Add("SByte", (b, t) => { b.Read(out sbyte v); return v; });
            ReadLookup.Add("Int16", (b, t) => { b.Read(out int v); return v; });
            ReadLookup.Add("UInt16", (b, t) => { b.Read(out uint v); return v; });
            ReadLookup.Add("Int32", (b, t) => { b.Read(out int v); return v; });
            ReadLookup.Add("UInt32", (b, t) => { b.Read(out uint v); return v; });
            ReadLookup.Add("Int64", (b, t) => { b.Read(out long v); return v; });
            ReadLookup.Add("UInt64", (b, t) => { b.Read(out ulong v); return v; });

            ReadLookup.Add("Single", (b, t) => { b.Read(out float v); return v; });
            ReadLookup.Add("Double", (b, t) => { b.Read(out double v); return v; });

            ReadLookup.Add("String", (b, t) => { b.ReadString(out string v); return v; });
            ReadLookup.Add("StringTableEntry", (b, t) => { b.ReadStringTableEntry(out StringTableEntry v); return v; });

            ReadLookup.Add("List", (b, t) =>
            {
                var ret = (IList) Activator.CreateInstance(t);
                var size = b.ReadInt(8);
                var memberType = t.GenericTypeArguments[0];

                for (var i = 0; i < size; ++i)
                    ret.Add(ReadLookup[memberType.Name](b, memberType));

                return ret;
            });

            ReadLookup.Add("IPEndPoint", (b, t) =>
            {
                b.Read(out uint netNum);
                b.Read(out ushort port);

                return new IPEndPoint(netNum, port);
            });

            ReadLookup.Add("ByteBuffer", (b, t) =>
            {
                var size = b.ReadInt(10);
                var ret = new ByteBuffer(size);

                b.Read(size, ret.GetBuffer());

                return ret;
            });

            // Write Functions
            WriteLookup.Add("Byte", (b, o) => b.Write((byte) o));
            WriteLookup.Add("SByte", (b, o) => b.Write((sbyte) o));
            WriteLookup.Add("Int16", (b, o) => b.Write((short) o));
            WriteLookup.Add("UInt16", (b, o) => b.Write((ushort) o));
            WriteLookup.Add("Int32", (b, o) => b.Write((int) o));
            WriteLookup.Add("UInt32", (b, o) => b.Write((uint) o));
            WriteLookup.Add("Int64", (b, o) => b.Write((long) o));
            WriteLookup.Add("UInt64", (b, o) => b.Write((ulong) o));

            WriteLookup.Add("Single", (b, o) => b.Write((float) o));
            WriteLookup.Add("Double", (b, o) => b.Write((double) o));

            WriteLookup.Add("String", (b, o) => b.WriteString((string) o));

            WriteLookup.Add("StringTableEntry", (b, o) => b.WriteStringTableEntry((StringTableEntry) o));

            WriteLookup.Add("List", (b, o) =>
            {
                var list = (IList) o;
                var memberType = o.GetType().GenericTypeArguments[0].Name;

                b.WriteInt((uint) list.Count, 8);

                foreach (var t in list)
                    WriteLookup[memberType](b, t);
            });

            WriteLookup.Add("IPEndPoint", (b, o) =>
            {
                var iep = (IPEndPoint) o;

                b.Write(BitConverter.ToUInt32(iep.Address.GetAddressBytes(), 0));
                b.Write((ushort) iep.Port);
            });

            WriteLookup.Add("ByteBuffer", (b, o) =>
            {
                var bb = (ByteBuffer) o;

                b.WriteInt(bb.GetBufferSize(), 10);
                b.Write(bb.GetBufferSize(), bb.GetBuffer());
            });
        }

        public static object Read(BitStream stream, Type type)
        {
            var typeName = type.Name;

            if (ReadLookup.ContainsKey(typeName))
                return ReadLookup[typeName](stream, type);

            Console.WriteLine("Reading {0} with reflecftion is not implemented!", type);
            return null;
        }

        public static void Write(BitStream stream, object obj, Type type)
        {
            var typeName = type.Name;

            if (!WriteLookup.ContainsKey(typeName))
            {
                Console.WriteLine("Writing {0} with reflecftion is not implemented!", type);
                return;
            }

            WriteLookup[typeName](stream, obj);
        }
    }
}
