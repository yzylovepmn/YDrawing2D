using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;

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
            UseProgram(_id);
        }

        public void SetBool(string name, bool value)
        {
            Uniform1i(GetUniformLocation(ID, name), value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            Uniform1i(GetUniformLocation(ID, name), value);
        }

        public void SetFloat(string name, float value)
        {
            Uniform1f(GetUniformLocation(ID, name), value);
        }

        public void SetVec2(string name, int count, float[] value)
        {
            Uniform2fv(GetUniformLocation(ID, name), count, value);
        }

        public void SetVec3(string name, int count, float[] value)
        {
            Uniform3fv(GetUniformLocation(ID, name), count, value);
        }

        public void SetVec4(string name, int count, float[] value)
        {
            Uniform4fv(GetUniformLocation(ID, name), count, value);
        }

        public void SetMat3(string name, MatrixF[] matrices)
        {
            var data = new List<float>();
            foreach (var matrix in matrices)
                data.AddRange(matrix.GetData());
            UniformMatrix3fv(GetUniformLocation(ID, name), matrices.Length, GL_FALSE, data.ToArray());
        }

        #region Static
        public static Shader GenShader(IEnumerable<ShaderSource> source)
        {
            var id = CreateProgram();
            foreach (var file in source)
            {
                uint shader = 0;
                switch (file.Type)
                {
                    case ShaderType.Vert:
                        shader = CreateShader(GL_VERTEX_SHADER);
                        break;
                    case ShaderType.Geom:
                        shader = CreateShader(GL_GEOMETRY_SHADER);
                        break;
                    case ShaderType.Frag:
                        shader = CreateShader(GL_FRAGMENT_SHADER);
                        break;
                }
                var code = file.Code;
                GLFunc.ShaderSource(shader, 1, new string[] { code }, null);
                CompileShader(shader);
                if (!CheckCompileErrors(shader, file.Type.ToString()))
                    return null;

                AttachShader(id, shader);
                DeleteShader(shader);
            }
            LinkProgram(id);
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
                GetShaderiv(id, GL_COMPILE_STATUS, success);
                if (success[0] == 0)
                    GetShaderInfoLog(id, 1024, null, infoLog);
            }
            else
            {
                GetProgramiv(id, GL_LINK_STATUS, success);
                if (success[0] == 0)
                    GetProgramInfoLog(id, 1024, null, infoLog);
            }
#if DEBUG
            var msg = Encoding.ASCII.GetString(infoLog);
#endif
            return success[0] != 0;
        }
        #endregion

        public void Dispose()
        {
            DeleteProgram(_id);
        }
    }
}