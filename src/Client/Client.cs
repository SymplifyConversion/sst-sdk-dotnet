using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Allocation.Config;

namespace Client
{
    public class Client
    {
        public string WebsiteID { get; set; }
        public string CdnBaseURL { get; set; }

        public SymplifyConfig Config { get; set; }

        public Client(ClientConfig clientConfig)
        {
            CdnBaseURL = clientConfig.CdnBaseURL;
            WebsiteID = clientConfig.WebsiteID;
            Config = null;
        }

        public List<string> listProjects()
        {
            if (this.Config != null)
            {
                return new List<string>();
            }

            List<string> projectList = new List<string>();

            foreach (ProjectConfig project in this.Config.Projects)
            {
                projectList.Add(project.Name);
            }

            return projectList;
        }
    }

}
