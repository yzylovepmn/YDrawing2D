using System;
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
        Geo,
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
            GLFunc.Uniform1i(GLFunc.glGetUniformLocation(ID, name), value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            GLFunc.Uniform1i(GLFunc.glGetUniformLocation(ID, name), value);
        }

        public void SetFloat(string name, float value)
        {
            GLFunc.Uniform1f(GLFunc.glGetUniformLocation(ID, name), value);
        }

        public void SetVec3(string name, float[] value)
        {
            GLFunc.Uniform3fv(GLFunc.glGetUniformLocation(ID, name), 1, value);
        }

        public void SetMat3(string name, MatrixF matrix)
        {
            GLFunc.UniformMatrix3fv(GLFunc.glGetUniformLocation(ID, name), 1, GLConst.GL_FALSE, matrix.GetData());
        }

        #region Static
        public static Shader CreateShader(IEnumerable<ShaderSource> source)
        {
            var id = GLFunc.glCreateProgram();
            foreach (var file in source)
            {
                uint shader = 0;
                switch (file.Type)
                {
                    case ShaderType.Vert:
                        shader = GLFunc.glCreateShader(GLConst.GL_VERTEX_SHADER);
                        break;
                    case ShaderType.Geo:
                        shader = GLFunc.glCreateShader(GLConst.GL_GEOMETRY_SHADER);
                        break;
                    case ShaderType.Frag:
                        shader = GLFunc.glCreateShader(GLConst.GL_FRAGMENT_SHADER);
                        break;
                }
                var code = file.Code;
                GLFunc.glShaderSource(shader, 1, new string[] { code }, null);
                GLFunc.glCompileShader(shader);
                if (!CheckCompileErrors(shader, file.Type.ToString()))
                    return null;

                GLFunc.glAttachShader(id, shader);
                GLFunc.glDeleteShader(shader);
            }
            GLFunc.glLinkProgram(id);
            if (!CheckCompileErrors(id, "PROGRAM"))
                return null;

            return new Shader(id);
        }

        private static bool CheckCompileErrors(uint shader, string type)
        {
            var success = new int[1];
            byte[] infoLog = new byte[1024];
            if (type != "PROGRAM")
            {
                GLFunc.glGetShaderiv(shader, GLConst.GL_COMPILE_STATUS, success);
                if (success[0] == 0)
                    GLFunc.glGetShaderInfoLog(shader, 1024, null, infoLog);
            }
            else
            {
                GLFunc.glGetProgramiv(shader, GLConst.GL_LINK_STATUS, success);
                if (success[0] == 0)
                    GLFunc.glGetProgramInfoLog(shader, 1024, null, infoLog);
            }
            return success[0] != 0;
        }
        #endregion

        public void Dispose()
        {
            GLFunc.DeleteShader(_id);
        }
    }
}