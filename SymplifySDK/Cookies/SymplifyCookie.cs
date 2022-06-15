using System.Collections.Generic;
using System.Net;
using System.Text.Json;

using Newtonsoft.Json.Linq;

namespace SymplifySDK.Cookies
{
    public enum ProjectAllocationStatus
    {
        Unknown,
        Allocated,
        NotAllocated,
    }

    public class SymplifyCookie
    {
        public const string CookieName = "sg_cookies";
        private const int SupportedCookieVersion = 1;
        private const string CookieVersionKey = "_g";
        private const string VisitorIdKey = "visid";
        private const string AllocatedProjectsKey = "aud_p";

        private JObject jobj;

        public SymplifyCookie()
        {
            jobj = new();
            jobj[CookieVersionKey] = 1;
        }

        public static SymplifyCookie FromCookies(ICookieJar cookies)
        {
            string cookieJSON = cookies.GetCookie(CookieName);

            if (cookieJSON == null)
            {
                return new();
            }

            return SymplifyCookie.Decode(cookieJSON);
        }

        public static SymplifyCookie Decode(string value)
        {
            var jsonString = WebUtility.UrlDecode(value);
            var cookie = JsonSerializer.Deserialize<SymplifyCookie>(jsonString);
            cookie.jobj = JObject.Parse(jsonString);
            return cookie;
        }

        public string Encode()
        {
            return WebUtility.UrlEncode(jobj.ToString());
        }

        public int GetVersion()
        {
            return (int)jobj[CookieVersionKey];
        }

        public bool IsSupported()
        {
            return (int)jobj[CookieVersionKey] == SupportedCookieVersion;
        }

        public string GetVisitorID(string websiteID)
        {
            var websiteData = GetWebsiteData(websiteID);
            return (string)websiteData[VisitorIdKey];
        }

        public void SetVisitorID(string websiteID, string visitorID)
        {
            var websiteData = GetWebsiteData(websiteID);
            websiteData[VisitorIdKey] = visitorID;
        }

        public long GetAllocatedVariationID(string websiteID, long projectID)
        {
            var websiteData = GetWebsiteData(websiteID);
            return (long)websiteData[$"{projectID}"][0];
        }

        public void SetAllocatedVariationID(string websiteID, long projectID, long variationID)
        {
            var websiteData = GetWebsiteData(websiteID);
            var variations = new JArray();
            variations.Add(variationID);
            websiteData[$"{projectID}"] = variations;
            websiteData[$"{projectID}_ch"] = "1";
            AddAllocatedProjectID(websiteID, projectID);
        }

        public void SetAllocatedNullVariation(string websiteID, long projectID)
        {
            var websiteData = GetWebsiteData(websiteID);
            websiteData[$"{projectID}_ch"] = "-1";
        }

        public ProjectAllocationStatus GetProjectAllocationStatus(string websiteID, long projectID)
        {
            var websiteData = GetWebsiteData(websiteID);
            var allocated = websiteData[$"{projectID}_ch"];

            if (allocated == null || allocated.Type == JTokenType.Null)
            {
                return ProjectAllocationStatus.Unknown;
            }

            if ((int)allocated == 1)
            {
                return ProjectAllocationStatus.Allocated;
            }

            return ProjectAllocationStatus.NotAllocated;
        }

        public IList<long> GetAllocatedProjectIDs(string websiteID)
        {
            var websiteData = GetWebsiteData(websiteID);
            var allocated = websiteData[AllocatedProjectsKey];

            if (allocated is JArray)
            {
                return allocated.ToObject<IList<long>>();
            }

            return new List<long>();
        }

        private JObject GetWebsiteData(string websiteID)
        {
            if (!jobj.ContainsKey(websiteID))
            {
                jobj[websiteID] = new JObject();
            }

            return (JObject)jobj[websiteID];
        }

        private void AddAllocatedProjectID(string websiteID, long projectID)
        {
            var websiteData = GetWebsiteData(websiteID);

            if (!websiteData.ContainsKey(AllocatedProjectsKey))
            {
                websiteData[AllocatedProjectsKey] = new JArray();
            }

            var allocatedProjects = (JArray)websiteData[AllocatedProjectsKey];

            foreach (var pid in allocatedProjects)
            {
                if (pid.Type == JTokenType.Integer && (long)pid == projectID)
                {
                    return;
                }
            }

            allocatedProjects.Add(projectID);
        }
    }
}
