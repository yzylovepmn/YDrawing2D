using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YRenderingSystem._3D
{
    public class AmbientLight : Light
    {
        public AmbientLight() : this(Colors.White)
        {

        }

        public AmbientLight(Color ambientColor) : this(ambientColor, 0.1f)
        {

        }

        public AmbientLight(Color ambientColor, float ambient)
        {
            Color = ambientColor;
            Ambient = ambient;
        }

        public override LightType Type { get { return LightType.Ambient; } }

        public override float Ambient
        {
            get { return _ambient; }
            set
            {
                if (_ambient != value)
                {
                    _ambient = value;
                    MathUtil.Clamp(ref _ambient, 0, float.MaxValue);
                    InvokePropertyChanged("Ambient");
                }
            }
        }
        private float _ambient;

        public override float Diffuse { get { return 0; } set { } }
        public override float Specular { get { return 0; } set { } }

        public override IEnumerable<float> GetData()
        {
            var data = new List<float>();
            data.Add(_color.ScR * _ambient);
            data.Add(_color.ScG * _ambient);
            data.Add(_color.ScB * _ambient);
            data.Add(_color.ScA);
            return data;
        }
    }
}