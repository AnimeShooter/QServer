using System;

namespace TNL.Utils
{
    public static class RandomUtil
    {
        public static void Read(byte[] data, uint size)
        {
            if (data != null)
                new Random().NextBytes(data);
        }

        public static uint ReadI()
        {
            var data = new byte[4];

            new Random().NextBytes(data);

            return BitConverter.ToUInt32(data, 0);
        }

        public static uint ReadI(uint rangeStart, uint rangeEnd)
        {
            return (ReadI() % (rangeEnd - rangeStart + 1)) + rangeStart;
        }

        public static float ReadF()
        {
            return ReadI() / (float) uint.MaxValue;
        }

        public static bool ReadB()
        {
            var data = new byte[1];

            new Random().NextBytes(data);

            return (data[0] & 1) != 0;
        }
    }
}
