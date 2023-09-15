using HG.Utility.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace HG.Engine.Types
{
    /// <summary>
    /// Keeps track of a collection time-renewable-resources such as "boost amount".
    /// </summary>
    internal class HgTimeRenewableResources
    {
        #region class:TimeRenewableResource.

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

                double addedResource = deltaMilliseconds * RebuildRatePerSecond / 1000.0;

                AvailableResource += addedResource;

                AvailableResource = AvailableResource.Box(0, MaxValue);
            }
        }

        #endregion

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
