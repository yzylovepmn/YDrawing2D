using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YRenderingSystem._3D
{
    public class DiffuseMaterial : Material
    {
        public DiffuseMaterial() : this(Colors.White)
        {
        }

        public DiffuseMaterial(Color emissiveColor)
        {
            Color = emissiveColor;
        }

        public override MaterialType Type { get { return MaterialType.Diffuse; } }
    }
}