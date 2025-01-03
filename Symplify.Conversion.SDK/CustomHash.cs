using System;

namespace Symplify.Conversion.SDK
{
    /// <summary>
    /// CustomHash houses functions for SDK specific hashing we need for compatibility reasons.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop", "SA1121: Use built-in type alias", Justification = "we want to be sure hash is 32 bit")]
    public static class CustomHash
    {
        /// <summary>
        /// Hash the given string key and scale the hash result to fit within the given window.
        /// </summary>
        public static double HashInWindow(string key, uint window)
        {
            UInt32 unsignedMax = 4_294_967_295;

            // we need a higher precision floating point value for the scaling operation
            double hash = Djb2Xor(key);

            // scale hash to the desired window
            hash /= unsignedMax;
            hash *= window;

            return hash;
        }

        /// <summary>
        /// Hash the given string using the djb2 algorithm, xor variant.
        /// See http://www.cse.yorku.ca/~oz/hash.html.
        /// </summary>
        public static UInt32 Djb2Xor(string str)
        {
            UInt32 hash = 5381;

            for (var i = 0; i < str.Length; i += 1)
            {
                hash = 33 * hash & 0xFFFFFFFF ^ str[i];
            }

            return hash;
        }
    }
}
