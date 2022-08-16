using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ObsidianToVuePress.Domain
{
    public class ObsidianFileInfo
    {
        /// <summary>
        /// 文件相对Vault文件夹下相对路径，包含文件名，是主键
        /// </summary>
        [Key]
        public string SrcPath { get; set; }
        
        /// <summary>
        /// 文件在映射后文件夹下相对根目录的相对路径
        /// </summary>
        public string DestPath { get; set; }

        /// <summary>
        /// 文件计算的Sha256哈希, 如果不是md文件, Sha认为0
        /// </summary>
        public string Sha256 { get; set; }


        /// <summary>
        /// 文件名，用于搜索WIKI链接时使用
        /// </summary>
        public string FileName { get; set; }
    }
}
