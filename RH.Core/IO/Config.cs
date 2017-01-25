using System;
using System.Collections.Generic;
using System.IO;

namespace RH.Core.IO
{
    /// <summary> Default cfg file for program configs </summary>
    public static class UserConfig
    {
        /// <summary> Directory, where configuration files storage. </summary>
        public static string AppDataDir { get; }

        static UserConfig()
        {
            AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Abalone", ProgramCore.ProgramFolderCaption);
        }

        /// <summary> List of loaded configs (not necessary each time reload the same file) </summary>
        private static readonly Dictionary<string, CfgFile> configs = new Dictionary<string, CfgFile>(StringComparer.CurrentCultureIgnoreCase);
        /// <summary> Get config file by name </summary>
        /// <param name="configName">Config name</param>
        /// <returns>Config file</returns>
        public static CfgFile ByName(string configName)
        {
            configName = configName.ToLower();
            if (Path.GetExtension(configName) != ".cfg")
                configName += ".cfg";
            if (!configs.ContainsKey(configName))
                configs.Add(configName, new CfgFile(Path.Combine(AppDataDir, configName)));
            return configs[configName];
        }
    }
}
