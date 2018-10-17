using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    internal static class Resources
    {
        static readonly string Prefix = typeof(Resources).FullName + ".";

        public static Stream OpenStream(string name)
        {
            var stream = typeof(Resources).Assembly.GetManifestResourceStream(Prefix + name);
            if (stream == null)
                throw new FileNotFoundException("The resource file '" + name + "' was not found!");
            return stream;
        }
    }
}