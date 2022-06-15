using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    public class ProjectConfig
    {
        public ProjectConfig(long id, string name, List<VariationConfig> variations)
        {
            ID = id;
            Name = name;
            Variations = variations;
        }

        [JsonPropertyName("id")]
        public long ID { get; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state")]
        public ProjectState State { get; set; }

        [JsonPropertyName("variations")]
        public List<VariationConfig> Variations { get; set; }

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

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
