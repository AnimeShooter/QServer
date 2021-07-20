namespace TNL.Entities
{
    using Data;
    using Interfaces;
    using Types;

    public class BaseObject : INetObject
    {
        public virtual NetClassRep GetClassRep()
        {
            return null;
        }

        public uint GetClassId(NetClassGroup group)
        {
            return GetClassRep().GetClassId(group);
        }

        public string GetClassName()
        {
            return GetClassRep().ClassName;
        }

        public static BaseObject Create(string className)
        {
            return NetClassRep.Create(className);
        }

        public static BaseObject Create(uint groupId, uint typeId, int classId)
        {
            return NetClassRep.Create(groupId, typeId, classId);
        }

        public static void Declare<T>(out NetClassRep rep) where T : BaseObject, new()
        {
            rep = new NetClassRepInstance<T>(typeof(T).Name, 0, NetClassType.NetClassTypeNone, 0);
        }
    }
}
