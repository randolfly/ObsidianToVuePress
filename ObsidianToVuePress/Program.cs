using ObsidianToVuePress.Domain;
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
    // TODO: optimize the logic to create dir, avoid duplicate create
    string dirRelativePath = Path.GetRelativePath(appConfig.FileSelectConfig.VaultPath, vaultFile.DirectoryName);
    string targetDirPath = Path.Combine(appConfig.FileSelectConfig.DestinationPath, dirRelativePath);
    Directory.CreateDirectory(targetDirPath);

    string targetFilePath = Path.Combine(targetDirPath, vaultFile.Name);
    string fileRelativePath = Path.Combine(dirRelativePath, vaultFile.Name);

    if (vaultFile.Extension == ".md")
    {
        string fileText = File.ReadAllText(vaultFile.FullName);
        string sha256Result = FileLib.ComputeSha256Hash(fileText);
        ObsidianFileInfo obsidianFileInfo = new ObsidianFileInfo { Path = fileRelativePath, Sha256 = sha256Result };
        int isUpdate = DbLib.UpdateFile(obsidianFileInfo);

        switch (isUpdate)
        {
            case -1 or 1:
                // 不存在/修改文件，转换过去
                if (File.Exists(targetFilePath))
                    File.Delete(targetFilePath);
                string newText = fileText;
                File.WriteAllText(targetFilePath, newText);
                break;
            case 0:
                Log.Information($"file: {targetFilePath} already exists, innored");
                break;
        }
    }
    else
    {
        // archive files do not change
        if (!File.Exists(targetFilePath))
        {
            File.Copy(vaultFile.FullName, targetFilePath);
            Log.Information($"copy file: {vaultFile.FullName} to {targetFilePath}");
        }
    }
}

Log.CloseAndFlush();


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