using System.Text.Json;
using System.Text.Json.Serialization;

namespace SymplifySDK.Allocation.Config
{
    public class VariationConfig
    {
        public VariationConfig(long id, string name, uint weight, ProjectState state)
        {
            ID = id;
            Name = name;
            Weight = weight;
            State = state;
        }

        [JsonPropertyName("id")]
        public long ID { get; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("weight")]
        public uint Weight { get; set; }

        [JsonPropertyName("state")]
        public ProjectState State { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
