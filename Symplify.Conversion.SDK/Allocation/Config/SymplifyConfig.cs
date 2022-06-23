using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Symplify.Conversion.SDK.Allocation.Config
{
    /// <summary>
    /// SymplifyConfig defines the configuration of all tests for a website. It is a reflection of the JSON configuration file the SDK receives from the backend.
    /// </summary>
    public class SymplifyConfig
    {
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
                SymplifyConfig config = JsonSerializer.Deserialize<SymplifyConfig>(json.Trim(charsToTrim));

                Updated = config.Updated;
                Projects = config.Projects;
                PrivacyMode = config.PrivacyMode;
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
        /// Gets or sets the updated timestamp.
        /// </summary>
        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        /// <summary>
        /// Gets or sets the site privacy mode.
        /// </summary>
        [JsonPropertyName("privacy_mode")]
        public uint PrivacyMode { get; set; }

        /// <summary>
        /// Gets or sets the current projects.
        /// </summary>
        [JsonPropertyName("projects")]
        public List<ProjectConfig> Projects { get; set; }

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
