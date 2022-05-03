using System;

namespace CustomHash
{
    public class CustomHash
    {
        public static int HashInWindow(string key, uint window)
        {
            uint unsignedMax = 4_294_967_295;

            double hash = Djb2Xor(key);

            // scale hash to the desired window
            hash /= unsignedMax;
            return (int)Math.Ceiling(hash * window);
        }

        public static ulong Djb2Xor(string str)
        {
            ulong hash = 5381;

            for (var i = 0; i < str.Length; i += 1)
            {
                hash = 33 * hash & 0xFFFFFFFF ^ str[i];
            }

            return hash;
        }
    }
}
