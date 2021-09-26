using System;
using System.Collections.Generic;
using System.Text;

namespace YGeometry.IO
{
    public interface IMeshBuilder
    {
        MeshData ConvertToMesh();
    }
}