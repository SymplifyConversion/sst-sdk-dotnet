using System;
using System.Collections.Generic;
using Allocation.Config;
using Allocation.Exceptions;

namespace Allocation
{
    public static class Allocation
    {
        public static VariationConfig FindVariationForVisitor(ProjectConfig project, string visitorId)
        {
            int allocation = GetAllocation(project, visitorId);

            return LookupVariationAt(project, allocation);
        }

        private static int GetAllocation(ProjectConfig project, string visitorId)
        {
            string hashKey = string.Format("{0}:{1}", visitorId, project.ID);
            uint totalWeight = 0;

            foreach (VariationConfig variation in project.Variations)
            {
                totalWeight += variation.Weight;
            }

            return CustomHash.CustomHash.HashInWindow(hashKey, totalWeight);
        }

        private static VariationConfig LookupVariationAt(ProjectConfig project, int allocation)
        {
            uint totalWeight = 0;
            List<(uint weight, int id)> variationThresholds = new();

            foreach (VariationConfig variationConfig in project.Variations)
            {
                totalWeight += variationConfig.Weight;
                variationThresholds.Add((totalWeight, variationConfig.ID));
            }

            VariationConfig allocatedVariation = null;
            try
            {
                foreach ((uint weight, int id) in variationThresholds)
                {
                    uint threshold = weight;
                    int variationID = id;

                    if (allocation <= threshold)
                    {
                        allocatedVariation = project.FindVariationWithId(variationID);
                        break;
                    }
                }
            }
            catch (AllocationException.NotFoundException)
            {
                throw new Exception("[SSTSDK] cannot allocate variation with $allocation in $totalWeight");
            }

            return allocatedVariation;
        }
    }
}
