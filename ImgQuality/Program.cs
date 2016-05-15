using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Xsl;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace ImgQuality
{
    class Program
    {
        protected static int Quality;
        protected static string OutputDir;
        protected static string InputDir;
        protected static Size Size;

        /// <summary>
        /// 获取控制台中的初始化参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Init(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-q")
                {
                    string arg = args[i + 1];
                    int.TryParse(arg, out Quality);
                    if (Quality < 0 || Quality > 100)
                        Quality = 80;
                    i++;
                }
                if (args[i] == "-o")
                {
                    string arg = args[i + 1];
                    OutputDir = arg;
                    i++;
                }
                if (args[i] == "-i")
                {
                    string arg = args[i + 1];
                    InputDir = arg;
                    i++;
                }
                if (args[i] == "-s")
                {
                    Regex regex = new Regex(@"^(?<w>\d+)\*(?<h>\d+)$");
                    var match = regex.Match(args[i + 1] ?? "");
                    Size = match.Success ? new Size(int.Parse(match.Groups["w"].Value), int.Parse(match.Groups["h"].Value)) : new Size(1280, 0);
                }
            }
            if (string.IsNullOrWhiteSpace(OutputDir) || string.IsNullOrWhiteSpace(InputDir))
            {
                Console.WriteLine("输出目录与输入目录不能为空");
                return false;
            }
            if (OutputDir == InputDir)
            {
                Console.WriteLine("输出目录与输入目录不能相同");
                return false;
            }
            try
            {
                if (!Directory.Exists(OutputDir))
                {
                    Directory.CreateDirectory(OutputDir);
                }
                if (!Directory.Exists(InputDir))
                {
                    Directory.CreateDirectory(InputDir);
                }
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        static void Main(string[] args)
        {
            Regex regex = new Regex(@"(\.jpg|\.jpeg|\.png|\.bmp)$", RegexOptions.IgnoreCase);
            if (!Init(args))
                return;
            string[] allfile = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
            int count = allfile.Length;
            int i = 0;
            foreach (string file in allfile)
            {
                i++;
                if (regex.IsMatch(file))
                {
                    ImageFactory factory = new ImageFactory();
                    string newFile = file.Replace(InputDir, OutputDir);
                    string newDir = Path.GetDirectoryName(newFile) ?? "";
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                    factory.Load(file).Resize(new ResizeLayer(Size, ResizeMode.Max)).Quality(Quality).Save(newFile);
                    Console.WriteLine("{0:f5}% - {1}", (i * 100.0) / count, newFile);
                }
            }
            Console.WriteLine("图片全部压缩完毕！");
            Console.ReadKey();
        }
    }
}
