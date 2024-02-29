using Si.Library.ExtensionMethods;

namespace Si.Library.Mathematics
{
    /// <summary>
    /// Keeps track of a collection of time-renewable-resources such as "boost amount" or health.
    /// </summary>
    public class SiTimeRenewableResources
    {
        #region class:TimeRenewableResource.

        /// <summary>
        /// Keeps track of time-renewable-resources such as "boost amount".
        /// </summary>
        public class TimeRenewableResource
        {
            public float RebuildRatePerSecond { get; set; }
            public float AvailableResource { get; private set; }
            public float MaxValue { get; set; }
            public DateTime LastBuildTime { get; private set; }

            /// <summary>
            /// Initializes the renewable resources.
            /// </summary>
            /// <param name="maxValue">The maximum value the resource can build to.</param>
            /// <param name="startingValue">The starting value of the resource.</param>
            /// <param name="RebuildRatePerSecond">The amount of resource to add per-second.</param>
            public TimeRenewableResource(float maxValue, float startingValue, float rebuildRatePerSecond)
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
            public float Consume(float amount)
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
                float deltaMilliseconds = (float)(DateTime.UtcNow - LastBuildTime).TotalMilliseconds;
                LastBuildTime = DateTime.UtcNow;

                float addedResource = deltaMilliseconds * RebuildRatePerSecond / 1000.0f;

                AvailableResource += addedResource;

                AvailableResource = AvailableResource.Clamp(0, MaxValue);
            }
        }

        #endregion

        public Dictionary<string, TimeRenewableResource> Resources { get; set; } = new();

        public void Create(string key, TimeRenewableResource renewableResource)
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
        public void Create(string key, float maxValue, float startingValue, float rebuildRatePerSecond)
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
        public float Consume(string key, float amount)
        {
            key = key.ToLower();
            return Resources[key].Consume(amount);
        }

        /// <summary>
        /// Gets the value of a resource but does not consume it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float Observe(string key)
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
