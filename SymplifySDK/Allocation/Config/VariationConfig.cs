﻿using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    /// <summary>
    /// VariationConfig defines the parameters for one variation within a test project.
    /// </summary>
    public class VariationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariationConfig"/> class.
        /// </summary>
        public VariationConfig(long id, string name, uint weight, ProjectState state)
        {
            ID = id;
            Name = name;
            Weight = weight;
            State = state;
        }

        /// <summary>
        /// Gets or sets the variation ID.
        /// </summary>
        [JsonPropertyName("id")]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the variation name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the allocation weight of the variation.
        /// </summary>
        [JsonPropertyName("weight")]
        public uint Weight { get; set; }

        /// <summary>
        /// Gets or sets the configured state of the variation.
        /// </summary>
        [JsonPropertyName("state")]
        public ProjectState State { get; set; }
    }
}
