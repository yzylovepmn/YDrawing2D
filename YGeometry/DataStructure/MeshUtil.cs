using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using YGeometry.DataStructure.HalfEdge;
using YGeometry.IO;

namespace YGeometry.DataStructure
{
    public class MeshUtil
    {
        public static HEMesh ConvertTo(MeshData data)
        {
            if (data == null) return null;
            var mesh = new HEMesh(data.Vertices.Count);
            foreach (var vertex in data.Vertices)
                mesh.AddVertex(vertex.Position);
            foreach (var face in data.Faces)
                mesh.AddFace(face.Vertices);
            mesh.UpdateFaceNormals();
            mesh.UpdateVerticeNormals();
            return mesh;
        }

        public static MeshData ConvertTo(HEMesh mesh)
        {
            var data = new MeshData();
            data.Vertices = mesh.Vertices.Select(v => new VertexData() { Position = v.Position, Normal = mesh.GetVertexNormal(v.ID) }).ToList();
            data.Faces = mesh.Faces.Select(f => new FaceData() { Vertices = mesh.GetVerticeOfFace(f.ID), Normal = mesh.GetFaceNormal(f.ID) }).ToList();
            return data;
        }
    }
}