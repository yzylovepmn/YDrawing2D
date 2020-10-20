using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public abstract class PointLightBase : Light
    {
        public sealed override float Ambient { get { return 0; } set { } }

        public sealed override float Diffuse
        {
            get { return _diffuse; }
            set
            {
                if (_diffuse != value)
                {
                    _diffuse = value;
                    MathUtil.Clamp(ref _diffuse, 0, 1);
                    InvokePropertyChanged("Diffuse");
                }
            }
        }
        protected float _diffuse;

        public sealed override float Specular
        {
            get { return _specular; }
            set
            {
                if (_specular != value)
                {
                    _specular = value;
                    MathUtil.Clamp(ref _specular, 0, 1);
                    InvokePropertyChanged("Specular");
                }
            }
        }
        protected float _specular;

        public Point3F Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    InvokePropertyChanged("Position");
                }
            }
        }
        protected Point3F _position;

        public float Range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = Math.Max(0, value);
                    InvokePropertyChanged("Range");
                }
            }
        }
        protected float _range;

        public float ConstantAttenuation
        {
            get { return _constantAttenuation; }
            set
            {
                if (_constantAttenuation != value)
                {
                    _constantAttenuation = Math.Max(0, value);
                    InvokePropertyChanged("ConstantAttenuation");
                }
            }
        }
        protected float _constantAttenuation;

        public float LinearAttenuation
        {
            get { return _linearAttenuation; }
            set
            {
                if (_linearAttenuation != value)
                {
                    _linearAttenuation = Math.Max(0, value);
                    InvokePropertyChanged("LinearAttenuation");
                }
            }
        }
        protected float _linearAttenuation;

        public float QuadraticAttenuation
        {
            get { return _quadraticAttenuation; }
            set
            {
                if (_quadraticAttenuation != value)
                {
                    _quadraticAttenuation = Math.Max(0, value);
                    InvokePropertyChanged("QuadraticAttenuation");
                }
            }
        }
        protected float _quadraticAttenuation;
    }
}