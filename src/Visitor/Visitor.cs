using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Visitor
{
    public class Visitor
    {
        const string CookieName = "sg_sst_vid";

        public static string EnsureVisitorID(CookieCollection cookieCollection, string logger, Func<string> makeID = null)
        {
            Cookie visitorCookie = cookieCollection[CookieName];
            if (visitorCookie != null)
            {
                return visitorCookie.Value;
            }

            string visitorID = makeID != null ? makeID() : uuidGenerator();
            cookieCollection.Add(new Cookie(CookieName, visitorID));

            return visitorID;
        }

        private static string uuidGenerator()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
