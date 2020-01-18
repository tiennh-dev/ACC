using System.Collections.Generic;

namespace iChibaShopping.Cms.PublicApi.Configs
{
    public class RequestConfigStoreSimple<TRequestConfig>
    {
        private readonly object lockObject = new object();
        private readonly IList<TRequestConfig> requestConfigs;
        private int currentIndex = 0;

        private int TotalRequestConfig
        {
            get
            {
                return requestConfigs.Count;
            }
        }

        public RequestConfigStoreSimple(List<TRequestConfig> requestConfigs)
        {
            this.requestConfigs = requestConfigs;
        }

        public TRequestConfig GetConfig()
        {
            lock (lockObject)
            {
                if (currentIndex >= TotalRequestConfig)
                {
                    currentIndex = 0;
                }

                return requestConfigs[currentIndex++];
            }
        }
    }
}
