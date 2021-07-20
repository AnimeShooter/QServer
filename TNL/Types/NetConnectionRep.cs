using System.Collections.Generic;
using System.Linq;

namespace TNL.Types
{
    using Entities;

    public class NetConnectionRep
    {
        public static readonly List<NetConnectionRep> LinkedList = new();

        public NetClassRep ClassRep { get; private set; }
        public bool CanRemoteCreate { get; private set; }

        public NetConnectionRep(NetClassRep classRep, bool canRemoteCreate)
        {
            LinkedList.Add(this);

            ClassRep = classRep;
            CanRemoteCreate = canRemoteCreate;
        }

        public static NetConnection Create(string name)
        {
            return (from walk in LinkedList where walk.CanRemoteCreate && walk.ClassRep.ClassName == name select walk.ClassRep.Create() as NetConnection).FirstOrDefault();
        }
    }
}
