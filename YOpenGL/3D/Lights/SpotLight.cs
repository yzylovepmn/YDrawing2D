using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public class SpotLight : PointLightBase
    {
        public SpotLight() : this(Colors.White, new Point3F(), new Vector3F(0, 0, 1), 0.8f, 1)
        {
        }

        public SpotLight(Color diffuseColor, Point3F position, Vector3F direction) : this(diffuseColor, position, direction, 0.8f, 1)
        {
        }

        public SpotLight(Color diffuseColor, Point3F position, Vector3F direction, float diffuse, float specular)
        {
            Color = diffuseColor;
            Position = position;
            Direction = direction;
            Diffuse = diffuse;
            Specular = specular;

            Range = float.PositiveInfinity;
            ConstantAttenuation = 1;
            LinearAttenuation = 0.007f;
            QuadraticAttenuation = 0.0002f;

            OuterConeAngle = 90;
            InnerConeAngle = 45;
        }

        public override LightType Type { get { return LightType.Spot; } }

        public Vector3F Direction
        {
            get { return _direction; }
            set
            {
                var newValue = value;
                newValue.Normalize();
                if (_direction != newValue && !newValue.IsZero)
                {
                    _direction = newValue;
                    InvokePropertyChanged("Direction");
                }
            }
        }
        private Vector3F _direction;

        public float OuterConeAngle
        {
            get { return _outerConeAngle; }
            set
            {
                if (_outerConeAngle != value)
                {
                    _outerConeAngle = Math.Max(Math.Max(0, value), _innerConeAngle);
                    InvokePropertyChanged("OuterConeAngle");
                }
            }
        }
        private float _outerConeAngle;

        public float InnerConeAngle
        {
            get { return _innerConeAngle; }
            set
            {
                if (_innerConeAngle != value)
                {
                    _innerConeAngle = Math.Min(Math.Max(0, value), _outerConeAngle);
                    InvokePropertyChanged("InnerConeAngle");
                }
            }
        }
        private float _innerConeAngle;

        public override IEnumerable<float> GetData()
        {
            var data = new List<float>();
            data.Add(_position.X);
            data.Add(_position.Y);
            data.Add(_position.Z);
            data.Add(0);
            data.Add(_direction.X);
            data.Add(_direction.Y);
            data.Add(_direction.Z);
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
            data.Add((float)Math.Cos(MathUtil.DegreesToRadians(_innerConeAngle)));
            data.Add((float)Math.Cos(MathUtil.DegreesToRadians(_outerConeAngle)));
            data.Add(0);
            data.Add(0);
            return data;
        }
    }
}