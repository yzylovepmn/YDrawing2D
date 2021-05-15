using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public class SpecularMaterial : Material
    {
        public SpecularMaterial() : this(Colors.White, 64)
        {
        }

        public SpecularMaterial(Color emissiveColor, float specularPower)
        {
            Color = emissiveColor;
            SpecularPower = specularPower;
        }

        public override MaterialType Type { get { return MaterialType.Specular; } }

        public float SpecularPower
        {
            get { return _specularPower; }
            set
            {
                if (_specularPower != value)
                {
                    _specularPower = Math.Max(1, value);
                    InvokePropertyChanged("SpecularPower");
                }
            }
        }
        private float _specularPower;
    }
}