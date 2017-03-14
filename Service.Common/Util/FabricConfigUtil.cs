using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Util
{
    public static class FabricConfigUtil
    {
        private const string DefaultConfigName = "Config";

        public static string GetConfigValue(string sectionName, string key, string configName = null)
        {
            if (string.IsNullOrEmpty(configName))
            {
                configName = DefaultConfigName;
            }
            using (var activationContext = FabricRuntime.GetActivationContext())
            {
                ConfigurationPackage config = activationContext.GetConfigurationPackageObject(configName);
                if (config.Settings.Sections.Contains(sectionName))
                {
                    if (config.Settings.Sections[sectionName].Parameters.Contains(key))
                    {
                        return config.Settings.Sections[sectionName].Parameters[key].Value;
                    }
                }
                return null;
            }
        }
    }
}
