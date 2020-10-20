using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public enum LightType
    {
        Ambient,
        Direction,
        Point,
        Spot,
    }

    public abstract class Light : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public abstract LightType Type { get; }

        public Color Color
        {
            get { return _color; } 
            set 
            {
                if (_color != value)
                {
                    _color = value;
                    InvokePropertyChanged("Color");
                }
            } 
        }
        protected Color _color;

        public abstract float Ambient { get; set; }

        public abstract float Diffuse { get; set; }

        public abstract float Specular { get; set; }

        public abstract IEnumerable<float> GetData();

        internal void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}