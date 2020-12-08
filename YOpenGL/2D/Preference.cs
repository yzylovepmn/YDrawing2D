using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public class Preference : IDisposable
    {
        internal Preference(GLPanel target)
        {
            _target = target;

            _tolerance = 0.2f;
            _aliased = true;
        }

        public GLPanel Target { get { return _target; } }
        private GLPanel _target;

        /// <summary>
        /// The samller the value, the higher the quality of the resulting graphics, but the rendering speed will be slower
        /// Range : [0.005, 10]
        /// Default value : 0.2
        /// </summary>
        public float Tolerance
        {
            get { return _tolerance; }
            set
            {
                if (_tolerance != value)
                {
                    if (value > 10 || value < 0.005f)
                        return;
                    _tolerance = value;
                    _target.UpdateAll();
                }
            }
        }
        private float _tolerance;

        public bool Aliased
        {
            get { return _aliased; }
            set
            {
                if (_aliased != value)
                {
                    _aliased = value;
                    if (_aliased)
                        _target.EnableAliased();
                    else _target.DisableAliased();
                }
            }
        }
        private bool _aliased;

        public void Dispose()
        {
            _target = null;
        }
    }
}