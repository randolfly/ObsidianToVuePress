using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianToVuePress.Shared
{
    /// <summary>
    /// Markdown文件的Yaml文件头
    /// </summary>
    public class MdYamlHead
    {
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// 文件Tag
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 文件分类，实际上对应Obsidian中文件夹层级
        /// </summary>
        public List<string> Categories { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("---\n");
            sb.Append($"date: {DateTime.ToString("yyyy-MM-dd")}\n");
            sb.Append("tag:\n");
            foreach (string tag in Tags)
            {
                sb.Append($"  - {tag}\n");
            }
            sb.Append("category:\n");
            foreach (string category in Categories)
            {
                sb.Append($"  - {category}\n");
            }
            sb.Append("---\n");
            return sb.ToString();
        }

        // 示例结构
        //---
        //date: 2019-10-10
        //tags: 
        //  - 线性代数
        //  - 测试1
        //  - 微积分
        //categories:
        //  - 从线性映射理解线性代数
        //  - test1
        //---
    }
}
