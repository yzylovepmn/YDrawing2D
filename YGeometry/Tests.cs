using Microsoft.Win32;
using ObjParser;
using QuantumConcepts.Formats.StereoLithography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YGeometry.DataStructure.HalfEdge;
using YGeometry.IO;
using YGeometry.Maths;

namespace YGeometry
{
    class Tests
    {
        public static MeshData TestImport()
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "3d file|*.obj;*.stl" };
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                IMeshBuilder builder;
                if (Path.GetExtension(fileName).Contains("obj"))
                    builder = ObjDocument.Open(fileName);
                else builder = STLDocument.Open(fileName);
                var meshData = builder.ConvertToMesh();
                return meshData;
            }
            return null;
        }

        public static HEMesh TestCreate()
        {
            var points = new List<Vector3D>()
            {
                new Vector3D(0, 0, 0), new Vector3D(100, 0, 0), new Vector3D(0, 100, 0), new Vector3D(100, 100, 0),
                new Vector3D(0, 0, 100), new Vector3D(100, 0, 100), new Vector3D(0, 100, 100), new Vector3D(100, 100, 100),
            };

            var indice = new List<int>()
            {
                0, 2, 1, 1, 2, 3,
                4, 5, 7, 4, 7, 6,
                0, 4, 2, 4, 6, 2,
                0, 1, 4, 1, 5, 4,
                1, 3, 7, 1, 7, 5,
                3, 2, 7, 2, 6, 7,
            };

            var mesh = new HEMesh();
            foreach (var p in points)
                mesh.AddVertex(p);

            for (int i = 0; i < indice.Count; i += 3)
                mesh.AddFace(new IndexN<int>(indice[i], indice[i + 1], indice[i + 2]));

            mesh.UpdateFaceNormals();
            mesh.UpdateVerticeNormals();
            return mesh;
        }
    }
}