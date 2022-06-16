using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    /// <summary>
    /// ProjectConfig defines the parameters for one serverside test.
    /// </summary>
    public class ProjectConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectConfig"/> class.
        /// </summary>
        public ProjectConfig(long id, string name, List<VariationConfig> variations)
        {
            ID = id;
            Name = name;
            Variations = variations;
        }

        /// <summary>
        /// Gets or sets the ID of the project.
        /// </summary>
        [JsonPropertyName("id")]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the project state.
        /// </summary>
        [JsonPropertyName("state")]
        public ProjectState State { get; set; }

        /// <summary>
        /// Gets or sets the variations in the project.
        /// </summary>
        [JsonPropertyName("variations")]
        public List<VariationConfig> Variations { get; set; }

        /// <summary>
        /// Returns the variation with the given ID, or null if there is no match.
        /// </summary>
        public VariationConfig FindVariationWithId(long variationId)
        {
            foreach (VariationConfig variation in this.Variations)
            {
                if (variation.ID == variationId)
                {
                    return variation;
                }
            }

            return null;
        }
    }
}
