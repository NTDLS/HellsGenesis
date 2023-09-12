using System.Collections.Generic;

namespace HG.Types
{
    /// <summary>
    /// Keeps track of a collection time-renewable-resources such as "boost amount".
    /// </summary>
    internal class TimeRenewableResources
    {
        public Dictionary<string, TimeRenewableResource> Resources { get; set; } = new();

        public void CreateResource(string key, TimeRenewableResource renewableResource)
        {
            key = key.ToLower();
            Resources.Add(key, renewableResource);
        }

        /// <summary>
        /// Creates a new renwewable resources and adds it to the resource collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="maxValue"></param>
        /// <param name="startingValue"></param>
        /// <param name="rebuildRatePerSecond"></param>
        public void CreateResource(string key, double maxValue, double startingValue, double rebuildRatePerSecond)
        {
            key = key.ToLower();
            Resources.Add(key, new TimeRenewableResource(maxValue, startingValue, rebuildRatePerSecond));
        }

        /// <summary>
        /// Consumes a given amount of resource.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public double Consume(string key, double amount)
        {
            key = key.ToLower();
            return Resources[key].Consume(amount);
        }

        /// <summary>
        /// Gets the value of a resource but does not consume it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double Observe(string key)
        {
            key = key.ToLower();
            return Resources[key].AvailableResource;
        }

        /// <summary>
        /// Accumulates new resources given each resources newable rate.
        /// </summary>
        public void RenewAllResources()
        {
            foreach (var item in Resources)
            {
                item.Value.RenewResource();
            }
        }

    }
}
