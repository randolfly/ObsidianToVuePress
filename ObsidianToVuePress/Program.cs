using ObsidianToVuePress.Domain;
using ObsidianToVuePress.Lib;
using ObsidianToVuePress.Shared;
using Serilog;
using System.Text.RegularExpressions;

ConfigLogger();


AppConfig appConfig = FileLib.ReadAppConfig();

// load and parse file into db set

// files to be update (must be *.md files)
List<UpdateFile>? updateFiles = new();

foreach (var moduleMap in appConfig.FileSelectConfig.ModuleMaps)
{
    string modulePath = Path.Combine(appConfig.FileSelectConfig.VaultPath, moduleMap.SrcPath);
    string destPath = Path.Combine(appConfig.FileSelectConfig.DestinationPath, moduleMap.DestPath);
    List<FileInfo>? moduleDictories = FileLib.SelectVaultFiles(modulePath,
        appConfig.FileSelectConfig.IgnoreRegexes);
    foreach (var file in moduleDictories)
    {
        string sha256Result = "0";
        if (file.Extension == ".md")
        {
            string fileText = File.ReadAllText(file.FullName);
            sha256Result = FileLib.ComputeSha256Hash(fileText);
        }

        string dirRelativePath = Path.GetRelativePath(modulePath, file.DirectoryName);
        // target dir path in file system
        string targetDirPath = Path.Combine(destPath, dirRelativePath);
        Directory.CreateDirectory(targetDirPath.ReplaceSpace());

        // the path relative to vault folder
        string srcFilePath = Path.GetRelativePath(appConfig.FileSelectConfig.VaultPath, file.FullName);
        // the path relative to dest folder, used to replace wiki links
        string destFilePath = "";
        if (dirRelativePath == ".")
        {
            destFilePath = Path.Combine(moduleMap.DestPath, file.Name);
        }
        else
        {
            destFilePath = Path.Combine(moduleMap.DestPath, dirRelativePath, file.Name);
        }

        ObsidianFileInfo fileInfo = new ObsidianFileInfo
        {
            SrcPath = srcFilePath,
            DestPath = destFilePath.ReplaceSpace(),
            Sha256 = sha256Result,
            FileName = file.Name,
        };

        int isUpdate = DbLib.UpdateFile(fileInfo);
        if (isUpdate == -1 || isUpdate == 1)
        {
            // the path in file system, used to move files
            string targetFilePath = Path.Combine(targetDirPath, file.Name).ReplaceSpace();
            updateFiles.Add(new UpdateFile { DestPath = targetFilePath, SrcFile = file });
        }
    }
}

// manage files to be update
foreach (var file in updateFiles)
{
    if (file.SrcFile.Extension == ".md")
    {
        if (File.Exists(file.DestPath))
            File.Delete(file.DestPath);
        string fileText = File.ReadAllText(file.SrcFile.FullName);
        string newText = fileText
            .ReplaceWikiLink(file.DestPath, appConfig.FileSelectConfig.DestinationPath)
            .ReplaceMathText()
            .ReplaceAdToVue()
            .ReplaceHtmlTag()
            .AppendHexoYamlInfo(Path.GetFullPath(file.SrcFile.FullName),
                appConfig.FileSelectConfig.VaultPath, file.SrcFile.Name.Split(".").First());

        File.WriteAllText(file.DestPath, newText);
    }
    else
    {
        // avoid file exists, thought not possible
        if (!File.Exists(file.DestPath))
        {
            File.Copy(file.SrcFile.FullName, file.DestPath);
        }
        else
        {
            Log.Error($"NOT MD FILE: {file.SrcFile.FullName} => {file.DestPath} EXISTS");
        }
    }
}


// manage folders (especially the README.md files)
foreach (var moduleMap in appConfig.FileSelectConfig.ModuleMaps)
{
    string modulePath = Path.Combine(appConfig.FileSelectConfig.VaultPath, moduleMap.SrcPath);
    string destPath = Path.Combine(appConfig.FileSelectConfig.DestinationPath, moduleMap.DestPath);

    DirectoryInfo destFolder = Directory.CreateDirectory(destPath);
    List<DirectoryInfo>? destFolders = destFolder.GetDirectories("*.*", System.IO.SearchOption.AllDirectories).ToList();

    foreach (var folder in destFolders)
    {
        // ignore assets
        if (!folder.FullName.Contains("assets"))
        {
            var fileList = folder.GetFiles().ToList();
            fileList.Find(x => string.Equals(x.Name, "README.md", StringComparison.CurrentCultureIgnoreCase))?.Delete();
            // also delete the file with same name of folder
            fileList.Find(x => string.Equals(x.Name.Split(".").First(), folder.Name, StringComparison.CurrentCultureIgnoreCase))?.Delete();

            // add new readme file with fixed contents 
            string readMeText = MdLib.CreateReadMe(folder, appConfig.FileSelectConfig.DestinationPath);
            string readMePath = Path.Combine(folder.FullName, "README.md");
            File.WriteAllText(readMePath, readMeText);
        }
    }
}



#region 废弃
//foreach (FileInfo vaultFile in vaultDictories)
//{
//    // TODO: optimize the logic to create dir, avoid duplicate create
//    string dirRelativePath = Path.GetRelativePath(appConfig.FileSelectConfig.VaultPath, vaultFile.DirectoryName);
//    string targetDirPath = Path.Combine(appConfig.FileSelectConfig.DestinationPath, dirRelativePath);
//    Directory.CreateDirectory(targetDirPath);

//    string targetFilePath = Path.Combine(targetDirPath, vaultFile.Name);
//    string fileRelativePath = Path.Combine(dirRelativePath, vaultFile.Name);

//    if (vaultFile.Extension == ".md")
//    {
//        string fileText = File.ReadAllText(vaultFile.FullName);
//        string sha256Result = FileLib.ComputeSha256Hash(fileText);
//        ObsidianFileInfo obsidianFileInfo = new ObsidianFileInfo { Path = fileRelativePath, Sha256 = sha256Result };
//        int isUpdate = DbLib.UpdateFile(obsidianFileInfo);

//        switch (isUpdate)
//        {
//            case -1 or 1:
//                // 不存在/修改文件，转换过去
//                if (File.Exists(targetFilePath))
//                    File.Delete(targetFilePath);
//                string newText = fileText;
//                File.WriteAllText(targetFilePath, newText);
//                break;
//            case 0:
//                Log.Information($"file: {targetFilePath} already exists, innored");
//                break;
//        }
//    }
//    else
//    {
//        // archive files do not change
//        if (!File.Exists(targetFilePath))
//        {
//            File.Copy(vaultFile.FullName, targetFilePath);
//            Log.Information($"copy file: {vaultFile.FullName} to {targetFilePath}");
//        }
//    }
//}
#endregion

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
