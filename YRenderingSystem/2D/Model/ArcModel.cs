﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YRenderingSystem.GLFunc;
using static YRenderingSystem.GLConst;

namespace YRenderingSystem
{
    public class ArcModel : MeshModel
    {
        internal ArcModel() { }

        protected override void _BindData()
        {
            var vertice = GenVertice();
            BindVertexArray(_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _vbo[0]);
            BufferData(GL_ARRAY_BUFFER, (_pointCount * 2 + _primitives.Count * 3) * sizeof(float), default(float[]), GL_STATIC_DRAW);
            BufferSubData(GL_ARRAY_BUFFER, 0, _pointCount * 2 * sizeof(float), vertice);
            BufferSubData(GL_ARRAY_BUFFER, _pointCount * 2 * sizeof(float), _primitives.Count * 3 * sizeof(float), GenAttribute());
            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), 0);
            EnableVertexAttribArray(1);
            VertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), _pointCount * 2 * sizeof(float));
        }

        private float[] GenAttribute()
        {
            var attributes = new List<float>();

            foreach (var pair in _primitives)
            {
                var arc = (_Arc)pair.Key;
                attributes.Add(arc.Radius);
                if (arc.IsCicle)
                {
                    // The startRadian and endRadian of cicle is zero!
                    attributes.Add(0);
                    attributes.Add(0);
                }
                else
                {
                    attributes.Add(arc.StartRadian);
                    attributes.Add(arc.EndRadian);
                }
            }

            return attributes.ToArray();
        }

        internal override void Draw(Shader shader)
        {
            if (!_hasInit) return;
            BindVertexArray(_vao[0]);
            DrawArrays(GL_POINTS, 0, _pointCount);
        }
    }
}