using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YOpenGL
{
    public static class Helper
    {
        static Regex funcRegex;
        static Regex initRegex;
        static Helper()
        {
            funcRegex = new Regex(@"^(\w+) (\w+) \((\w+) (\w+)\) (\(.+\));$");
            initRegex = new Regex(@"^\w+ = \((\w+)\) get_proc\(.(\w+).\);$");
        }

        public static void GenGLConst()
        {
            var lines = File.ReadAllLines(@"Y:\Files\Lib\OGLPG-9th-Edition\OGLPG-9th-Edition\include\GL3\gl3.h");
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.StartsWith("#define"))
                {
                    var eles = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (eles.Length == 3)
                        sb.AppendLine(string.Format("public const uint {0} = {1};", eles[1], eles[2]));
                }
                if (line.StartsWith("typedef"))
                {
                    var ret = funcRegex.Match(line);
                    if (ret.Success)
                    {
                        sb.AppendLine(string.Format("public delegate {0} {1}{2};", ret.Groups[2], ret.Groups[4], ret.Groups[5]));
                    }
                }
            }
            using (var stream = new StreamWriter(File.Create(@"Y:\const.txt")))
                stream.Write(sb.ToString());
        }

        public static void GenGLFunc()
        {
            var lines = File.ReadAllLines(@"Y:\Files\Practice\OpenGL\OpenGL_Practice\OpenGL_Practice\src\gl3w.c");
            var sb_delegate = new StringBuilder();
            var sb_init = new StringBuilder();
            for (int i = 831; i <= 1530; i++)
            {
                var line = lines[i].Trim();
                var ret = initRegex.Match(line);
                if (ret.Success)
                {
                    sb_delegate.AppendLine(string.Format("public static {0} {1};", ret.Groups[1], ret.Groups[2]));
                    sb_init.AppendLine(string.Format("{0} = GetDelegate(\"{0}\", typeof({1})) as {1};", ret.Groups[2], ret.Groups[1]));
                }
            }
            using (var stream = new StreamWriter(File.Create(@"Y:\delegate.txt")))
                stream.Write(sb_delegate.ToString());
            using (var stream = new StreamWriter(File.Create(@"Y:\init.txt")))
                stream.Write(sb_init.ToString());
        }
    }
}