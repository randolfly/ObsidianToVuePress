using ObsidianToVuePress.Lib;
using ObsidianToVuePress.Shared;
using Serilog;
using System.Text.RegularExpressions;

ConfigLogger();

AppConfig appConfig = FileLib.ReadAppConfig();

// load and parse file

List<FileInfo>?vaultDictories = FileLib.SelectVaultFiles(appConfig.FileSelectConfig.VaultPath,
    appConfig.FileSelectConfig.IgnoreRegexes);

foreach (FileInfo vaultFile in vaultDictories)
{
    Log.Debug(vaultFile.Name);
}


static void ConfigLogger()
{
    //Log.Logger = new LoggerConfiguration()
    //.MinimumLevel.Information()
    //.WriteTo.Console()
    //.WriteTo.File(ObsidianSystemInfo.LogPath,
    //    rollingInterval: RollingInterval.Day,
    //    rollOnFileSizeLimit: true)
    //.CreateLogger();
    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    //.WriteTo.File(ObsidianSystemInfo.LogPath)
    .CreateLogger();
}