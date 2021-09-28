using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YRenderingSystem._3D
{
    public class DirectionLight : Light
    {
        public DirectionLight() : this(Colors.White, new Vector3F(0, 0, 1))
        {

        }

        public DirectionLight(Color diffuseColor, Vector3F direction) : this(diffuseColor, direction, 0.5f, 0.5f)
        {
        }

        public DirectionLight(Color diffuseColor, Vector3F direction, float diffuse, float specular)
        {
            Color = diffuseColor;
            Direction = direction;
            Diffuse = diffuse;
            Specular = specular;
        }

        public override LightType Type { get { return LightType.Direction; } }

        public override float Ambient { get { return 0; } set { } }

        public override float Diffuse
        {
            get { return _diffuse; }
            set
            {
                if (_diffuse != value)
                {
                    _diffuse = value;
                    MathUtil.Clamp(ref _diffuse, 0, float.MaxValue);
                    InvokePropertyChanged("Diffuse");
                }
            }
        }
        private float _diffuse;

        public override float Specular
        {
            get { return _specular; }
            set
            {
                if (_specular != value)
                {
                    _specular = value;
                    MathUtil.Clamp(ref _specular, 0, float.MaxValue);
                    InvokePropertyChanged("Specular");
                }
            }
        }
        private float _specular;

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

        public override IEnumerable<float> GetData()
        {
            var data = new List<float>();
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
            return data;
        }
    }
}