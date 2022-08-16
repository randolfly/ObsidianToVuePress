using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianToVuePress.Shared
{
    public class AppConfig
    {
        public LogConfig LogConfig { get; set; }
        public DbConfig DbConfig { get; set; }
        public FileSelectConfig FileSelectConfig { get; set; }
        public FileParseConfig FileParseConfig { get; set; }
    }

    /// <summary>
    /// App Log Service config
    /// </summary>
    public class LogConfig
    {
        public string LogFile { get; set; }
    }

    /// <summary>
    /// App Db service config
    /// </summary>
    public class DbConfig
    {
        public string DbFile { get; set; }
    }

    /// <summary>
    /// App SelectFile service config
    /// </summary>
    public class FileSelectConfig
    {
        public string VaultPath { get; set; }
        /// <summary>
        /// The Regex to choose file/folder to ignore
        /// </summary>
        public List<string> IgnoreRegexes { get; set; }
        
        public List<ModuleMap> ModuleMaps { get; set; }

        public string DestinationPath { get; set; }
    }

    /// <summary>
    /// Vault modules to be handle
    /// </summary>
    public class ModuleMap
    {
        // dir path of vault
        public string SrcPath { get; set; }
        // (mapped) dir path in blog
        public string DestPath { get; set; }
    }

    /// <summary>
    /// App ParseFile service config
    /// </summary>
    public class FileParseConfig
    {
        public bool ReplaceWikiLink { get; set; }
        public bool ReplaceAdmontion { get; set; }
        public bool AddYamlInfo { get; set; }
    }
}
