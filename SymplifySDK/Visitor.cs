using System;

using SymplifySDK.Cookies;

namespace SymplifySDK
{
    /// <summary>
    /// Visitor contains functions for managing visitor IDs.
    /// </summary>
    public static class Visitor
    {
        /// <summary>
        /// Get the current visitor id for this website in <paramref name="cookies"/>.
        /// If none exists, generate it, set it in the website cookie, and return it.
        /// </summary>
        public static string EnsureVisitorID(SymplifyCookie cookies, Func<string> makeID = null)
        {
            var currID = cookies.GetVisitorID();

            if (currID == null || currID == string.Empty)
            {
                string visitorID = makeID != null ? makeID() : UuidGenerator();
                currID = visitorID;
                cookies.SetVisitorID(currID);
            }

            return currID;
        }

        private static string UuidGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
