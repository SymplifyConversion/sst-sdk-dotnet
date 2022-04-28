using System.Text.Json;

namespace Allocation.Config
{
    public class VariationConfig
    {
        public int ID { get; }
        public string Name { get; set; }

        public uint Weight { get; set; }

        public VariationConfig(int id, string name, uint weight)
        {
            ID = id;
            Name = name;
            Weight = weight;
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
