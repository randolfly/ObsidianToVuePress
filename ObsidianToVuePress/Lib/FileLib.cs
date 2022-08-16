using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ObsidianToVuePress.Shared;
using Serilog;
using ObsidianToVuePress.Domain;
using System.Text.RegularExpressions;

namespace ObsidianToVuePress.Lib
{
    public class FileLib
    {
        /// <summary>
        /// 计算文件的HASH值(SHA256)
        /// </summary>
        /// <param name="rawData">文本数据</param>
        /// <returns>SHA256数据</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        /// <summary>
        /// 读取config.yaml配置的运行参数
        /// </summary>
        /// <returns></returns>
        public static AppConfig ReadAppConfig()
        {
            string configYml = System.IO.File.ReadAllText("config.yaml");
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            //yaml contains a string containing your YAML
            AppConfig appConfig = deserializer.Deserialize<AppConfig>(configYml);
            Log.Information("Load App Config Successed!!");
            return appConfig;
        }

        /// <summary>
        /// 读取并返回被Ignore后的所有文件/目录完整路径
        /// </summary>
        /// <param name="vaultPath">vault目录</param>
        /// <param name="ignoreRegexes">用于Parse文件的regex</param>
        /// <returns>文件路径列表</returns>
        public static List<FileInfo> SelectVaultFiles(string vaultPath, List<string> ignoreRegexes)
        {
            DirectoryInfo vaultRootDir = Directory.CreateDirectory(vaultPath);

            #region M1: 手动剪枝，未优化速度还不如直接恩搜
            //// 1. 剪枝: 首先删除所有符合条件的文件夹
            //List<DirectoryInfo>? vaultDictories = vaultRootDir.GetDirectories("*.*", System.IO.SearchOption.AllDirectories).ToList();

            //Log.Debug($"All Dir Num: {vaultDictories.Count}");


            //foreach (string regex in ignoreRegexes)
            //{
            //    vaultDictories.RemoveAll((DirectoryInfo dirInfo) =>
            //    {
            //        string dirRelativePath = Path.GetRelativePath(vaultRootDir.FullName, dirInfo.FullName);
            //        string match = Regex.Match(dirRelativePath, regex).Value;
            //        // delelte ignore Paths
            //        if (match == "")
            //            return false;
            //        return true;
            //    });
            //}

            //List<FileInfo>? fileLists = new List<FileInfo>();

            //// 2. 针对剩下的文件夹分析
            //foreach(string regex in ignoreRegexes)
            //{
            //    foreach (DirectoryInfo dir in vaultDictories)
            //    {
            //        foreach (var fileInfo in dir.GetFiles("*.*"))
            //        {
            //            string dirRelativePath = Path.GetRelativePath(vaultRootDir.FullName, fileInfo.FullName);
            //            string match = Regex.Match(dirRelativePath, regex).Value;
            //            // delelte ignore Paths
            //            if (match == "")
            //                fileLists.Add(fileInfo);
            //        }
            //    }

            //}

            //return fileLists;
            #endregion

            #region APPLIED-M2: 直接恩搜，循环少但理论上不够好
            List<FileInfo>? vaultFiles = vaultRootDir.GetFiles("*.*", System.IO.SearchOption.AllDirectories).ToList();
  
            Log.Debug($"All File Num: {vaultFiles.Count}");

            // config regex builder

            foreach (string regex in ignoreRegexes)
            {
                vaultFiles.RemoveAll((FileInfo fileInfo) =>
                {
                    string relativePath = Path.GetRelativePath(vaultRootDir.FullName, fileInfo.FullName);
                    string match = Regex.Match(relativePath, regex).Value;
                    // delelte ignore Paths
                    if (match == "")
                        return false;
                    return true;
                });
            }

            return vaultFiles;

            #endregion
        }

    }
}
