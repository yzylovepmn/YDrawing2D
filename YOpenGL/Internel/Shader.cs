﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public enum ShaderType
    {
        Vert,
        Geom,
        Frag
    }

    public struct ShaderSource
    {
        public ShaderSource(ShaderType type, string code)
        {
            _type = type;
            _code = code;
        }

        public ShaderType Type { get { return _type; } }
        private ShaderType _type;

        public string Code { get { return _code; } }
        private string _code;
    }

    public class Shader : IDisposable
    {
        public Shader(uint id)
        {
            _id = id;
        }

        public uint ID { get { return _id; } }
        private uint _id;

        public void Use()
        {
            GLFunc.UseProgram(_id);
        }

        public void SetBool(string name, bool value)
        {
            GLFunc.Uniform1i(GLFunc.GetUniformLocation(ID, name), value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            GLFunc.Uniform1i(GLFunc.GetUniformLocation(ID, name), value);
        }

        public void SetFloat(string name, float value)
        {
            GLFunc.Uniform1f(GLFunc.GetUniformLocation(ID, name), value);
        }

        public void SetVec2(string name, int count, float[] value)
        {
            GLFunc.Uniform2fv(GLFunc.GetUniformLocation(ID, name), count, value);
        }

        public void SetVec3(string name, int count, float[] value)
        {
            GLFunc.Uniform3fv(GLFunc.GetUniformLocation(ID, name), count, value);
        }

        public void SetVec4(string name, int count, float[] value)
        {
            GLFunc.Uniform4fv(GLFunc.GetUniformLocation(ID, name), count, value);
        }

        public void SetMat3(string name, MatrixF[] matrices)
        {
            var data = new List<float>();
            foreach (var matrix in matrices)
                data.AddRange(matrix.GetData());
            GLFunc.UniformMatrix3fv(GLFunc.GetUniformLocation(ID, name), matrices.Length, GLConst.GL_FALSE, data.ToArray());
        }

        #region Static
        public static Shader CreateShader(IEnumerable<ShaderSource> source)
        {
            var id = GLFunc.CreateProgram();
            foreach (var file in source)
            {
                uint shader = 0;
                switch (file.Type)
                {
                    case ShaderType.Vert:
                        shader = GLFunc.CreateShader(GLConst.GL_VERTEX_SHADER);
                        break;
                    case ShaderType.Geom:
                        shader = GLFunc.CreateShader(GLConst.GL_GEOMETRY_SHADER);
                        break;
                    case ShaderType.Frag:
                        shader = GLFunc.CreateShader(GLConst.GL_FRAGMENT_SHADER);
                        break;
                }
                var code = file.Code;
                GLFunc.ShaderSource(shader, 1, new string[] { code }, null);
                GLFunc.CompileShader(shader);
                if (!CheckCompileErrors(shader, file.Type.ToString()))
                    return null;

                GLFunc.AttachShader(id, shader);
                GLFunc.DeleteShader(shader);
            }
            GLFunc.LinkProgram(id);
            if (!CheckCompileErrors(id, "PROGRAM"))
                return null;

            return new Shader(id);
        }

        private static bool CheckCompileErrors(uint id, string type)
        {
            var success = new int[1];
            byte[] infoLog = new byte[1024];
            if (type != "PROGRAM")
            {
                GLFunc.GetShaderiv(id, GLConst.GL_COMPILE_STATUS, success);
                if (success[0] == 0)
                    GLFunc.GetShaderInfoLog(id, 1024, null, infoLog);
            }
            else
            {
                GLFunc.GetProgramiv(id, GLConst.GL_LINK_STATUS, success);
                if (success[0] == 0)
                    GLFunc.GetProgramInfoLog(id, 1024, null, infoLog);
            }
            var msg = Encoding.ASCII.GetString(infoLog);
            return success[0] != 0;
        }
        #endregion

        public void Dispose()
        {
            GLFunc.DeleteProgram(_id);
        }
    }
}