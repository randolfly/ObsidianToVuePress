using ObsidianToVuePress.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObsidianToVuePress.Lib
{
    public static class MdLib
    {

        /// <summary>
        /// 替换文件中的不严谨的Html Tag, <center>, and <mark>
        /// </summary>
        /// <param name="fileText">文件信息</param>
        /// <returns>替换完成的文本</returns>
        public static string ReplaceHtmlTag(this string fileText)
        {
            Dictionary<string, bool> patternList = new Dictionary<string, bool>
            {
                {@"<center>(?<content>.*?)<\/center>", false },
                {@"<mark.*?>(?<content>.*?)<\/mark>", true },
            };
            string tempText = fileText;
            string finalText = "";

            foreach (var pattern in patternList)
            {
                //var matches = Regex.Matches(tempText, pattern.Key, RegexOptions.IgnoreCase);
                MatchEvaluator evaluator = new MatchEvaluator(x => ChangeHtmlTag(x, pattern.Value));
                finalText = Regex.Replace(tempText, pattern.Key, evaluator);
                tempText = finalText;
            }

            return finalText;

            static string ChangeHtmlTag(Match match, bool isHighlight)
            {
                // https://regexr.com/
                string content = match.Groups["content"].Value.Trim();
                if (isHighlight)
                {
                    content = $"=={content}==";
                }

                return content;
            }
        }


        /// <summary>
        /// 给文件中数学公式上下行增加空格，修改公式颜色设置
        /// </summary>
        /// <param name="fileText">文件信息</param>
        /// <returns>替换完成的文本</returns>
        public static string ReplaceMathText(this string fileText)
        {
            string newText = fileText.Replace("$$", "\n$$\n");

            // equivalnet bracket search
            var pattern = @"
                \{                          # the first {
                \\color\[RGB\]\{[^{}]+\}    # The func name
                (?<content>                 # the content
                    (?:                 
                    [^\{\}]                 # Match all non-braces
                    |
                    (?<open> \{ )           # Match '{', and capture into 'open'
                    |
                    (?<-open> \} )          # Match '}', and delete the 'open' capture
                    )+
                    (?(open)(?!))           # Fails if 'open' stack isn't empty!
                )
                \}                          # Last '}'
            ";
            MatchEvaluator evaluator = new MatchEvaluator(ReplaceTeXColor);
            string newFileText = Regex.Replace(newText, pattern, evaluator, RegexOptions.IgnorePatternWhitespace);
            return newFileText;

            static string ReplaceTeXColor(Match match)
            {
                string content = match.Groups["content"].Value.Trim();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(@"{\color{red}{");
                stringBuilder.Append(content);
                stringBuilder.Append("}}");
                return stringBuilder.ToString();
            }
        }


        /// <summary>
        /// 替换文件中的Wiki Link
        /// </summary>
        /// <param name="fileText">文件信息</param>
        /// <param name="filePath">文件在当前file system下路径，用于生成相对链接(使用映射后路径)</param>
        /// <param name="destVaultPath">根目录在file system下路径(映射后路径)</param>
        /// <returns>替换完成的文本</returns>
        public static string ReplaceWikiLink(this string fileText, string filePath, string destVaultPath)
        {
            // ignore http(s): link
            // replace md links first
            string patternMd = @"(?<show>!?)\[(?<name>.*)\]\((?!http)(?<content>.+?)\)";
            // var matches = Regex.Matches(fileText, patternMd, RegexOptions.IgnoreCase);
            MatchEvaluator evaluator = new MatchEvaluator(x => ChangeWikiLink(x, filePath, destVaultPath));
            string newFileText = Regex.Replace(fileText, patternMd, evaluator);


            string patternWiki = @"(?<show>!?)\[\[\s*(?<content>[^\|#\^]+?)(?<name>[\|#\^].+?)*?\s*\]\]";
            // matches = Regex.Matches(newFileText, patternWiki, RegexOptions.IgnoreCase);
            evaluator = new MatchEvaluator(x => ChangeWikiLink(x, filePath, destVaultPath));
            string finalText = Regex.Replace(newFileText, patternWiki, evaluator);

            return finalText;

            static string ChangeWikiLink(Match match, string filePath, string destVaultPath)
            {
                // https://regexr.com/
                string content = match.Groups["content"].Value.Trim();
                string name = match.Groups["name"].Value.Trim();
                // md type link do not support space , replaced with %20

                string fileName = content.Split("/").Last().Replace("%20", " ");

                if (!fileName.Contains('.'))
                {
                    fileName = $"{fileName}.md";
                }

                // get file path
                string linkFilePath = DbLib.SearchFile(fileName);
                // not found such file
                if (linkFilePath == "/")
                {
                    name = fileName + "-NOT FOUND";
                }
                else
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = fileName;
                    }

                    // get relative path
                    string linkFileFullPath = Path.Combine(destVaultPath, linkFilePath);

                    linkFilePath = Path.GetRelativePath(filePath, linkFileFullPath);

                    if (linkFilePath == ".")
                    {
                        linkFilePath = "";
                    }
                    else
                    {
                        // GetRelativePath threat the file relative to as a dir, so take from 3rd pos
                        linkFilePath = linkFilePath.Substring(3);
                    }
                    switch (name.Substring(0, 1))
                    {
                        case "#":
                            linkFilePath = $"{linkFilePath}{name.ToLower().Replace(' ', '-')}";
                            name = name.Substring(1);
                            break;
                        case "^":
                            linkFilePath = $"{linkFilePath}{name}";
                            name = name.Substring(1);
                            break;
                        case "|":
                            name = name.Substring(1);
                            break;
                        default:
                            break;
                    }
                }

                StringBuilder stringBuilder = new StringBuilder();

                // choose display type
                if (match.Groups["show"].Value.Trim() == "!")
                {
                    string extensionType = linkFilePath.Split(".").Last();
                    if (extensionType == "jpg" || extensionType == "png" || extensionType == "pdf" || extensionType == "jpeg")
                        stringBuilder.Append("!");
                }

                stringBuilder.Append($"[{name.Split(".").First()}]");
                // repalce space with "%20"
                stringBuilder.Append($"(./{linkFilePath.ReplaceSpace().Replace(@"\", "/")})");

                return stringBuilder.ToString();
            }
        }


        /// <summary>
        /// 解析文件tags
        /// </summary>
        /// <param name="fileText">文件内容</param>
        /// <returns>解析的Tags</returns>
        public static List<String> ParseTags(string fileText)
        {
            //string tagPattern = @"tags[:|：]\s*(?:#[\w|\u4e00-\u9fa5]+\s?)+";
            string tagPattern = @"tags[:|：]\s*\[(?<tag>.+)\]";

            var match = Regex.Match(fileText, tagPattern);
            string matche = match.Groups["tag"].Value.Trim();
            var tagsListRaw = matche.Replace('/', ',')
                .Replace("，", ",")
                .Split(",").ToList();
            List<String> tagsList = new List<String>();

            foreach (var tag in tagsListRaw)
            {
                string tagTrim = tag.Trim();
                if ((tagTrim != "") && (!tagsList.Contains(tagTrim)))
                {
                    tagsList.Add(tagTrim);
                }
            }

            if (tagsList.Count == 0)
            {
                tagsList.Add("default");
            }

            return tagsList;
        }

        /// <summary>
        /// 获取文件对应的Yaml信息
        /// </summary>
        /// <param name="fileText">文件内容</param>
        /// <param name="filePath">文件全局路径</param>
        /// <param name="vaultPath">Obsidian Vault路径</param>
        /// <returns>解析的Yaml头文件</returns>
        public static MdYamlHead GetMdYamlHead(string fileText, string filePath, string vaultPath)
        {
            MdYamlHead yamlHead = new MdYamlHead();

            FileInfo fileInfo = new FileInfo(filePath);
            DateTime dateTime = fileInfo.CreationTime;

            string relativePath = Path.GetRelativePath(vaultPath, filePath).Replace(@"\", "/");
            List<string> categories = relativePath.Split(@"/").ToList();

            if (categories.Count > 0)
            {
                categories.RemoveAt((categories.Count - 1));
            }

            List<string> Tags = ParseTags(fileText);

            yamlHead.Categories = categories;
            yamlHead.Tags = Tags;
            yamlHead.DateTime = dateTime;


            return yamlHead;
        }
        /// <summary>
        /// 删除原有text中的yaml
        /// </summary>
        /// <param name="fileText">需要删除的文本</param>
        /// <returns>删除后的文本</returns>
        public static string DeleteRawYaml(string fileText)
        {
            string pattern = @"---\s*[\S\s]*?---";
            Regex rgx = new Regex(pattern); 
            string newFileText = rgx.Replace(fileText, "", 1);
            
            return newFileText;
        }

        /// <summary>
        /// 添加文件头部Hexo需要的Yaml信息, 并添加一级文件名
        /// </summary>
        /// <param name="fileText">需要添加yaml的文本</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="vaultPath">vault的目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>添加yaml后的文本</returns>
        public static string AppendHexoYamlInfo(this string fileText, string filePath, string vaultPath, string fileName)
        {
            MdYamlHead MdYamlHead = GetMdYamlHead(fileText, filePath, vaultPath);
            StringBuilder stringBuilder = new StringBuilder();
            string yamlHead = MdYamlHead.ToString();
            stringBuilder.Append(yamlHead + "\n");
            stringBuilder.Append($"# {fileName}\n");
            // 删除原先可能有的yaml头部
            stringBuilder.Append(DeleteRawYaml(fileText));
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 创建README文件
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="destPath"></param>
        /// <returns></returns>
        public static string CreateReadMe(DirectoryInfo folder, string destPath)
        {
            ReadMeYamlHead readMeYamlHead = new ReadMeYamlHead();
            readMeYamlHead.Title = folder.Name;
            string relativePath = Path.GetRelativePath(destPath, folder.FullName).Replace(@"\", "/");
            List<string> categories = relativePath.Split(@"/").ToList();

            if (categories.Count > 0)
            {
                categories.RemoveAt((categories.Count - 1));
            }
            readMeYamlHead.Categories = categories;

            StringBuilder sb = new StringBuilder();
            sb.Append(readMeYamlHead);


            sb.Append("\n ## 目录\n");
            // create TOC
            var fileList = folder.GetFiles().ToList();

            foreach (var file in fileList)
            {
                sb.Append($"- [{file.Name.Split(".").First()}]({file.Name.ReplaceSpace()})\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 替换space with -
        /// </summary>
        /// <param name="text">替换文本</param>
        /// <returns></returns>
        public static string ReplaceSpace(this string text)
        {
            return text.Replace(' ', '-');
        }

        /// <summary>
        /// 替换文件中ad-xx为vue press格式
        /// </summary>
        /// <param name="fileText">替换文本</param>
        /// <returns>替换完成的文本</returns>
        public static string ReplaceAdToVue(this string fileText)
        {
            //string pattern = @"```ad-(?<head>\w+)\s*((title[:|：]\s*)(?<title>[\w\u4e00-\u9fa5]+))*(?<content>[\s\S]*?)(?<tail>```)";
            string pattern = @"```ad-(?<head>\w+)\s*((title[:|：]\s*)(?<title>[\S ]+))*(?<content>[\s\S]*?)(?<tail>```)";
            // var matches = Regex.Matches(fileText, pattern, RegexOptions.IgnoreCase);
            MatchEvaluator evaluator = new MatchEvaluator(ChangeAdTag);
            string newFileText = Regex.Replace(fileText, pattern, evaluator);
            return newFileText;

            static string ChangeAdTag(Match match)
            {
                string head = match.Groups["head"].Value.Trim();
                string title = match.Groups["title"].Value.Trim();
                string content = match.Groups["content"].Value;

                string newHead = AdmontionMap.AdmontionPairs.GetValueOrDefault(head, "note");
                if (title == "")
                {
                    title = head;
                }
                string hexoHead = string.Concat("::: ", $"{newHead} ", $"{title}\n");
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(hexoHead);
                stringBuilder.Append(content);
                stringBuilder.Append(":::\n");
                return stringBuilder.ToString();
            }

        }

    }
}
