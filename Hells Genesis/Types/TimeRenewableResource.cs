using HG.Utility.ExtensionMethods;
using System;

namespace HG.Types
{
    /// <summary>
    /// Keeps track of time-renewable-resources such as "boost amount".
    /// </summary>
    internal class TimeRenewableResource
    {
        public double RebuildRatePerSecond { get; set; }
        public double AvailableResource { get; private set; }
        public double MaxValue { get; set; }
        public DateTime LastBuildTime { get; private set; }

        /// <summary>
        /// Initializes the renewable resources.
        /// </summary>
        /// <param name="maxValue">The maximum value the resource can build to.</param>
        /// <param name="startingValue">The starting value of the resource.</param>
        /// <param name="RebuildRatePerSecond">The amount of resource to add per-second.</param>
        public TimeRenewableResource(double maxValue, double startingValue, double rebuildRatePerSecond)
        {
            MaxValue = maxValue;
            AvailableResource = startingValue;
            RebuildRatePerSecond = rebuildRatePerSecond;
            LastBuildTime = DateTime.UtcNow;
        }


        /// <summary>
        /// Consumes a given amount of resource.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public double Consume(double amount)
        {
            if (AvailableResource > amount)
            {
                //Take the requested amount of given resource.
                AvailableResource -= amount;
                return amount;
            }
            else
            {
                //Deplete the remaining amount.
                amount = AvailableResource;
                AvailableResource = 0;
                return amount;
            }
        }

        /// <summary>
        /// Accumulates new resources given the resources newable rate.
        /// </summary>
        public void RenewResource()
        {
            double deltaMilliseconds = (DateTime.UtcNow - LastBuildTime).TotalMilliseconds;
            LastBuildTime = DateTime.UtcNow;

            double addedResource = (deltaMilliseconds * RebuildRatePerSecond) / 1000.0;

            AvailableResource += addedResource;

            AvailableResource = AvailableResource.Box(0, MaxValue);
        }
    }
}
