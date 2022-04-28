using System.Collections.Generic;
using System.Text.Json;

namespace Allocation.Config
{
    public class ProjectConfig
    {
        public int ID { get; }
        public string Name { get; set; }

        public List<VariationConfig> Variations { get; set; }

        public ProjectConfig(int id, string name, List<VariationConfig> variations)
        {
            ID = id;
            Name = name;
            Variations = variations;
        }

        public VariationConfig FindVariationWithId(int variationId)
        {
            foreach (VariationConfig variation in this.Variations)
            {
                if (variation.ID == variationId)
                {
                    return variation;
                }
            }

            // TODO Add better message
            throw new AllocationException.NotFoundException("Allocation not found");
        }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}