using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianToVuePress.Shared
{
    public class ReadMeYamlHead
    {
        /// <summary>
        /// Folder Name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文件分类，实际上对应Obsidian中文件夹层级
        /// </summary>
        public List<string> Categories { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("---\n");
            sb.Append($"title: {Title}\n");
            sb.Append("index: false\nicon: creative\n");
            sb.Append("category:\n");
            foreach (string category in Categories)
            {
                sb.Append($"  - {category}\n");
            }
            sb.Append("---\n");
            return sb.ToString();
        }
        //---
        //title: testSUB
        //index: false
        //icon: creative
        //category:
        //  - 使用指南
        //---
    }
}
