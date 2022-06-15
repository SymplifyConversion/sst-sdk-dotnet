using System;
using SymplifySDK.Cookies;

namespace SymplifySDK
{
    public static class Visitor
    {
        /// <summary>
        /// Get the current visitor id for this website in <paramref name="cookies"/>.
        /// If none exists, generate it, set it in the website cookie, and return it.
        /// </summary>
        /// <param name="cookies">The SymplifyCookie housing visitor info</param>
        /// <param name="websiteID">The ID of the current website</param>
        /// <param name="makeID">An optional ID generation override, used for testing</param>
        public static string EnsureVisitorID(SymplifyCookie cookies, string websiteID, Func<string> makeID = null)
        {
            var currID = cookies.GetVisitorID(websiteID);

            if (currID == null || currID == string.Empty)
            {
                string visitorID = makeID != null ? makeID() : UuidGenerator();
                currID = visitorID;
                cookies.SetVisitorID(websiteID, currID);
            }

            return currID;
        }

        private static string UuidGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
