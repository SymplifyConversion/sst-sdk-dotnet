﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symplify.Conversion.SDK.Cookies
{
    /// <summary>
    /// ProjectAllocationStatus describes what we know about a visitor's allocation in a project.
    /// </summary>
    public enum ProjectAllocationStatus
    {
        /// <summary>
        /// Unknown means there is no persisted status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Allocated means we know the visitor has a variation allocated.
        /// </summary>
        Allocated,

        /// <summary>
        /// NotAllocated means we know the visitor has been allocated outside of any variation.
        /// </summary>
        NotAllocated,
    }

    /// <summary>
    /// SymplifyCookie is a cross platform JSON cookie used by our SDKs.
    /// </summary>
    public class SymplifyCookie
    {
        /// <summary>
        /// The name of the JSON cookie.
        /// </summary>
        public const string CookieName = "sg_cookies";
        private const int SupportedCookieVersion = 1;
        private const string CookieVersionKey = "_g";
        private const string VisitorIdKey = "visid";
        private const string AllocatedProjectsKey = "aud_p";
        private const string CookiePreviewProjectKey = "pmr";
        private const string CookiePreviewVariationKey = "pmv";
        private readonly JObject jobj;
        private readonly string currentWebsiteID;

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyCookie"/> class.
        /// </summary>
        public SymplifyCookie(string websiteID)
        : this(websiteID, new JObject())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyCookie"/> class.
        /// </summary>
        public SymplifyCookie(string websiteID, JObject underlying)
        {
            currentWebsiteID = websiteID;
            jobj = underlying;

            if (jobj[CookieVersionKey] == null)
            {
                jobj[CookieVersionKey] = 1;
            }
        }

        /// <summary>
        /// Finds the symplify JSON cookie and decode it.
        /// </summary>
        public static SymplifyCookie FromCookies(string websiteID, ICookieJar cookies)
        {
            string cookieJSON = cookies.GetCookie(CookieName);

            if (cookieJSON == null)
            {
                return new SymplifyCookie(websiteID);
            }

            return SymplifyCookie.FromJSON(websiteID, cookieJSON);
        }

        /// <summary>
        /// Deserializes the cookie from JSON.
        /// </summary>
        public static SymplifyCookie FromJSON(string websiteID, string value)
        {
            var underlying = JObject.Parse(value);
            return new SymplifyCookie(websiteID, underlying);
        }

        /// <summary>
        /// Serializes the JSON cookie.
        /// </summary>
        public string ToJSON()
        {
            return jobj.ToString(Formatting.None);
        }

        /// <summary>
        /// Gets the version in the underlying JSON cookie data.
        /// </summary>
        public int GetVersion()
        {
            return (int)jobj[CookieVersionKey];
        }

        /// <summary>
        /// Returns true if and only if the underlying JSON cookie data is compatible.
        /// </summary>
        public bool IsSupported()
        {
            return (int)jobj[CookieVersionKey] == SupportedCookieVersion;
        }

        /// <summary>
        /// Gets the cookie's visitor.
        /// </summary>
        public string GetVisitorID()
        {
            var websiteData = GetWebsiteData();
            return (string)websiteData[VisitorIdKey];
        }

        /// <summary>
        /// Sets the cookie's visitor.
        /// </summary>
        public void SetVisitorID(string visitorID)
        {
            var websiteData = GetWebsiteData();
            websiteData[VisitorIdKey] = visitorID;
        }

        /// <summary>
        /// Gets the allocated variation ID for the cookie's visitor in the given project.
        /// </summary>
        public long GetAllocatedVariationID(long projectID)
        {
            var websiteData = GetWebsiteData();
            return (long)websiteData[$"{projectID}"][0];
        }

        /// <summary>
        /// Sets the cookie's visitor's variation allocation in the given project.
        /// </summary>
        public void SetAllocatedVariationID(long projectID, long variationID)
        {
            var websiteData = GetWebsiteData();
            var variations = new JArray();
            variations.Add(variationID);
            websiteData[$"{projectID}"] = variations;
            websiteData[$"{projectID}_ch"] = "1";
            AddAllocatedProjectID(projectID);
        }

        /// <summary>
        /// Sets a null allocation for the cookie's visitor in the given project.
        /// </summary>
        public void SetAllocatedNullVariation(long projectID)
        {
            var websiteData = GetWebsiteData();
            websiteData[$"{projectID}_ch"] = "-1";
        }

        /// <summary>
        /// Gets the allocation status for the cookie's visitor in the given project.
        /// </summary>
        public ProjectAllocationStatus GetProjectAllocationStatus(long projectID)
        {
            var websiteData = GetWebsiteData();
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

        /// <summary>
        /// Gets all project IDs the cookie's visitor has been allocated (not null) in for the given website.
        /// </summary>
        public IList<long> GetAllocatedProjectIDs()
        {
            var websiteData = GetWebsiteData();
            var allocated = websiteData[AllocatedProjectsKey];

            if (allocated is JArray)
            {
                return allocated.ToObject<IList<long>>();
            }

            return new List<long>();
        }

        /// <summary>
        /// returns the preview data if one exists otherwise an empty dict.
        /// </summary>
        public Dictionary<string, int> GetPreviewData()
        {
            var websiteData = GetWebsiteData();
            JToken projectId = websiteData[CookiePreviewProjectKey];

            if (projectId == null)
            {
                return new Dictionary<string, int>();
            }

            if (projectId.Type != JTokenType.Integer)
            {
                return new Dictionary<string, int>();
            }

            JToken variationId = websiteData[CookiePreviewVariationKey];
            if (variationId == null)
            {
                return new Dictionary<string, int>();
            }

            if (variationId.Type != JTokenType.Integer)
            {
                return new Dictionary<string, int>();
            }

            return new Dictionary<string, int>
            {
                { "projectId", (int)projectId },
                { "variationId", (int)variationId },
            };
        }

        private JObject GetWebsiteData()
        {
            if (!jobj.ContainsKey(currentWebsiteID))
            {
                jobj[currentWebsiteID] = new JObject();
            }

            return (JObject)jobj[currentWebsiteID];
        }

        private void AddAllocatedProjectID(long projectID)
        {
            var websiteData = GetWebsiteData();

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
