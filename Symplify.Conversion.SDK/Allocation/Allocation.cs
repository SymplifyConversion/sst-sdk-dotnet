using System.Collections.Generic;
using System.Globalization;

using Symplify.Conversion.SDK.Allocation.Config;

namespace Symplify.Conversion.SDK.Allocation
{
    /// <summary>
    /// Allocation houses functions used for calculating variation allocation.
    /// </summary>
    public static class Allocation
    {
        /// <summary>
        /// Finds the variation the given visitor should have in the given project.
        /// </summary>
        public static VariationConfig FindVariationForVisitor(ProjectConfig project, string visitorId)
        {
            if (visitorId == null || visitorId == string.Empty)
            {
                return null;
            }

            double allocation = GetAllocation(project, visitorId);

            var variation = LookupVariationAt(project, allocation);

            if (variation?.State != ProjectState.Active)
            {
                return null;
            }

            return variation;
        }

        private static double GetAllocation(ProjectConfig project, string visitorId)
        {
            string hashKey = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", visitorId, project.ID);
            uint totalWeight = 100;

            return CustomHash.HashInWindow(hashKey, totalWeight);
        }

        private static VariationConfig LookupVariationAt(ProjectConfig project, double allocation)
        {
            double totalWeight = 0;
            List<(double weight, long id)> variationThresholds = new List<(double weight, long id)>();

            foreach (VariationConfig variationConfig in project.Variations)
            {
                totalWeight += variationConfig.Distribution > 0
                ? variationConfig.Distribution
                : variationConfig.Weight;

                variationThresholds.Add((totalWeight, variationConfig.ID));
            }

            VariationConfig allocatedVariation = null;
            foreach ((double weight, long id) in variationThresholds)
            {
                double threshold = weight;
                long variationID = id;
                if (allocation <= threshold)
                {
                    allocatedVariation = project.FindVariationWithId(variationID);
                    break;
                }
            }

            return allocatedVariation;
        }
    }
}
