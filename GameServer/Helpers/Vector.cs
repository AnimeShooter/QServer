using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qserver.GameServer.Helpers
{
    public class Vector
    {
        public ushort X;
        public ushort Y;

        public Vector()
        {
            X = 0;
            Y = 0;
        }

        public Vector(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ushort Distance(Vector v)
        {
            return (ushort)Math.Sqrt(DistanceSqrd(v));
        }

        public ushort DistanceSqrd(Vector v)
        {
            ushort dX = (ushort)(X - v.X);
            ushort dY = (ushort)(Y - v.Y);
            return (ushort)(dX * dX + dY * dY);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector((ushort)(v1.X - v2.X), (ushort)(v1.Y - v2.Y));
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector((ushort)(v1.X - v2.X), (ushort)(v1.Y - v2.Y));
        }

        public ushort Angle(Vector v)
        {
            return (ushort)Math.Atan2(v.X - X, v.Y - Y);
        }

        public ushort Angle(ushort x, ushort y)
        {
            return (ushort)Math.Atan2(x - X, y - Y);
        }
    }
}
