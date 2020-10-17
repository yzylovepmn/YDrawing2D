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
        public static Stream OpenStream(string prefix, string name)
        {
            var assembly = typeof(Resources).Assembly;
            var stream = assembly.GetManifestResourceStream(prefix + name);
            if (stream == null)
                throw new FileNotFoundException("The resource file '" + name + "' was not found!");
            return stream;
        }
    }
}