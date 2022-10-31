using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Symplify.Conversion.SDK.Allocation.Config
{
    /// <summary>
    /// SymplifyConfig defines the configuration of all tests for a website. It is a reflection of the JSON configuration file the SDK receives from the backend.
    /// </summary>
    public class SymplifyConfig
    {
        /// <summary>
        /// Gets or sets the updated timestamp.
        /// </summary>
        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        /// <summary>
        /// Gets or sets the site privacy mode.
        /// </summary>
        [JsonPropertyName("privacy_mode")]
        public int Privacy_mode { get; set; }
        // Hack because serializer dont want to work with the camelCase above.
        //public int privacy_mode { get; set; }


        /// <summary>
        /// Gets or sets the current projects.
        /// </summary>
        [JsonPropertyName("projects")]
        public List<ProjectConfig> Projects { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyConfig"/> class.
        /// </summary>
        public SymplifyConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyConfig"/> class.
        /// </summary>
        public SymplifyConfig(int updated, List<ProjectConfig> projects)
        {
            Updated = updated;
            Projects = projects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymplifyConfig"/> class serialized in the given JSON.
        /// </summary>
        public SymplifyConfig(string json)
        {
            try
            {
                char[] charsToTrim = { '\xEF', ' ', '\xBF', '\xBB' };
                json = json.Trim(charsToTrim);

                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.FloatParseHandling = FloatParseHandling.Decimal;
                JsonTextReader reader = new JsonTextReader(new StringReader(json));
                SymplifyConfig config = serializer.Deserialize<SymplifyConfig>(reader);
                
                Updated = config.Updated;
                Projects = config.Projects;
                Privacy_mode = config.Privacy_mode;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid configuration JSON.", ex);
            }

            if (Updated <= 0)
            {
                throw new ArgumentException("Invalid JSON, missing 'updated' property.");
            }

            if (Projects == null)
            {
                throw new ArgumentException("Invalid JSON, missing 'projects' property.");
            }
        }

        /// <summary>
        /// Returns the project with the given name, or null if there is no match.
        /// </summary>
        public ProjectConfig FindProjectWithName(string projectName)
        {
            foreach (ProjectConfig project in this.Projects)
            {
                if (project.Name == projectName)
                {
                    return project;
                }
            }

            return null;
        }
    }
}
