using System;

namespace Symplify.Conversion.SDK
{
    /// <summary>
    /// CustomHash houses functions for SDK specific hashing we need for compatibility reasons.
    /// </summary>
    public static class CustomHash
    {
        /// <summary>
        /// Hash the given string key and scale the hash result to fit within the given window.
        /// </summary>
        public static int HashInWindow(string key, uint window)
        {
            uint unsignedMax = 4_294_967_295;

            double hash = Djb2Xor(key);

            // scale hash to the desired window
            hash /= unsignedMax;
            return (int)Math.Ceiling(hash * window);
        }

        /// <summary>
        /// Hash the given string using the djb2 algorithm, xor variant.
        /// See http://www.cse.yorku.ca/~oz/hash.html.
        /// </summary>
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
