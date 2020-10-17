﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public interface IGLContext
    {
        ContextHandle Context { get; }

        void ShaderBinding(Shader shader);
    }
}