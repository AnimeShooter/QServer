namespace TNL.Utils
{
    public static class Utils
    {
        public static bool IsPow2(uint number)
        {
            return number > 0 && (number & (number - 1)) == 0;
        }

        public static uint GetBinLog2(uint value)
        {
            var floatValue = (float) value;
            //var ret = Math.Floor(Math.Log(value, 2));

            unsafe
            {
                return (*((uint*)&floatValue) >> 23) - 127;
            }
        }

        public static uint GetNextBinLog2(uint number)
        {
            return GetBinLog2(number) + (IsPow2(number) ? 0U : 1U);
        }

        public static uint GetNextPow2(uint value)
        {
            return IsPow2(value) ? value : (1U << ((int) GetBinLog2(value) + 1));
        }
    }
}
