using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YOpenGL._3D
{
    public enum MaterialType
    {
        Emissive,
        Diffuse,
        Specular
    }

    public abstract class Material : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public abstract MaterialType Type { get; }

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

        internal void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}