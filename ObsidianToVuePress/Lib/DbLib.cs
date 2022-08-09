using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObsidianToVuePress.Context;
using ObsidianToVuePress.Domain;
using Serilog;

namespace ObsidianToVuePress.Lib
{
    public class DbLib
    {
        /// <summary>
        /// 删除filePath对应的地址
        /// </summary>
        /// <param name="filePath">文件相对地址</param>
        public static void DeleteFile(string filePath)
        {
            using (var ObsidianDb = new ObsidianFileInfoContext())
            {
                var dbFileInfos = ObsidianDb.Files
                    .Where(f => f.Path == filePath);
                if (dbFileInfos.Count() != 0)
                {
                    var dbFileInfo = dbFileInfos.First();
                    ObsidianDb.Remove(dbFileInfo);
                    ObsidianDb.SaveChanges();
                }
                else
                {
                    Log.Debug($"{filePath} 不存在!");
                }
            }
        }
        /// <summary>
        /// 修改文件信息
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <returns>返回是否需要更新File=>更新: 1; 不更新: 0; 不存在该文件: -1</returns>
        public static int UpdateFile(ObsidianFileInfo fileInfo)
        {
            using (var ObsidianDb = new ObsidianFileInfoContext())
            {
                var dbFileInfos = ObsidianDb.Files
                    .Where(f => f.Path == fileInfo.Path);
                if (dbFileInfos.Count() != 0)
                {
                    var dbFileInfo = dbFileInfos.First();
                    if (fileInfo.Sha256 == dbFileInfo.Sha256)
                    {
                        ObsidianDb.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        dbFileInfo.Sha256 = fileInfo.Sha256;
                        ObsidianDb.SaveChanges();
                        return 1;
                    }
                }
                else
                {
                    ObsidianDb.Add(fileInfo);
                    ObsidianDb.SaveChanges();
                    Log.Debug($"{fileInfo.Path} 不存在! 添加了这个文件");
                    return -1;
                }
            }
        }
    }
}
