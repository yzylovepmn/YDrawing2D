using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public class PointLight : PointLightBase
    {
        public PointLight() : this(Colors.White, new Point3F())
        {
        }

        public PointLight(Color diffuseColor, Point3F position) : this(diffuseColor, position, 0.8f, 1)
        {
        }

        public PointLight(Color diffuseColor, Point3F position, float diffuse, float specular)
        {
            Color = diffuseColor;
            Position = position;
            Diffuse = diffuse;
            Specular = specular;

            Range = float.PositiveInfinity;
            ConstantAttenuation = 1;
            LinearAttenuation = 0.007f;
            QuadraticAttenuation = 0.0002f;
        }

        public override LightType Type { get { return LightType.Point; } }

        public override IEnumerable<float> GetData()
        {
            var data = new List<float>();
            data.Add(_position.X);
            data.Add(_position.Y);
            data.Add(_position.Z);
            data.Add(0);
            var color = _color.GetData();
            data.Add(color[0] * _diffuse);
            data.Add(color[1] * _diffuse);
            data.Add(color[2] * _diffuse);
            data.Add(color[3]);
            data.Add(color[0] * _specular);
            data.Add(color[1] * _specular);
            data.Add(color[2] * _specular);
            data.Add(color[3]);
            data.Add(_constantAttenuation);
            data.Add(_linearAttenuation);
            data.Add(_quadraticAttenuation);
            data.Add(_range);
            return data;
        }
    }
}