using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public interface IMesh
    {
        MeshType Type { get; }

        Point3F GetPoint();
    }
}