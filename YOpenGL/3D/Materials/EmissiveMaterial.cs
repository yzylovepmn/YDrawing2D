using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public class EmissiveMaterial : Material
    {
        public EmissiveMaterial() : this(Colors.White)
        {
        }

        public EmissiveMaterial(Color emissiveColor)
        {
            Color = emissiveColor;
        }

        public override MaterialType Type { get { return MaterialType.Emissive; } }
    }
}