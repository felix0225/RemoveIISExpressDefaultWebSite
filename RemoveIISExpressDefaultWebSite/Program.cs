using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveIISExpressDefaultWebSite
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            const string SourcePath = @"D:\OneDrive\MyTest\Visual Studio";

            var fileList = Directory.EnumerateFiles(SourcePath, "applicationhost.config", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                Console.WriteLine("file= " + file);

                //將檔案的隱藏屬性移除，不然回寫會有問題
                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                    File.SetAttributes(file, attributes);
                }


                var lines = new List<string>(File.ReadAllLines(file));

                //移除 WebSite1
                var siteIndex = lines.FindIndex(x => x.Contains("name=\"WebSite1\""));
                if (siteIndex > 0)
                {
                    Console.WriteLine("siteIndex=" + siteIndex);
                    lines.RemoveRange(siteIndex, 8);
                }

                //重設新 site 的 id
                var siteIdIndex = lines.FindIndex(x => x.Contains("id=\"2\""));
                if (siteIdIndex > 0)
                {
                    Console.WriteLine("siteIdIndex=" + siteIdIndex);
                    lines[siteIdIndex] = lines[siteIdIndex].Replace("\"2\"", "\"1\"");
                }

                //移除 Default.asp 的預設文件
                var removeIndex = lines.FindIndex(x => x.Contains("Default.asp"));
                if (removeIndex > 0)
                {
                    Console.WriteLine("removeIndex=" + removeIndex);
                    lines.RemoveAt(removeIndex);
                }

                //增加 index.aspx 的預設文件
                var index = lines.FindIndex(x => x.Contains("iisstart.htm"));
                if (index > 0)
                {
                    Console.WriteLine("index=" + index);
                    lines[index] = lines[index].Replace("iisstart.htm", "index.aspx");
                }

                File.WriteAllLines(file, lines.ToArray());
            }

            Console.WriteLine("press any key!!");
            Console.ReadKey();
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
