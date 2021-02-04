using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    using GLchar = Byte;
    using GLenum = UInt32;
    using GLboolean = Byte;
    using GLbitfield = UInt32;
    using GLbyte = Byte;
    using GLshort = Int16;
    using GLint = Int32;
    using GLsizei = Int32;
    using GLubyte = SByte;
    using GLushort = UInt16;
    using GLuint = UInt32;
    using GLhalf = UInt16;
    using GLfloat = Single;
    using GLclampf = Single;
    using GLdouble = Double;
    using GLclampd = Double;
    using GLvoid = IntPtr;
    using GLsizeiptr = Int32;
    using GLintptr = Int32;
    using GLint64 = Int64;
    using GLuint64 = UInt64;
    using GLuint64EXT = UInt64;
    using GLDEBUGPROCARBP = IntPtr;
    using _cl_context = IntPtr;
    using _cl_event = IntPtr;
    using GLsync = IntPtr;

    public static class GLFunc
    {
        private static IntPtr OpenGLHandle { get; set; }
        internal const string OpenGLName = "OPENGL32.DLL";
        private static bool _isInit;

        public static float[] PointSizeRange;
        public static float[] LineWidthRange;

        static GLFunc()
        {
            _isInit = false;
        }

        /// <summary>
        /// Must be initialized before calling any function
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            if (_isInit) return true;
            _isInit = true;

            OpenGLHandle = Win32Helper.LoadLibrary(OpenGLName);
            _Init();
            _ParseVersion();
            _CalcParams();
            Win32Helper.FreeLibrary(OpenGLHandle);
            return GL.Version >= GLVersion.MinimumSupportedVersion;
        }

        public static void Dispose()
        {
            if (!_isInit) return;
            _isInit = false;

            GL.Version = new GLVersion();
            glCullFace = null;
            glFrontFace = null;
            glHint = null;
            glLineWidth = null;
            glPointSize = null;
            glPolygonMode = null;
            glScissor = null;
            glTexParameterf = null;
            glTexParameterfv = null;
            glTexParameteri = null;
            glTexParameteriv = null;
            glTexImage1D = null;
            glTexImage2D = null;
            glDrawBuffer = null;
            glClear = null;
            glClearColor = null;
            glClearStencil = null;
            glClearDepth = null;
            glStencilMask = null;
            glColorMask = null;
            glDepthMask = null;
            glDisable = null;
            glEnable = null;
            glFinish = null;
            glFlush = null;
            glBlendFunc = null;
            glLogicOp = null;
            glStencilFunc = null;
            glStencilOp = null;
            glDepthFunc = null;
            glPixelStoref = null;
            glPixelStorei = null;
            glReadBuffer = null;
            glReadPixels = null;
            glGetBooleanv = null;
            glGetDoublev = null;
            glGetError = null;
            glGetFloatv = null;
            glGetIntegerv = null;
            glGetString = null;
            glGetTexImage = null;
            glGetTexParameterfv = null;
            glGetTexParameteriv = null;
            glGetTexLevelParameterfv = null;
            glGetTexLevelParameteriv = null;
            glIsEnabled = null;
            glDepthRange = null;
            glViewport = null;
            glDrawArrays = null;
            glDrawElements = null;
            glGetPointerv = null;
            glPolygonOffset = null;
            glCopyTexImage1D = null;
            glCopyTexImage2D = null;
            glCopyTexSubImage1D = null;
            glCopyTexSubImage2D = null;
            glTexSubImage1D = null;
            glTexSubImage2D = null;
            glBindTexture = null;
            glDeleteTextures = null;
            glGenTextures = null;
            glIsTexture = null;
            glDrawRangeElements = null;
            glTexImage3D = null;
            glTexSubImage3D = null;
            glCopyTexSubImage3D = null;
            glActiveTexture = null;
            glSampleCoverage = null;
            glCompressedTexImage3D = null;
            glCompressedTexImage2D = null;
            glCompressedTexImage1D = null;
            glCompressedTexSubImage3D = null;
            glCompressedTexSubImage2D = null;
            glCompressedTexSubImage1D = null;
            glGetCompressedTexImage = null;
            glBlendFuncSeparate = null;
            glMultiDrawArrays = null;
            glMultiDrawElements = null;
            glPointParameterf = null;
            glPointParameterfv = null;
            glPointParameteri = null;
            glPointParameteriv = null;
            glBlendColor = null;
            glBlendEquation = null;
            glGenQueries = null;
            glDeleteQueries = null;
            glIsQuery = null;
            glBeginQuery = null;
            glEndQuery = null;
            glGetQueryiv = null;
            glGetQueryObjectiv = null;
            glGetQueryObjectuiv = null;
            glBindBuffer = null;
            glDeleteBuffers = null;
            glGenBuffers = null;
            glIsBuffer = null;
            glBufferData = null;
            glBufferSubData = null;
            glGetBufferSubData = null;
            glMapBuffer = null;
            glUnmapBuffer = null;
            glGetBufferParameteriv = null;
            glGetBufferPointerv = null;
            glBlendEquationSeparate = null;
            glDrawBuffers = null;
            glStencilOpSeparate = null;
            glStencilFuncSeparate = null;
            glStencilMaskSeparate = null;
            glAttachShader = null;
            glBindAttribLocation = null;
            glCompileShader = null;
            glCreateProgram = null;
            glCreateShader = null;
            glDeleteProgram = null;
            glDeleteShader = null;
            glDetachShader = null;
            glDisableVertexAttribArray = null;
            glEnableVertexAttribArray = null;
            glGetActiveAttrib = null;
            glGetActiveUniform = null;
            glGetAttachedShaders = null;
            glGetAttribLocation = null;
            glGetProgramiv = null;
            glGetProgramInfoLog = null;
            glGetShaderiv = null;
            glGetShaderInfoLog = null;
            glGetShaderSource = null;
            glGetUniformLocation = null;
            glGetUniformfv = null;
            glGetUniformiv = null;
            glGetVertexAttribdv = null;
            glGetVertexAttribfv = null;
            glGetVertexAttribiv = null;
            glGetVertexAttribPointerv = null;
            glIsProgram = null;
            glIsShader = null;
            glLinkProgram = null;
            glShaderSource = null;
            glUseProgram = null;
            glUniform1f = null;
            glUniform2f = null;
            glUniform3f = null;
            glUniform4f = null;
            glUniform1i = null;
            glUniform2i = null;
            glUniform3i = null;
            glUniform4i = null;
            glUniform1fv = null;
            glUniform2fv = null;
            glUniform3fv = null;
            glUniform4fv = null;
            glUniform1iv = null;
            glUniform2iv = null;
            glUniform3iv = null;
            glUniform4iv = null;
            glUniformMatrix2fv = null;
            glUniformMatrix3fv = null;
            glUniformMatrix4fv = null;
            glValidateProgram = null;
            glVertexAttrib1d = null;
            glVertexAttrib1dv = null;
            glVertexAttrib1f = null;
            glVertexAttrib1fv = null;
            glVertexAttrib1s = null;
            glVertexAttrib1sv = null;
            glVertexAttrib2d = null;
            glVertexAttrib2dv = null;
            glVertexAttrib2f = null;
            glVertexAttrib2fv = null;
            glVertexAttrib2s = null;
            glVertexAttrib2sv = null;
            glVertexAttrib3d = null;
            glVertexAttrib3dv = null;
            glVertexAttrib3f = null;
            glVertexAttrib3fv = null;
            glVertexAttrib3s = null;
            glVertexAttrib3sv = null;
            glVertexAttrib4Nbv = null;
            glVertexAttrib4Niv = null;
            glVertexAttrib4Nsv = null;
            glVertexAttrib4Nub = null;
            glVertexAttrib4Nubv = null;
            glVertexAttrib4Nuiv = null;
            glVertexAttrib4Nusv = null;
            glVertexAttrib4bv = null;
            glVertexAttrib4d = null;
            glVertexAttrib4dv = null;
            glVertexAttrib4f = null;
            glVertexAttrib4fv = null;
            glVertexAttrib4iv = null;
            glVertexAttrib4s = null;
            glVertexAttrib4sv = null;
            glVertexAttrib4ubv = null;
            glVertexAttrib4uiv = null;
            glVertexAttrib4usv = null;
            glVertexAttribPointer = null;
            glUniformMatrix2x3fv = null;
            glUniformMatrix3x2fv = null;
            glUniformMatrix2x4fv = null;
            glUniformMatrix4x2fv = null;
            glUniformMatrix3x4fv = null;
            glUniformMatrix4x3fv = null;
            glColorMaski = null;
            glGetBooleani_v = null;
            glGetIntegeri_v = null;
            glEnablei = null;
            glDisablei = null;
            glIsEnabledi = null;
            glBeginTransformFeedback = null;
            glEndTransformFeedback = null;
            glBindBufferRange = null;
            glBindBufferBase = null;
            glTransformFeedbackVaryings = null;
            glGetTransformFeedbackVarying = null;
            glClampColor = null;
            glBeginConditionalRender = null;
            glEndConditionalRender = null;
            glVertexAttribIPointer = null;
            glGetVertexAttribIiv = null;
            glGetVertexAttribIuiv = null;
            glVertexAttribI1i = null;
            glVertexAttribI2i = null;
            glVertexAttribI3i = null;
            glVertexAttribI4i = null;
            glVertexAttribI1ui = null;
            glVertexAttribI2ui = null;
            glVertexAttribI3ui = null;
            glVertexAttribI4ui = null;
            glVertexAttribI1iv = null;
            glVertexAttribI2iv = null;
            glVertexAttribI3iv = null;
            glVertexAttribI4iv = null;
            glVertexAttribI1uiv = null;
            glVertexAttribI2uiv = null;
            glVertexAttribI3uiv = null;
            glVertexAttribI4uiv = null;
            glVertexAttribI4bv = null;
            glVertexAttribI4sv = null;
            glVertexAttribI4ubv = null;
            glVertexAttribI4usv = null;
            glGetUniformuiv = null;
            glBindFragDataLocation = null;
            glGetFragDataLocation = null;
            glUniform1ui = null;
            glUniform2ui = null;
            glUniform3ui = null;
            glUniform4ui = null;
            glUniform1uiv = null;
            glUniform2uiv = null;
            glUniform3uiv = null;
            glUniform4uiv = null;
            glTexParameterIiv = null;
            glTexParameterIuiv = null;
            glGetTexParameterIiv = null;
            glGetTexParameterIuiv = null;
            glClearBufferiv = null;
            glClearBufferuiv = null;
            glClearBufferfv = null;
            glClearBufferfi = null;
            glGetStringi = null;
            glIsRenderbuffer = null;
            glBindRenderbuffer = null;
            glDeleteRenderbuffers = null;
            glGenRenderbuffers = null;
            glRenderbufferStorage = null;
            glGetRenderbufferParameteriv = null;
            glIsFramebuffer = null;
            glBindFramebuffer = null;
            glDeleteFramebuffers = null;
            glGenFramebuffers = null;
            glCheckFramebufferStatus = null;
            glFramebufferTexture1D = null;
            glFramebufferTexture2D = null;
            glFramebufferTexture3D = null;
            glFramebufferRenderbuffer = null;
            glGetFramebufferAttachmentParameteriv = null;
            glGenerateMipmap = null;
            glBlitFramebuffer = null;
            glRenderbufferStorageMultisample = null;
            glFramebufferTextureLayer = null;
            glMapBufferRange = null;
            glFlushMappedBufferRange = null;
            glBindVertexArray = null;
            glDeleteVertexArrays = null;
            glGenVertexArrays = null;
            glIsVertexArray = null;
            glDrawArraysInstanced = null;
            glDrawElementsInstanced = null;
            glTexBuffer = null;
            glPrimitiveRestartIndex = null;
            glCopyBufferSubData = null;
            glGetUniformIndices = null;
            glGetActiveUniformsiv = null;
            glGetActiveUniformName = null;
            glGetUniformBlockIndex = null;
            glGetActiveUniformBlockiv = null;
            glGetActiveUniformBlockName = null;
            glUniformBlockBinding = null;
            glDrawElementsBaseVertex = null;
            glDrawRangeElementsBaseVertex = null;
            glDrawElementsInstancedBaseVertex = null;
            glMultiDrawElementsBaseVertex = null;
            glProvokingVertex = null;
            glFenceSync = null;
            glIsSync = null;
            glDeleteSync = null;
            glClientWaitSync = null;
            glWaitSync = null;
            glGetInteger64v = null;
            glGetSynciv = null;
            glGetInteger64i_v = null;
            glGetBufferParameteri64v = null;
            glFramebufferTexture = null;
            glTexImage2DMultisample = null;
            glTexImage3DMultisample = null;
            glGetMultisamplefv = null;
            glSampleMaski = null;
            glBindFragDataLocationIndexed = null;
            glGetFragDataIndex = null;
            glGenSamplers = null;
            glDeleteSamplers = null;
            glIsSampler = null;
            glBindSampler = null;
            glSamplerParameteri = null;
            glSamplerParameteriv = null;
            glSamplerParameterf = null;
            glSamplerParameterfv = null;
            glSamplerParameterIiv = null;
            glSamplerParameterIuiv = null;
            glGetSamplerParameteriv = null;
            glGetSamplerParameterIiv = null;
            glGetSamplerParameterfv = null;
            glGetSamplerParameterIuiv = null;
            glQueryCounter = null;
            glGetQueryObjecti64v = null;
            glGetQueryObjectui64v = null;
            glVertexAttribDivisor = null;
            glVertexAttribP1ui = null;
            glVertexAttribP1uiv = null;
            glVertexAttribP2ui = null;
            glVertexAttribP2uiv = null;
            glVertexAttribP3ui = null;
            glVertexAttribP3uiv = null;
            glVertexAttribP4ui = null;
            glVertexAttribP4uiv = null;
            glMinSampleShading = null;
            glBlendEquationi = null;
            glBlendEquationSeparatei = null;
            glBlendFunci = null;
            glBlendFuncSeparatei = null;
            glDrawArraysIndirect = null;
            glDrawElementsIndirect = null;
            glUniform1d = null;
            glUniform2d = null;
            glUniform3d = null;
            glUniform4d = null;
            glUniform1dv = null;
            glUniform2dv = null;
            glUniform3dv = null;
            glUniform4dv = null;
            glUniformMatrix2dv = null;
            glUniformMatrix3dv = null;
            glUniformMatrix4dv = null;
            glUniformMatrix2x3dv = null;
            glUniformMatrix2x4dv = null;
            glUniformMatrix3x2dv = null;
            glUniformMatrix3x4dv = null;
            glUniformMatrix4x2dv = null;
            glUniformMatrix4x3dv = null;
            glGetUniformdv = null;
            glGetSubroutineUniformLocation = null;
            glGetSubroutineIndex = null;
            glGetActiveSubroutineUniformiv = null;
            glGetActiveSubroutineUniformName = null;
            glGetActiveSubroutineName = null;
            glUniformSubroutinesuiv = null;
            glGetUniformSubroutineuiv = null;
            glGetProgramStageiv = null;
            glPatchParameteri = null;
            glPatchParameterfv = null;
            glBindTransformFeedback = null;
            glDeleteTransformFeedbacks = null;
            glGenTransformFeedbacks = null;
            glIsTransformFeedback = null;
            glPauseTransformFeedback = null;
            glResumeTransformFeedback = null;
            glDrawTransformFeedback = null;
            glDrawTransformFeedbackStream = null;
            glBeginQueryIndexed = null;
            glEndQueryIndexed = null;
            glGetQueryIndexediv = null;
            glReleaseShaderCompiler = null;
            glShaderBinary = null;
            glGetShaderPrecisionFormat = null;
            glDepthRangef = null;
            glClearDepthf = null;
            glGetProgramBinary = null;
            glProgramBinary = null;
            glProgramParameteri = null;
            glUseProgramStages = null;
            glActiveShaderProgram = null;
            glCreateShaderProgramv = null;
            glBindProgramPipeline = null;
            glDeleteProgramPipelines = null;
            glGenProgramPipelines = null;
            glIsProgramPipeline = null;
            glGetProgramPipelineiv = null;
            glProgramUniform1i = null;
            glProgramUniform1iv = null;
            glProgramUniform1f = null;
            glProgramUniform1fv = null;
            glProgramUniform1d = null;
            glProgramUniform1dv = null;
            glProgramUniform1ui = null;
            glProgramUniform1uiv = null;
            glProgramUniform2i = null;
            glProgramUniform2iv = null;
            glProgramUniform2f = null;
            glProgramUniform2fv = null;
            glProgramUniform2d = null;
            glProgramUniform2dv = null;
            glProgramUniform2ui = null;
            glProgramUniform2uiv = null;
            glProgramUniform3i = null;
            glProgramUniform3iv = null;
            glProgramUniform3f = null;
            glProgramUniform3fv = null;
            glProgramUniform3d = null;
            glProgramUniform3dv = null;
            glProgramUniform3ui = null;
            glProgramUniform3uiv = null;
            glProgramUniform4i = null;
            glProgramUniform4iv = null;
            glProgramUniform4f = null;
            glProgramUniform4fv = null;
            glProgramUniform4d = null;
            glProgramUniform4dv = null;
            glProgramUniform4ui = null;
            glProgramUniform4uiv = null;
            glProgramUniformMatrix2fv = null;
            glProgramUniformMatrix3fv = null;
            glProgramUniformMatrix4fv = null;
            glProgramUniformMatrix2dv = null;
            glProgramUniformMatrix3dv = null;
            glProgramUniformMatrix4dv = null;
            glProgramUniformMatrix2x3fv = null;
            glProgramUniformMatrix3x2fv = null;
            glProgramUniformMatrix2x4fv = null;
            glProgramUniformMatrix4x2fv = null;
            glProgramUniformMatrix3x4fv = null;
            glProgramUniformMatrix4x3fv = null;
            glProgramUniformMatrix2x3dv = null;
            glProgramUniformMatrix3x2dv = null;
            glProgramUniformMatrix2x4dv = null;
            glProgramUniformMatrix4x2dv = null;
            glProgramUniformMatrix3x4dv = null;
            glProgramUniformMatrix4x3dv = null;
            glValidateProgramPipeline = null;
            glGetProgramPipelineInfoLog = null;
            glVertexAttribL1d = null;
            glVertexAttribL2d = null;
            glVertexAttribL3d = null;
            glVertexAttribL4d = null;
            glVertexAttribL1dv = null;
            glVertexAttribL2dv = null;
            glVertexAttribL3dv = null;
            glVertexAttribL4dv = null;
            glVertexAttribLPointer = null;
            glGetVertexAttribLdv = null;
            glViewportArrayv = null;
            glViewportIndexedf = null;
            glViewportIndexedfv = null;
            glScissorArrayv = null;
            glScissorIndexed = null;
            glScissorIndexedv = null;
            glDepthRangeArrayv = null;
            glDepthRangeIndexed = null;
            glGetFloati_v = null;
            glGetDoublei_v = null;
            glDrawArraysInstancedBaseInstance = null;
            glDrawElementsInstancedBaseInstance = null;
            glDrawElementsInstancedBaseVertexBaseInstance = null;
            glGetInternalformativ = null;
            glGetActiveAtomicCounterBufferiv = null;
            glBindImageTexture = null;
            glMemoryBarrier = null;
            glTexStorage1D = null;
            glTexStorage2D = null;
            glTexStorage3D = null;
            glDrawTransformFeedbackInstanced = null;
            glDrawTransformFeedbackStreamInstanced = null;
            glClearBufferData = null;
            glClearBufferSubData = null;
            glDispatchCompute = null;
            glDispatchComputeIndirect = null;
            glCopyImageSubData = null;
            glFramebufferParameteri = null;
            glGetFramebufferParameteriv = null;
            glGetInternalformati64v = null;
            glInvalidateTexSubImage = null;
            glInvalidateTexImage = null;
            glInvalidateBufferSubData = null;
            glInvalidateBufferData = null;
            glInvalidateFramebuffer = null;
            glInvalidateSubFramebuffer = null;
            glMultiDrawArraysIndirect = null;
            glMultiDrawElementsIndirect = null;
            glGetProgramInterfaceiv = null;
            glGetProgramResourceIndex = null;
            glGetProgramResourceName = null;
            glGetProgramResourceiv = null;
            glGetProgramResourceLocation = null;
            glGetProgramResourceLocationIndex = null;
            glShaderStorageBlockBinding = null;
            glTexBufferRange = null;
            glTexStorage2DMultisample = null;
            glTexStorage3DMultisample = null;
            glTextureView = null;
            glBindVertexBuffer = null;
            glVertexAttribFormat = null;
            glVertexAttribIFormat = null;
            glVertexAttribLFormat = null;
            glVertexAttribBinding = null;
            glVertexBindingDivisor = null;
            glDebugMessageControl = null;
            glDebugMessageInsert = null;
            glDebugMessageCallback = null;
            glGetDebugMessageLog = null;
            glPushDebugGroup = null;
            glPopDebugGroup = null;
            glObjectLabel = null;
            glGetObjectLabel = null;
            glObjectPtrLabel = null;
            glGetObjectPtrLabel = null;
            glBufferStorage = null;
            glClearTexImage = null;
            glClearTexSubImage = null;
            glBindBuffersBase = null;
            glBindBuffersRange = null;
            glBindTextures = null;
            glBindSamplers = null;
            glBindImageTextures = null;
            glBindVertexBuffers = null;
            glClipControl = null;
            glCreateTransformFeedbacks = null;
            glTransformFeedbackBufferBase = null;
            glTransformFeedbackBufferRange = null;
            glGetTransformFeedbackiv = null;
            glGetTransformFeedbacki_v = null;
            glGetTransformFeedbacki64_v = null;
            glCreateBuffers = null;
            glNamedBufferStorage = null;
            glNamedBufferData = null;
            glNamedBufferSubData = null;
            glCopyNamedBufferSubData = null;
            glClearNamedBufferData = null;
            glClearNamedBufferSubData = null;
            glMapNamedBuffer = null;
            glMapNamedBufferRange = null;
            glUnmapNamedBuffer = null;
            glFlushMappedNamedBufferRange = null;
            glGetNamedBufferParameteriv = null;
            glGetNamedBufferParameteri64v = null;
            glGetNamedBufferPointerv = null;
            glGetNamedBufferSubData = null;
            glCreateFramebuffers = null;
            glNamedFramebufferRenderbuffer = null;
            glNamedFramebufferParameteri = null;
            glNamedFramebufferTexture = null;
            glNamedFramebufferTextureLayer = null;
            glNamedFramebufferDrawBuffer = null;
            glNamedFramebufferDrawBuffers = null;
            glNamedFramebufferReadBuffer = null;
            glInvalidateNamedFramebufferData = null;
            glInvalidateNamedFramebufferSubData = null;
            glClearNamedFramebufferiv = null;
            glClearNamedFramebufferuiv = null;
            glClearNamedFramebufferfv = null;
            glClearNamedFramebufferfi = null;
            glBlitNamedFramebuffer = null;
            glCheckNamedFramebufferStatus = null;
            glGetNamedFramebufferParameteriv = null;
            glGetNamedFramebufferAttachmentParameteriv = null;
            glCreateRenderbuffers = null;
            glNamedRenderbufferStorage = null;
            glNamedRenderbufferStorageMultisample = null;
            glGetNamedRenderbufferParameteriv = null;
            glCreateTextures = null;
            glTextureBuffer = null;
            glTextureBufferRange = null;
            glTextureStorage1D = null;
            glTextureStorage2D = null;
            glTextureStorage3D = null;
            glTextureStorage2DMultisample = null;
            glTextureStorage3DMultisample = null;
            glTextureSubImage1D = null;
            glTextureSubImage2D = null;
            glTextureSubImage3D = null;
            glCompressedTextureSubImage1D = null;
            glCompressedTextureSubImage2D = null;
            glCompressedTextureSubImage3D = null;
            glCopyTextureSubImage1D = null;
            glCopyTextureSubImage2D = null;
            glCopyTextureSubImage3D = null;
            glTextureParameterf = null;
            glTextureParameterfv = null;
            glTextureParameteri = null;
            glTextureParameterIiv = null;
            glTextureParameterIuiv = null;
            glTextureParameteriv = null;
            glGenerateTextureMipmap = null;
            glBindTextureUnit = null;
            glGetTextureImage = null;
            glGetCompressedTextureImage = null;
            glGetTextureLevelParameterfv = null;
            glGetTextureLevelParameteriv = null;
            glGetTextureParameterfv = null;
            glGetTextureParameterIiv = null;
            glGetTextureParameterIuiv = null;
            glGetTextureParameteriv = null;
            glCreateVertexArrays = null;
            glDisableVertexArrayAttrib = null;
            glEnableVertexArrayAttrib = null;
            glVertexArrayElementBuffer = null;
            glVertexArrayVertexBuffer = null;
            glVertexArrayVertexBuffers = null;
            glVertexArrayAttribBinding = null;
            glVertexArrayAttribFormat = null;
            glVertexArrayAttribIFormat = null;
            glVertexArrayAttribLFormat = null;
            glVertexArrayBindingDivisor = null;
            glGetVertexArrayiv = null;
            glGetVertexArrayIndexediv = null;
            glGetVertexArrayIndexed64iv = null;
            glCreateSamplers = null;
            glCreateProgramPipelines = null;
            glCreateQueries = null;
            glGetQueryBufferObjecti64v = null;
            glGetQueryBufferObjectiv = null;
            glGetQueryBufferObjectui64v = null;
            glGetQueryBufferObjectuiv = null;
            glMemoryBarrierByRegion = null;
            glGetTextureSubImage = null;
            glGetCompressedTextureSubImage = null;
            glGetGraphicsResetStatus = null;
            glGetnCompressedTexImage = null;
            glGetnTexImage = null;
            glGetnUniformdv = null;
            glGetnUniformfv = null;
            glGetnUniformiv = null;
            glGetnUniformuiv = null;
            glReadnPixels = null;
            glTextureBarrier = null;
            glGetTextureHandleARB = null;
            glGetTextureSamplerHandleARB = null;
            glMakeTextureHandleResidentARB = null;
            glMakeTextureHandleNonResidentARB = null;
            glGetImageHandleARB = null;
            glMakeImageHandleResidentARB = null;
            glMakeImageHandleNonResidentARB = null;
            glUniformHandleui64ARB = null;
            glUniformHandleui64vARB = null;
            glProgramUniformHandleui64ARB = null;
            glProgramUniformHandleui64vARB = null;
            glIsTextureHandleResidentARB = null;
            glIsImageHandleResidentARB = null;
            glVertexAttribL1ui64ARB = null;
            glVertexAttribL1ui64vARB = null;
            glGetVertexAttribLui64vARB = null;
            glCreateSyncFromCLeventARB = null;
            glDispatchComputeGroupSizeARB = null;
            glDebugMessageControlARB = null;
            glDebugMessageInsertARB = null;
            glDebugMessageCallbackARB = null;
            glGetDebugMessageLogARB = null;
            glBlendEquationiARB = null;
            glBlendEquationSeparateiARB = null;
            glBlendFunciARB = null;
            glBlendFuncSeparateiARB = null;
            glMultiDrawArraysIndirectCountARB = null;
            glMultiDrawElementsIndirectCountARB = null;
            glGetGraphicsResetStatusARB = null;
            glGetnTexImageARB = null;
            glReadnPixelsARB = null;
            glGetnCompressedTexImageARB = null;
            glGetnUniformfvARB = null;
            glGetnUniformivARB = null;
            glGetnUniformuivARB = null;
            glGetnUniformdvARB = null;
            glMinSampleShadingARB = null;
            glNamedStringARB = null;
            glDeleteNamedStringARB = null;
            glCompileShaderIncludeARB = null;
            glIsNamedStringARB = null;
            glGetNamedStringARB = null;
            glGetNamedStringivARB = null;
            glBufferPageCommitmentARB = null;
            glNamedBufferPageCommitmentEXT = null;
            glNamedBufferPageCommitmentARB = null;
            glTexPageCommitmentARB = null;
        }

        private static void _ParseVersion()
        {
            GL.Version = new GLVersion();

            GetIntegerv(GLConst.GL_MAJOR_VERSION, ref GL.Version._major);
            GetIntegerv(GLConst.GL_MINOR_VERSION, ref GL.Version._minor);
        }

        private static void _CalcParams()
        {
            PointSizeRange = new float[2];
            GetFloatv(GLConst.GL_POINT_SIZE_RANGE, PointSizeRange);

            LineWidthRange = new float[2];
            GetFloatv(GLConst.GL_LINE_WIDTH_RANGE, LineWidthRange);
        }

        private static void _Init()
        {
            glCullFace = GetDelegate("glCullFace", typeof(PFNGLCULLFACEPROC)) as PFNGLCULLFACEPROC;
            glFrontFace = GetDelegate("glFrontFace", typeof(PFNGLFRONTFACEPROC)) as PFNGLFRONTFACEPROC;
            glHint = GetDelegate("glHint", typeof(PFNGLHINTPROC)) as PFNGLHINTPROC;
            glLineWidth = GetDelegate("glLineWidth", typeof(PFNGLLINEWIDTHPROC)) as PFNGLLINEWIDTHPROC;
            glPointSize = GetDelegate("glPointSize", typeof(PFNGLPOINTSIZEPROC)) as PFNGLPOINTSIZEPROC;
            glPolygonMode = GetDelegate("glPolygonMode", typeof(PFNGLPOLYGONMODEPROC)) as PFNGLPOLYGONMODEPROC;
            glScissor = GetDelegate("glScissor", typeof(PFNGLSCISSORPROC)) as PFNGLSCISSORPROC;
            glTexParameterf = GetDelegate("glTexParameterf", typeof(PFNGLTEXPARAMETERFPROC)) as PFNGLTEXPARAMETERFPROC;
            glTexParameterfv = GetDelegate("glTexParameterfv", typeof(PFNGLTEXPARAMETERFVPROC)) as PFNGLTEXPARAMETERFVPROC;
            glTexParameteri = GetDelegate("glTexParameteri", typeof(PFNGLTEXPARAMETERIPROC)) as PFNGLTEXPARAMETERIPROC;
            glTexParameteriv = GetDelegate("glTexParameteriv", typeof(PFNGLTEXPARAMETERIVPROC)) as PFNGLTEXPARAMETERIVPROC;
            glTexImage1D = GetDelegate("glTexImage1D", typeof(PFNGLTEXIMAGE1DPROC)) as PFNGLTEXIMAGE1DPROC;
            glTexImage2D = GetDelegate("glTexImage2D", typeof(PFNGLTEXIMAGE2DPROC)) as PFNGLTEXIMAGE2DPROC;
            glDrawBuffer = GetDelegate("glDrawBuffer", typeof(PFNGLDRAWBUFFERPROC)) as PFNGLDRAWBUFFERPROC;
            glClear = GetDelegate("glClear", typeof(PFNGLCLEARPROC)) as PFNGLCLEARPROC;
            glClearColor = GetDelegate("glClearColor", typeof(PFNGLCLEARCOLORPROC)) as PFNGLCLEARCOLORPROC;
            glClearStencil = GetDelegate("glClearStencil", typeof(PFNGLCLEARSTENCILPROC)) as PFNGLCLEARSTENCILPROC;
            glClearDepth = GetDelegate("glClearDepth", typeof(PFNGLCLEARDEPTHPROC)) as PFNGLCLEARDEPTHPROC;
            glStencilMask = GetDelegate("glStencilMask", typeof(PFNGLSTENCILMASKPROC)) as PFNGLSTENCILMASKPROC;
            glColorMask = GetDelegate("glColorMask", typeof(PFNGLCOLORMASKPROC)) as PFNGLCOLORMASKPROC;
            glDepthMask = GetDelegate("glDepthMask", typeof(PFNGLDEPTHMASKPROC)) as PFNGLDEPTHMASKPROC;
            glDisable = GetDelegate("glDisable", typeof(PFNGLDISABLEPROC)) as PFNGLDISABLEPROC;
            glEnable = GetDelegate("glEnable", typeof(PFNGLENABLEPROC)) as PFNGLENABLEPROC;
            glFinish = GetDelegate("glFinish", typeof(PFNGLFINISHPROC)) as PFNGLFINISHPROC;
            glFlush = GetDelegate("glFlush", typeof(PFNGLFLUSHPROC)) as PFNGLFLUSHPROC;
            glBlendFunc = GetDelegate("glBlendFunc", typeof(PFNGLBLENDFUNCPROC)) as PFNGLBLENDFUNCPROC;
            glLogicOp = GetDelegate("glLogicOp", typeof(PFNGLLOGICOPPROC)) as PFNGLLOGICOPPROC;
            glStencilFunc = GetDelegate("glStencilFunc", typeof(PFNGLSTENCILFUNCPROC)) as PFNGLSTENCILFUNCPROC;
            glStencilOp = GetDelegate("glStencilOp", typeof(PFNGLSTENCILOPPROC)) as PFNGLSTENCILOPPROC;
            glDepthFunc = GetDelegate("glDepthFunc", typeof(PFNGLDEPTHFUNCPROC)) as PFNGLDEPTHFUNCPROC;
            glPixelStoref = GetDelegate("glPixelStoref", typeof(PFNGLPIXELSTOREFPROC)) as PFNGLPIXELSTOREFPROC;
            glPixelStorei = GetDelegate("glPixelStorei", typeof(PFNGLPIXELSTOREIPROC)) as PFNGLPIXELSTOREIPROC;
            glReadBuffer = GetDelegate("glReadBuffer", typeof(PFNGLREADBUFFERPROC)) as PFNGLREADBUFFERPROC;
            glReadPixels = GetDelegate("glReadPixels", typeof(PFNGLREADPIXELSPROC)) as PFNGLREADPIXELSPROC;
            glGetBooleanv = GetDelegate("glGetBooleanv", typeof(PFNGLGETBOOLEANVPROC)) as PFNGLGETBOOLEANVPROC;
            glGetDoublev = GetDelegate("glGetDoublev", typeof(PFNGLGETDOUBLEVPROC)) as PFNGLGETDOUBLEVPROC;
            glGetError = GetDelegate("glGetError", typeof(PFNGLGETERRORPROC)) as PFNGLGETERRORPROC;
            glGetFloatv = GetDelegate("glGetFloatv", typeof(PFNGLGETFLOATVPROC)) as PFNGLGETFLOATVPROC;
            glGetIntegerv = GetDelegate("glGetIntegerv", typeof(PFNGLGETINTEGERVPROC)) as PFNGLGETINTEGERVPROC;
            glGetString = GetDelegate("glGetString", typeof(PFNGLGETSTRINGPROC)) as PFNGLGETSTRINGPROC;
            glGetTexImage = GetDelegate("glGetTexImage", typeof(PFNGLGETTEXIMAGEPROC)) as PFNGLGETTEXIMAGEPROC;
            glGetTexParameterfv = GetDelegate("glGetTexParameterfv", typeof(PFNGLGETTEXPARAMETERFVPROC)) as PFNGLGETTEXPARAMETERFVPROC;
            glGetTexParameteriv = GetDelegate("glGetTexParameteriv", typeof(PFNGLGETTEXPARAMETERIVPROC)) as PFNGLGETTEXPARAMETERIVPROC;
            glGetTexLevelParameterfv = GetDelegate("glGetTexLevelParameterfv", typeof(PFNGLGETTEXLEVELPARAMETERFVPROC)) as PFNGLGETTEXLEVELPARAMETERFVPROC;
            glGetTexLevelParameteriv = GetDelegate("glGetTexLevelParameteriv", typeof(PFNGLGETTEXLEVELPARAMETERIVPROC)) as PFNGLGETTEXLEVELPARAMETERIVPROC;
            glIsEnabled = GetDelegate("glIsEnabled", typeof(PFNGLISENABLEDPROC)) as PFNGLISENABLEDPROC;
            glDepthRange = GetDelegate("glDepthRange", typeof(PFNGLDEPTHRANGEPROC)) as PFNGLDEPTHRANGEPROC;
            glViewport = GetDelegate("glViewport", typeof(PFNGLVIEWPORTPROC)) as PFNGLVIEWPORTPROC;
            glDrawArrays = GetDelegate("glDrawArrays", typeof(PFNGLDRAWARRAYSPROC)) as PFNGLDRAWARRAYSPROC;
            glDrawElements = GetDelegate("glDrawElements", typeof(PFNGLDRAWELEMENTSPROC)) as PFNGLDRAWELEMENTSPROC;
            glGetPointerv = GetDelegate("glGetPointerv", typeof(PFNGLGETPOINTERVPROC)) as PFNGLGETPOINTERVPROC;
            glPolygonOffset = GetDelegate("glPolygonOffset", typeof(PFNGLPOLYGONOFFSETPROC)) as PFNGLPOLYGONOFFSETPROC;
            glCopyTexImage1D = GetDelegate("glCopyTexImage1D", typeof(PFNGLCOPYTEXIMAGE1DPROC)) as PFNGLCOPYTEXIMAGE1DPROC;
            glCopyTexImage2D = GetDelegate("glCopyTexImage2D", typeof(PFNGLCOPYTEXIMAGE2DPROC)) as PFNGLCOPYTEXIMAGE2DPROC;
            glCopyTexSubImage1D = GetDelegate("glCopyTexSubImage1D", typeof(PFNGLCOPYTEXSUBIMAGE1DPROC)) as PFNGLCOPYTEXSUBIMAGE1DPROC;
            glCopyTexSubImage2D = GetDelegate("glCopyTexSubImage2D", typeof(PFNGLCOPYTEXSUBIMAGE2DPROC)) as PFNGLCOPYTEXSUBIMAGE2DPROC;
            glTexSubImage1D = GetDelegate("glTexSubImage1D", typeof(PFNGLTEXSUBIMAGE1DPROC)) as PFNGLTEXSUBIMAGE1DPROC;
            glTexSubImage2D = GetDelegate("glTexSubImage2D", typeof(PFNGLTEXSUBIMAGE2DPROC)) as PFNGLTEXSUBIMAGE2DPROC;
            glBindTexture = GetDelegate("glBindTexture", typeof(PFNGLBINDTEXTUREPROC)) as PFNGLBINDTEXTUREPROC;
            glDeleteTextures = GetDelegate("glDeleteTextures", typeof(PFNGLDELETETEXTURESPROC)) as PFNGLDELETETEXTURESPROC;
            glGenTextures = GetDelegate("glGenTextures", typeof(PFNGLGENTEXTURESPROC)) as PFNGLGENTEXTURESPROC;
            glIsTexture = GetDelegate("glIsTexture", typeof(PFNGLISTEXTUREPROC)) as PFNGLISTEXTUREPROC;
            glDrawRangeElements = GetDelegate("glDrawRangeElements", typeof(PFNGLDRAWRANGEELEMENTSPROC)) as PFNGLDRAWRANGEELEMENTSPROC;
            glTexImage3D = GetDelegate("glTexImage3D", typeof(PFNGLTEXIMAGE3DPROC)) as PFNGLTEXIMAGE3DPROC;
            glTexSubImage3D = GetDelegate("glTexSubImage3D", typeof(PFNGLTEXSUBIMAGE3DPROC)) as PFNGLTEXSUBIMAGE3DPROC;
            glCopyTexSubImage3D = GetDelegate("glCopyTexSubImage3D", typeof(PFNGLCOPYTEXSUBIMAGE3DPROC)) as PFNGLCOPYTEXSUBIMAGE3DPROC;
            glActiveTexture = GetDelegate("glActiveTexture", typeof(PFNGLACTIVETEXTUREPROC)) as PFNGLACTIVETEXTUREPROC;
            glSampleCoverage = GetDelegate("glSampleCoverage", typeof(PFNGLSAMPLECOVERAGEPROC)) as PFNGLSAMPLECOVERAGEPROC;
            glCompressedTexImage3D = GetDelegate("glCompressedTexImage3D", typeof(PFNGLCOMPRESSEDTEXIMAGE3DPROC)) as PFNGLCOMPRESSEDTEXIMAGE3DPROC;
            glCompressedTexImage2D = GetDelegate("glCompressedTexImage2D", typeof(PFNGLCOMPRESSEDTEXIMAGE2DPROC)) as PFNGLCOMPRESSEDTEXIMAGE2DPROC;
            glCompressedTexImage1D = GetDelegate("glCompressedTexImage1D", typeof(PFNGLCOMPRESSEDTEXIMAGE1DPROC)) as PFNGLCOMPRESSEDTEXIMAGE1DPROC;
            glCompressedTexSubImage3D = GetDelegate("glCompressedTexSubImage3D", typeof(PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC)) as PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC;
            glCompressedTexSubImage2D = GetDelegate("glCompressedTexSubImage2D", typeof(PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC)) as PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC;
            glCompressedTexSubImage1D = GetDelegate("glCompressedTexSubImage1D", typeof(PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC)) as PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC;
            glGetCompressedTexImage = GetDelegate("glGetCompressedTexImage", typeof(PFNGLGETCOMPRESSEDTEXIMAGEPROC)) as PFNGLGETCOMPRESSEDTEXIMAGEPROC;
            glBlendFuncSeparate = GetDelegate("glBlendFuncSeparate", typeof(PFNGLBLENDFUNCSEPARATEPROC)) as PFNGLBLENDFUNCSEPARATEPROC;
            glMultiDrawArrays = GetDelegate("glMultiDrawArrays", typeof(PFNGLMULTIDRAWARRAYSPROC)) as PFNGLMULTIDRAWARRAYSPROC;
            glMultiDrawElements = GetDelegate("glMultiDrawElements", typeof(PFNGLMULTIDRAWELEMENTSPROC)) as PFNGLMULTIDRAWELEMENTSPROC;
            glPointParameterf = GetDelegate("glPointParameterf", typeof(PFNGLPOINTPARAMETERFPROC)) as PFNGLPOINTPARAMETERFPROC;
            glPointParameterfv = GetDelegate("glPointParameterfv", typeof(PFNGLPOINTPARAMETERFVPROC)) as PFNGLPOINTPARAMETERFVPROC;
            glPointParameteri = GetDelegate("glPointParameteri", typeof(PFNGLPOINTPARAMETERIPROC)) as PFNGLPOINTPARAMETERIPROC;
            glPointParameteriv = GetDelegate("glPointParameteriv", typeof(PFNGLPOINTPARAMETERIVPROC)) as PFNGLPOINTPARAMETERIVPROC;
            glBlendColor = GetDelegate("glBlendColor", typeof(PFNGLBLENDCOLORPROC)) as PFNGLBLENDCOLORPROC;
            glBlendEquation = GetDelegate("glBlendEquation", typeof(PFNGLBLENDEQUATIONPROC)) as PFNGLBLENDEQUATIONPROC;
            glGenQueries = GetDelegate("glGenQueries", typeof(PFNGLGENQUERIESPROC)) as PFNGLGENQUERIESPROC;
            glDeleteQueries = GetDelegate("glDeleteQueries", typeof(PFNGLDELETEQUERIESPROC)) as PFNGLDELETEQUERIESPROC;
            glIsQuery = GetDelegate("glIsQuery", typeof(PFNGLISQUERYPROC)) as PFNGLISQUERYPROC;
            glBeginQuery = GetDelegate("glBeginQuery", typeof(PFNGLBEGINQUERYPROC)) as PFNGLBEGINQUERYPROC;
            glEndQuery = GetDelegate("glEndQuery", typeof(PFNGLENDQUERYPROC)) as PFNGLENDQUERYPROC;
            glGetQueryiv = GetDelegate("glGetQueryiv", typeof(PFNGLGETQUERYIVPROC)) as PFNGLGETQUERYIVPROC;
            glGetQueryObjectiv = GetDelegate("glGetQueryObjectiv", typeof(PFNGLGETQUERYOBJECTIVPROC)) as PFNGLGETQUERYOBJECTIVPROC;
            glGetQueryObjectuiv = GetDelegate("glGetQueryObjectuiv", typeof(PFNGLGETQUERYOBJECTUIVPROC)) as PFNGLGETQUERYOBJECTUIVPROC;
            glBindBuffer = GetDelegate("glBindBuffer", typeof(PFNGLBINDBUFFERPROC)) as PFNGLBINDBUFFERPROC;
            glDeleteBuffers = GetDelegate("glDeleteBuffers", typeof(PFNGLDELETEBUFFERSPROC)) as PFNGLDELETEBUFFERSPROC;
            glGenBuffers = GetDelegate("glGenBuffers", typeof(PFNGLGENBUFFERSPROC)) as PFNGLGENBUFFERSPROC;
            glIsBuffer = GetDelegate("glIsBuffer", typeof(PFNGLISBUFFERPROC)) as PFNGLISBUFFERPROC;
            glBufferData = GetDelegate("glBufferData", typeof(PFNGLBUFFERDATAPROC)) as PFNGLBUFFERDATAPROC;
            glBufferSubData = GetDelegate("glBufferSubData", typeof(PFNGLBUFFERSUBDATAPROC)) as PFNGLBUFFERSUBDATAPROC;
            glGetBufferSubData = GetDelegate("glGetBufferSubData", typeof(PFNGLGETBUFFERSUBDATAPROC)) as PFNGLGETBUFFERSUBDATAPROC;
            glMapBuffer = GetDelegate("glMapBuffer", typeof(PFNGLMAPBUFFERPROC)) as PFNGLMAPBUFFERPROC;
            glUnmapBuffer = GetDelegate("glUnmapBuffer", typeof(PFNGLUNMAPBUFFERPROC)) as PFNGLUNMAPBUFFERPROC;
            glGetBufferParameteriv = GetDelegate("glGetBufferParameteriv", typeof(PFNGLGETBUFFERPARAMETERIVPROC)) as PFNGLGETBUFFERPARAMETERIVPROC;
            glGetBufferPointerv = GetDelegate("glGetBufferPointerv", typeof(PFNGLGETBUFFERPOINTERVPROC)) as PFNGLGETBUFFERPOINTERVPROC;
            glBlendEquationSeparate = GetDelegate("glBlendEquationSeparate", typeof(PFNGLBLENDEQUATIONSEPARATEPROC)) as PFNGLBLENDEQUATIONSEPARATEPROC;
            glDrawBuffers = GetDelegate("glDrawBuffers", typeof(PFNGLDRAWBUFFERSPROC)) as PFNGLDRAWBUFFERSPROC;
            glStencilOpSeparate = GetDelegate("glStencilOpSeparate", typeof(PFNGLSTENCILOPSEPARATEPROC)) as PFNGLSTENCILOPSEPARATEPROC;
            glStencilFuncSeparate = GetDelegate("glStencilFuncSeparate", typeof(PFNGLSTENCILFUNCSEPARATEPROC)) as PFNGLSTENCILFUNCSEPARATEPROC;
            glStencilMaskSeparate = GetDelegate("glStencilMaskSeparate", typeof(PFNGLSTENCILMASKSEPARATEPROC)) as PFNGLSTENCILMASKSEPARATEPROC;
            glAttachShader = GetDelegate("glAttachShader", typeof(PFNGLATTACHSHADERPROC)) as PFNGLATTACHSHADERPROC;
            glBindAttribLocation = GetDelegate("glBindAttribLocation", typeof(PFNGLBINDATTRIBLOCATIONPROC)) as PFNGLBINDATTRIBLOCATIONPROC;
            glCompileShader = GetDelegate("glCompileShader", typeof(PFNGLCOMPILESHADERPROC)) as PFNGLCOMPILESHADERPROC;
            glCreateProgram = GetDelegate("glCreateProgram", typeof(PFNGLCREATEPROGRAMPROC)) as PFNGLCREATEPROGRAMPROC;
            glCreateShader = GetDelegate("glCreateShader", typeof(PFNGLCREATESHADERPROC)) as PFNGLCREATESHADERPROC;
            glDeleteProgram = GetDelegate("glDeleteProgram", typeof(PFNGLDELETEPROGRAMPROC)) as PFNGLDELETEPROGRAMPROC;
            glDeleteShader = GetDelegate("glDeleteShader", typeof(PFNGLDELETESHADERPROC)) as PFNGLDELETESHADERPROC;
            glDetachShader = GetDelegate("glDetachShader", typeof(PFNGLDETACHSHADERPROC)) as PFNGLDETACHSHADERPROC;
            glDisableVertexAttribArray = GetDelegate("glDisableVertexAttribArray", typeof(PFNGLDISABLEVERTEXATTRIBARRAYPROC)) as PFNGLDISABLEVERTEXATTRIBARRAYPROC;
            glEnableVertexAttribArray = GetDelegate("glEnableVertexAttribArray", typeof(PFNGLENABLEVERTEXATTRIBARRAYPROC)) as PFNGLENABLEVERTEXATTRIBARRAYPROC;
            glGetActiveAttrib = GetDelegate("glGetActiveAttrib", typeof(PFNGLGETACTIVEATTRIBPROC)) as PFNGLGETACTIVEATTRIBPROC;
            glGetActiveUniform = GetDelegate("glGetActiveUniform", typeof(PFNGLGETACTIVEUNIFORMPROC)) as PFNGLGETACTIVEUNIFORMPROC;
            glGetAttachedShaders = GetDelegate("glGetAttachedShaders", typeof(PFNGLGETATTACHEDSHADERSPROC)) as PFNGLGETATTACHEDSHADERSPROC;
            glGetAttribLocation = GetDelegate("glGetAttribLocation", typeof(PFNGLGETATTRIBLOCATIONPROC)) as PFNGLGETATTRIBLOCATIONPROC;
            glGetProgramiv = GetDelegate("glGetProgramiv", typeof(PFNGLGETPROGRAMIVPROC)) as PFNGLGETPROGRAMIVPROC;
            glGetProgramInfoLog = GetDelegate("glGetProgramInfoLog", typeof(PFNGLGETPROGRAMINFOLOGPROC)) as PFNGLGETPROGRAMINFOLOGPROC;
            glGetShaderiv = GetDelegate("glGetShaderiv", typeof(PFNGLGETSHADERIVPROC)) as PFNGLGETSHADERIVPROC;
            glGetShaderInfoLog = GetDelegate("glGetShaderInfoLog", typeof(PFNGLGETSHADERINFOLOGPROC)) as PFNGLGETSHADERINFOLOGPROC;
            glGetShaderSource = GetDelegate("glGetShaderSource", typeof(PFNGLGETSHADERSOURCEPROC)) as PFNGLGETSHADERSOURCEPROC;
            glGetUniformLocation = GetDelegate("glGetUniformLocation", typeof(PFNGLGETUNIFORMLOCATIONPROC)) as PFNGLGETUNIFORMLOCATIONPROC;
            glGetUniformfv = GetDelegate("glGetUniformfv", typeof(PFNGLGETUNIFORMFVPROC)) as PFNGLGETUNIFORMFVPROC;
            glGetUniformiv = GetDelegate("glGetUniformiv", typeof(PFNGLGETUNIFORMIVPROC)) as PFNGLGETUNIFORMIVPROC;
            glGetVertexAttribdv = GetDelegate("glGetVertexAttribdv", typeof(PFNGLGETVERTEXATTRIBDVPROC)) as PFNGLGETVERTEXATTRIBDVPROC;
            glGetVertexAttribfv = GetDelegate("glGetVertexAttribfv", typeof(PFNGLGETVERTEXATTRIBFVPROC)) as PFNGLGETVERTEXATTRIBFVPROC;
            glGetVertexAttribiv = GetDelegate("glGetVertexAttribiv", typeof(PFNGLGETVERTEXATTRIBIVPROC)) as PFNGLGETVERTEXATTRIBIVPROC;
            glGetVertexAttribPointerv = GetDelegate("glGetVertexAttribPointerv", typeof(PFNGLGETVERTEXATTRIBPOINTERVPROC)) as PFNGLGETVERTEXATTRIBPOINTERVPROC;
            glIsProgram = GetDelegate("glIsProgram", typeof(PFNGLISPROGRAMPROC)) as PFNGLISPROGRAMPROC;
            glIsShader = GetDelegate("glIsShader", typeof(PFNGLISSHADERPROC)) as PFNGLISSHADERPROC;
            glLinkProgram = GetDelegate("glLinkProgram", typeof(PFNGLLINKPROGRAMPROC)) as PFNGLLINKPROGRAMPROC;
            glShaderSource = GetDelegate("glShaderSource", typeof(PFNGLSHADERSOURCEPROC)) as PFNGLSHADERSOURCEPROC;
            glUseProgram = GetDelegate("glUseProgram", typeof(PFNGLUSEPROGRAMPROC)) as PFNGLUSEPROGRAMPROC;
            glUniform1f = GetDelegate("glUniform1f", typeof(PFNGLUNIFORM1FPROC)) as PFNGLUNIFORM1FPROC;
            glUniform2f = GetDelegate("glUniform2f", typeof(PFNGLUNIFORM2FPROC)) as PFNGLUNIFORM2FPROC;
            glUniform3f = GetDelegate("glUniform3f", typeof(PFNGLUNIFORM3FPROC)) as PFNGLUNIFORM3FPROC;
            glUniform4f = GetDelegate("glUniform4f", typeof(PFNGLUNIFORM4FPROC)) as PFNGLUNIFORM4FPROC;
            glUniform1i = GetDelegate("glUniform1i", typeof(PFNGLUNIFORM1IPROC)) as PFNGLUNIFORM1IPROC;
            glUniform2i = GetDelegate("glUniform2i", typeof(PFNGLUNIFORM2IPROC)) as PFNGLUNIFORM2IPROC;
            glUniform3i = GetDelegate("glUniform3i", typeof(PFNGLUNIFORM3IPROC)) as PFNGLUNIFORM3IPROC;
            glUniform4i = GetDelegate("glUniform4i", typeof(PFNGLUNIFORM4IPROC)) as PFNGLUNIFORM4IPROC;
            glUniform1fv = GetDelegate("glUniform1fv", typeof(PFNGLUNIFORM1FVPROC)) as PFNGLUNIFORM1FVPROC;
            glUniform2fv = GetDelegate("glUniform2fv", typeof(PFNGLUNIFORM2FVPROC)) as PFNGLUNIFORM2FVPROC;
            glUniform3fv = GetDelegate("glUniform3fv", typeof(PFNGLUNIFORM3FVPROC)) as PFNGLUNIFORM3FVPROC;
            glUniform4fv = GetDelegate("glUniform4fv", typeof(PFNGLUNIFORM4FVPROC)) as PFNGLUNIFORM4FVPROC;
            glUniform1iv = GetDelegate("glUniform1iv", typeof(PFNGLUNIFORM1IVPROC)) as PFNGLUNIFORM1IVPROC;
            glUniform2iv = GetDelegate("glUniform2iv", typeof(PFNGLUNIFORM2IVPROC)) as PFNGLUNIFORM2IVPROC;
            glUniform3iv = GetDelegate("glUniform3iv", typeof(PFNGLUNIFORM3IVPROC)) as PFNGLUNIFORM3IVPROC;
            glUniform4iv = GetDelegate("glUniform4iv", typeof(PFNGLUNIFORM4IVPROC)) as PFNGLUNIFORM4IVPROC;
            glUniformMatrix2fv = GetDelegate("glUniformMatrix2fv", typeof(PFNGLUNIFORMMATRIX2FVPROC)) as PFNGLUNIFORMMATRIX2FVPROC;
            glUniformMatrix3fv = GetDelegate("glUniformMatrix3fv", typeof(PFNGLUNIFORMMATRIX3FVPROC)) as PFNGLUNIFORMMATRIX3FVPROC;
            glUniformMatrix4fv = GetDelegate("glUniformMatrix4fv", typeof(PFNGLUNIFORMMATRIX4FVPROC)) as PFNGLUNIFORMMATRIX4FVPROC;
            glValidateProgram = GetDelegate("glValidateProgram", typeof(PFNGLVALIDATEPROGRAMPROC)) as PFNGLVALIDATEPROGRAMPROC;
            glVertexAttrib1d = GetDelegate("glVertexAttrib1d", typeof(PFNGLVERTEXATTRIB1DPROC)) as PFNGLVERTEXATTRIB1DPROC;
            glVertexAttrib1dv = GetDelegate("glVertexAttrib1dv", typeof(PFNGLVERTEXATTRIB1DVPROC)) as PFNGLVERTEXATTRIB1DVPROC;
            glVertexAttrib1f = GetDelegate("glVertexAttrib1f", typeof(PFNGLVERTEXATTRIB1FPROC)) as PFNGLVERTEXATTRIB1FPROC;
            glVertexAttrib1fv = GetDelegate("glVertexAttrib1fv", typeof(PFNGLVERTEXATTRIB1FVPROC)) as PFNGLVERTEXATTRIB1FVPROC;
            glVertexAttrib1s = GetDelegate("glVertexAttrib1s", typeof(PFNGLVERTEXATTRIB1SPROC)) as PFNGLVERTEXATTRIB1SPROC;
            glVertexAttrib1sv = GetDelegate("glVertexAttrib1sv", typeof(PFNGLVERTEXATTRIB1SVPROC)) as PFNGLVERTEXATTRIB1SVPROC;
            glVertexAttrib2d = GetDelegate("glVertexAttrib2d", typeof(PFNGLVERTEXATTRIB2DPROC)) as PFNGLVERTEXATTRIB2DPROC;
            glVertexAttrib2dv = GetDelegate("glVertexAttrib2dv", typeof(PFNGLVERTEXATTRIB2DVPROC)) as PFNGLVERTEXATTRIB2DVPROC;
            glVertexAttrib2f = GetDelegate("glVertexAttrib2f", typeof(PFNGLVERTEXATTRIB2FPROC)) as PFNGLVERTEXATTRIB2FPROC;
            glVertexAttrib2fv = GetDelegate("glVertexAttrib2fv", typeof(PFNGLVERTEXATTRIB2FVPROC)) as PFNGLVERTEXATTRIB2FVPROC;
            glVertexAttrib2s = GetDelegate("glVertexAttrib2s", typeof(PFNGLVERTEXATTRIB2SPROC)) as PFNGLVERTEXATTRIB2SPROC;
            glVertexAttrib2sv = GetDelegate("glVertexAttrib2sv", typeof(PFNGLVERTEXATTRIB2SVPROC)) as PFNGLVERTEXATTRIB2SVPROC;
            glVertexAttrib3d = GetDelegate("glVertexAttrib3d", typeof(PFNGLVERTEXATTRIB3DPROC)) as PFNGLVERTEXATTRIB3DPROC;
            glVertexAttrib3dv = GetDelegate("glVertexAttrib3dv", typeof(PFNGLVERTEXATTRIB3DVPROC)) as PFNGLVERTEXATTRIB3DVPROC;
            glVertexAttrib3f = GetDelegate("glVertexAttrib3f", typeof(PFNGLVERTEXATTRIB3FPROC)) as PFNGLVERTEXATTRIB3FPROC;
            glVertexAttrib3fv = GetDelegate("glVertexAttrib3fv", typeof(PFNGLVERTEXATTRIB3FVPROC)) as PFNGLVERTEXATTRIB3FVPROC;
            glVertexAttrib3s = GetDelegate("glVertexAttrib3s", typeof(PFNGLVERTEXATTRIB3SPROC)) as PFNGLVERTEXATTRIB3SPROC;
            glVertexAttrib3sv = GetDelegate("glVertexAttrib3sv", typeof(PFNGLVERTEXATTRIB3SVPROC)) as PFNGLVERTEXATTRIB3SVPROC;
            glVertexAttrib4Nbv = GetDelegate("glVertexAttrib4Nbv", typeof(PFNGLVERTEXATTRIB4NBVPROC)) as PFNGLVERTEXATTRIB4NBVPROC;
            glVertexAttrib4Niv = GetDelegate("glVertexAttrib4Niv", typeof(PFNGLVERTEXATTRIB4NIVPROC)) as PFNGLVERTEXATTRIB4NIVPROC;
            glVertexAttrib4Nsv = GetDelegate("glVertexAttrib4Nsv", typeof(PFNGLVERTEXATTRIB4NSVPROC)) as PFNGLVERTEXATTRIB4NSVPROC;
            glVertexAttrib4Nub = GetDelegate("glVertexAttrib4Nub", typeof(PFNGLVERTEXATTRIB4NUBPROC)) as PFNGLVERTEXATTRIB4NUBPROC;
            glVertexAttrib4Nubv = GetDelegate("glVertexAttrib4Nubv", typeof(PFNGLVERTEXATTRIB4NUBVPROC)) as PFNGLVERTEXATTRIB4NUBVPROC;
            glVertexAttrib4Nuiv = GetDelegate("glVertexAttrib4Nuiv", typeof(PFNGLVERTEXATTRIB4NUIVPROC)) as PFNGLVERTEXATTRIB4NUIVPROC;
            glVertexAttrib4Nusv = GetDelegate("glVertexAttrib4Nusv", typeof(PFNGLVERTEXATTRIB4NUSVPROC)) as PFNGLVERTEXATTRIB4NUSVPROC;
            glVertexAttrib4bv = GetDelegate("glVertexAttrib4bv", typeof(PFNGLVERTEXATTRIB4BVPROC)) as PFNGLVERTEXATTRIB4BVPROC;
            glVertexAttrib4d = GetDelegate("glVertexAttrib4d", typeof(PFNGLVERTEXATTRIB4DPROC)) as PFNGLVERTEXATTRIB4DPROC;
            glVertexAttrib4dv = GetDelegate("glVertexAttrib4dv", typeof(PFNGLVERTEXATTRIB4DVPROC)) as PFNGLVERTEXATTRIB4DVPROC;
            glVertexAttrib4f = GetDelegate("glVertexAttrib4f", typeof(PFNGLVERTEXATTRIB4FPROC)) as PFNGLVERTEXATTRIB4FPROC;
            glVertexAttrib4fv = GetDelegate("glVertexAttrib4fv", typeof(PFNGLVERTEXATTRIB4FVPROC)) as PFNGLVERTEXATTRIB4FVPROC;
            glVertexAttrib4iv = GetDelegate("glVertexAttrib4iv", typeof(PFNGLVERTEXATTRIB4IVPROC)) as PFNGLVERTEXATTRIB4IVPROC;
            glVertexAttrib4s = GetDelegate("glVertexAttrib4s", typeof(PFNGLVERTEXATTRIB4SPROC)) as PFNGLVERTEXATTRIB4SPROC;
            glVertexAttrib4sv = GetDelegate("glVertexAttrib4sv", typeof(PFNGLVERTEXATTRIB4SVPROC)) as PFNGLVERTEXATTRIB4SVPROC;
            glVertexAttrib4ubv = GetDelegate("glVertexAttrib4ubv", typeof(PFNGLVERTEXATTRIB4UBVPROC)) as PFNGLVERTEXATTRIB4UBVPROC;
            glVertexAttrib4uiv = GetDelegate("glVertexAttrib4uiv", typeof(PFNGLVERTEXATTRIB4UIVPROC)) as PFNGLVERTEXATTRIB4UIVPROC;
            glVertexAttrib4usv = GetDelegate("glVertexAttrib4usv", typeof(PFNGLVERTEXATTRIB4USVPROC)) as PFNGLVERTEXATTRIB4USVPROC;
            glVertexAttribPointer = GetDelegate("glVertexAttribPointer", typeof(PFNGLVERTEXATTRIBPOINTERPROC)) as PFNGLVERTEXATTRIBPOINTERPROC;
            glUniformMatrix2x3fv = GetDelegate("glUniformMatrix2x3fv", typeof(PFNGLUNIFORMMATRIX2X3FVPROC)) as PFNGLUNIFORMMATRIX2X3FVPROC;
            glUniformMatrix3x2fv = GetDelegate("glUniformMatrix3x2fv", typeof(PFNGLUNIFORMMATRIX3X2FVPROC)) as PFNGLUNIFORMMATRIX3X2FVPROC;
            glUniformMatrix2x4fv = GetDelegate("glUniformMatrix2x4fv", typeof(PFNGLUNIFORMMATRIX2X4FVPROC)) as PFNGLUNIFORMMATRIX2X4FVPROC;
            glUniformMatrix4x2fv = GetDelegate("glUniformMatrix4x2fv", typeof(PFNGLUNIFORMMATRIX4X2FVPROC)) as PFNGLUNIFORMMATRIX4X2FVPROC;
            glUniformMatrix3x4fv = GetDelegate("glUniformMatrix3x4fv", typeof(PFNGLUNIFORMMATRIX3X4FVPROC)) as PFNGLUNIFORMMATRIX3X4FVPROC;
            glUniformMatrix4x3fv = GetDelegate("glUniformMatrix4x3fv", typeof(PFNGLUNIFORMMATRIX4X3FVPROC)) as PFNGLUNIFORMMATRIX4X3FVPROC;
            glColorMaski = GetDelegate("glColorMaski", typeof(PFNGLCOLORMASKIPROC)) as PFNGLCOLORMASKIPROC;
            glGetBooleani_v = GetDelegate("glGetBooleani_v", typeof(PFNGLGETBOOLEANI_VPROC)) as PFNGLGETBOOLEANI_VPROC;
            glGetIntegeri_v = GetDelegate("glGetIntegeri_v", typeof(PFNGLGETINTEGERI_VPROC)) as PFNGLGETINTEGERI_VPROC;
            glEnablei = GetDelegate("glEnablei", typeof(PFNGLENABLEIPROC)) as PFNGLENABLEIPROC;
            glDisablei = GetDelegate("glDisablei", typeof(PFNGLDISABLEIPROC)) as PFNGLDISABLEIPROC;
            glIsEnabledi = GetDelegate("glIsEnabledi", typeof(PFNGLISENABLEDIPROC)) as PFNGLISENABLEDIPROC;
            glBeginTransformFeedback = GetDelegate("glBeginTransformFeedback", typeof(PFNGLBEGINTRANSFORMFEEDBACKPROC)) as PFNGLBEGINTRANSFORMFEEDBACKPROC;
            glEndTransformFeedback = GetDelegate("glEndTransformFeedback", typeof(PFNGLENDTRANSFORMFEEDBACKPROC)) as PFNGLENDTRANSFORMFEEDBACKPROC;
            glBindBufferRange = GetDelegate("glBindBufferRange", typeof(PFNGLBINDBUFFERRANGEPROC)) as PFNGLBINDBUFFERRANGEPROC;
            glBindBufferBase = GetDelegate("glBindBufferBase", typeof(PFNGLBINDBUFFERBASEPROC)) as PFNGLBINDBUFFERBASEPROC;
            glTransformFeedbackVaryings = GetDelegate("glTransformFeedbackVaryings", typeof(PFNGLTRANSFORMFEEDBACKVARYINGSPROC)) as PFNGLTRANSFORMFEEDBACKVARYINGSPROC;
            glGetTransformFeedbackVarying = GetDelegate("glGetTransformFeedbackVarying", typeof(PFNGLGETTRANSFORMFEEDBACKVARYINGPROC)) as PFNGLGETTRANSFORMFEEDBACKVARYINGPROC;
            glClampColor = GetDelegate("glClampColor", typeof(PFNGLCLAMPCOLORPROC)) as PFNGLCLAMPCOLORPROC;
            glBeginConditionalRender = GetDelegate("glBeginConditionalRender", typeof(PFNGLBEGINCONDITIONALRENDERPROC)) as PFNGLBEGINCONDITIONALRENDERPROC;
            glEndConditionalRender = GetDelegate("glEndConditionalRender", typeof(PFNGLENDCONDITIONALRENDERPROC)) as PFNGLENDCONDITIONALRENDERPROC;
            glVertexAttribIPointer = GetDelegate("glVertexAttribIPointer", typeof(PFNGLVERTEXATTRIBIPOINTERPROC)) as PFNGLVERTEXATTRIBIPOINTERPROC;
            glGetVertexAttribIiv = GetDelegate("glGetVertexAttribIiv", typeof(PFNGLGETVERTEXATTRIBIIVPROC)) as PFNGLGETVERTEXATTRIBIIVPROC;
            glGetVertexAttribIuiv = GetDelegate("glGetVertexAttribIuiv", typeof(PFNGLGETVERTEXATTRIBIUIVPROC)) as PFNGLGETVERTEXATTRIBIUIVPROC;
            glVertexAttribI1i = GetDelegate("glVertexAttribI1i", typeof(PFNGLVERTEXATTRIBI1IPROC)) as PFNGLVERTEXATTRIBI1IPROC;
            glVertexAttribI2i = GetDelegate("glVertexAttribI2i", typeof(PFNGLVERTEXATTRIBI2IPROC)) as PFNGLVERTEXATTRIBI2IPROC;
            glVertexAttribI3i = GetDelegate("glVertexAttribI3i", typeof(PFNGLVERTEXATTRIBI3IPROC)) as PFNGLVERTEXATTRIBI3IPROC;
            glVertexAttribI4i = GetDelegate("glVertexAttribI4i", typeof(PFNGLVERTEXATTRIBI4IPROC)) as PFNGLVERTEXATTRIBI4IPROC;
            glVertexAttribI1ui = GetDelegate("glVertexAttribI1ui", typeof(PFNGLVERTEXATTRIBI1UIPROC)) as PFNGLVERTEXATTRIBI1UIPROC;
            glVertexAttribI2ui = GetDelegate("glVertexAttribI2ui", typeof(PFNGLVERTEXATTRIBI2UIPROC)) as PFNGLVERTEXATTRIBI2UIPROC;
            glVertexAttribI3ui = GetDelegate("glVertexAttribI3ui", typeof(PFNGLVERTEXATTRIBI3UIPROC)) as PFNGLVERTEXATTRIBI3UIPROC;
            glVertexAttribI4ui = GetDelegate("glVertexAttribI4ui", typeof(PFNGLVERTEXATTRIBI4UIPROC)) as PFNGLVERTEXATTRIBI4UIPROC;
            glVertexAttribI1iv = GetDelegate("glVertexAttribI1iv", typeof(PFNGLVERTEXATTRIBI1IVPROC)) as PFNGLVERTEXATTRIBI1IVPROC;
            glVertexAttribI2iv = GetDelegate("glVertexAttribI2iv", typeof(PFNGLVERTEXATTRIBI2IVPROC)) as PFNGLVERTEXATTRIBI2IVPROC;
            glVertexAttribI3iv = GetDelegate("glVertexAttribI3iv", typeof(PFNGLVERTEXATTRIBI3IVPROC)) as PFNGLVERTEXATTRIBI3IVPROC;
            glVertexAttribI4iv = GetDelegate("glVertexAttribI4iv", typeof(PFNGLVERTEXATTRIBI4IVPROC)) as PFNGLVERTEXATTRIBI4IVPROC;
            glVertexAttribI1uiv = GetDelegate("glVertexAttribI1uiv", typeof(PFNGLVERTEXATTRIBI1UIVPROC)) as PFNGLVERTEXATTRIBI1UIVPROC;
            glVertexAttribI2uiv = GetDelegate("glVertexAttribI2uiv", typeof(PFNGLVERTEXATTRIBI2UIVPROC)) as PFNGLVERTEXATTRIBI2UIVPROC;
            glVertexAttribI3uiv = GetDelegate("glVertexAttribI3uiv", typeof(PFNGLVERTEXATTRIBI3UIVPROC)) as PFNGLVERTEXATTRIBI3UIVPROC;
            glVertexAttribI4uiv = GetDelegate("glVertexAttribI4uiv", typeof(PFNGLVERTEXATTRIBI4UIVPROC)) as PFNGLVERTEXATTRIBI4UIVPROC;
            glVertexAttribI4bv = GetDelegate("glVertexAttribI4bv", typeof(PFNGLVERTEXATTRIBI4BVPROC)) as PFNGLVERTEXATTRIBI4BVPROC;
            glVertexAttribI4sv = GetDelegate("glVertexAttribI4sv", typeof(PFNGLVERTEXATTRIBI4SVPROC)) as PFNGLVERTEXATTRIBI4SVPROC;
            glVertexAttribI4ubv = GetDelegate("glVertexAttribI4ubv", typeof(PFNGLVERTEXATTRIBI4UBVPROC)) as PFNGLVERTEXATTRIBI4UBVPROC;
            glVertexAttribI4usv = GetDelegate("glVertexAttribI4usv", typeof(PFNGLVERTEXATTRIBI4USVPROC)) as PFNGLVERTEXATTRIBI4USVPROC;
            glGetUniformuiv = GetDelegate("glGetUniformuiv", typeof(PFNGLGETUNIFORMUIVPROC)) as PFNGLGETUNIFORMUIVPROC;
            glBindFragDataLocation = GetDelegate("glBindFragDataLocation", typeof(PFNGLBINDFRAGDATALOCATIONPROC)) as PFNGLBINDFRAGDATALOCATIONPROC;
            glGetFragDataLocation = GetDelegate("glGetFragDataLocation", typeof(PFNGLGETFRAGDATALOCATIONPROC)) as PFNGLGETFRAGDATALOCATIONPROC;
            glUniform1ui = GetDelegate("glUniform1ui", typeof(PFNGLUNIFORM1UIPROC)) as PFNGLUNIFORM1UIPROC;
            glUniform2ui = GetDelegate("glUniform2ui", typeof(PFNGLUNIFORM2UIPROC)) as PFNGLUNIFORM2UIPROC;
            glUniform3ui = GetDelegate("glUniform3ui", typeof(PFNGLUNIFORM3UIPROC)) as PFNGLUNIFORM3UIPROC;
            glUniform4ui = GetDelegate("glUniform4ui", typeof(PFNGLUNIFORM4UIPROC)) as PFNGLUNIFORM4UIPROC;
            glUniform1uiv = GetDelegate("glUniform1uiv", typeof(PFNGLUNIFORM1UIVPROC)) as PFNGLUNIFORM1UIVPROC;
            glUniform2uiv = GetDelegate("glUniform2uiv", typeof(PFNGLUNIFORM2UIVPROC)) as PFNGLUNIFORM2UIVPROC;
            glUniform3uiv = GetDelegate("glUniform3uiv", typeof(PFNGLUNIFORM3UIVPROC)) as PFNGLUNIFORM3UIVPROC;
            glUniform4uiv = GetDelegate("glUniform4uiv", typeof(PFNGLUNIFORM4UIVPROC)) as PFNGLUNIFORM4UIVPROC;
            glTexParameterIiv = GetDelegate("glTexParameterIiv", typeof(PFNGLTEXPARAMETERIIVPROC)) as PFNGLTEXPARAMETERIIVPROC;
            glTexParameterIuiv = GetDelegate("glTexParameterIuiv", typeof(PFNGLTEXPARAMETERIUIVPROC)) as PFNGLTEXPARAMETERIUIVPROC;
            glGetTexParameterIiv = GetDelegate("glGetTexParameterIiv", typeof(PFNGLGETTEXPARAMETERIIVPROC)) as PFNGLGETTEXPARAMETERIIVPROC;
            glGetTexParameterIuiv = GetDelegate("glGetTexParameterIuiv", typeof(PFNGLGETTEXPARAMETERIUIVPROC)) as PFNGLGETTEXPARAMETERIUIVPROC;
            glClearBufferiv = GetDelegate("glClearBufferiv", typeof(PFNGLCLEARBUFFERIVPROC)) as PFNGLCLEARBUFFERIVPROC;
            glClearBufferuiv = GetDelegate("glClearBufferuiv", typeof(PFNGLCLEARBUFFERUIVPROC)) as PFNGLCLEARBUFFERUIVPROC;
            glClearBufferfv = GetDelegate("glClearBufferfv", typeof(PFNGLCLEARBUFFERFVPROC)) as PFNGLCLEARBUFFERFVPROC;
            glClearBufferfi = GetDelegate("glClearBufferfi", typeof(PFNGLCLEARBUFFERFIPROC)) as PFNGLCLEARBUFFERFIPROC;
            glGetStringi = GetDelegate("glGetStringi", typeof(PFNGLGETSTRINGIPROC)) as PFNGLGETSTRINGIPROC;
            glIsRenderbuffer = GetDelegate("glIsRenderbuffer", typeof(PFNGLISRENDERBUFFERPROC)) as PFNGLISRENDERBUFFERPROC;
            glBindRenderbuffer = GetDelegate("glBindRenderbuffer", typeof(PFNGLBINDRENDERBUFFERPROC)) as PFNGLBINDRENDERBUFFERPROC;
            glDeleteRenderbuffers = GetDelegate("glDeleteRenderbuffers", typeof(PFNGLDELETERENDERBUFFERSPROC)) as PFNGLDELETERENDERBUFFERSPROC;
            glGenRenderbuffers = GetDelegate("glGenRenderbuffers", typeof(PFNGLGENRENDERBUFFERSPROC)) as PFNGLGENRENDERBUFFERSPROC;
            glRenderbufferStorage = GetDelegate("glRenderbufferStorage", typeof(PFNGLRENDERBUFFERSTORAGEPROC)) as PFNGLRENDERBUFFERSTORAGEPROC;
            glGetRenderbufferParameteriv = GetDelegate("glGetRenderbufferParameteriv", typeof(PFNGLGETRENDERBUFFERPARAMETERIVPROC)) as PFNGLGETRENDERBUFFERPARAMETERIVPROC;
            glIsFramebuffer = GetDelegate("glIsFramebuffer", typeof(PFNGLISFRAMEBUFFERPROC)) as PFNGLISFRAMEBUFFERPROC;
            glBindFramebuffer = GetDelegate("glBindFramebuffer", typeof(PFNGLBINDFRAMEBUFFERPROC)) as PFNGLBINDFRAMEBUFFERPROC;
            glDeleteFramebuffers = GetDelegate("glDeleteFramebuffers", typeof(PFNGLDELETEFRAMEBUFFERSPROC)) as PFNGLDELETEFRAMEBUFFERSPROC;
            glGenFramebuffers = GetDelegate("glGenFramebuffers", typeof(PFNGLGENFRAMEBUFFERSPROC)) as PFNGLGENFRAMEBUFFERSPROC;
            glCheckFramebufferStatus = GetDelegate("glCheckFramebufferStatus", typeof(PFNGLCHECKFRAMEBUFFERSTATUSPROC)) as PFNGLCHECKFRAMEBUFFERSTATUSPROC;
            glFramebufferTexture1D = GetDelegate("glFramebufferTexture1D", typeof(PFNGLFRAMEBUFFERTEXTURE1DPROC)) as PFNGLFRAMEBUFFERTEXTURE1DPROC;
            glFramebufferTexture2D = GetDelegate("glFramebufferTexture2D", typeof(PFNGLFRAMEBUFFERTEXTURE2DPROC)) as PFNGLFRAMEBUFFERTEXTURE2DPROC;
            glFramebufferTexture3D = GetDelegate("glFramebufferTexture3D", typeof(PFNGLFRAMEBUFFERTEXTURE3DPROC)) as PFNGLFRAMEBUFFERTEXTURE3DPROC;
            glFramebufferRenderbuffer = GetDelegate("glFramebufferRenderbuffer", typeof(PFNGLFRAMEBUFFERRENDERBUFFERPROC)) as PFNGLFRAMEBUFFERRENDERBUFFERPROC;
            glGetFramebufferAttachmentParameteriv = GetDelegate("glGetFramebufferAttachmentParameteriv", typeof(PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC)) as PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC;
            glGenerateMipmap = GetDelegate("glGenerateMipmap", typeof(PFNGLGENERATEMIPMAPPROC)) as PFNGLGENERATEMIPMAPPROC;
            glBlitFramebuffer = GetDelegate("glBlitFramebuffer", typeof(PFNGLBLITFRAMEBUFFERPROC)) as PFNGLBLITFRAMEBUFFERPROC;
            glRenderbufferStorageMultisample = GetDelegate("glRenderbufferStorageMultisample", typeof(PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC)) as PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC;
            glFramebufferTextureLayer = GetDelegate("glFramebufferTextureLayer", typeof(PFNGLFRAMEBUFFERTEXTURELAYERPROC)) as PFNGLFRAMEBUFFERTEXTURELAYERPROC;
            glMapBufferRange = GetDelegate("glMapBufferRange", typeof(PFNGLMAPBUFFERRANGEPROC)) as PFNGLMAPBUFFERRANGEPROC;
            glFlushMappedBufferRange = GetDelegate("glFlushMappedBufferRange", typeof(PFNGLFLUSHMAPPEDBUFFERRANGEPROC)) as PFNGLFLUSHMAPPEDBUFFERRANGEPROC;
            glBindVertexArray = GetDelegate("glBindVertexArray", typeof(PFNGLBINDVERTEXARRAYPROC)) as PFNGLBINDVERTEXARRAYPROC;
            glDeleteVertexArrays = GetDelegate("glDeleteVertexArrays", typeof(PFNGLDELETEVERTEXARRAYSPROC)) as PFNGLDELETEVERTEXARRAYSPROC;
            glGenVertexArrays = GetDelegate("glGenVertexArrays", typeof(PFNGLGENVERTEXARRAYSPROC)) as PFNGLGENVERTEXARRAYSPROC;
            glIsVertexArray = GetDelegate("glIsVertexArray", typeof(PFNGLISVERTEXARRAYPROC)) as PFNGLISVERTEXARRAYPROC;
            glDrawArraysInstanced = GetDelegate("glDrawArraysInstanced", typeof(PFNGLDRAWARRAYSINSTANCEDPROC)) as PFNGLDRAWARRAYSINSTANCEDPROC;
            glDrawElementsInstanced = GetDelegate("glDrawElementsInstanced", typeof(PFNGLDRAWELEMENTSINSTANCEDPROC)) as PFNGLDRAWELEMENTSINSTANCEDPROC;
            glTexBuffer = GetDelegate("glTexBuffer", typeof(PFNGLTEXBUFFERPROC)) as PFNGLTEXBUFFERPROC;
            glPrimitiveRestartIndex = GetDelegate("glPrimitiveRestartIndex", typeof(PFNGLPRIMITIVERESTARTINDEXPROC)) as PFNGLPRIMITIVERESTARTINDEXPROC;
            glCopyBufferSubData = GetDelegate("glCopyBufferSubData", typeof(PFNGLCOPYBUFFERSUBDATAPROC)) as PFNGLCOPYBUFFERSUBDATAPROC;
            glGetUniformIndices = GetDelegate("glGetUniformIndices", typeof(PFNGLGETUNIFORMINDICESPROC)) as PFNGLGETUNIFORMINDICESPROC;
            glGetActiveUniformsiv = GetDelegate("glGetActiveUniformsiv", typeof(PFNGLGETACTIVEUNIFORMSIVPROC)) as PFNGLGETACTIVEUNIFORMSIVPROC;
            glGetActiveUniformName = GetDelegate("glGetActiveUniformName", typeof(PFNGLGETACTIVEUNIFORMNAMEPROC)) as PFNGLGETACTIVEUNIFORMNAMEPROC;
            glGetUniformBlockIndex = GetDelegate("glGetUniformBlockIndex", typeof(PFNGLGETUNIFORMBLOCKINDEXPROC)) as PFNGLGETUNIFORMBLOCKINDEXPROC;
            glGetActiveUniformBlockiv = GetDelegate("glGetActiveUniformBlockiv", typeof(PFNGLGETACTIVEUNIFORMBLOCKIVPROC)) as PFNGLGETACTIVEUNIFORMBLOCKIVPROC;
            glGetActiveUniformBlockName = GetDelegate("glGetActiveUniformBlockName", typeof(PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC)) as PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC;
            glUniformBlockBinding = GetDelegate("glUniformBlockBinding", typeof(PFNGLUNIFORMBLOCKBINDINGPROC)) as PFNGLUNIFORMBLOCKBINDINGPROC;
            glDrawElementsBaseVertex = GetDelegate("glDrawElementsBaseVertex", typeof(PFNGLDRAWELEMENTSBASEVERTEXPROC)) as PFNGLDRAWELEMENTSBASEVERTEXPROC;
            glDrawRangeElementsBaseVertex = GetDelegate("glDrawRangeElementsBaseVertex", typeof(PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC)) as PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC;
            glDrawElementsInstancedBaseVertex = GetDelegate("glDrawElementsInstancedBaseVertex", typeof(PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC)) as PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC;
            glMultiDrawElementsBaseVertex = GetDelegate("glMultiDrawElementsBaseVertex", typeof(PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC)) as PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC;
            glProvokingVertex = GetDelegate("glProvokingVertex", typeof(PFNGLPROVOKINGVERTEXPROC)) as PFNGLPROVOKINGVERTEXPROC;
            glFenceSync = GetDelegate("glFenceSync", typeof(PFNGLFENCESYNCPROC)) as PFNGLFENCESYNCPROC;
            glIsSync = GetDelegate("glIsSync", typeof(PFNGLISSYNCPROC)) as PFNGLISSYNCPROC;
            glDeleteSync = GetDelegate("glDeleteSync", typeof(PFNGLDELETESYNCPROC)) as PFNGLDELETESYNCPROC;
            glClientWaitSync = GetDelegate("glClientWaitSync", typeof(PFNGLCLIENTWAITSYNCPROC)) as PFNGLCLIENTWAITSYNCPROC;
            glWaitSync = GetDelegate("glWaitSync", typeof(PFNGLWAITSYNCPROC)) as PFNGLWAITSYNCPROC;
            glGetInteger64v = GetDelegate("glGetInteger64v", typeof(PFNGLGETINTEGER64VPROC)) as PFNGLGETINTEGER64VPROC;
            glGetSynciv = GetDelegate("glGetSynciv", typeof(PFNGLGETSYNCIVPROC)) as PFNGLGETSYNCIVPROC;
            glGetInteger64i_v = GetDelegate("glGetInteger64i_v", typeof(PFNGLGETINTEGER64I_VPROC)) as PFNGLGETINTEGER64I_VPROC;
            glGetBufferParameteri64v = GetDelegate("glGetBufferParameteri64v", typeof(PFNGLGETBUFFERPARAMETERI64VPROC)) as PFNGLGETBUFFERPARAMETERI64VPROC;
            glFramebufferTexture = GetDelegate("glFramebufferTexture", typeof(PFNGLFRAMEBUFFERTEXTUREPROC)) as PFNGLFRAMEBUFFERTEXTUREPROC;
            glTexImage2DMultisample = GetDelegate("glTexImage2DMultisample", typeof(PFNGLTEXIMAGE2DMULTISAMPLEPROC)) as PFNGLTEXIMAGE2DMULTISAMPLEPROC;
            glTexImage3DMultisample = GetDelegate("glTexImage3DMultisample", typeof(PFNGLTEXIMAGE3DMULTISAMPLEPROC)) as PFNGLTEXIMAGE3DMULTISAMPLEPROC;
            glGetMultisamplefv = GetDelegate("glGetMultisamplefv", typeof(PFNGLGETMULTISAMPLEFVPROC)) as PFNGLGETMULTISAMPLEFVPROC;
            glSampleMaski = GetDelegate("glSampleMaski", typeof(PFNGLSAMPLEMASKIPROC)) as PFNGLSAMPLEMASKIPROC;
            glBindFragDataLocationIndexed = GetDelegate("glBindFragDataLocationIndexed", typeof(PFNGLBINDFRAGDATALOCATIONINDEXEDPROC)) as PFNGLBINDFRAGDATALOCATIONINDEXEDPROC;
            glGetFragDataIndex = GetDelegate("glGetFragDataIndex", typeof(PFNGLGETFRAGDATAINDEXPROC)) as PFNGLGETFRAGDATAINDEXPROC;
            glGenSamplers = GetDelegate("glGenSamplers", typeof(PFNGLGENSAMPLERSPROC)) as PFNGLGENSAMPLERSPROC;
            glDeleteSamplers = GetDelegate("glDeleteSamplers", typeof(PFNGLDELETESAMPLERSPROC)) as PFNGLDELETESAMPLERSPROC;
            glIsSampler = GetDelegate("glIsSampler", typeof(PFNGLISSAMPLERPROC)) as PFNGLISSAMPLERPROC;
            glBindSampler = GetDelegate("glBindSampler", typeof(PFNGLBINDSAMPLERPROC)) as PFNGLBINDSAMPLERPROC;
            glSamplerParameteri = GetDelegate("glSamplerParameteri", typeof(PFNGLSAMPLERPARAMETERIPROC)) as PFNGLSAMPLERPARAMETERIPROC;
            glSamplerParameteriv = GetDelegate("glSamplerParameteriv", typeof(PFNGLSAMPLERPARAMETERIVPROC)) as PFNGLSAMPLERPARAMETERIVPROC;
            glSamplerParameterf = GetDelegate("glSamplerParameterf", typeof(PFNGLSAMPLERPARAMETERFPROC)) as PFNGLSAMPLERPARAMETERFPROC;
            glSamplerParameterfv = GetDelegate("glSamplerParameterfv", typeof(PFNGLSAMPLERPARAMETERFVPROC)) as PFNGLSAMPLERPARAMETERFVPROC;
            glSamplerParameterIiv = GetDelegate("glSamplerParameterIiv", typeof(PFNGLSAMPLERPARAMETERIIVPROC)) as PFNGLSAMPLERPARAMETERIIVPROC;
            glSamplerParameterIuiv = GetDelegate("glSamplerParameterIuiv", typeof(PFNGLSAMPLERPARAMETERIUIVPROC)) as PFNGLSAMPLERPARAMETERIUIVPROC;
            glGetSamplerParameteriv = GetDelegate("glGetSamplerParameteriv", typeof(PFNGLGETSAMPLERPARAMETERIVPROC)) as PFNGLGETSAMPLERPARAMETERIVPROC;
            glGetSamplerParameterIiv = GetDelegate("glGetSamplerParameterIiv", typeof(PFNGLGETSAMPLERPARAMETERIIVPROC)) as PFNGLGETSAMPLERPARAMETERIIVPROC;
            glGetSamplerParameterfv = GetDelegate("glGetSamplerParameterfv", typeof(PFNGLGETSAMPLERPARAMETERFVPROC)) as PFNGLGETSAMPLERPARAMETERFVPROC;
            glGetSamplerParameterIuiv = GetDelegate("glGetSamplerParameterIuiv", typeof(PFNGLGETSAMPLERPARAMETERIUIVPROC)) as PFNGLGETSAMPLERPARAMETERIUIVPROC;
            glQueryCounter = GetDelegate("glQueryCounter", typeof(PFNGLQUERYCOUNTERPROC)) as PFNGLQUERYCOUNTERPROC;
            glGetQueryObjecti64v = GetDelegate("glGetQueryObjecti64v", typeof(PFNGLGETQUERYOBJECTI64VPROC)) as PFNGLGETQUERYOBJECTI64VPROC;
            glGetQueryObjectui64v = GetDelegate("glGetQueryObjectui64v", typeof(PFNGLGETQUERYOBJECTUI64VPROC)) as PFNGLGETQUERYOBJECTUI64VPROC;
            glVertexAttribDivisor = GetDelegate("glVertexAttribDivisor", typeof(PFNGLVERTEXATTRIBDIVISORPROC)) as PFNGLVERTEXATTRIBDIVISORPROC;
            glVertexAttribP1ui = GetDelegate("glVertexAttribP1ui", typeof(PFNGLVERTEXATTRIBP1UIPROC)) as PFNGLVERTEXATTRIBP1UIPROC;
            glVertexAttribP1uiv = GetDelegate("glVertexAttribP1uiv", typeof(PFNGLVERTEXATTRIBP1UIVPROC)) as PFNGLVERTEXATTRIBP1UIVPROC;
            glVertexAttribP2ui = GetDelegate("glVertexAttribP2ui", typeof(PFNGLVERTEXATTRIBP2UIPROC)) as PFNGLVERTEXATTRIBP2UIPROC;
            glVertexAttribP2uiv = GetDelegate("glVertexAttribP2uiv", typeof(PFNGLVERTEXATTRIBP2UIVPROC)) as PFNGLVERTEXATTRIBP2UIVPROC;
            glVertexAttribP3ui = GetDelegate("glVertexAttribP3ui", typeof(PFNGLVERTEXATTRIBP3UIPROC)) as PFNGLVERTEXATTRIBP3UIPROC;
            glVertexAttribP3uiv = GetDelegate("glVertexAttribP3uiv", typeof(PFNGLVERTEXATTRIBP3UIVPROC)) as PFNGLVERTEXATTRIBP3UIVPROC;
            glVertexAttribP4ui = GetDelegate("glVertexAttribP4ui", typeof(PFNGLVERTEXATTRIBP4UIPROC)) as PFNGLVERTEXATTRIBP4UIPROC;
            glVertexAttribP4uiv = GetDelegate("glVertexAttribP4uiv", typeof(PFNGLVERTEXATTRIBP4UIVPROC)) as PFNGLVERTEXATTRIBP4UIVPROC;
            glMinSampleShading = GetDelegate("glMinSampleShading", typeof(PFNGLMINSAMPLESHADINGPROC)) as PFNGLMINSAMPLESHADINGPROC;
            glBlendEquationi = GetDelegate("glBlendEquationi", typeof(PFNGLBLENDEQUATIONIPROC)) as PFNGLBLENDEQUATIONIPROC;
            glBlendEquationSeparatei = GetDelegate("glBlendEquationSeparatei", typeof(PFNGLBLENDEQUATIONSEPARATEIPROC)) as PFNGLBLENDEQUATIONSEPARATEIPROC;
            glBlendFunci = GetDelegate("glBlendFunci", typeof(PFNGLBLENDFUNCIPROC)) as PFNGLBLENDFUNCIPROC;
            glBlendFuncSeparatei = GetDelegate("glBlendFuncSeparatei", typeof(PFNGLBLENDFUNCSEPARATEIPROC)) as PFNGLBLENDFUNCSEPARATEIPROC;
            glDrawArraysIndirect = GetDelegate("glDrawArraysIndirect", typeof(PFNGLDRAWARRAYSINDIRECTPROC)) as PFNGLDRAWARRAYSINDIRECTPROC;
            glDrawElementsIndirect = GetDelegate("glDrawElementsIndirect", typeof(PFNGLDRAWELEMENTSINDIRECTPROC)) as PFNGLDRAWELEMENTSINDIRECTPROC;
            glUniform1d = GetDelegate("glUniform1d", typeof(PFNGLUNIFORM1DPROC)) as PFNGLUNIFORM1DPROC;
            glUniform2d = GetDelegate("glUniform2d", typeof(PFNGLUNIFORM2DPROC)) as PFNGLUNIFORM2DPROC;
            glUniform3d = GetDelegate("glUniform3d", typeof(PFNGLUNIFORM3DPROC)) as PFNGLUNIFORM3DPROC;
            glUniform4d = GetDelegate("glUniform4d", typeof(PFNGLUNIFORM4DPROC)) as PFNGLUNIFORM4DPROC;
            glUniform1dv = GetDelegate("glUniform1dv", typeof(PFNGLUNIFORM1DVPROC)) as PFNGLUNIFORM1DVPROC;
            glUniform2dv = GetDelegate("glUniform2dv", typeof(PFNGLUNIFORM2DVPROC)) as PFNGLUNIFORM2DVPROC;
            glUniform3dv = GetDelegate("glUniform3dv", typeof(PFNGLUNIFORM3DVPROC)) as PFNGLUNIFORM3DVPROC;
            glUniform4dv = GetDelegate("glUniform4dv", typeof(PFNGLUNIFORM4DVPROC)) as PFNGLUNIFORM4DVPROC;
            glUniformMatrix2dv = GetDelegate("glUniformMatrix2dv", typeof(PFNGLUNIFORMMATRIX2DVPROC)) as PFNGLUNIFORMMATRIX2DVPROC;
            glUniformMatrix3dv = GetDelegate("glUniformMatrix3dv", typeof(PFNGLUNIFORMMATRIX3DVPROC)) as PFNGLUNIFORMMATRIX3DVPROC;
            glUniformMatrix4dv = GetDelegate("glUniformMatrix4dv", typeof(PFNGLUNIFORMMATRIX4DVPROC)) as PFNGLUNIFORMMATRIX4DVPROC;
            glUniformMatrix2x3dv = GetDelegate("glUniformMatrix2x3dv", typeof(PFNGLUNIFORMMATRIX2X3DVPROC)) as PFNGLUNIFORMMATRIX2X3DVPROC;
            glUniformMatrix2x4dv = GetDelegate("glUniformMatrix2x4dv", typeof(PFNGLUNIFORMMATRIX2X4DVPROC)) as PFNGLUNIFORMMATRIX2X4DVPROC;
            glUniformMatrix3x2dv = GetDelegate("glUniformMatrix3x2dv", typeof(PFNGLUNIFORMMATRIX3X2DVPROC)) as PFNGLUNIFORMMATRIX3X2DVPROC;
            glUniformMatrix3x4dv = GetDelegate("glUniformMatrix3x4dv", typeof(PFNGLUNIFORMMATRIX3X4DVPROC)) as PFNGLUNIFORMMATRIX3X4DVPROC;
            glUniformMatrix4x2dv = GetDelegate("glUniformMatrix4x2dv", typeof(PFNGLUNIFORMMATRIX4X2DVPROC)) as PFNGLUNIFORMMATRIX4X2DVPROC;
            glUniformMatrix4x3dv = GetDelegate("glUniformMatrix4x3dv", typeof(PFNGLUNIFORMMATRIX4X3DVPROC)) as PFNGLUNIFORMMATRIX4X3DVPROC;
            glGetUniformdv = GetDelegate("glGetUniformdv", typeof(PFNGLGETUNIFORMDVPROC)) as PFNGLGETUNIFORMDVPROC;
            glGetSubroutineUniformLocation = GetDelegate("glGetSubroutineUniformLocation", typeof(PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC)) as PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC;
            glGetSubroutineIndex = GetDelegate("glGetSubroutineIndex", typeof(PFNGLGETSUBROUTINEINDEXPROC)) as PFNGLGETSUBROUTINEINDEXPROC;
            glGetActiveSubroutineUniformiv = GetDelegate("glGetActiveSubroutineUniformiv", typeof(PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC)) as PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC;
            glGetActiveSubroutineUniformName = GetDelegate("glGetActiveSubroutineUniformName", typeof(PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC)) as PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC;
            glGetActiveSubroutineName = GetDelegate("glGetActiveSubroutineName", typeof(PFNGLGETACTIVESUBROUTINENAMEPROC)) as PFNGLGETACTIVESUBROUTINENAMEPROC;
            glUniformSubroutinesuiv = GetDelegate("glUniformSubroutinesuiv", typeof(PFNGLUNIFORMSUBROUTINESUIVPROC)) as PFNGLUNIFORMSUBROUTINESUIVPROC;
            glGetUniformSubroutineuiv = GetDelegate("glGetUniformSubroutineuiv", typeof(PFNGLGETUNIFORMSUBROUTINEUIVPROC)) as PFNGLGETUNIFORMSUBROUTINEUIVPROC;
            glGetProgramStageiv = GetDelegate("glGetProgramStageiv", typeof(PFNGLGETPROGRAMSTAGEIVPROC)) as PFNGLGETPROGRAMSTAGEIVPROC;
            glPatchParameteri = GetDelegate("glPatchParameteri", typeof(PFNGLPATCHPARAMETERIPROC)) as PFNGLPATCHPARAMETERIPROC;
            glPatchParameterfv = GetDelegate("glPatchParameterfv", typeof(PFNGLPATCHPARAMETERFVPROC)) as PFNGLPATCHPARAMETERFVPROC;
            glBindTransformFeedback = GetDelegate("glBindTransformFeedback", typeof(PFNGLBINDTRANSFORMFEEDBACKPROC)) as PFNGLBINDTRANSFORMFEEDBACKPROC;
            glDeleteTransformFeedbacks = GetDelegate("glDeleteTransformFeedbacks", typeof(PFNGLDELETETRANSFORMFEEDBACKSPROC)) as PFNGLDELETETRANSFORMFEEDBACKSPROC;
            glGenTransformFeedbacks = GetDelegate("glGenTransformFeedbacks", typeof(PFNGLGENTRANSFORMFEEDBACKSPROC)) as PFNGLGENTRANSFORMFEEDBACKSPROC;
            glIsTransformFeedback = GetDelegate("glIsTransformFeedback", typeof(PFNGLISTRANSFORMFEEDBACKPROC)) as PFNGLISTRANSFORMFEEDBACKPROC;
            glPauseTransformFeedback = GetDelegate("glPauseTransformFeedback", typeof(PFNGLPAUSETRANSFORMFEEDBACKPROC)) as PFNGLPAUSETRANSFORMFEEDBACKPROC;
            glResumeTransformFeedback = GetDelegate("glResumeTransformFeedback", typeof(PFNGLRESUMETRANSFORMFEEDBACKPROC)) as PFNGLRESUMETRANSFORMFEEDBACKPROC;
            glDrawTransformFeedback = GetDelegate("glDrawTransformFeedback", typeof(PFNGLDRAWTRANSFORMFEEDBACKPROC)) as PFNGLDRAWTRANSFORMFEEDBACKPROC;
            glDrawTransformFeedbackStream = GetDelegate("glDrawTransformFeedbackStream", typeof(PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC)) as PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC;
            glBeginQueryIndexed = GetDelegate("glBeginQueryIndexed", typeof(PFNGLBEGINQUERYINDEXEDPROC)) as PFNGLBEGINQUERYINDEXEDPROC;
            glEndQueryIndexed = GetDelegate("glEndQueryIndexed", typeof(PFNGLENDQUERYINDEXEDPROC)) as PFNGLENDQUERYINDEXEDPROC;
            glGetQueryIndexediv = GetDelegate("glGetQueryIndexediv", typeof(PFNGLGETQUERYINDEXEDIVPROC)) as PFNGLGETQUERYINDEXEDIVPROC;
            glReleaseShaderCompiler = GetDelegate("glReleaseShaderCompiler", typeof(PFNGLRELEASESHADERCOMPILERPROC)) as PFNGLRELEASESHADERCOMPILERPROC;
            glShaderBinary = GetDelegate("glShaderBinary", typeof(PFNGLSHADERBINARYPROC)) as PFNGLSHADERBINARYPROC;
            glGetShaderPrecisionFormat = GetDelegate("glGetShaderPrecisionFormat", typeof(PFNGLGETSHADERPRECISIONFORMATPROC)) as PFNGLGETSHADERPRECISIONFORMATPROC;
            glDepthRangef = GetDelegate("glDepthRangef", typeof(PFNGLDEPTHRANGEFPROC)) as PFNGLDEPTHRANGEFPROC;
            glClearDepthf = GetDelegate("glClearDepthf", typeof(PFNGLCLEARDEPTHFPROC)) as PFNGLCLEARDEPTHFPROC;
            glGetProgramBinary = GetDelegate("glGetProgramBinary", typeof(PFNGLGETPROGRAMBINARYPROC)) as PFNGLGETPROGRAMBINARYPROC;
            glProgramBinary = GetDelegate("glProgramBinary", typeof(PFNGLPROGRAMBINARYPROC)) as PFNGLPROGRAMBINARYPROC;
            glProgramParameteri = GetDelegate("glProgramParameteri", typeof(PFNGLPROGRAMPARAMETERIPROC)) as PFNGLPROGRAMPARAMETERIPROC;
            glUseProgramStages = GetDelegate("glUseProgramStages", typeof(PFNGLUSEPROGRAMSTAGESPROC)) as PFNGLUSEPROGRAMSTAGESPROC;
            glActiveShaderProgram = GetDelegate("glActiveShaderProgram", typeof(PFNGLACTIVESHADERPROGRAMPROC)) as PFNGLACTIVESHADERPROGRAMPROC;
            glCreateShaderProgramv = GetDelegate("glCreateShaderProgramv", typeof(PFNGLCREATESHADERPROGRAMVPROC)) as PFNGLCREATESHADERPROGRAMVPROC;
            glBindProgramPipeline = GetDelegate("glBindProgramPipeline", typeof(PFNGLBINDPROGRAMPIPELINEPROC)) as PFNGLBINDPROGRAMPIPELINEPROC;
            glDeleteProgramPipelines = GetDelegate("glDeleteProgramPipelines", typeof(PFNGLDELETEPROGRAMPIPELINESPROC)) as PFNGLDELETEPROGRAMPIPELINESPROC;
            glGenProgramPipelines = GetDelegate("glGenProgramPipelines", typeof(PFNGLGENPROGRAMPIPELINESPROC)) as PFNGLGENPROGRAMPIPELINESPROC;
            glIsProgramPipeline = GetDelegate("glIsProgramPipeline", typeof(PFNGLISPROGRAMPIPELINEPROC)) as PFNGLISPROGRAMPIPELINEPROC;
            glGetProgramPipelineiv = GetDelegate("glGetProgramPipelineiv", typeof(PFNGLGETPROGRAMPIPELINEIVPROC)) as PFNGLGETPROGRAMPIPELINEIVPROC;
            glProgramUniform1i = GetDelegate("glProgramUniform1i", typeof(PFNGLPROGRAMUNIFORM1IPROC)) as PFNGLPROGRAMUNIFORM1IPROC;
            glProgramUniform1iv = GetDelegate("glProgramUniform1iv", typeof(PFNGLPROGRAMUNIFORM1IVPROC)) as PFNGLPROGRAMUNIFORM1IVPROC;
            glProgramUniform1f = GetDelegate("glProgramUniform1f", typeof(PFNGLPROGRAMUNIFORM1FPROC)) as PFNGLPROGRAMUNIFORM1FPROC;
            glProgramUniform1fv = GetDelegate("glProgramUniform1fv", typeof(PFNGLPROGRAMUNIFORM1FVPROC)) as PFNGLPROGRAMUNIFORM1FVPROC;
            glProgramUniform1d = GetDelegate("glProgramUniform1d", typeof(PFNGLPROGRAMUNIFORM1DPROC)) as PFNGLPROGRAMUNIFORM1DPROC;
            glProgramUniform1dv = GetDelegate("glProgramUniform1dv", typeof(PFNGLPROGRAMUNIFORM1DVPROC)) as PFNGLPROGRAMUNIFORM1DVPROC;
            glProgramUniform1ui = GetDelegate("glProgramUniform1ui", typeof(PFNGLPROGRAMUNIFORM1UIPROC)) as PFNGLPROGRAMUNIFORM1UIPROC;
            glProgramUniform1uiv = GetDelegate("glProgramUniform1uiv", typeof(PFNGLPROGRAMUNIFORM1UIVPROC)) as PFNGLPROGRAMUNIFORM1UIVPROC;
            glProgramUniform2i = GetDelegate("glProgramUniform2i", typeof(PFNGLPROGRAMUNIFORM2IPROC)) as PFNGLPROGRAMUNIFORM2IPROC;
            glProgramUniform2iv = GetDelegate("glProgramUniform2iv", typeof(PFNGLPROGRAMUNIFORM2IVPROC)) as PFNGLPROGRAMUNIFORM2IVPROC;
            glProgramUniform2f = GetDelegate("glProgramUniform2f", typeof(PFNGLPROGRAMUNIFORM2FPROC)) as PFNGLPROGRAMUNIFORM2FPROC;
            glProgramUniform2fv = GetDelegate("glProgramUniform2fv", typeof(PFNGLPROGRAMUNIFORM2FVPROC)) as PFNGLPROGRAMUNIFORM2FVPROC;
            glProgramUniform2d = GetDelegate("glProgramUniform2d", typeof(PFNGLPROGRAMUNIFORM2DPROC)) as PFNGLPROGRAMUNIFORM2DPROC;
            glProgramUniform2dv = GetDelegate("glProgramUniform2dv", typeof(PFNGLPROGRAMUNIFORM2DVPROC)) as PFNGLPROGRAMUNIFORM2DVPROC;
            glProgramUniform2ui = GetDelegate("glProgramUniform2ui", typeof(PFNGLPROGRAMUNIFORM2UIPROC)) as PFNGLPROGRAMUNIFORM2UIPROC;
            glProgramUniform2uiv = GetDelegate("glProgramUniform2uiv", typeof(PFNGLPROGRAMUNIFORM2UIVPROC)) as PFNGLPROGRAMUNIFORM2UIVPROC;
            glProgramUniform3i = GetDelegate("glProgramUniform3i", typeof(PFNGLPROGRAMUNIFORM3IPROC)) as PFNGLPROGRAMUNIFORM3IPROC;
            glProgramUniform3iv = GetDelegate("glProgramUniform3iv", typeof(PFNGLPROGRAMUNIFORM3IVPROC)) as PFNGLPROGRAMUNIFORM3IVPROC;
            glProgramUniform3f = GetDelegate("glProgramUniform3f", typeof(PFNGLPROGRAMUNIFORM3FPROC)) as PFNGLPROGRAMUNIFORM3FPROC;
            glProgramUniform3fv = GetDelegate("glProgramUniform3fv", typeof(PFNGLPROGRAMUNIFORM3FVPROC)) as PFNGLPROGRAMUNIFORM3FVPROC;
            glProgramUniform3d = GetDelegate("glProgramUniform3d", typeof(PFNGLPROGRAMUNIFORM3DPROC)) as PFNGLPROGRAMUNIFORM3DPROC;
            glProgramUniform3dv = GetDelegate("glProgramUniform3dv", typeof(PFNGLPROGRAMUNIFORM3DVPROC)) as PFNGLPROGRAMUNIFORM3DVPROC;
            glProgramUniform3ui = GetDelegate("glProgramUniform3ui", typeof(PFNGLPROGRAMUNIFORM3UIPROC)) as PFNGLPROGRAMUNIFORM3UIPROC;
            glProgramUniform3uiv = GetDelegate("glProgramUniform3uiv", typeof(PFNGLPROGRAMUNIFORM3UIVPROC)) as PFNGLPROGRAMUNIFORM3UIVPROC;
            glProgramUniform4i = GetDelegate("glProgramUniform4i", typeof(PFNGLPROGRAMUNIFORM4IPROC)) as PFNGLPROGRAMUNIFORM4IPROC;
            glProgramUniform4iv = GetDelegate("glProgramUniform4iv", typeof(PFNGLPROGRAMUNIFORM4IVPROC)) as PFNGLPROGRAMUNIFORM4IVPROC;
            glProgramUniform4f = GetDelegate("glProgramUniform4f", typeof(PFNGLPROGRAMUNIFORM4FPROC)) as PFNGLPROGRAMUNIFORM4FPROC;
            glProgramUniform4fv = GetDelegate("glProgramUniform4fv", typeof(PFNGLPROGRAMUNIFORM4FVPROC)) as PFNGLPROGRAMUNIFORM4FVPROC;
            glProgramUniform4d = GetDelegate("glProgramUniform4d", typeof(PFNGLPROGRAMUNIFORM4DPROC)) as PFNGLPROGRAMUNIFORM4DPROC;
            glProgramUniform4dv = GetDelegate("glProgramUniform4dv", typeof(PFNGLPROGRAMUNIFORM4DVPROC)) as PFNGLPROGRAMUNIFORM4DVPROC;
            glProgramUniform4ui = GetDelegate("glProgramUniform4ui", typeof(PFNGLPROGRAMUNIFORM4UIPROC)) as PFNGLPROGRAMUNIFORM4UIPROC;
            glProgramUniform4uiv = GetDelegate("glProgramUniform4uiv", typeof(PFNGLPROGRAMUNIFORM4UIVPROC)) as PFNGLPROGRAMUNIFORM4UIVPROC;
            glProgramUniformMatrix2fv = GetDelegate("glProgramUniformMatrix2fv", typeof(PFNGLPROGRAMUNIFORMMATRIX2FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2FVPROC;
            glProgramUniformMatrix3fv = GetDelegate("glProgramUniformMatrix3fv", typeof(PFNGLPROGRAMUNIFORMMATRIX3FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3FVPROC;
            glProgramUniformMatrix4fv = GetDelegate("glProgramUniformMatrix4fv", typeof(PFNGLPROGRAMUNIFORMMATRIX4FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4FVPROC;
            glProgramUniformMatrix2dv = GetDelegate("glProgramUniformMatrix2dv", typeof(PFNGLPROGRAMUNIFORMMATRIX2DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2DVPROC;
            glProgramUniformMatrix3dv = GetDelegate("glProgramUniformMatrix3dv", typeof(PFNGLPROGRAMUNIFORMMATRIX3DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3DVPROC;
            glProgramUniformMatrix4dv = GetDelegate("glProgramUniformMatrix4dv", typeof(PFNGLPROGRAMUNIFORMMATRIX4DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4DVPROC;
            glProgramUniformMatrix2x3fv = GetDelegate("glProgramUniformMatrix2x3fv", typeof(PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC;
            glProgramUniformMatrix3x2fv = GetDelegate("glProgramUniformMatrix3x2fv", typeof(PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC;
            glProgramUniformMatrix2x4fv = GetDelegate("glProgramUniformMatrix2x4fv", typeof(PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC;
            glProgramUniformMatrix4x2fv = GetDelegate("glProgramUniformMatrix4x2fv", typeof(PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC;
            glProgramUniformMatrix3x4fv = GetDelegate("glProgramUniformMatrix3x4fv", typeof(PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC;
            glProgramUniformMatrix4x3fv = GetDelegate("glProgramUniformMatrix4x3fv", typeof(PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC;
            glProgramUniformMatrix2x3dv = GetDelegate("glProgramUniformMatrix2x3dv", typeof(PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC;
            glProgramUniformMatrix3x2dv = GetDelegate("glProgramUniformMatrix3x2dv", typeof(PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC;
            glProgramUniformMatrix2x4dv = GetDelegate("glProgramUniformMatrix2x4dv", typeof(PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC;
            glProgramUniformMatrix4x2dv = GetDelegate("glProgramUniformMatrix4x2dv", typeof(PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC;
            glProgramUniformMatrix3x4dv = GetDelegate("glProgramUniformMatrix3x4dv", typeof(PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC;
            glProgramUniformMatrix4x3dv = GetDelegate("glProgramUniformMatrix4x3dv", typeof(PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC)) as PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC;
            glValidateProgramPipeline = GetDelegate("glValidateProgramPipeline", typeof(PFNGLVALIDATEPROGRAMPIPELINEPROC)) as PFNGLVALIDATEPROGRAMPIPELINEPROC;
            glGetProgramPipelineInfoLog = GetDelegate("glGetProgramPipelineInfoLog", typeof(PFNGLGETPROGRAMPIPELINEINFOLOGPROC)) as PFNGLGETPROGRAMPIPELINEINFOLOGPROC;
            glVertexAttribL1d = GetDelegate("glVertexAttribL1d", typeof(PFNGLVERTEXATTRIBL1DPROC)) as PFNGLVERTEXATTRIBL1DPROC;
            glVertexAttribL2d = GetDelegate("glVertexAttribL2d", typeof(PFNGLVERTEXATTRIBL2DPROC)) as PFNGLVERTEXATTRIBL2DPROC;
            glVertexAttribL3d = GetDelegate("glVertexAttribL3d", typeof(PFNGLVERTEXATTRIBL3DPROC)) as PFNGLVERTEXATTRIBL3DPROC;
            glVertexAttribL4d = GetDelegate("glVertexAttribL4d", typeof(PFNGLVERTEXATTRIBL4DPROC)) as PFNGLVERTEXATTRIBL4DPROC;
            glVertexAttribL1dv = GetDelegate("glVertexAttribL1dv", typeof(PFNGLVERTEXATTRIBL1DVPROC)) as PFNGLVERTEXATTRIBL1DVPROC;
            glVertexAttribL2dv = GetDelegate("glVertexAttribL2dv", typeof(PFNGLVERTEXATTRIBL2DVPROC)) as PFNGLVERTEXATTRIBL2DVPROC;
            glVertexAttribL3dv = GetDelegate("glVertexAttribL3dv", typeof(PFNGLVERTEXATTRIBL3DVPROC)) as PFNGLVERTEXATTRIBL3DVPROC;
            glVertexAttribL4dv = GetDelegate("glVertexAttribL4dv", typeof(PFNGLVERTEXATTRIBL4DVPROC)) as PFNGLVERTEXATTRIBL4DVPROC;
            glVertexAttribLPointer = GetDelegate("glVertexAttribLPointer", typeof(PFNGLVERTEXATTRIBLPOINTERPROC)) as PFNGLVERTEXATTRIBLPOINTERPROC;
            glGetVertexAttribLdv = GetDelegate("glGetVertexAttribLdv", typeof(PFNGLGETVERTEXATTRIBLDVPROC)) as PFNGLGETVERTEXATTRIBLDVPROC;
            glViewportArrayv = GetDelegate("glViewportArrayv", typeof(PFNGLVIEWPORTARRAYVPROC)) as PFNGLVIEWPORTARRAYVPROC;
            glViewportIndexedf = GetDelegate("glViewportIndexedf", typeof(PFNGLVIEWPORTINDEXEDFPROC)) as PFNGLVIEWPORTINDEXEDFPROC;
            glViewportIndexedfv = GetDelegate("glViewportIndexedfv", typeof(PFNGLVIEWPORTINDEXEDFVPROC)) as PFNGLVIEWPORTINDEXEDFVPROC;
            glScissorArrayv = GetDelegate("glScissorArrayv", typeof(PFNGLSCISSORARRAYVPROC)) as PFNGLSCISSORARRAYVPROC;
            glScissorIndexed = GetDelegate("glScissorIndexed", typeof(PFNGLSCISSORINDEXEDPROC)) as PFNGLSCISSORINDEXEDPROC;
            glScissorIndexedv = GetDelegate("glScissorIndexedv", typeof(PFNGLSCISSORINDEXEDVPROC)) as PFNGLSCISSORINDEXEDVPROC;
            glDepthRangeArrayv = GetDelegate("glDepthRangeArrayv", typeof(PFNGLDEPTHRANGEARRAYVPROC)) as PFNGLDEPTHRANGEARRAYVPROC;
            glDepthRangeIndexed = GetDelegate("glDepthRangeIndexed", typeof(PFNGLDEPTHRANGEINDEXEDPROC)) as PFNGLDEPTHRANGEINDEXEDPROC;
            glGetFloati_v = GetDelegate("glGetFloati_v", typeof(PFNGLGETFLOATI_VPROC)) as PFNGLGETFLOATI_VPROC;
            glGetDoublei_v = GetDelegate("glGetDoublei_v", typeof(PFNGLGETDOUBLEI_VPROC)) as PFNGLGETDOUBLEI_VPROC;
            glDrawArraysInstancedBaseInstance = GetDelegate("glDrawArraysInstancedBaseInstance", typeof(PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC)) as PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC;
            glDrawElementsInstancedBaseInstance = GetDelegate("glDrawElementsInstancedBaseInstance", typeof(PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC)) as PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC;
            glDrawElementsInstancedBaseVertexBaseInstance = GetDelegate("glDrawElementsInstancedBaseVertexBaseInstance", typeof(PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC)) as PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC;
            glGetInternalformativ = GetDelegate("glGetInternalformativ", typeof(PFNGLGETINTERNALFORMATIVPROC)) as PFNGLGETINTERNALFORMATIVPROC;
            glGetActiveAtomicCounterBufferiv = GetDelegate("glGetActiveAtomicCounterBufferiv", typeof(PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC)) as PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC;
            glBindImageTexture = GetDelegate("glBindImageTexture", typeof(PFNGLBINDIMAGETEXTUREPROC)) as PFNGLBINDIMAGETEXTUREPROC;
            glMemoryBarrier = GetDelegate("glMemoryBarrier", typeof(PFNGLMEMORYBARRIERPROC)) as PFNGLMEMORYBARRIERPROC;
            glTexStorage1D = GetDelegate("glTexStorage1D", typeof(PFNGLTEXSTORAGE1DPROC)) as PFNGLTEXSTORAGE1DPROC;
            glTexStorage2D = GetDelegate("glTexStorage2D", typeof(PFNGLTEXSTORAGE2DPROC)) as PFNGLTEXSTORAGE2DPROC;
            glTexStorage3D = GetDelegate("glTexStorage3D", typeof(PFNGLTEXSTORAGE3DPROC)) as PFNGLTEXSTORAGE3DPROC;
            glDrawTransformFeedbackInstanced = GetDelegate("glDrawTransformFeedbackInstanced", typeof(PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC)) as PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC;
            glDrawTransformFeedbackStreamInstanced = GetDelegate("glDrawTransformFeedbackStreamInstanced", typeof(PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC)) as PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC;
            glClearBufferData = GetDelegate("glClearBufferData", typeof(PFNGLCLEARBUFFERDATAPROC)) as PFNGLCLEARBUFFERDATAPROC;
            glClearBufferSubData = GetDelegate("glClearBufferSubData", typeof(PFNGLCLEARBUFFERSUBDATAPROC)) as PFNGLCLEARBUFFERSUBDATAPROC;
            glDispatchCompute = GetDelegate("glDispatchCompute", typeof(PFNGLDISPATCHCOMPUTEPROC)) as PFNGLDISPATCHCOMPUTEPROC;
            glDispatchComputeIndirect = GetDelegate("glDispatchComputeIndirect", typeof(PFNGLDISPATCHCOMPUTEINDIRECTPROC)) as PFNGLDISPATCHCOMPUTEINDIRECTPROC;
            glCopyImageSubData = GetDelegate("glCopyImageSubData", typeof(PFNGLCOPYIMAGESUBDATAPROC)) as PFNGLCOPYIMAGESUBDATAPROC;
            glFramebufferParameteri = GetDelegate("glFramebufferParameteri", typeof(PFNGLFRAMEBUFFERPARAMETERIPROC)) as PFNGLFRAMEBUFFERPARAMETERIPROC;
            glGetFramebufferParameteriv = GetDelegate("glGetFramebufferParameteriv", typeof(PFNGLGETFRAMEBUFFERPARAMETERIVPROC)) as PFNGLGETFRAMEBUFFERPARAMETERIVPROC;
            glGetInternalformati64v = GetDelegate("glGetInternalformati64v", typeof(PFNGLGETINTERNALFORMATI64VPROC)) as PFNGLGETINTERNALFORMATI64VPROC;
            glInvalidateTexSubImage = GetDelegate("glInvalidateTexSubImage", typeof(PFNGLINVALIDATETEXSUBIMAGEPROC)) as PFNGLINVALIDATETEXSUBIMAGEPROC;
            glInvalidateTexImage = GetDelegate("glInvalidateTexImage", typeof(PFNGLINVALIDATETEXIMAGEPROC)) as PFNGLINVALIDATETEXIMAGEPROC;
            glInvalidateBufferSubData = GetDelegate("glInvalidateBufferSubData", typeof(PFNGLINVALIDATEBUFFERSUBDATAPROC)) as PFNGLINVALIDATEBUFFERSUBDATAPROC;
            glInvalidateBufferData = GetDelegate("glInvalidateBufferData", typeof(PFNGLINVALIDATEBUFFERDATAPROC)) as PFNGLINVALIDATEBUFFERDATAPROC;
            glInvalidateFramebuffer = GetDelegate("glInvalidateFramebuffer", typeof(PFNGLINVALIDATEFRAMEBUFFERPROC)) as PFNGLINVALIDATEFRAMEBUFFERPROC;
            glInvalidateSubFramebuffer = GetDelegate("glInvalidateSubFramebuffer", typeof(PFNGLINVALIDATESUBFRAMEBUFFERPROC)) as PFNGLINVALIDATESUBFRAMEBUFFERPROC;
            glMultiDrawArraysIndirect = GetDelegate("glMultiDrawArraysIndirect", typeof(PFNGLMULTIDRAWARRAYSINDIRECTPROC)) as PFNGLMULTIDRAWARRAYSINDIRECTPROC;
            glMultiDrawElementsIndirect = GetDelegate("glMultiDrawElementsIndirect", typeof(PFNGLMULTIDRAWELEMENTSINDIRECTPROC)) as PFNGLMULTIDRAWELEMENTSINDIRECTPROC;
            glGetProgramInterfaceiv = GetDelegate("glGetProgramInterfaceiv", typeof(PFNGLGETPROGRAMINTERFACEIVPROC)) as PFNGLGETPROGRAMINTERFACEIVPROC;
            glGetProgramResourceIndex = GetDelegate("glGetProgramResourceIndex", typeof(PFNGLGETPROGRAMRESOURCEINDEXPROC)) as PFNGLGETPROGRAMRESOURCEINDEXPROC;
            glGetProgramResourceName = GetDelegate("glGetProgramResourceName", typeof(PFNGLGETPROGRAMRESOURCENAMEPROC)) as PFNGLGETPROGRAMRESOURCENAMEPROC;
            glGetProgramResourceiv = GetDelegate("glGetProgramResourceiv", typeof(PFNGLGETPROGRAMRESOURCEIVPROC)) as PFNGLGETPROGRAMRESOURCEIVPROC;
            glGetProgramResourceLocation = GetDelegate("glGetProgramResourceLocation", typeof(PFNGLGETPROGRAMRESOURCELOCATIONPROC)) as PFNGLGETPROGRAMRESOURCELOCATIONPROC;
            glGetProgramResourceLocationIndex = GetDelegate("glGetProgramResourceLocationIndex", typeof(PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC)) as PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC;
            glShaderStorageBlockBinding = GetDelegate("glShaderStorageBlockBinding", typeof(PFNGLSHADERSTORAGEBLOCKBINDINGPROC)) as PFNGLSHADERSTORAGEBLOCKBINDINGPROC;
            glTexBufferRange = GetDelegate("glTexBufferRange", typeof(PFNGLTEXBUFFERRANGEPROC)) as PFNGLTEXBUFFERRANGEPROC;
            glTexStorage2DMultisample = GetDelegate("glTexStorage2DMultisample", typeof(PFNGLTEXSTORAGE2DMULTISAMPLEPROC)) as PFNGLTEXSTORAGE2DMULTISAMPLEPROC;
            glTexStorage3DMultisample = GetDelegate("glTexStorage3DMultisample", typeof(PFNGLTEXSTORAGE3DMULTISAMPLEPROC)) as PFNGLTEXSTORAGE3DMULTISAMPLEPROC;
            glTextureView = GetDelegate("glTextureView", typeof(PFNGLTEXTUREVIEWPROC)) as PFNGLTEXTUREVIEWPROC;
            glBindVertexBuffer = GetDelegate("glBindVertexBuffer", typeof(PFNGLBINDVERTEXBUFFERPROC)) as PFNGLBINDVERTEXBUFFERPROC;
            glVertexAttribFormat = GetDelegate("glVertexAttribFormat", typeof(PFNGLVERTEXATTRIBFORMATPROC)) as PFNGLVERTEXATTRIBFORMATPROC;
            glVertexAttribIFormat = GetDelegate("glVertexAttribIFormat", typeof(PFNGLVERTEXATTRIBIFORMATPROC)) as PFNGLVERTEXATTRIBIFORMATPROC;
            glVertexAttribLFormat = GetDelegate("glVertexAttribLFormat", typeof(PFNGLVERTEXATTRIBLFORMATPROC)) as PFNGLVERTEXATTRIBLFORMATPROC;
            glVertexAttribBinding = GetDelegate("glVertexAttribBinding", typeof(PFNGLVERTEXATTRIBBINDINGPROC)) as PFNGLVERTEXATTRIBBINDINGPROC;
            glVertexBindingDivisor = GetDelegate("glVertexBindingDivisor", typeof(PFNGLVERTEXBINDINGDIVISORPROC)) as PFNGLVERTEXBINDINGDIVISORPROC;
            glDebugMessageControl = GetDelegate("glDebugMessageControl", typeof(PFNGLDEBUGMESSAGECONTROLPROC)) as PFNGLDEBUGMESSAGECONTROLPROC;
            glDebugMessageInsert = GetDelegate("glDebugMessageInsert", typeof(PFNGLDEBUGMESSAGEINSERTPROC)) as PFNGLDEBUGMESSAGEINSERTPROC;
            glDebugMessageCallback = GetDelegate("glDebugMessageCallback", typeof(PFNGLDEBUGMESSAGECALLBACKPROC)) as PFNGLDEBUGMESSAGECALLBACKPROC;
            glGetDebugMessageLog = GetDelegate("glGetDebugMessageLog", typeof(PFNGLGETDEBUGMESSAGELOGPROC)) as PFNGLGETDEBUGMESSAGELOGPROC;
            glPushDebugGroup = GetDelegate("glPushDebugGroup", typeof(PFNGLPUSHDEBUGGROUPPROC)) as PFNGLPUSHDEBUGGROUPPROC;
            glPopDebugGroup = GetDelegate("glPopDebugGroup", typeof(PFNGLPOPDEBUGGROUPPROC)) as PFNGLPOPDEBUGGROUPPROC;
            glObjectLabel = GetDelegate("glObjectLabel", typeof(PFNGLOBJECTLABELPROC)) as PFNGLOBJECTLABELPROC;
            glGetObjectLabel = GetDelegate("glGetObjectLabel", typeof(PFNGLGETOBJECTLABELPROC)) as PFNGLGETOBJECTLABELPROC;
            glObjectPtrLabel = GetDelegate("glObjectPtrLabel", typeof(PFNGLOBJECTPTRLABELPROC)) as PFNGLOBJECTPTRLABELPROC;
            glGetObjectPtrLabel = GetDelegate("glGetObjectPtrLabel", typeof(PFNGLGETOBJECTPTRLABELPROC)) as PFNGLGETOBJECTPTRLABELPROC;
            glBufferStorage = GetDelegate("glBufferStorage", typeof(PFNGLBUFFERSTORAGEPROC)) as PFNGLBUFFERSTORAGEPROC;
            glClearTexImage = GetDelegate("glClearTexImage", typeof(PFNGLCLEARTEXIMAGEPROC)) as PFNGLCLEARTEXIMAGEPROC;
            glClearTexSubImage = GetDelegate("glClearTexSubImage", typeof(PFNGLCLEARTEXSUBIMAGEPROC)) as PFNGLCLEARTEXSUBIMAGEPROC;
            glBindBuffersBase = GetDelegate("glBindBuffersBase", typeof(PFNGLBINDBUFFERSBASEPROC)) as PFNGLBINDBUFFERSBASEPROC;
            glBindBuffersRange = GetDelegate("glBindBuffersRange", typeof(PFNGLBINDBUFFERSRANGEPROC)) as PFNGLBINDBUFFERSRANGEPROC;
            glBindTextures = GetDelegate("glBindTextures", typeof(PFNGLBINDTEXTURESPROC)) as PFNGLBINDTEXTURESPROC;
            glBindSamplers = GetDelegate("glBindSamplers", typeof(PFNGLBINDSAMPLERSPROC)) as PFNGLBINDSAMPLERSPROC;
            glBindImageTextures = GetDelegate("glBindImageTextures", typeof(PFNGLBINDIMAGETEXTURESPROC)) as PFNGLBINDIMAGETEXTURESPROC;
            glBindVertexBuffers = GetDelegate("glBindVertexBuffers", typeof(PFNGLBINDVERTEXBUFFERSPROC)) as PFNGLBINDVERTEXBUFFERSPROC;
            glClipControl = GetDelegate("glClipControl", typeof(PFNGLCLIPCONTROLPROC)) as PFNGLCLIPCONTROLPROC;
            glCreateTransformFeedbacks = GetDelegate("glCreateTransformFeedbacks", typeof(PFNGLCREATETRANSFORMFEEDBACKSPROC)) as PFNGLCREATETRANSFORMFEEDBACKSPROC;
            glTransformFeedbackBufferBase = GetDelegate("glTransformFeedbackBufferBase", typeof(PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC)) as PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC;
            glTransformFeedbackBufferRange = GetDelegate("glTransformFeedbackBufferRange", typeof(PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC)) as PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC;
            glGetTransformFeedbackiv = GetDelegate("glGetTransformFeedbackiv", typeof(PFNGLGETTRANSFORMFEEDBACKIVPROC)) as PFNGLGETTRANSFORMFEEDBACKIVPROC;
            glGetTransformFeedbacki_v = GetDelegate("glGetTransformFeedbacki_v", typeof(PFNGLGETTRANSFORMFEEDBACKI_VPROC)) as PFNGLGETTRANSFORMFEEDBACKI_VPROC;
            glGetTransformFeedbacki64_v = GetDelegate("glGetTransformFeedbacki64_v", typeof(PFNGLGETTRANSFORMFEEDBACKI64_VPROC)) as PFNGLGETTRANSFORMFEEDBACKI64_VPROC;
            glCreateBuffers = GetDelegate("glCreateBuffers", typeof(PFNGLCREATEBUFFERSPROC)) as PFNGLCREATEBUFFERSPROC;
            glNamedBufferStorage = GetDelegate("glNamedBufferStorage", typeof(PFNGLNAMEDBUFFERSTORAGEPROC)) as PFNGLNAMEDBUFFERSTORAGEPROC;
            glNamedBufferData = GetDelegate("glNamedBufferData", typeof(PFNGLNAMEDBUFFERDATAPROC)) as PFNGLNAMEDBUFFERDATAPROC;
            glNamedBufferSubData = GetDelegate("glNamedBufferSubData", typeof(PFNGLNAMEDBUFFERSUBDATAPROC)) as PFNGLNAMEDBUFFERSUBDATAPROC;
            glCopyNamedBufferSubData = GetDelegate("glCopyNamedBufferSubData", typeof(PFNGLCOPYNAMEDBUFFERSUBDATAPROC)) as PFNGLCOPYNAMEDBUFFERSUBDATAPROC;
            glClearNamedBufferData = GetDelegate("glClearNamedBufferData", typeof(PFNGLCLEARNAMEDBUFFERDATAPROC)) as PFNGLCLEARNAMEDBUFFERDATAPROC;
            glClearNamedBufferSubData = GetDelegate("glClearNamedBufferSubData", typeof(PFNGLCLEARNAMEDBUFFERSUBDATAPROC)) as PFNGLCLEARNAMEDBUFFERSUBDATAPROC;
            glMapNamedBuffer = GetDelegate("glMapNamedBuffer", typeof(PFNGLMAPNAMEDBUFFERPROC)) as PFNGLMAPNAMEDBUFFERPROC;
            glMapNamedBufferRange = GetDelegate("glMapNamedBufferRange", typeof(PFNGLMAPNAMEDBUFFERRANGEPROC)) as PFNGLMAPNAMEDBUFFERRANGEPROC;
            glUnmapNamedBuffer = GetDelegate("glUnmapNamedBuffer", typeof(PFNGLUNMAPNAMEDBUFFERPROC)) as PFNGLUNMAPNAMEDBUFFERPROC;
            glFlushMappedNamedBufferRange = GetDelegate("glFlushMappedNamedBufferRange", typeof(PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC)) as PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC;
            glGetNamedBufferParameteriv = GetDelegate("glGetNamedBufferParameteriv", typeof(PFNGLGETNAMEDBUFFERPARAMETERIVPROC)) as PFNGLGETNAMEDBUFFERPARAMETERIVPROC;
            glGetNamedBufferParameteri64v = GetDelegate("glGetNamedBufferParameteri64v", typeof(PFNGLGETNAMEDBUFFERPARAMETERI64VPROC)) as PFNGLGETNAMEDBUFFERPARAMETERI64VPROC;
            glGetNamedBufferPointerv = GetDelegate("glGetNamedBufferPointerv", typeof(PFNGLGETNAMEDBUFFERPOINTERVPROC)) as PFNGLGETNAMEDBUFFERPOINTERVPROC;
            glGetNamedBufferSubData = GetDelegate("glGetNamedBufferSubData", typeof(PFNGLGETNAMEDBUFFERSUBDATAPROC)) as PFNGLGETNAMEDBUFFERSUBDATAPROC;
            glCreateFramebuffers = GetDelegate("glCreateFramebuffers", typeof(PFNGLCREATEFRAMEBUFFERSPROC)) as PFNGLCREATEFRAMEBUFFERSPROC;
            glNamedFramebufferRenderbuffer = GetDelegate("glNamedFramebufferRenderbuffer", typeof(PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC)) as PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC;
            glNamedFramebufferParameteri = GetDelegate("glNamedFramebufferParameteri", typeof(PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC)) as PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC;
            glNamedFramebufferTexture = GetDelegate("glNamedFramebufferTexture", typeof(PFNGLNAMEDFRAMEBUFFERTEXTUREPROC)) as PFNGLNAMEDFRAMEBUFFERTEXTUREPROC;
            glNamedFramebufferTextureLayer = GetDelegate("glNamedFramebufferTextureLayer", typeof(PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC)) as PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC;
            glNamedFramebufferDrawBuffer = GetDelegate("glNamedFramebufferDrawBuffer", typeof(PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC)) as PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC;
            glNamedFramebufferDrawBuffers = GetDelegate("glNamedFramebufferDrawBuffers", typeof(PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC)) as PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC;
            glNamedFramebufferReadBuffer = GetDelegate("glNamedFramebufferReadBuffer", typeof(PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC)) as PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC;
            glInvalidateNamedFramebufferData = GetDelegate("glInvalidateNamedFramebufferData", typeof(PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC)) as PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC;
            glInvalidateNamedFramebufferSubData = GetDelegate("glInvalidateNamedFramebufferSubData", typeof(PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC)) as PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC;
            glClearNamedFramebufferiv = GetDelegate("glClearNamedFramebufferiv", typeof(PFNGLCLEARNAMEDFRAMEBUFFERIVPROC)) as PFNGLCLEARNAMEDFRAMEBUFFERIVPROC;
            glClearNamedFramebufferuiv = GetDelegate("glClearNamedFramebufferuiv", typeof(PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC)) as PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC;
            glClearNamedFramebufferfv = GetDelegate("glClearNamedFramebufferfv", typeof(PFNGLCLEARNAMEDFRAMEBUFFERFVPROC)) as PFNGLCLEARNAMEDFRAMEBUFFERFVPROC;
            glClearNamedFramebufferfi = GetDelegate("glClearNamedFramebufferfi", typeof(PFNGLCLEARNAMEDFRAMEBUFFERFIPROC)) as PFNGLCLEARNAMEDFRAMEBUFFERFIPROC;
            glBlitNamedFramebuffer = GetDelegate("glBlitNamedFramebuffer", typeof(PFNGLBLITNAMEDFRAMEBUFFERPROC)) as PFNGLBLITNAMEDFRAMEBUFFERPROC;
            glCheckNamedFramebufferStatus = GetDelegate("glCheckNamedFramebufferStatus", typeof(PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC)) as PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC;
            glGetNamedFramebufferParameteriv = GetDelegate("glGetNamedFramebufferParameteriv", typeof(PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC)) as PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC;
            glGetNamedFramebufferAttachmentParameteriv = GetDelegate("glGetNamedFramebufferAttachmentParameteriv", typeof(PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC)) as PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC;
            glCreateRenderbuffers = GetDelegate("glCreateRenderbuffers", typeof(PFNGLCREATERENDERBUFFERSPROC)) as PFNGLCREATERENDERBUFFERSPROC;
            glNamedRenderbufferStorage = GetDelegate("glNamedRenderbufferStorage", typeof(PFNGLNAMEDRENDERBUFFERSTORAGEPROC)) as PFNGLNAMEDRENDERBUFFERSTORAGEPROC;
            glNamedRenderbufferStorageMultisample = GetDelegate("glNamedRenderbufferStorageMultisample", typeof(PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC)) as PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC;
            glGetNamedRenderbufferParameteriv = GetDelegate("glGetNamedRenderbufferParameteriv", typeof(PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC)) as PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC;
            glCreateTextures = GetDelegate("glCreateTextures", typeof(PFNGLCREATETEXTURESPROC)) as PFNGLCREATETEXTURESPROC;
            glTextureBuffer = GetDelegate("glTextureBuffer", typeof(PFNGLTEXTUREBUFFERPROC)) as PFNGLTEXTUREBUFFERPROC;
            glTextureBufferRange = GetDelegate("glTextureBufferRange", typeof(PFNGLTEXTUREBUFFERRANGEPROC)) as PFNGLTEXTUREBUFFERRANGEPROC;
            glTextureStorage1D = GetDelegate("glTextureStorage1D", typeof(PFNGLTEXTURESTORAGE1DPROC)) as PFNGLTEXTURESTORAGE1DPROC;
            glTextureStorage2D = GetDelegate("glTextureStorage2D", typeof(PFNGLTEXTURESTORAGE2DPROC)) as PFNGLTEXTURESTORAGE2DPROC;
            glTextureStorage3D = GetDelegate("glTextureStorage3D", typeof(PFNGLTEXTURESTORAGE3DPROC)) as PFNGLTEXTURESTORAGE3DPROC;
            glTextureStorage2DMultisample = GetDelegate("glTextureStorage2DMultisample", typeof(PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC)) as PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC;
            glTextureStorage3DMultisample = GetDelegate("glTextureStorage3DMultisample", typeof(PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC)) as PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC;
            glTextureSubImage1D = GetDelegate("glTextureSubImage1D", typeof(PFNGLTEXTURESUBIMAGE1DPROC)) as PFNGLTEXTURESUBIMAGE1DPROC;
            glTextureSubImage2D = GetDelegate("glTextureSubImage2D", typeof(PFNGLTEXTURESUBIMAGE2DPROC)) as PFNGLTEXTURESUBIMAGE2DPROC;
            glTextureSubImage3D = GetDelegate("glTextureSubImage3D", typeof(PFNGLTEXTURESUBIMAGE3DPROC)) as PFNGLTEXTURESUBIMAGE3DPROC;
            glCompressedTextureSubImage1D = GetDelegate("glCompressedTextureSubImage1D", typeof(PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC)) as PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC;
            glCompressedTextureSubImage2D = GetDelegate("glCompressedTextureSubImage2D", typeof(PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC)) as PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC;
            glCompressedTextureSubImage3D = GetDelegate("glCompressedTextureSubImage3D", typeof(PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC)) as PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC;
            glCopyTextureSubImage1D = GetDelegate("glCopyTextureSubImage1D", typeof(PFNGLCOPYTEXTURESUBIMAGE1DPROC)) as PFNGLCOPYTEXTURESUBIMAGE1DPROC;
            glCopyTextureSubImage2D = GetDelegate("glCopyTextureSubImage2D", typeof(PFNGLCOPYTEXTURESUBIMAGE2DPROC)) as PFNGLCOPYTEXTURESUBIMAGE2DPROC;
            glCopyTextureSubImage3D = GetDelegate("glCopyTextureSubImage3D", typeof(PFNGLCOPYTEXTURESUBIMAGE3DPROC)) as PFNGLCOPYTEXTURESUBIMAGE3DPROC;
            glTextureParameterf = GetDelegate("glTextureParameterf", typeof(PFNGLTEXTUREPARAMETERFPROC)) as PFNGLTEXTUREPARAMETERFPROC;
            glTextureParameterfv = GetDelegate("glTextureParameterfv", typeof(PFNGLTEXTUREPARAMETERFVPROC)) as PFNGLTEXTUREPARAMETERFVPROC;
            glTextureParameteri = GetDelegate("glTextureParameteri", typeof(PFNGLTEXTUREPARAMETERIPROC)) as PFNGLTEXTUREPARAMETERIPROC;
            glTextureParameterIiv = GetDelegate("glTextureParameterIiv", typeof(PFNGLTEXTUREPARAMETERIIVPROC)) as PFNGLTEXTUREPARAMETERIIVPROC;
            glTextureParameterIuiv = GetDelegate("glTextureParameterIuiv", typeof(PFNGLTEXTUREPARAMETERIUIVPROC)) as PFNGLTEXTUREPARAMETERIUIVPROC;
            glTextureParameteriv = GetDelegate("glTextureParameteriv", typeof(PFNGLTEXTUREPARAMETERIVPROC)) as PFNGLTEXTUREPARAMETERIVPROC;
            glGenerateTextureMipmap = GetDelegate("glGenerateTextureMipmap", typeof(PFNGLGENERATETEXTUREMIPMAPPROC)) as PFNGLGENERATETEXTUREMIPMAPPROC;
            glBindTextureUnit = GetDelegate("glBindTextureUnit", typeof(PFNGLBINDTEXTUREUNITPROC)) as PFNGLBINDTEXTUREUNITPROC;
            glGetTextureImage = GetDelegate("glGetTextureImage", typeof(PFNGLGETTEXTUREIMAGEPROC)) as PFNGLGETTEXTUREIMAGEPROC;
            glGetCompressedTextureImage = GetDelegate("glGetCompressedTextureImage", typeof(PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC)) as PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC;
            glGetTextureLevelParameterfv = GetDelegate("glGetTextureLevelParameterfv", typeof(PFNGLGETTEXTURELEVELPARAMETERFVPROC)) as PFNGLGETTEXTURELEVELPARAMETERFVPROC;
            glGetTextureLevelParameteriv = GetDelegate("glGetTextureLevelParameteriv", typeof(PFNGLGETTEXTURELEVELPARAMETERIVPROC)) as PFNGLGETTEXTURELEVELPARAMETERIVPROC;
            glGetTextureParameterfv = GetDelegate("glGetTextureParameterfv", typeof(PFNGLGETTEXTUREPARAMETERFVPROC)) as PFNGLGETTEXTUREPARAMETERFVPROC;
            glGetTextureParameterIiv = GetDelegate("glGetTextureParameterIiv", typeof(PFNGLGETTEXTUREPARAMETERIIVPROC)) as PFNGLGETTEXTUREPARAMETERIIVPROC;
            glGetTextureParameterIuiv = GetDelegate("glGetTextureParameterIuiv", typeof(PFNGLGETTEXTUREPARAMETERIUIVPROC)) as PFNGLGETTEXTUREPARAMETERIUIVPROC;
            glGetTextureParameteriv = GetDelegate("glGetTextureParameteriv", typeof(PFNGLGETTEXTUREPARAMETERIVPROC)) as PFNGLGETTEXTUREPARAMETERIVPROC;
            glCreateVertexArrays = GetDelegate("glCreateVertexArrays", typeof(PFNGLCREATEVERTEXARRAYSPROC)) as PFNGLCREATEVERTEXARRAYSPROC;
            glDisableVertexArrayAttrib = GetDelegate("glDisableVertexArrayAttrib", typeof(PFNGLDISABLEVERTEXARRAYATTRIBPROC)) as PFNGLDISABLEVERTEXARRAYATTRIBPROC;
            glEnableVertexArrayAttrib = GetDelegate("glEnableVertexArrayAttrib", typeof(PFNGLENABLEVERTEXARRAYATTRIBPROC)) as PFNGLENABLEVERTEXARRAYATTRIBPROC;
            glVertexArrayElementBuffer = GetDelegate("glVertexArrayElementBuffer", typeof(PFNGLVERTEXARRAYELEMENTBUFFERPROC)) as PFNGLVERTEXARRAYELEMENTBUFFERPROC;
            glVertexArrayVertexBuffer = GetDelegate("glVertexArrayVertexBuffer", typeof(PFNGLVERTEXARRAYVERTEXBUFFERPROC)) as PFNGLVERTEXARRAYVERTEXBUFFERPROC;
            glVertexArrayVertexBuffers = GetDelegate("glVertexArrayVertexBuffers", typeof(PFNGLVERTEXARRAYVERTEXBUFFERSPROC)) as PFNGLVERTEXARRAYVERTEXBUFFERSPROC;
            glVertexArrayAttribBinding = GetDelegate("glVertexArrayAttribBinding", typeof(PFNGLVERTEXARRAYATTRIBBINDINGPROC)) as PFNGLVERTEXARRAYATTRIBBINDINGPROC;
            glVertexArrayAttribFormat = GetDelegate("glVertexArrayAttribFormat", typeof(PFNGLVERTEXARRAYATTRIBFORMATPROC)) as PFNGLVERTEXARRAYATTRIBFORMATPROC;
            glVertexArrayAttribIFormat = GetDelegate("glVertexArrayAttribIFormat", typeof(PFNGLVERTEXARRAYATTRIBIFORMATPROC)) as PFNGLVERTEXARRAYATTRIBIFORMATPROC;
            glVertexArrayAttribLFormat = GetDelegate("glVertexArrayAttribLFormat", typeof(PFNGLVERTEXARRAYATTRIBLFORMATPROC)) as PFNGLVERTEXARRAYATTRIBLFORMATPROC;
            glVertexArrayBindingDivisor = GetDelegate("glVertexArrayBindingDivisor", typeof(PFNGLVERTEXARRAYBINDINGDIVISORPROC)) as PFNGLVERTEXARRAYBINDINGDIVISORPROC;
            glGetVertexArrayiv = GetDelegate("glGetVertexArrayiv", typeof(PFNGLGETVERTEXARRAYIVPROC)) as PFNGLGETVERTEXARRAYIVPROC;
            glGetVertexArrayIndexediv = GetDelegate("glGetVertexArrayIndexediv", typeof(PFNGLGETVERTEXARRAYINDEXEDIVPROC)) as PFNGLGETVERTEXARRAYINDEXEDIVPROC;
            glGetVertexArrayIndexed64iv = GetDelegate("glGetVertexArrayIndexed64iv", typeof(PFNGLGETVERTEXARRAYINDEXED64IVPROC)) as PFNGLGETVERTEXARRAYINDEXED64IVPROC;
            glCreateSamplers = GetDelegate("glCreateSamplers", typeof(PFNGLCREATESAMPLERSPROC)) as PFNGLCREATESAMPLERSPROC;
            glCreateProgramPipelines = GetDelegate("glCreateProgramPipelines", typeof(PFNGLCREATEPROGRAMPIPELINESPROC)) as PFNGLCREATEPROGRAMPIPELINESPROC;
            glCreateQueries = GetDelegate("glCreateQueries", typeof(PFNGLCREATEQUERIESPROC)) as PFNGLCREATEQUERIESPROC;
            glGetQueryBufferObjecti64v = GetDelegate("glGetQueryBufferObjecti64v", typeof(PFNGLGETQUERYBUFFEROBJECTI64VPROC)) as PFNGLGETQUERYBUFFEROBJECTI64VPROC;
            glGetQueryBufferObjectiv = GetDelegate("glGetQueryBufferObjectiv", typeof(PFNGLGETQUERYBUFFEROBJECTIVPROC)) as PFNGLGETQUERYBUFFEROBJECTIVPROC;
            glGetQueryBufferObjectui64v = GetDelegate("glGetQueryBufferObjectui64v", typeof(PFNGLGETQUERYBUFFEROBJECTUI64VPROC)) as PFNGLGETQUERYBUFFEROBJECTUI64VPROC;
            glGetQueryBufferObjectuiv = GetDelegate("glGetQueryBufferObjectuiv", typeof(PFNGLGETQUERYBUFFEROBJECTUIVPROC)) as PFNGLGETQUERYBUFFEROBJECTUIVPROC;
            glMemoryBarrierByRegion = GetDelegate("glMemoryBarrierByRegion", typeof(PFNGLMEMORYBARRIERBYREGIONPROC)) as PFNGLMEMORYBARRIERBYREGIONPROC;
            glGetTextureSubImage = GetDelegate("glGetTextureSubImage", typeof(PFNGLGETTEXTURESUBIMAGEPROC)) as PFNGLGETTEXTURESUBIMAGEPROC;
            glGetCompressedTextureSubImage = GetDelegate("glGetCompressedTextureSubImage", typeof(PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC)) as PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC;
            glGetGraphicsResetStatus = GetDelegate("glGetGraphicsResetStatus", typeof(PFNGLGETGRAPHICSRESETSTATUSPROC)) as PFNGLGETGRAPHICSRESETSTATUSPROC;
            glGetnCompressedTexImage = GetDelegate("glGetnCompressedTexImage", typeof(PFNGLGETNCOMPRESSEDTEXIMAGEPROC)) as PFNGLGETNCOMPRESSEDTEXIMAGEPROC;
            glGetnTexImage = GetDelegate("glGetnTexImage", typeof(PFNGLGETNTEXIMAGEPROC)) as PFNGLGETNTEXIMAGEPROC;
            glGetnUniformdv = GetDelegate("glGetnUniformdv", typeof(PFNGLGETNUNIFORMDVPROC)) as PFNGLGETNUNIFORMDVPROC;
            glGetnUniformfv = GetDelegate("glGetnUniformfv", typeof(PFNGLGETNUNIFORMFVPROC)) as PFNGLGETNUNIFORMFVPROC;
            glGetnUniformiv = GetDelegate("glGetnUniformiv", typeof(PFNGLGETNUNIFORMIVPROC)) as PFNGLGETNUNIFORMIVPROC;
            glGetnUniformuiv = GetDelegate("glGetnUniformuiv", typeof(PFNGLGETNUNIFORMUIVPROC)) as PFNGLGETNUNIFORMUIVPROC;
            glReadnPixels = GetDelegate("glReadnPixels", typeof(PFNGLREADNPIXELSPROC)) as PFNGLREADNPIXELSPROC;
            glTextureBarrier = GetDelegate("glTextureBarrier", typeof(PFNGLTEXTUREBARRIERPROC)) as PFNGLTEXTUREBARRIERPROC;
            glGetTextureHandleARB = GetDelegate("glGetTextureHandleARB", typeof(PFNGLGETTEXTUREHANDLEARBPROC)) as PFNGLGETTEXTUREHANDLEARBPROC;
            glGetTextureSamplerHandleARB = GetDelegate("glGetTextureSamplerHandleARB", typeof(PFNGLGETTEXTURESAMPLERHANDLEARBPROC)) as PFNGLGETTEXTURESAMPLERHANDLEARBPROC;
            glMakeTextureHandleResidentARB = GetDelegate("glMakeTextureHandleResidentARB", typeof(PFNGLMAKETEXTUREHANDLERESIDENTARBPROC)) as PFNGLMAKETEXTUREHANDLERESIDENTARBPROC;
            glMakeTextureHandleNonResidentARB = GetDelegate("glMakeTextureHandleNonResidentARB", typeof(PFNGLMAKETEXTUREHANDLENONRESIDENTARBPROC)) as PFNGLMAKETEXTUREHANDLENONRESIDENTARBPROC;
            glGetImageHandleARB = GetDelegate("glGetImageHandleARB", typeof(PFNGLGETIMAGEHANDLEARBPROC)) as PFNGLGETIMAGEHANDLEARBPROC;
            glMakeImageHandleResidentARB = GetDelegate("glMakeImageHandleResidentARB", typeof(PFNGLMAKEIMAGEHANDLERESIDENTARBPROC)) as PFNGLMAKEIMAGEHANDLERESIDENTARBPROC;
            glMakeImageHandleNonResidentARB = GetDelegate("glMakeImageHandleNonResidentARB", typeof(PFNGLMAKEIMAGEHANDLENONRESIDENTARBPROC)) as PFNGLMAKEIMAGEHANDLENONRESIDENTARBPROC;
            glUniformHandleui64ARB = GetDelegate("glUniformHandleui64ARB", typeof(PFNGLUNIFORMHANDLEUI64ARBPROC)) as PFNGLUNIFORMHANDLEUI64ARBPROC;
            glUniformHandleui64vARB = GetDelegate("glUniformHandleui64vARB", typeof(PFNGLUNIFORMHANDLEUI64VARBPROC)) as PFNGLUNIFORMHANDLEUI64VARBPROC;
            glProgramUniformHandleui64ARB = GetDelegate("glProgramUniformHandleui64ARB", typeof(PFNGLPROGRAMUNIFORMHANDLEUI64ARBPROC)) as PFNGLPROGRAMUNIFORMHANDLEUI64ARBPROC;
            glProgramUniformHandleui64vARB = GetDelegate("glProgramUniformHandleui64vARB", typeof(PFNGLPROGRAMUNIFORMHANDLEUI64VARBPROC)) as PFNGLPROGRAMUNIFORMHANDLEUI64VARBPROC;
            glIsTextureHandleResidentARB = GetDelegate("glIsTextureHandleResidentARB", typeof(PFNGLISTEXTUREHANDLERESIDENTARBPROC)) as PFNGLISTEXTUREHANDLERESIDENTARBPROC;
            glIsImageHandleResidentARB = GetDelegate("glIsImageHandleResidentARB", typeof(PFNGLISIMAGEHANDLERESIDENTARBPROC)) as PFNGLISIMAGEHANDLERESIDENTARBPROC;
            glVertexAttribL1ui64ARB = GetDelegate("glVertexAttribL1ui64ARB", typeof(PFNGLVERTEXATTRIBL1UI64ARBPROC)) as PFNGLVERTEXATTRIBL1UI64ARBPROC;
            glVertexAttribL1ui64vARB = GetDelegate("glVertexAttribL1ui64vARB", typeof(PFNGLVERTEXATTRIBL1UI64VARBPROC)) as PFNGLVERTEXATTRIBL1UI64VARBPROC;
            glGetVertexAttribLui64vARB = GetDelegate("glGetVertexAttribLui64vARB", typeof(PFNGLGETVERTEXATTRIBLUI64VARBPROC)) as PFNGLGETVERTEXATTRIBLUI64VARBPROC;
            glCreateSyncFromCLeventARB = GetDelegate("glCreateSyncFromCLeventARB", typeof(PFNGLCREATESYNCFROMCLEVENTARBPROC)) as PFNGLCREATESYNCFROMCLEVENTARBPROC;
            glDispatchComputeGroupSizeARB = GetDelegate("glDispatchComputeGroupSizeARB", typeof(PFNGLDISPATCHCOMPUTEGROUPSIZEARBPROC)) as PFNGLDISPATCHCOMPUTEGROUPSIZEARBPROC;
            glDebugMessageControlARB = GetDelegate("glDebugMessageControlARB", typeof(PFNGLDEBUGMESSAGECONTROLARBPROC)) as PFNGLDEBUGMESSAGECONTROLARBPROC;
            glDebugMessageInsertARB = GetDelegate("glDebugMessageInsertARB", typeof(PFNGLDEBUGMESSAGEINSERTARBPROC)) as PFNGLDEBUGMESSAGEINSERTARBPROC;
            glDebugMessageCallbackARB = GetDelegate("glDebugMessageCallbackARB", typeof(PFNGLDEBUGMESSAGECALLBACKARBPROC)) as PFNGLDEBUGMESSAGECALLBACKARBPROC;
            glGetDebugMessageLogARB = GetDelegate("glGetDebugMessageLogARB", typeof(PFNGLGETDEBUGMESSAGELOGARBPROC)) as PFNGLGETDEBUGMESSAGELOGARBPROC;
            glBlendEquationiARB = GetDelegate("glBlendEquationiARB", typeof(PFNGLBLENDEQUATIONIARBPROC)) as PFNGLBLENDEQUATIONIARBPROC;
            glBlendEquationSeparateiARB = GetDelegate("glBlendEquationSeparateiARB", typeof(PFNGLBLENDEQUATIONSEPARATEIARBPROC)) as PFNGLBLENDEQUATIONSEPARATEIARBPROC;
            glBlendFunciARB = GetDelegate("glBlendFunciARB", typeof(PFNGLBLENDFUNCIARBPROC)) as PFNGLBLENDFUNCIARBPROC;
            glBlendFuncSeparateiARB = GetDelegate("glBlendFuncSeparateiARB", typeof(PFNGLBLENDFUNCSEPARATEIARBPROC)) as PFNGLBLENDFUNCSEPARATEIARBPROC;
            glMultiDrawArraysIndirectCountARB = GetDelegate("glMultiDrawArraysIndirectCountARB", typeof(PFNGLMULTIDRAWARRAYSINDIRECTCOUNTARBPROC)) as PFNGLMULTIDRAWARRAYSINDIRECTCOUNTARBPROC;
            glMultiDrawElementsIndirectCountARB = GetDelegate("glMultiDrawElementsIndirectCountARB", typeof(PFNGLMULTIDRAWELEMENTSINDIRECTCOUNTARBPROC)) as PFNGLMULTIDRAWELEMENTSINDIRECTCOUNTARBPROC;
            glGetGraphicsResetStatusARB = GetDelegate("glGetGraphicsResetStatusARB", typeof(PFNGLGETGRAPHICSRESETSTATUSARBPROC)) as PFNGLGETGRAPHICSRESETSTATUSARBPROC;
            glGetnTexImageARB = GetDelegate("glGetnTexImageARB", typeof(PFNGLGETNTEXIMAGEARBPROC)) as PFNGLGETNTEXIMAGEARBPROC;
            glReadnPixelsARB = GetDelegate("glReadnPixelsARB", typeof(PFNGLREADNPIXELSARBPROC)) as PFNGLREADNPIXELSARBPROC;
            glGetnCompressedTexImageARB = GetDelegate("glGetnCompressedTexImageARB", typeof(PFNGLGETNCOMPRESSEDTEXIMAGEARBPROC)) as PFNGLGETNCOMPRESSEDTEXIMAGEARBPROC;
            glGetnUniformfvARB = GetDelegate("glGetnUniformfvARB", typeof(PFNGLGETNUNIFORMFVARBPROC)) as PFNGLGETNUNIFORMFVARBPROC;
            glGetnUniformivARB = GetDelegate("glGetnUniformivARB", typeof(PFNGLGETNUNIFORMIVARBPROC)) as PFNGLGETNUNIFORMIVARBPROC;
            glGetnUniformuivARB = GetDelegate("glGetnUniformuivARB", typeof(PFNGLGETNUNIFORMUIVARBPROC)) as PFNGLGETNUNIFORMUIVARBPROC;
            glGetnUniformdvARB = GetDelegate("glGetnUniformdvARB", typeof(PFNGLGETNUNIFORMDVARBPROC)) as PFNGLGETNUNIFORMDVARBPROC;
            glMinSampleShadingARB = GetDelegate("glMinSampleShadingARB", typeof(PFNGLMINSAMPLESHADINGARBPROC)) as PFNGLMINSAMPLESHADINGARBPROC;
            glNamedStringARB = GetDelegate("glNamedStringARB", typeof(PFNGLNAMEDSTRINGARBPROC)) as PFNGLNAMEDSTRINGARBPROC;
            glDeleteNamedStringARB = GetDelegate("glDeleteNamedStringARB", typeof(PFNGLDELETENAMEDSTRINGARBPROC)) as PFNGLDELETENAMEDSTRINGARBPROC;
            glCompileShaderIncludeARB = GetDelegate("glCompileShaderIncludeARB", typeof(PFNGLCOMPILESHADERINCLUDEARBPROC)) as PFNGLCOMPILESHADERINCLUDEARBPROC;
            glIsNamedStringARB = GetDelegate("glIsNamedStringARB", typeof(PFNGLISNAMEDSTRINGARBPROC)) as PFNGLISNAMEDSTRINGARBPROC;
            glGetNamedStringARB = GetDelegate("glGetNamedStringARB", typeof(PFNGLGETNAMEDSTRINGARBPROC)) as PFNGLGETNAMEDSTRINGARBPROC;
            glGetNamedStringivARB = GetDelegate("glGetNamedStringivARB", typeof(PFNGLGETNAMEDSTRINGIVARBPROC)) as PFNGLGETNAMEDSTRINGIVARBPROC;
            glBufferPageCommitmentARB = GetDelegate("glBufferPageCommitmentARB", typeof(PFNGLBUFFERPAGECOMMITMENTARBPROC)) as PFNGLBUFFERPAGECOMMITMENTARBPROC;
            glNamedBufferPageCommitmentEXT = GetDelegate("glNamedBufferPageCommitmentEXT", typeof(PFNGLNAMEDBUFFERPAGECOMMITMENTEXTPROC)) as PFNGLNAMEDBUFFERPAGECOMMITMENTEXTPROC;
            glNamedBufferPageCommitmentARB = GetDelegate("glNamedBufferPageCommitmentARB", typeof(PFNGLNAMEDBUFFERPAGECOMMITMENTARBPROC)) as PFNGLNAMEDBUFFERPAGECOMMITMENTARBPROC;
            glTexPageCommitmentARB = GetDelegate("glTexPageCommitmentARB", typeof(PFNGLTEXPAGECOMMITMENTARBPROC)) as PFNGLTEXPAGECOMMITMENTARBPROC;
        }

        internal static IntPtr GetAddress(String lpszProc)
        {
            var p = Win32Helper.GetProcAddress(lpszProc);
            if (!IsValid(p))
                p = Win32Helper.GetProcAddress(OpenGLHandle, lpszProc);
            return p;
        }

        internal static Delegate GetDelegate(String lpszProc, Type t)
        {
            var ptr = GetAddress(lpszProc);
            if (IsValid(ptr))
                return Marshal.GetDelegateForFunctionPointer(ptr, t);
            return null;
        }

        private static bool IsValid(IntPtr address)
        {
            // https://www.opengl.org/wiki/Load_OpenGL_Functions
            long a = address.ToInt64();
            bool is_valid = (a < -1) || (a > 3);
            return is_valid;
        }

        #region Func
        public static void SwapBuffers(IntPtr hdc)
        {
            Win32Helper.SwapBuffers(hdc);
        }

        public static void UseProgram(GLuint program)
        {
            glUseProgram?.Invoke(program);
        }

        public static void Uniform1i(GLint location, GLint v0)
        {
            glUniform1i?.Invoke(location, v0);
        }

        public static void Uniform1f(GLint location, GLfloat v0)
        {
            glUniform1f?.Invoke(location, v0);
        }

        public static void Uniform2fv(GLint location, GLsizei count, GLfloat[] value)
        {
            glUniform2fv?.Invoke(location, count, value);
        }

        public static void Uniform3fv(GLint location, GLsizei count, GLfloat[] value)
        {
            glUniform3fv?.Invoke(location, count, value);
        }

        public static void Uniform4fv(GLint location, GLsizei count, GLfloat[] value)
        {
            glUniform4fv?.Invoke(location, count, value);
        }

        public static void UniformMatrix3fv(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value)
        {
            glUniformMatrix3fv?.Invoke(location, count, transpose, value);
        }

        public static void GetIntegerv(GLenum pname, ref GLint param)
        {
            if (glGetIntegerv != null)
            {
                unsafe
                {
                    fixed(GLint* p = &param)
                        glGetIntegerv(pname, (GLvoid)p);
                }
            }
        }

        public static void GetIntegerv(GLenum pname, GLint[] param)
        {
            if (glGetIntegerv != null)
            {
                unsafe
                {
                    fixed (GLint* p = param)
                        glGetIntegerv(pname, (GLvoid)p);
                }
            }
        }

        public static void GetFloatv(GLenum pname, ref GLfloat param)
        {
            if (glGetFloatv != null)
            {
                unsafe
                {
                    fixed (GLfloat* p = &param)
                        glGetFloatv(pname, (GLvoid)p);
                }
            }
        }

        public static void GetFloatv(GLenum pname, GLfloat[] param)
        {
            if (glGetFloatv != null)
            {
                unsafe
                {
                    fixed (GLfloat* p = param)
                        glGetFloatv(pname, (GLvoid)p);
                }
            }
        }

        public static GLuint CreateProgram()
        {
            if (glCreateProgram != null)
                return glCreateProgram();
            return 0;
        }

        public static GLuint CreateShader(GLenum type)
        {
            if (glCreateShader != null)
                return glCreateShader(type);
            return 0;
        }

        public static void ShaderSource(GLuint shader, GLsizei count, [MarshalAs(UnmanagedType.LPArray)]string[] program, GLint[] length)
        {
            glShaderSource?.Invoke(shader, count, program, length);
        }

        public static void CompileShader(GLuint shader)
        {
            glCompileShader?.Invoke(shader);
        }

        public static void AttachShader(GLuint program, GLuint shader)
        {
            glAttachShader?.Invoke(program, shader);
        }

        public static void DeleteShader(GLuint shader)
        {
            glDeleteShader?.Invoke(shader);
        }

        public static void LinkProgram(GLuint program)
        {
            glLinkProgram?.Invoke(program);
        }

        public static void DeleteProgram(GLuint program)
        {
            glDeleteProgram?.Invoke(program);
        }

        public static void Viewport(GLint x, GLint y, GLsizei width, GLsizei height)
        {
            glViewport?.Invoke(x, y, width, height);
        }

        public static void Enable(GLenum cap)
        {
            glEnable?.Invoke(cap);
        }

        public static void Disable(GLenum cap)
        {
            glDisable?.Invoke(cap);
        }

        public static void GenVertexArrays(GLsizei n, GLuint[] arrays)
        {
            glGenVertexArrays?.Invoke(n, arrays);
        }

        public static void DeleteVertexArrays(GLsizei n, GLuint[] arrays)
        {
            glDeleteVertexArrays?.Invoke(n, arrays);
        }

        public static void BindVertexArray(GLuint array)
        {
            glBindVertexArray?.Invoke(array);
        }

        public static void GenBuffers(GLsizei n, GLuint[] buffers)
        {
            glGenBuffers?.Invoke(n, buffers);
        }

        public static void DeleteBuffers(GLsizei n, GLuint[] buffers)
        {
            glDeleteBuffers?.Invoke(n, buffers);
        }

        public static void BindBuffer(GLenum target, GLuint buffer)
        {
            glBindBuffer?.Invoke(target, buffer);
        }

        public static void BufferData(GLenum target, GLsizeiptr size, byte[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (byte* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, sbyte[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (sbyte* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, Int16[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (Int16* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, UInt16[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (UInt16* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, float[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (float* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, double[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (double* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, int[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (int* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, uint[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (uint* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, Int64[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (Int64* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferData(GLenum target, GLsizeiptr size, UInt64[] data, GLenum usage)
        {
            if (glBufferData != null)
            {
                if (data == null)
                    glBufferData(target, size, GLvoid.Zero, usage);
                else
                {
                    unsafe
                    {
                        fixed (UInt64* ptr = data)
                            glBufferData(target, size, (GLvoid)ptr, usage);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, byte[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (byte* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, sbyte[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (sbyte* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, short[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (short* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, ushort[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (ushort* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, int[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (int* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, uint[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (uint* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, long[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (long* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, ulong[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (ulong* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, float[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (float* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void BufferSubData(GLenum target, GLintptr offset, GLsizeiptr size, double[] data)
        {
            if (glBufferSubData != null)
            {
                if (data == null)
                    glBufferSubData(target, offset, size, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (double* ptr = data)
                            glBufferSubData(target, offset, size, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void EnableVertexAttribArray(GLuint index)
        {
            glEnableVertexAttribArray?.Invoke(index);
        }

        public static void DisableVertexAttribArray(GLuint index)
        {
            glDisableVertexAttribArray?.Invoke(index);
        }

        public static void VertexAttribPointer(GLuint index, GLint size, GLenum type, GLboolean normalized, GLsizei stride, int pointer)
        {
            glVertexAttribPointer?.Invoke(index, size, type, normalized, stride, (GLvoid)pointer);
        }

        public static void ClearColor(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha)
        {
            glClearColor?.Invoke(red, green, blue, alpha);
        }

        public static void ClearStencil(GLint s)
        {
            glClearStencil?.Invoke(s);
        }

        public static void Clear(GLbitfield mask)
        {
            glClear?.Invoke(mask);
        }

        public static void LineWidth(GLfloat width)
        {
            glLineWidth?.Invoke(width);
        }

        public static void PointSize(GLfloat size)
        {
            glPointSize?.Invoke(size);
        }

        public static void DrawArrays(GLenum mode, GLint first, GLsizei count)
        {
            glDrawArrays?.Invoke(mode, first, count);
        }

        public static void MultiDrawArrays(GLenum mode, GLint[] first, GLsizei[] count, GLsizei primcount)
        {
            glMultiDrawArrays?.Invoke(mode, first, count, primcount);
        }

        public static void DrawArraysInstanced(GLenum mode, GLint first, GLsizei count, GLsizei primcount)
        {
            glDrawArraysInstanced?.Invoke(mode, first, count, primcount);
        }

        public static void DrawElements(GLenum mode, GLsizei count, GLenum type, int indices)
        {
            glDrawElements?.Invoke(mode, count, type, (GLvoid)indices);
        }

        public static void MultiDrawElements(GLenum mode, GLsizei[] count, GLenum type, GLvoid[] indices, GLsizei primcount)
        {
            glMultiDrawElements?.Invoke(mode, count, type, indices, primcount);
        }

        public static void PolygonMode(GLenum face, GLenum mode)
        {
            glPolygonMode?.Invoke(face, mode);
        }

        public static void CullFace(GLenum mode)
        {
            glCullFace?.Invoke(mode);
        }

        public static void FrontFace(GLenum mode)
        {
            glFrontFace?.Invoke(mode);
        }

        public static void Hint(GLenum target, GLenum mode)
        {
            glHint?.Invoke(target, mode);
        }

        public static void GenFramebuffers(GLsizei n, GLuint[] framebuffers)
        {
            glGenFramebuffers?.Invoke(n, framebuffers);
        }

        public static void BindFramebuffer(GLenum target, GLuint framebuffer)
        {
            glBindFramebuffer?.Invoke(target, framebuffer);
        }

        public static void GenTextures(GLsizei n, GLuint[] textures)
        {
            glGenTextures?.Invoke(n, textures);
        }

        public static void BindTexture(GLenum target, GLuint texture)
        {
            glBindTexture?.Invoke(target, texture);
        }

        public static void ActiveTexture(GLenum texture)
        {
            glActiveTexture?.Invoke(texture);
        }

        public static void DeleteTextures(GLsizei n, GLuint[] textures)
        {
            glDeleteTextures?.Invoke(n, textures);
        }

        public static void DeleteFramebuffers(GLsizei n, GLuint[] framebuffers)
        {
            glDeleteFramebuffers?.Invoke(n, framebuffers);
        }

        public static void TexImage2DMultisample(GLenum target, GLsizei samples, GLint internalformat, GLsizei width, GLsizei height, GLboolean fixedsamplelocations)
        {
            glTexImage2DMultisample?.Invoke(target, samples, internalformat, width, height, fixedsamplelocations);
        }

        public static void TexParameteri(GLenum target, GLenum pname, GLint param)
        {
            glTexParameteri?.Invoke(target, pname, param);
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, byte[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (byte* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, sbyte[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (sbyte* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, ushort[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (ushort* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, short[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (short* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, int[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (int* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, uint[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (uint* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, long[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (long* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, ulong[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (ulong* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, float[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (float* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void TexImage1D(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, double[] pixels)
        {
            if (glTexImage1D != null)
            {
                if (pixels == null)
                    glTexImage1D(target, level, internalformat, width, border, format, type, GLvoid.Zero);
                else
                {
                    unsafe
                    {
                        fixed (double* ptr = pixels)
                            glTexImage1D(target, level, internalformat, width, border, format, type, (GLvoid)ptr);
                    }
                }
            }
        }

        public static void FramebufferTexture2D(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level)
        {
            glFramebufferTexture2D?.Invoke(target, attachment, textarget, texture, level);
        }

        public static void BlitFramebuffer(GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, GLbitfield mask, GLenum filter)
        {
            glBlitFramebuffer?.Invoke(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
        }

        public static void BlendFunc(GLenum sfactor, GLenum dfactor)
        {
            glBlendFunc?.Invoke(sfactor, dfactor);
        }

        public static void BindBufferBase(GLenum target, GLuint index, GLuint buffer)
        {
            glBindBufferBase?.Invoke(target, index, buffer);
        }

        public static void BindBufferRange(GLenum target, GLuint index, GLuint buffer, GLintptr offset, GLsizeiptr size)
        {
            glBindBufferRange?.Invoke(target, index, buffer, offset, size);
        }

        public static GLuint GetProgramResourceIndex(GLuint program, GLenum programInterface, string name)
        {
            if (glGetProgramResourceIndex != null)
                glGetProgramResourceIndex(program, programInterface, name);
            return 0;
        }

        public static GLuint GetUniformBlockIndex(GLuint program, string uniformBlockName)
        {
            if (glGetUniformBlockIndex != null)
                return glGetUniformBlockIndex(program, uniformBlockName);
            return 0;
        }

        public static GLint GetUniformLocation(GLuint program, string name)
        {
            if (glGetUniformLocation != null)
                return glGetUniformLocation(program, name);
            return 0;
        }

        public static void ShaderStorageBlockBinding(GLuint program, GLuint storageBlockIndex, GLuint storageBlockBinding)
        {
            glShaderStorageBlockBinding?.Invoke(program, storageBlockIndex, storageBlockBinding);
        }

        public static void UniformBlockBinding(GLuint program, GLuint uniformBlockIndex, GLuint uniformBlockBinding)
        {
            glUniformBlockBinding?.Invoke(program, uniformBlockIndex, uniformBlockBinding);
        }

        public static void GetShaderiv(GLuint shader, GLenum pname, GLint[] _params)
        {
            glGetShaderiv?.Invoke(shader, pname, _params);
        }

        public static void GetShaderInfoLog(GLuint shader, GLsizei bufSize, GLsizei[] length, GLchar[] infoLog)
        {
            glGetShaderInfoLog?.Invoke(shader, bufSize, length, infoLog);
        }

        public static void GetProgramiv(GLuint program, GLenum pname, GLint[] _params)
        {
            glGetProgramiv?.Invoke(program, pname, _params);
        }

        public static void GetProgramInfoLog(GLuint program, GLsizei bufSize, GLsizei[] length, GLchar[] infoLog)
        {
            glGetProgramInfoLog?.Invoke(program, bufSize, length, infoLog);
        }

        public static void GenRenderbuffers(GLsizei n, GLuint[] renderbuffers)
        {
            glGenRenderbuffers?.Invoke(n, renderbuffers);
        }

        public static void BindRenderbuffer(GLenum target, GLuint renderbuffer)
        {
            glBindRenderbuffer?.Invoke(target, renderbuffer);
        }

        public static void RenderbufferStorage(GLenum target, GLenum internalformat, GLsizei width, GLsizei height)
        {
            glRenderbufferStorage?.Invoke(target, internalformat, width, height);
        }

        public static void RenderbufferStorageMultisample(GLenum target, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height)
        {
            glRenderbufferStorageMultisample?.Invoke(target, samples, internalformat, width, height);
        }

        public static void FramebufferRenderbuffer(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer)
        {
            glFramebufferRenderbuffer?.Invoke(target, attachment, renderbuffertarget, renderbuffer);
        }

        public static void DeleteRenderbuffers(GLsizei n, GLuint[] renderbuffers)
        {
            glDeleteRenderbuffers?.Invoke(n, renderbuffers);
        }

        public static void StencilMask(GLuint mask)
        {
            glStencilMask?.Invoke(mask);
        }

        public static void StencilFunc(GLenum func, GLint _ref, GLuint mask)
        {
            glStencilFunc?.Invoke(func, _ref, mask);
        }

        public static void StencilOp(GLenum fail, GLenum zfail, GLenum zpass)
        {
            glStencilOp?.Invoke(fail, zfail, zpass);
        }

        public static void ColorMask(GLboolean red, GLboolean green, GLboolean blue, GLboolean alpha)
        {
            glColorMask?.Invoke(red, green, blue, alpha);
        }

        public static GLenum CheckFramebufferStatus(GLenum target)
        {
            if (glCheckFramebufferStatus != null)
                return glCheckFramebufferStatus(target);
            return 0;
        }
        #endregion

        #region Delegate
        public static PFNGLCULLFACEPROC glCullFace;
        public static PFNGLFRONTFACEPROC glFrontFace;
        public static PFNGLHINTPROC glHint;
        public static PFNGLLINEWIDTHPROC glLineWidth;
        public static PFNGLPOINTSIZEPROC glPointSize;
        public static PFNGLPOLYGONMODEPROC glPolygonMode;
        public static PFNGLSCISSORPROC glScissor;
        public static PFNGLTEXPARAMETERFPROC glTexParameterf;
        public static PFNGLTEXPARAMETERFVPROC glTexParameterfv;
        public static PFNGLTEXPARAMETERIPROC glTexParameteri;
        public static PFNGLTEXPARAMETERIVPROC glTexParameteriv;
        public static PFNGLTEXIMAGE1DPROC glTexImage1D;
        public static PFNGLTEXIMAGE2DPROC glTexImage2D;
        public static PFNGLDRAWBUFFERPROC glDrawBuffer;
        public static PFNGLCLEARPROC glClear;
        public static PFNGLCLEARCOLORPROC glClearColor;
        public static PFNGLCLEARSTENCILPROC glClearStencil;
        public static PFNGLCLEARDEPTHPROC glClearDepth;
        public static PFNGLSTENCILMASKPROC glStencilMask;
        public static PFNGLCOLORMASKPROC glColorMask;
        public static PFNGLDEPTHMASKPROC glDepthMask;
        public static PFNGLDISABLEPROC glDisable;
        public static PFNGLENABLEPROC glEnable;
        public static PFNGLFINISHPROC glFinish;
        public static PFNGLFLUSHPROC glFlush;
        public static PFNGLBLENDFUNCPROC glBlendFunc;
        public static PFNGLLOGICOPPROC glLogicOp;
        public static PFNGLSTENCILFUNCPROC glStencilFunc;
        public static PFNGLSTENCILOPPROC glStencilOp;
        public static PFNGLDEPTHFUNCPROC glDepthFunc;
        public static PFNGLPIXELSTOREFPROC glPixelStoref;
        public static PFNGLPIXELSTOREIPROC glPixelStorei;
        public static PFNGLREADBUFFERPROC glReadBuffer;
        public static PFNGLREADPIXELSPROC glReadPixels;
        public static PFNGLGETBOOLEANVPROC glGetBooleanv;
        public static PFNGLGETDOUBLEVPROC glGetDoublev;
        public static PFNGLGETERRORPROC glGetError;
        public static PFNGLGETFLOATVPROC glGetFloatv;
        public static PFNGLGETINTEGERVPROC glGetIntegerv;
        public static PFNGLGETSTRINGPROC glGetString;
        public static PFNGLGETTEXIMAGEPROC glGetTexImage;
        public static PFNGLGETTEXPARAMETERFVPROC glGetTexParameterfv;
        public static PFNGLGETTEXPARAMETERIVPROC glGetTexParameteriv;
        public static PFNGLGETTEXLEVELPARAMETERFVPROC glGetTexLevelParameterfv;
        public static PFNGLGETTEXLEVELPARAMETERIVPROC glGetTexLevelParameteriv;
        public static PFNGLISENABLEDPROC glIsEnabled;
        public static PFNGLDEPTHRANGEPROC glDepthRange;
        public static PFNGLVIEWPORTPROC glViewport;
        public static PFNGLDRAWARRAYSPROC glDrawArrays;
        public static PFNGLDRAWELEMENTSPROC glDrawElements;
        public static PFNGLGETPOINTERVPROC glGetPointerv;
        public static PFNGLPOLYGONOFFSETPROC glPolygonOffset;
        public static PFNGLCOPYTEXIMAGE1DPROC glCopyTexImage1D;
        public static PFNGLCOPYTEXIMAGE2DPROC glCopyTexImage2D;
        public static PFNGLCOPYTEXSUBIMAGE1DPROC glCopyTexSubImage1D;
        public static PFNGLCOPYTEXSUBIMAGE2DPROC glCopyTexSubImage2D;
        public static PFNGLTEXSUBIMAGE1DPROC glTexSubImage1D;
        public static PFNGLTEXSUBIMAGE2DPROC glTexSubImage2D;
        public static PFNGLBINDTEXTUREPROC glBindTexture;
        public static PFNGLDELETETEXTURESPROC glDeleteTextures;
        public static PFNGLGENTEXTURESPROC glGenTextures;
        public static PFNGLISTEXTUREPROC glIsTexture;
        public static PFNGLDRAWRANGEELEMENTSPROC glDrawRangeElements;
        public static PFNGLTEXIMAGE3DPROC glTexImage3D;
        public static PFNGLTEXSUBIMAGE3DPROC glTexSubImage3D;
        public static PFNGLCOPYTEXSUBIMAGE3DPROC glCopyTexSubImage3D;
        public static PFNGLACTIVETEXTUREPROC glActiveTexture;
        public static PFNGLSAMPLECOVERAGEPROC glSampleCoverage;
        public static PFNGLCOMPRESSEDTEXIMAGE3DPROC glCompressedTexImage3D;
        public static PFNGLCOMPRESSEDTEXIMAGE2DPROC glCompressedTexImage2D;
        public static PFNGLCOMPRESSEDTEXIMAGE1DPROC glCompressedTexImage1D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC glCompressedTexSubImage3D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC glCompressedTexSubImage2D;
        public static PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC glCompressedTexSubImage1D;
        public static PFNGLGETCOMPRESSEDTEXIMAGEPROC glGetCompressedTexImage;
        public static PFNGLBLENDFUNCSEPARATEPROC glBlendFuncSeparate;
        public static PFNGLMULTIDRAWARRAYSPROC glMultiDrawArrays;
        public static PFNGLMULTIDRAWELEMENTSPROC glMultiDrawElements;
        public static PFNGLPOINTPARAMETERFPROC glPointParameterf;
        public static PFNGLPOINTPARAMETERFVPROC glPointParameterfv;
        public static PFNGLPOINTPARAMETERIPROC glPointParameteri;
        public static PFNGLPOINTPARAMETERIVPROC glPointParameteriv;
        public static PFNGLBLENDCOLORPROC glBlendColor;
        public static PFNGLBLENDEQUATIONPROC glBlendEquation;
        public static PFNGLGENQUERIESPROC glGenQueries;
        public static PFNGLDELETEQUERIESPROC glDeleteQueries;
        public static PFNGLISQUERYPROC glIsQuery;
        public static PFNGLBEGINQUERYPROC glBeginQuery;
        public static PFNGLENDQUERYPROC glEndQuery;
        public static PFNGLGETQUERYIVPROC glGetQueryiv;
        public static PFNGLGETQUERYOBJECTIVPROC glGetQueryObjectiv;
        public static PFNGLGETQUERYOBJECTUIVPROC glGetQueryObjectuiv;
        public static PFNGLBINDBUFFERPROC glBindBuffer;
        public static PFNGLDELETEBUFFERSPROC glDeleteBuffers;
        public static PFNGLGENBUFFERSPROC glGenBuffers;
        public static PFNGLISBUFFERPROC glIsBuffer;
        public static PFNGLBUFFERDATAPROC glBufferData;
        public static PFNGLBUFFERSUBDATAPROC glBufferSubData;
        public static PFNGLGETBUFFERSUBDATAPROC glGetBufferSubData;
        public static PFNGLMAPBUFFERPROC glMapBuffer;
        public static PFNGLUNMAPBUFFERPROC glUnmapBuffer;
        public static PFNGLGETBUFFERPARAMETERIVPROC glGetBufferParameteriv;
        public static PFNGLGETBUFFERPOINTERVPROC glGetBufferPointerv;
        public static PFNGLBLENDEQUATIONSEPARATEPROC glBlendEquationSeparate;
        public static PFNGLDRAWBUFFERSPROC glDrawBuffers;
        public static PFNGLSTENCILOPSEPARATEPROC glStencilOpSeparate;
        public static PFNGLSTENCILFUNCSEPARATEPROC glStencilFuncSeparate;
        public static PFNGLSTENCILMASKSEPARATEPROC glStencilMaskSeparate;
        public static PFNGLATTACHSHADERPROC glAttachShader;
        public static PFNGLBINDATTRIBLOCATIONPROC glBindAttribLocation;
        public static PFNGLCOMPILESHADERPROC glCompileShader;
        public static PFNGLCREATEPROGRAMPROC glCreateProgram;
        public static PFNGLCREATESHADERPROC glCreateShader;
        public static PFNGLDELETEPROGRAMPROC glDeleteProgram;
        public static PFNGLDELETESHADERPROC glDeleteShader;
        public static PFNGLDETACHSHADERPROC glDetachShader;
        public static PFNGLDISABLEVERTEXATTRIBARRAYPROC glDisableVertexAttribArray;
        public static PFNGLENABLEVERTEXATTRIBARRAYPROC glEnableVertexAttribArray;
        public static PFNGLGETACTIVEATTRIBPROC glGetActiveAttrib;
        public static PFNGLGETACTIVEUNIFORMPROC glGetActiveUniform;
        public static PFNGLGETATTACHEDSHADERSPROC glGetAttachedShaders;
        public static PFNGLGETATTRIBLOCATIONPROC glGetAttribLocation;
        public static PFNGLGETPROGRAMIVPROC glGetProgramiv;
        public static PFNGLGETPROGRAMINFOLOGPROC glGetProgramInfoLog;
        public static PFNGLGETSHADERIVPROC glGetShaderiv;
        public static PFNGLGETSHADERINFOLOGPROC glGetShaderInfoLog;
        public static PFNGLGETSHADERSOURCEPROC glGetShaderSource;
        public static PFNGLGETUNIFORMLOCATIONPROC glGetUniformLocation;
        public static PFNGLGETUNIFORMFVPROC glGetUniformfv;
        public static PFNGLGETUNIFORMIVPROC glGetUniformiv;
        public static PFNGLGETVERTEXATTRIBDVPROC glGetVertexAttribdv;
        public static PFNGLGETVERTEXATTRIBFVPROC glGetVertexAttribfv;
        public static PFNGLGETVERTEXATTRIBIVPROC glGetVertexAttribiv;
        public static PFNGLGETVERTEXATTRIBPOINTERVPROC glGetVertexAttribPointerv;
        public static PFNGLISPROGRAMPROC glIsProgram;
        public static PFNGLISSHADERPROC glIsShader;
        public static PFNGLLINKPROGRAMPROC glLinkProgram;
        public static PFNGLSHADERSOURCEPROC glShaderSource;
        public static PFNGLUSEPROGRAMPROC glUseProgram;
        public static PFNGLUNIFORM1FPROC glUniform1f;
        public static PFNGLUNIFORM2FPROC glUniform2f;
        public static PFNGLUNIFORM3FPROC glUniform3f;
        public static PFNGLUNIFORM4FPROC glUniform4f;
        public static PFNGLUNIFORM1IPROC glUniform1i;
        public static PFNGLUNIFORM2IPROC glUniform2i;
        public static PFNGLUNIFORM3IPROC glUniform3i;
        public static PFNGLUNIFORM4IPROC glUniform4i;
        public static PFNGLUNIFORM1FVPROC glUniform1fv;
        public static PFNGLUNIFORM2FVPROC glUniform2fv;
        public static PFNGLUNIFORM3FVPROC glUniform3fv;
        public static PFNGLUNIFORM4FVPROC glUniform4fv;
        public static PFNGLUNIFORM1IVPROC glUniform1iv;
        public static PFNGLUNIFORM2IVPROC glUniform2iv;
        public static PFNGLUNIFORM3IVPROC glUniform3iv;
        public static PFNGLUNIFORM4IVPROC glUniform4iv;
        public static PFNGLUNIFORMMATRIX2FVPROC glUniformMatrix2fv;
        public static PFNGLUNIFORMMATRIX3FVPROC glUniformMatrix3fv;
        public static PFNGLUNIFORMMATRIX4FVPROC glUniformMatrix4fv;
        public static PFNGLVALIDATEPROGRAMPROC glValidateProgram;
        public static PFNGLVERTEXATTRIB1DPROC glVertexAttrib1d;
        public static PFNGLVERTEXATTRIB1DVPROC glVertexAttrib1dv;
        public static PFNGLVERTEXATTRIB1FPROC glVertexAttrib1f;
        public static PFNGLVERTEXATTRIB1FVPROC glVertexAttrib1fv;
        public static PFNGLVERTEXATTRIB1SPROC glVertexAttrib1s;
        public static PFNGLVERTEXATTRIB1SVPROC glVertexAttrib1sv;
        public static PFNGLVERTEXATTRIB2DPROC glVertexAttrib2d;
        public static PFNGLVERTEXATTRIB2DVPROC glVertexAttrib2dv;
        public static PFNGLVERTEXATTRIB2FPROC glVertexAttrib2f;
        public static PFNGLVERTEXATTRIB2FVPROC glVertexAttrib2fv;
        public static PFNGLVERTEXATTRIB2SPROC glVertexAttrib2s;
        public static PFNGLVERTEXATTRIB2SVPROC glVertexAttrib2sv;
        public static PFNGLVERTEXATTRIB3DPROC glVertexAttrib3d;
        public static PFNGLVERTEXATTRIB3DVPROC glVertexAttrib3dv;
        public static PFNGLVERTEXATTRIB3FPROC glVertexAttrib3f;
        public static PFNGLVERTEXATTRIB3FVPROC glVertexAttrib3fv;
        public static PFNGLVERTEXATTRIB3SPROC glVertexAttrib3s;
        public static PFNGLVERTEXATTRIB3SVPROC glVertexAttrib3sv;
        public static PFNGLVERTEXATTRIB4NBVPROC glVertexAttrib4Nbv;
        public static PFNGLVERTEXATTRIB4NIVPROC glVertexAttrib4Niv;
        public static PFNGLVERTEXATTRIB4NSVPROC glVertexAttrib4Nsv;
        public static PFNGLVERTEXATTRIB4NUBPROC glVertexAttrib4Nub;
        public static PFNGLVERTEXATTRIB4NUBVPROC glVertexAttrib4Nubv;
        public static PFNGLVERTEXATTRIB4NUIVPROC glVertexAttrib4Nuiv;
        public static PFNGLVERTEXATTRIB4NUSVPROC glVertexAttrib4Nusv;
        public static PFNGLVERTEXATTRIB4BVPROC glVertexAttrib4bv;
        public static PFNGLVERTEXATTRIB4DPROC glVertexAttrib4d;
        public static PFNGLVERTEXATTRIB4DVPROC glVertexAttrib4dv;
        public static PFNGLVERTEXATTRIB4FPROC glVertexAttrib4f;
        public static PFNGLVERTEXATTRIB4FVPROC glVertexAttrib4fv;
        public static PFNGLVERTEXATTRIB4IVPROC glVertexAttrib4iv;
        public static PFNGLVERTEXATTRIB4SPROC glVertexAttrib4s;
        public static PFNGLVERTEXATTRIB4SVPROC glVertexAttrib4sv;
        public static PFNGLVERTEXATTRIB4UBVPROC glVertexAttrib4ubv;
        public static PFNGLVERTEXATTRIB4UIVPROC glVertexAttrib4uiv;
        public static PFNGLVERTEXATTRIB4USVPROC glVertexAttrib4usv;
        public static PFNGLVERTEXATTRIBPOINTERPROC glVertexAttribPointer;
        public static PFNGLUNIFORMMATRIX2X3FVPROC glUniformMatrix2x3fv;
        public static PFNGLUNIFORMMATRIX3X2FVPROC glUniformMatrix3x2fv;
        public static PFNGLUNIFORMMATRIX2X4FVPROC glUniformMatrix2x4fv;
        public static PFNGLUNIFORMMATRIX4X2FVPROC glUniformMatrix4x2fv;
        public static PFNGLUNIFORMMATRIX3X4FVPROC glUniformMatrix3x4fv;
        public static PFNGLUNIFORMMATRIX4X3FVPROC glUniformMatrix4x3fv;
        public static PFNGLCOLORMASKIPROC glColorMaski;
        public static PFNGLGETBOOLEANI_VPROC glGetBooleani_v;
        public static PFNGLGETINTEGERI_VPROC glGetIntegeri_v;
        public static PFNGLENABLEIPROC glEnablei;
        public static PFNGLDISABLEIPROC glDisablei;
        public static PFNGLISENABLEDIPROC glIsEnabledi;
        public static PFNGLBEGINTRANSFORMFEEDBACKPROC glBeginTransformFeedback;
        public static PFNGLENDTRANSFORMFEEDBACKPROC glEndTransformFeedback;
        public static PFNGLBINDBUFFERRANGEPROC glBindBufferRange;
        public static PFNGLBINDBUFFERBASEPROC glBindBufferBase;
        public static PFNGLTRANSFORMFEEDBACKVARYINGSPROC glTransformFeedbackVaryings;
        public static PFNGLGETTRANSFORMFEEDBACKVARYINGPROC glGetTransformFeedbackVarying;
        public static PFNGLCLAMPCOLORPROC glClampColor;
        public static PFNGLBEGINCONDITIONALRENDERPROC glBeginConditionalRender;
        public static PFNGLENDCONDITIONALRENDERPROC glEndConditionalRender;
        public static PFNGLVERTEXATTRIBIPOINTERPROC glVertexAttribIPointer;
        public static PFNGLGETVERTEXATTRIBIIVPROC glGetVertexAttribIiv;
        public static PFNGLGETVERTEXATTRIBIUIVPROC glGetVertexAttribIuiv;
        public static PFNGLVERTEXATTRIBI1IPROC glVertexAttribI1i;
        public static PFNGLVERTEXATTRIBI2IPROC glVertexAttribI2i;
        public static PFNGLVERTEXATTRIBI3IPROC glVertexAttribI3i;
        public static PFNGLVERTEXATTRIBI4IPROC glVertexAttribI4i;
        public static PFNGLVERTEXATTRIBI1UIPROC glVertexAttribI1ui;
        public static PFNGLVERTEXATTRIBI2UIPROC glVertexAttribI2ui;
        public static PFNGLVERTEXATTRIBI3UIPROC glVertexAttribI3ui;
        public static PFNGLVERTEXATTRIBI4UIPROC glVertexAttribI4ui;
        public static PFNGLVERTEXATTRIBI1IVPROC glVertexAttribI1iv;
        public static PFNGLVERTEXATTRIBI2IVPROC glVertexAttribI2iv;
        public static PFNGLVERTEXATTRIBI3IVPROC glVertexAttribI3iv;
        public static PFNGLVERTEXATTRIBI4IVPROC glVertexAttribI4iv;
        public static PFNGLVERTEXATTRIBI1UIVPROC glVertexAttribI1uiv;
        public static PFNGLVERTEXATTRIBI2UIVPROC glVertexAttribI2uiv;
        public static PFNGLVERTEXATTRIBI3UIVPROC glVertexAttribI3uiv;
        public static PFNGLVERTEXATTRIBI4UIVPROC glVertexAttribI4uiv;
        public static PFNGLVERTEXATTRIBI4BVPROC glVertexAttribI4bv;
        public static PFNGLVERTEXATTRIBI4SVPROC glVertexAttribI4sv;
        public static PFNGLVERTEXATTRIBI4UBVPROC glVertexAttribI4ubv;
        public static PFNGLVERTEXATTRIBI4USVPROC glVertexAttribI4usv;
        public static PFNGLGETUNIFORMUIVPROC glGetUniformuiv;
        public static PFNGLBINDFRAGDATALOCATIONPROC glBindFragDataLocation;
        public static PFNGLGETFRAGDATALOCATIONPROC glGetFragDataLocation;
        public static PFNGLUNIFORM1UIPROC glUniform1ui;
        public static PFNGLUNIFORM2UIPROC glUniform2ui;
        public static PFNGLUNIFORM3UIPROC glUniform3ui;
        public static PFNGLUNIFORM4UIPROC glUniform4ui;
        public static PFNGLUNIFORM1UIVPROC glUniform1uiv;
        public static PFNGLUNIFORM2UIVPROC glUniform2uiv;
        public static PFNGLUNIFORM3UIVPROC glUniform3uiv;
        public static PFNGLUNIFORM4UIVPROC glUniform4uiv;
        public static PFNGLTEXPARAMETERIIVPROC glTexParameterIiv;
        public static PFNGLTEXPARAMETERIUIVPROC glTexParameterIuiv;
        public static PFNGLGETTEXPARAMETERIIVPROC glGetTexParameterIiv;
        public static PFNGLGETTEXPARAMETERIUIVPROC glGetTexParameterIuiv;
        public static PFNGLCLEARBUFFERIVPROC glClearBufferiv;
        public static PFNGLCLEARBUFFERUIVPROC glClearBufferuiv;
        public static PFNGLCLEARBUFFERFVPROC glClearBufferfv;
        public static PFNGLCLEARBUFFERFIPROC glClearBufferfi;
        public static PFNGLGETSTRINGIPROC glGetStringi;
        public static PFNGLISRENDERBUFFERPROC glIsRenderbuffer;
        public static PFNGLBINDRENDERBUFFERPROC glBindRenderbuffer;
        public static PFNGLDELETERENDERBUFFERSPROC glDeleteRenderbuffers;
        public static PFNGLGENRENDERBUFFERSPROC glGenRenderbuffers;
        public static PFNGLRENDERBUFFERSTORAGEPROC glRenderbufferStorage;
        public static PFNGLGETRENDERBUFFERPARAMETERIVPROC glGetRenderbufferParameteriv;
        public static PFNGLISFRAMEBUFFERPROC glIsFramebuffer;
        public static PFNGLBINDFRAMEBUFFERPROC glBindFramebuffer;
        public static PFNGLDELETEFRAMEBUFFERSPROC glDeleteFramebuffers;
        public static PFNGLGENFRAMEBUFFERSPROC glGenFramebuffers;
        public static PFNGLCHECKFRAMEBUFFERSTATUSPROC glCheckFramebufferStatus;
        public static PFNGLFRAMEBUFFERTEXTURE1DPROC glFramebufferTexture1D;
        public static PFNGLFRAMEBUFFERTEXTURE2DPROC glFramebufferTexture2D;
        public static PFNGLFRAMEBUFFERTEXTURE3DPROC glFramebufferTexture3D;
        public static PFNGLFRAMEBUFFERRENDERBUFFERPROC glFramebufferRenderbuffer;
        public static PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC glGetFramebufferAttachmentParameteriv;
        public static PFNGLGENERATEMIPMAPPROC glGenerateMipmap;
        public static PFNGLBLITFRAMEBUFFERPROC glBlitFramebuffer;
        public static PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC glRenderbufferStorageMultisample;
        public static PFNGLFRAMEBUFFERTEXTURELAYERPROC glFramebufferTextureLayer;
        public static PFNGLMAPBUFFERRANGEPROC glMapBufferRange;
        public static PFNGLFLUSHMAPPEDBUFFERRANGEPROC glFlushMappedBufferRange;
        public static PFNGLBINDVERTEXARRAYPROC glBindVertexArray;
        public static PFNGLDELETEVERTEXARRAYSPROC glDeleteVertexArrays;
        public static PFNGLGENVERTEXARRAYSPROC glGenVertexArrays;
        public static PFNGLISVERTEXARRAYPROC glIsVertexArray;
        public static PFNGLDRAWARRAYSINSTANCEDPROC glDrawArraysInstanced;
        public static PFNGLDRAWELEMENTSINSTANCEDPROC glDrawElementsInstanced;
        public static PFNGLTEXBUFFERPROC glTexBuffer;
        public static PFNGLPRIMITIVERESTARTINDEXPROC glPrimitiveRestartIndex;
        public static PFNGLCOPYBUFFERSUBDATAPROC glCopyBufferSubData;
        public static PFNGLGETUNIFORMINDICESPROC glGetUniformIndices;
        public static PFNGLGETACTIVEUNIFORMSIVPROC glGetActiveUniformsiv;
        public static PFNGLGETACTIVEUNIFORMNAMEPROC glGetActiveUniformName;
        public static PFNGLGETUNIFORMBLOCKINDEXPROC glGetUniformBlockIndex;
        public static PFNGLGETACTIVEUNIFORMBLOCKIVPROC glGetActiveUniformBlockiv;
        public static PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC glGetActiveUniformBlockName;
        public static PFNGLUNIFORMBLOCKBINDINGPROC glUniformBlockBinding;
        public static PFNGLDRAWELEMENTSBASEVERTEXPROC glDrawElementsBaseVertex;
        public static PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC glDrawRangeElementsBaseVertex;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC glDrawElementsInstancedBaseVertex;
        public static PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC glMultiDrawElementsBaseVertex;
        public static PFNGLPROVOKINGVERTEXPROC glProvokingVertex;
        public static PFNGLFENCESYNCPROC glFenceSync;
        public static PFNGLISSYNCPROC glIsSync;
        public static PFNGLDELETESYNCPROC glDeleteSync;
        public static PFNGLCLIENTWAITSYNCPROC glClientWaitSync;
        public static PFNGLWAITSYNCPROC glWaitSync;
        public static PFNGLGETINTEGER64VPROC glGetInteger64v;
        public static PFNGLGETSYNCIVPROC glGetSynciv;
        public static PFNGLGETINTEGER64I_VPROC glGetInteger64i_v;
        public static PFNGLGETBUFFERPARAMETERI64VPROC glGetBufferParameteri64v;
        public static PFNGLFRAMEBUFFERTEXTUREPROC glFramebufferTexture;
        public static PFNGLTEXIMAGE2DMULTISAMPLEPROC glTexImage2DMultisample;
        public static PFNGLTEXIMAGE3DMULTISAMPLEPROC glTexImage3DMultisample;
        public static PFNGLGETMULTISAMPLEFVPROC glGetMultisamplefv;
        public static PFNGLSAMPLEMASKIPROC glSampleMaski;
        public static PFNGLBINDFRAGDATALOCATIONINDEXEDPROC glBindFragDataLocationIndexed;
        public static PFNGLGETFRAGDATAINDEXPROC glGetFragDataIndex;
        public static PFNGLGENSAMPLERSPROC glGenSamplers;
        public static PFNGLDELETESAMPLERSPROC glDeleteSamplers;
        public static PFNGLISSAMPLERPROC glIsSampler;
        public static PFNGLBINDSAMPLERPROC glBindSampler;
        public static PFNGLSAMPLERPARAMETERIPROC glSamplerParameteri;
        public static PFNGLSAMPLERPARAMETERIVPROC glSamplerParameteriv;
        public static PFNGLSAMPLERPARAMETERFPROC glSamplerParameterf;
        public static PFNGLSAMPLERPARAMETERFVPROC glSamplerParameterfv;
        public static PFNGLSAMPLERPARAMETERIIVPROC glSamplerParameterIiv;
        public static PFNGLSAMPLERPARAMETERIUIVPROC glSamplerParameterIuiv;
        public static PFNGLGETSAMPLERPARAMETERIVPROC glGetSamplerParameteriv;
        public static PFNGLGETSAMPLERPARAMETERIIVPROC glGetSamplerParameterIiv;
        public static PFNGLGETSAMPLERPARAMETERFVPROC glGetSamplerParameterfv;
        public static PFNGLGETSAMPLERPARAMETERIUIVPROC glGetSamplerParameterIuiv;
        public static PFNGLQUERYCOUNTERPROC glQueryCounter;
        public static PFNGLGETQUERYOBJECTI64VPROC glGetQueryObjecti64v;
        public static PFNGLGETQUERYOBJECTUI64VPROC glGetQueryObjectui64v;
        public static PFNGLVERTEXATTRIBDIVISORPROC glVertexAttribDivisor;
        public static PFNGLVERTEXATTRIBP1UIPROC glVertexAttribP1ui;
        public static PFNGLVERTEXATTRIBP1UIVPROC glVertexAttribP1uiv;
        public static PFNGLVERTEXATTRIBP2UIPROC glVertexAttribP2ui;
        public static PFNGLVERTEXATTRIBP2UIVPROC glVertexAttribP2uiv;
        public static PFNGLVERTEXATTRIBP3UIPROC glVertexAttribP3ui;
        public static PFNGLVERTEXATTRIBP3UIVPROC glVertexAttribP3uiv;
        public static PFNGLVERTEXATTRIBP4UIPROC glVertexAttribP4ui;
        public static PFNGLVERTEXATTRIBP4UIVPROC glVertexAttribP4uiv;
        public static PFNGLMINSAMPLESHADINGPROC glMinSampleShading;
        public static PFNGLBLENDEQUATIONIPROC glBlendEquationi;
        public static PFNGLBLENDEQUATIONSEPARATEIPROC glBlendEquationSeparatei;
        public static PFNGLBLENDFUNCIPROC glBlendFunci;
        public static PFNGLBLENDFUNCSEPARATEIPROC glBlendFuncSeparatei;
        public static PFNGLDRAWARRAYSINDIRECTPROC glDrawArraysIndirect;
        public static PFNGLDRAWELEMENTSINDIRECTPROC glDrawElementsIndirect;
        public static PFNGLUNIFORM1DPROC glUniform1d;
        public static PFNGLUNIFORM2DPROC glUniform2d;
        public static PFNGLUNIFORM3DPROC glUniform3d;
        public static PFNGLUNIFORM4DPROC glUniform4d;
        public static PFNGLUNIFORM1DVPROC glUniform1dv;
        public static PFNGLUNIFORM2DVPROC glUniform2dv;
        public static PFNGLUNIFORM3DVPROC glUniform3dv;
        public static PFNGLUNIFORM4DVPROC glUniform4dv;
        public static PFNGLUNIFORMMATRIX2DVPROC glUniformMatrix2dv;
        public static PFNGLUNIFORMMATRIX3DVPROC glUniformMatrix3dv;
        public static PFNGLUNIFORMMATRIX4DVPROC glUniformMatrix4dv;
        public static PFNGLUNIFORMMATRIX2X3DVPROC glUniformMatrix2x3dv;
        public static PFNGLUNIFORMMATRIX2X4DVPROC glUniformMatrix2x4dv;
        public static PFNGLUNIFORMMATRIX3X2DVPROC glUniformMatrix3x2dv;
        public static PFNGLUNIFORMMATRIX3X4DVPROC glUniformMatrix3x4dv;
        public static PFNGLUNIFORMMATRIX4X2DVPROC glUniformMatrix4x2dv;
        public static PFNGLUNIFORMMATRIX4X3DVPROC glUniformMatrix4x3dv;
        public static PFNGLGETUNIFORMDVPROC glGetUniformdv;
        public static PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC glGetSubroutineUniformLocation;
        public static PFNGLGETSUBROUTINEINDEXPROC glGetSubroutineIndex;
        public static PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC glGetActiveSubroutineUniformiv;
        public static PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC glGetActiveSubroutineUniformName;
        public static PFNGLGETACTIVESUBROUTINENAMEPROC glGetActiveSubroutineName;
        public static PFNGLUNIFORMSUBROUTINESUIVPROC glUniformSubroutinesuiv;
        public static PFNGLGETUNIFORMSUBROUTINEUIVPROC glGetUniformSubroutineuiv;
        public static PFNGLGETPROGRAMSTAGEIVPROC glGetProgramStageiv;
        public static PFNGLPATCHPARAMETERIPROC glPatchParameteri;
        public static PFNGLPATCHPARAMETERFVPROC glPatchParameterfv;
        public static PFNGLBINDTRANSFORMFEEDBACKPROC glBindTransformFeedback;
        public static PFNGLDELETETRANSFORMFEEDBACKSPROC glDeleteTransformFeedbacks;
        public static PFNGLGENTRANSFORMFEEDBACKSPROC glGenTransformFeedbacks;
        public static PFNGLISTRANSFORMFEEDBACKPROC glIsTransformFeedback;
        public static PFNGLPAUSETRANSFORMFEEDBACKPROC glPauseTransformFeedback;
        public static PFNGLRESUMETRANSFORMFEEDBACKPROC glResumeTransformFeedback;
        public static PFNGLDRAWTRANSFORMFEEDBACKPROC glDrawTransformFeedback;
        public static PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC glDrawTransformFeedbackStream;
        public static PFNGLBEGINQUERYINDEXEDPROC glBeginQueryIndexed;
        public static PFNGLENDQUERYINDEXEDPROC glEndQueryIndexed;
        public static PFNGLGETQUERYINDEXEDIVPROC glGetQueryIndexediv;
        public static PFNGLRELEASESHADERCOMPILERPROC glReleaseShaderCompiler;
        public static PFNGLSHADERBINARYPROC glShaderBinary;
        public static PFNGLGETSHADERPRECISIONFORMATPROC glGetShaderPrecisionFormat;
        public static PFNGLDEPTHRANGEFPROC glDepthRangef;
        public static PFNGLCLEARDEPTHFPROC glClearDepthf;
        public static PFNGLGETPROGRAMBINARYPROC glGetProgramBinary;
        public static PFNGLPROGRAMBINARYPROC glProgramBinary;
        public static PFNGLPROGRAMPARAMETERIPROC glProgramParameteri;
        public static PFNGLUSEPROGRAMSTAGESPROC glUseProgramStages;
        public static PFNGLACTIVESHADERPROGRAMPROC glActiveShaderProgram;
        public static PFNGLCREATESHADERPROGRAMVPROC glCreateShaderProgramv;
        public static PFNGLBINDPROGRAMPIPELINEPROC glBindProgramPipeline;
        public static PFNGLDELETEPROGRAMPIPELINESPROC glDeleteProgramPipelines;
        public static PFNGLGENPROGRAMPIPELINESPROC glGenProgramPipelines;
        public static PFNGLISPROGRAMPIPELINEPROC glIsProgramPipeline;
        public static PFNGLGETPROGRAMPIPELINEIVPROC glGetProgramPipelineiv;
        public static PFNGLPROGRAMUNIFORM1IPROC glProgramUniform1i;
        public static PFNGLPROGRAMUNIFORM1IVPROC glProgramUniform1iv;
        public static PFNGLPROGRAMUNIFORM1FPROC glProgramUniform1f;
        public static PFNGLPROGRAMUNIFORM1FVPROC glProgramUniform1fv;
        public static PFNGLPROGRAMUNIFORM1DPROC glProgramUniform1d;
        public static PFNGLPROGRAMUNIFORM1DVPROC glProgramUniform1dv;
        public static PFNGLPROGRAMUNIFORM1UIPROC glProgramUniform1ui;
        public static PFNGLPROGRAMUNIFORM1UIVPROC glProgramUniform1uiv;
        public static PFNGLPROGRAMUNIFORM2IPROC glProgramUniform2i;
        public static PFNGLPROGRAMUNIFORM2IVPROC glProgramUniform2iv;
        public static PFNGLPROGRAMUNIFORM2FPROC glProgramUniform2f;
        public static PFNGLPROGRAMUNIFORM2FVPROC glProgramUniform2fv;
        public static PFNGLPROGRAMUNIFORM2DPROC glProgramUniform2d;
        public static PFNGLPROGRAMUNIFORM2DVPROC glProgramUniform2dv;
        public static PFNGLPROGRAMUNIFORM2UIPROC glProgramUniform2ui;
        public static PFNGLPROGRAMUNIFORM2UIVPROC glProgramUniform2uiv;
        public static PFNGLPROGRAMUNIFORM3IPROC glProgramUniform3i;
        public static PFNGLPROGRAMUNIFORM3IVPROC glProgramUniform3iv;
        public static PFNGLPROGRAMUNIFORM3FPROC glProgramUniform3f;
        public static PFNGLPROGRAMUNIFORM3FVPROC glProgramUniform3fv;
        public static PFNGLPROGRAMUNIFORM3DPROC glProgramUniform3d;
        public static PFNGLPROGRAMUNIFORM3DVPROC glProgramUniform3dv;
        public static PFNGLPROGRAMUNIFORM3UIPROC glProgramUniform3ui;
        public static PFNGLPROGRAMUNIFORM3UIVPROC glProgramUniform3uiv;
        public static PFNGLPROGRAMUNIFORM4IPROC glProgramUniform4i;
        public static PFNGLPROGRAMUNIFORM4IVPROC glProgramUniform4iv;
        public static PFNGLPROGRAMUNIFORM4FPROC glProgramUniform4f;
        public static PFNGLPROGRAMUNIFORM4FVPROC glProgramUniform4fv;
        public static PFNGLPROGRAMUNIFORM4DPROC glProgramUniform4d;
        public static PFNGLPROGRAMUNIFORM4DVPROC glProgramUniform4dv;
        public static PFNGLPROGRAMUNIFORM4UIPROC glProgramUniform4ui;
        public static PFNGLPROGRAMUNIFORM4UIVPROC glProgramUniform4uiv;
        public static PFNGLPROGRAMUNIFORMMATRIX2FVPROC glProgramUniformMatrix2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3FVPROC glProgramUniformMatrix3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4FVPROC glProgramUniformMatrix4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2DVPROC glProgramUniformMatrix2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3DVPROC glProgramUniformMatrix3dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4DVPROC glProgramUniformMatrix4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC glProgramUniformMatrix2x3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC glProgramUniformMatrix3x2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC glProgramUniformMatrix2x4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC glProgramUniformMatrix4x2fv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC glProgramUniformMatrix3x4fv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC glProgramUniformMatrix4x3fv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC glProgramUniformMatrix2x3dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC glProgramUniformMatrix3x2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC glProgramUniformMatrix2x4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC glProgramUniformMatrix4x2dv;
        public static PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC glProgramUniformMatrix3x4dv;
        public static PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC glProgramUniformMatrix4x3dv;
        public static PFNGLVALIDATEPROGRAMPIPELINEPROC glValidateProgramPipeline;
        public static PFNGLGETPROGRAMPIPELINEINFOLOGPROC glGetProgramPipelineInfoLog;
        public static PFNGLVERTEXATTRIBL1DPROC glVertexAttribL1d;
        public static PFNGLVERTEXATTRIBL2DPROC glVertexAttribL2d;
        public static PFNGLVERTEXATTRIBL3DPROC glVertexAttribL3d;
        public static PFNGLVERTEXATTRIBL4DPROC glVertexAttribL4d;
        public static PFNGLVERTEXATTRIBL1DVPROC glVertexAttribL1dv;
        public static PFNGLVERTEXATTRIBL2DVPROC glVertexAttribL2dv;
        public static PFNGLVERTEXATTRIBL3DVPROC glVertexAttribL3dv;
        public static PFNGLVERTEXATTRIBL4DVPROC glVertexAttribL4dv;
        public static PFNGLVERTEXATTRIBLPOINTERPROC glVertexAttribLPointer;
        public static PFNGLGETVERTEXATTRIBLDVPROC glGetVertexAttribLdv;
        public static PFNGLVIEWPORTARRAYVPROC glViewportArrayv;
        public static PFNGLVIEWPORTINDEXEDFPROC glViewportIndexedf;
        public static PFNGLVIEWPORTINDEXEDFVPROC glViewportIndexedfv;
        public static PFNGLSCISSORARRAYVPROC glScissorArrayv;
        public static PFNGLSCISSORINDEXEDPROC glScissorIndexed;
        public static PFNGLSCISSORINDEXEDVPROC glScissorIndexedv;
        public static PFNGLDEPTHRANGEARRAYVPROC glDepthRangeArrayv;
        public static PFNGLDEPTHRANGEINDEXEDPROC glDepthRangeIndexed;
        public static PFNGLGETFLOATI_VPROC glGetFloati_v;
        public static PFNGLGETDOUBLEI_VPROC glGetDoublei_v;
        public static PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC glDrawArraysInstancedBaseInstance;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC glDrawElementsInstancedBaseInstance;
        public static PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC glDrawElementsInstancedBaseVertexBaseInstance;
        public static PFNGLGETINTERNALFORMATIVPROC glGetInternalformativ;
        public static PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC glGetActiveAtomicCounterBufferiv;
        public static PFNGLBINDIMAGETEXTUREPROC glBindImageTexture;
        public static PFNGLMEMORYBARRIERPROC glMemoryBarrier;
        public static PFNGLTEXSTORAGE1DPROC glTexStorage1D;
        public static PFNGLTEXSTORAGE2DPROC glTexStorage2D;
        public static PFNGLTEXSTORAGE3DPROC glTexStorage3D;
        public static PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC glDrawTransformFeedbackInstanced;
        public static PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC glDrawTransformFeedbackStreamInstanced;
        public static PFNGLCLEARBUFFERDATAPROC glClearBufferData;
        public static PFNGLCLEARBUFFERSUBDATAPROC glClearBufferSubData;
        public static PFNGLDISPATCHCOMPUTEPROC glDispatchCompute;
        public static PFNGLDISPATCHCOMPUTEINDIRECTPROC glDispatchComputeIndirect;
        public static PFNGLCOPYIMAGESUBDATAPROC glCopyImageSubData;
        public static PFNGLFRAMEBUFFERPARAMETERIPROC glFramebufferParameteri;
        public static PFNGLGETFRAMEBUFFERPARAMETERIVPROC glGetFramebufferParameteriv;
        public static PFNGLGETINTERNALFORMATI64VPROC glGetInternalformati64v;
        public static PFNGLINVALIDATETEXSUBIMAGEPROC glInvalidateTexSubImage;
        public static PFNGLINVALIDATETEXIMAGEPROC glInvalidateTexImage;
        public static PFNGLINVALIDATEBUFFERSUBDATAPROC glInvalidateBufferSubData;
        public static PFNGLINVALIDATEBUFFERDATAPROC glInvalidateBufferData;
        public static PFNGLINVALIDATEFRAMEBUFFERPROC glInvalidateFramebuffer;
        public static PFNGLINVALIDATESUBFRAMEBUFFERPROC glInvalidateSubFramebuffer;
        public static PFNGLMULTIDRAWARRAYSINDIRECTPROC glMultiDrawArraysIndirect;
        public static PFNGLMULTIDRAWELEMENTSINDIRECTPROC glMultiDrawElementsIndirect;
        public static PFNGLGETPROGRAMINTERFACEIVPROC glGetProgramInterfaceiv;
        public static PFNGLGETPROGRAMRESOURCEINDEXPROC glGetProgramResourceIndex;
        public static PFNGLGETPROGRAMRESOURCENAMEPROC glGetProgramResourceName;
        public static PFNGLGETPROGRAMRESOURCEIVPROC glGetProgramResourceiv;
        public static PFNGLGETPROGRAMRESOURCELOCATIONPROC glGetProgramResourceLocation;
        public static PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC glGetProgramResourceLocationIndex;
        public static PFNGLSHADERSTORAGEBLOCKBINDINGPROC glShaderStorageBlockBinding;
        public static PFNGLTEXBUFFERRANGEPROC glTexBufferRange;
        public static PFNGLTEXSTORAGE2DMULTISAMPLEPROC glTexStorage2DMultisample;
        public static PFNGLTEXSTORAGE3DMULTISAMPLEPROC glTexStorage3DMultisample;
        public static PFNGLTEXTUREVIEWPROC glTextureView;
        public static PFNGLBINDVERTEXBUFFERPROC glBindVertexBuffer;
        public static PFNGLVERTEXATTRIBFORMATPROC glVertexAttribFormat;
        public static PFNGLVERTEXATTRIBIFORMATPROC glVertexAttribIFormat;
        public static PFNGLVERTEXATTRIBLFORMATPROC glVertexAttribLFormat;
        public static PFNGLVERTEXATTRIBBINDINGPROC glVertexAttribBinding;
        public static PFNGLVERTEXBINDINGDIVISORPROC glVertexBindingDivisor;
        public static PFNGLDEBUGMESSAGECONTROLPROC glDebugMessageControl;
        public static PFNGLDEBUGMESSAGEINSERTPROC glDebugMessageInsert;
        public static PFNGLDEBUGMESSAGECALLBACKPROC glDebugMessageCallback;
        public static PFNGLGETDEBUGMESSAGELOGPROC glGetDebugMessageLog;
        public static PFNGLPUSHDEBUGGROUPPROC glPushDebugGroup;
        public static PFNGLPOPDEBUGGROUPPROC glPopDebugGroup;
        public static PFNGLOBJECTLABELPROC glObjectLabel;
        public static PFNGLGETOBJECTLABELPROC glGetObjectLabel;
        public static PFNGLOBJECTPTRLABELPROC glObjectPtrLabel;
        public static PFNGLGETOBJECTPTRLABELPROC glGetObjectPtrLabel;
        public static PFNGLBUFFERSTORAGEPROC glBufferStorage;
        public static PFNGLCLEARTEXIMAGEPROC glClearTexImage;
        public static PFNGLCLEARTEXSUBIMAGEPROC glClearTexSubImage;
        public static PFNGLBINDBUFFERSBASEPROC glBindBuffersBase;
        public static PFNGLBINDBUFFERSRANGEPROC glBindBuffersRange;
        public static PFNGLBINDTEXTURESPROC glBindTextures;
        public static PFNGLBINDSAMPLERSPROC glBindSamplers;
        public static PFNGLBINDIMAGETEXTURESPROC glBindImageTextures;
        public static PFNGLBINDVERTEXBUFFERSPROC glBindVertexBuffers;
        public static PFNGLCLIPCONTROLPROC glClipControl;
        public static PFNGLCREATETRANSFORMFEEDBACKSPROC glCreateTransformFeedbacks;
        public static PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC glTransformFeedbackBufferBase;
        public static PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC glTransformFeedbackBufferRange;
        public static PFNGLGETTRANSFORMFEEDBACKIVPROC glGetTransformFeedbackiv;
        public static PFNGLGETTRANSFORMFEEDBACKI_VPROC glGetTransformFeedbacki_v;
        public static PFNGLGETTRANSFORMFEEDBACKI64_VPROC glGetTransformFeedbacki64_v;
        public static PFNGLCREATEBUFFERSPROC glCreateBuffers;
        public static PFNGLNAMEDBUFFERSTORAGEPROC glNamedBufferStorage;
        public static PFNGLNAMEDBUFFERDATAPROC glNamedBufferData;
        public static PFNGLNAMEDBUFFERSUBDATAPROC glNamedBufferSubData;
        public static PFNGLCOPYNAMEDBUFFERSUBDATAPROC glCopyNamedBufferSubData;
        public static PFNGLCLEARNAMEDBUFFERDATAPROC glClearNamedBufferData;
        public static PFNGLCLEARNAMEDBUFFERSUBDATAPROC glClearNamedBufferSubData;
        public static PFNGLMAPNAMEDBUFFERPROC glMapNamedBuffer;
        public static PFNGLMAPNAMEDBUFFERRANGEPROC glMapNamedBufferRange;
        public static PFNGLUNMAPNAMEDBUFFERPROC glUnmapNamedBuffer;
        public static PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC glFlushMappedNamedBufferRange;
        public static PFNGLGETNAMEDBUFFERPARAMETERIVPROC glGetNamedBufferParameteriv;
        public static PFNGLGETNAMEDBUFFERPARAMETERI64VPROC glGetNamedBufferParameteri64v;
        public static PFNGLGETNAMEDBUFFERPOINTERVPROC glGetNamedBufferPointerv;
        public static PFNGLGETNAMEDBUFFERSUBDATAPROC glGetNamedBufferSubData;
        public static PFNGLCREATEFRAMEBUFFERSPROC glCreateFramebuffers;
        public static PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC glNamedFramebufferRenderbuffer;
        public static PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC glNamedFramebufferParameteri;
        public static PFNGLNAMEDFRAMEBUFFERTEXTUREPROC glNamedFramebufferTexture;
        public static PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC glNamedFramebufferTextureLayer;
        public static PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC glNamedFramebufferDrawBuffer;
        public static PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC glNamedFramebufferDrawBuffers;
        public static PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC glNamedFramebufferReadBuffer;
        public static PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC glInvalidateNamedFramebufferData;
        public static PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC glInvalidateNamedFramebufferSubData;
        public static PFNGLCLEARNAMEDFRAMEBUFFERIVPROC glClearNamedFramebufferiv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC glClearNamedFramebufferuiv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERFVPROC glClearNamedFramebufferfv;
        public static PFNGLCLEARNAMEDFRAMEBUFFERFIPROC glClearNamedFramebufferfi;
        public static PFNGLBLITNAMEDFRAMEBUFFERPROC glBlitNamedFramebuffer;
        public static PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC glCheckNamedFramebufferStatus;
        public static PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC glGetNamedFramebufferParameteriv;
        public static PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC glGetNamedFramebufferAttachmentParameteriv;
        public static PFNGLCREATERENDERBUFFERSPROC glCreateRenderbuffers;
        public static PFNGLNAMEDRENDERBUFFERSTORAGEPROC glNamedRenderbufferStorage;
        public static PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC glNamedRenderbufferStorageMultisample;
        public static PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC glGetNamedRenderbufferParameteriv;
        public static PFNGLCREATETEXTURESPROC glCreateTextures;
        public static PFNGLTEXTUREBUFFERPROC glTextureBuffer;
        public static PFNGLTEXTUREBUFFERRANGEPROC glTextureBufferRange;
        public static PFNGLTEXTURESTORAGE1DPROC glTextureStorage1D;
        public static PFNGLTEXTURESTORAGE2DPROC glTextureStorage2D;
        public static PFNGLTEXTURESTORAGE3DPROC glTextureStorage3D;
        public static PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC glTextureStorage2DMultisample;
        public static PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC glTextureStorage3DMultisample;
        public static PFNGLTEXTURESUBIMAGE1DPROC glTextureSubImage1D;
        public static PFNGLTEXTURESUBIMAGE2DPROC glTextureSubImage2D;
        public static PFNGLTEXTURESUBIMAGE3DPROC glTextureSubImage3D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC glCompressedTextureSubImage1D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC glCompressedTextureSubImage2D;
        public static PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC glCompressedTextureSubImage3D;
        public static PFNGLCOPYTEXTURESUBIMAGE1DPROC glCopyTextureSubImage1D;
        public static PFNGLCOPYTEXTURESUBIMAGE2DPROC glCopyTextureSubImage2D;
        public static PFNGLCOPYTEXTURESUBIMAGE3DPROC glCopyTextureSubImage3D;
        public static PFNGLTEXTUREPARAMETERFPROC glTextureParameterf;
        public static PFNGLTEXTUREPARAMETERFVPROC glTextureParameterfv;
        public static PFNGLTEXTUREPARAMETERIPROC glTextureParameteri;
        public static PFNGLTEXTUREPARAMETERIIVPROC glTextureParameterIiv;
        public static PFNGLTEXTUREPARAMETERIUIVPROC glTextureParameterIuiv;
        public static PFNGLTEXTUREPARAMETERIVPROC glTextureParameteriv;
        public static PFNGLGENERATETEXTUREMIPMAPPROC glGenerateTextureMipmap;
        public static PFNGLBINDTEXTUREUNITPROC glBindTextureUnit;
        public static PFNGLGETTEXTUREIMAGEPROC glGetTextureImage;
        public static PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC glGetCompressedTextureImage;
        public static PFNGLGETTEXTURELEVELPARAMETERFVPROC glGetTextureLevelParameterfv;
        public static PFNGLGETTEXTURELEVELPARAMETERIVPROC glGetTextureLevelParameteriv;
        public static PFNGLGETTEXTUREPARAMETERFVPROC glGetTextureParameterfv;
        public static PFNGLGETTEXTUREPARAMETERIIVPROC glGetTextureParameterIiv;
        public static PFNGLGETTEXTUREPARAMETERIUIVPROC glGetTextureParameterIuiv;
        public static PFNGLGETTEXTUREPARAMETERIVPROC glGetTextureParameteriv;
        public static PFNGLCREATEVERTEXARRAYSPROC glCreateVertexArrays;
        public static PFNGLDISABLEVERTEXARRAYATTRIBPROC glDisableVertexArrayAttrib;
        public static PFNGLENABLEVERTEXARRAYATTRIBPROC glEnableVertexArrayAttrib;
        public static PFNGLVERTEXARRAYELEMENTBUFFERPROC glVertexArrayElementBuffer;
        public static PFNGLVERTEXARRAYVERTEXBUFFERPROC glVertexArrayVertexBuffer;
        public static PFNGLVERTEXARRAYVERTEXBUFFERSPROC glVertexArrayVertexBuffers;
        public static PFNGLVERTEXARRAYATTRIBBINDINGPROC glVertexArrayAttribBinding;
        public static PFNGLVERTEXARRAYATTRIBFORMATPROC glVertexArrayAttribFormat;
        public static PFNGLVERTEXARRAYATTRIBIFORMATPROC glVertexArrayAttribIFormat;
        public static PFNGLVERTEXARRAYATTRIBLFORMATPROC glVertexArrayAttribLFormat;
        public static PFNGLVERTEXARRAYBINDINGDIVISORPROC glVertexArrayBindingDivisor;
        public static PFNGLGETVERTEXARRAYIVPROC glGetVertexArrayiv;
        public static PFNGLGETVERTEXARRAYINDEXEDIVPROC glGetVertexArrayIndexediv;
        public static PFNGLGETVERTEXARRAYINDEXED64IVPROC glGetVertexArrayIndexed64iv;
        public static PFNGLCREATESAMPLERSPROC glCreateSamplers;
        public static PFNGLCREATEPROGRAMPIPELINESPROC glCreateProgramPipelines;
        public static PFNGLCREATEQUERIESPROC glCreateQueries;
        public static PFNGLGETQUERYBUFFEROBJECTI64VPROC glGetQueryBufferObjecti64v;
        public static PFNGLGETQUERYBUFFEROBJECTIVPROC glGetQueryBufferObjectiv;
        public static PFNGLGETQUERYBUFFEROBJECTUI64VPROC glGetQueryBufferObjectui64v;
        public static PFNGLGETQUERYBUFFEROBJECTUIVPROC glGetQueryBufferObjectuiv;
        public static PFNGLMEMORYBARRIERBYREGIONPROC glMemoryBarrierByRegion;
        public static PFNGLGETTEXTURESUBIMAGEPROC glGetTextureSubImage;
        public static PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC glGetCompressedTextureSubImage;
        public static PFNGLGETGRAPHICSRESETSTATUSPROC glGetGraphicsResetStatus;
        public static PFNGLGETNCOMPRESSEDTEXIMAGEPROC glGetnCompressedTexImage;
        public static PFNGLGETNTEXIMAGEPROC glGetnTexImage;
        public static PFNGLGETNUNIFORMDVPROC glGetnUniformdv;
        public static PFNGLGETNUNIFORMFVPROC glGetnUniformfv;
        public static PFNGLGETNUNIFORMIVPROC glGetnUniformiv;
        public static PFNGLGETNUNIFORMUIVPROC glGetnUniformuiv;
        public static PFNGLREADNPIXELSPROC glReadnPixels;
        public static PFNGLTEXTUREBARRIERPROC glTextureBarrier;
        public static PFNGLGETTEXTUREHANDLEARBPROC glGetTextureHandleARB;
        public static PFNGLGETTEXTURESAMPLERHANDLEARBPROC glGetTextureSamplerHandleARB;
        public static PFNGLMAKETEXTUREHANDLERESIDENTARBPROC glMakeTextureHandleResidentARB;
        public static PFNGLMAKETEXTUREHANDLENONRESIDENTARBPROC glMakeTextureHandleNonResidentARB;
        public static PFNGLGETIMAGEHANDLEARBPROC glGetImageHandleARB;
        public static PFNGLMAKEIMAGEHANDLERESIDENTARBPROC glMakeImageHandleResidentARB;
        public static PFNGLMAKEIMAGEHANDLENONRESIDENTARBPROC glMakeImageHandleNonResidentARB;
        public static PFNGLUNIFORMHANDLEUI64ARBPROC glUniformHandleui64ARB;
        public static PFNGLUNIFORMHANDLEUI64VARBPROC glUniformHandleui64vARB;
        public static PFNGLPROGRAMUNIFORMHANDLEUI64ARBPROC glProgramUniformHandleui64ARB;
        public static PFNGLPROGRAMUNIFORMHANDLEUI64VARBPROC glProgramUniformHandleui64vARB;
        public static PFNGLISTEXTUREHANDLERESIDENTARBPROC glIsTextureHandleResidentARB;
        public static PFNGLISIMAGEHANDLERESIDENTARBPROC glIsImageHandleResidentARB;
        public static PFNGLVERTEXATTRIBL1UI64ARBPROC glVertexAttribL1ui64ARB;
        public static PFNGLVERTEXATTRIBL1UI64VARBPROC glVertexAttribL1ui64vARB;
        public static PFNGLGETVERTEXATTRIBLUI64VARBPROC glGetVertexAttribLui64vARB;
        public static PFNGLCREATESYNCFROMCLEVENTARBPROC glCreateSyncFromCLeventARB;
        public static PFNGLDISPATCHCOMPUTEGROUPSIZEARBPROC glDispatchComputeGroupSizeARB;
        public static PFNGLDEBUGMESSAGECONTROLARBPROC glDebugMessageControlARB;
        public static PFNGLDEBUGMESSAGEINSERTARBPROC glDebugMessageInsertARB;
        public static PFNGLDEBUGMESSAGECALLBACKARBPROC glDebugMessageCallbackARB;
        public static PFNGLGETDEBUGMESSAGELOGARBPROC glGetDebugMessageLogARB;
        public static PFNGLBLENDEQUATIONIARBPROC glBlendEquationiARB;
        public static PFNGLBLENDEQUATIONSEPARATEIARBPROC glBlendEquationSeparateiARB;
        public static PFNGLBLENDFUNCIARBPROC glBlendFunciARB;
        public static PFNGLBLENDFUNCSEPARATEIARBPROC glBlendFuncSeparateiARB;
        public static PFNGLMULTIDRAWARRAYSINDIRECTCOUNTARBPROC glMultiDrawArraysIndirectCountARB;
        public static PFNGLMULTIDRAWELEMENTSINDIRECTCOUNTARBPROC glMultiDrawElementsIndirectCountARB;
        public static PFNGLGETGRAPHICSRESETSTATUSARBPROC glGetGraphicsResetStatusARB;
        public static PFNGLGETNTEXIMAGEARBPROC glGetnTexImageARB;
        public static PFNGLREADNPIXELSARBPROC glReadnPixelsARB;
        public static PFNGLGETNCOMPRESSEDTEXIMAGEARBPROC glGetnCompressedTexImageARB;
        public static PFNGLGETNUNIFORMFVARBPROC glGetnUniformfvARB;
        public static PFNGLGETNUNIFORMIVARBPROC glGetnUniformivARB;
        public static PFNGLGETNUNIFORMUIVARBPROC glGetnUniformuivARB;
        public static PFNGLGETNUNIFORMDVARBPROC glGetnUniformdvARB;
        public static PFNGLMINSAMPLESHADINGARBPROC glMinSampleShadingARB;
        public static PFNGLNAMEDSTRINGARBPROC glNamedStringARB;
        public static PFNGLDELETENAMEDSTRINGARBPROC glDeleteNamedStringARB;
        public static PFNGLCOMPILESHADERINCLUDEARBPROC glCompileShaderIncludeARB;
        public static PFNGLISNAMEDSTRINGARBPROC glIsNamedStringARB;
        public static PFNGLGETNAMEDSTRINGARBPROC glGetNamedStringARB;
        public static PFNGLGETNAMEDSTRINGIVARBPROC glGetNamedStringivARB;
        public static PFNGLBUFFERPAGECOMMITMENTARBPROC glBufferPageCommitmentARB;
        public static PFNGLNAMEDBUFFERPAGECOMMITMENTEXTPROC glNamedBufferPageCommitmentEXT;
        public static PFNGLNAMEDBUFFERPAGECOMMITMENTARBPROC glNamedBufferPageCommitmentARB;
        public static PFNGLTEXPAGECOMMITMENTARBPROC glTexPageCommitmentARB;
        #endregion
    }

    public delegate void PFNGLCULLFACEPROC(GLenum mode);
    public delegate void PFNGLFRONTFACEPROC(GLenum mode);
    public delegate void PFNGLHINTPROC(GLenum target, GLenum mode);
    public delegate void PFNGLLINEWIDTHPROC(GLfloat width);
    public delegate void PFNGLPOINTSIZEPROC(GLfloat size);
    public delegate void PFNGLPOLYGONMODEPROC(GLenum face, GLenum mode);
    public delegate void PFNGLSCISSORPROC(GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXPARAMETERFPROC(GLenum target, GLenum pname, GLfloat param);
    public delegate void PFNGLTEXPARAMETERFVPROC(GLenum target, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLTEXPARAMETERIPROC(GLenum target, GLenum pname, GLint param);
    public delegate void PFNGLTEXPARAMETERIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLTEXIMAGE1DPROC(GLenum target, GLint level, GLint internalformat, GLsizei width, GLint border, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLTEXIMAGE2DPROC(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLDRAWBUFFERPROC(GLenum mode);
    public delegate void PFNGLCLEARPROC(GLbitfield mask);
    public delegate void PFNGLCLEARCOLORPROC(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);
    public delegate void PFNGLCLEARSTENCILPROC(GLint s);
    public delegate void PFNGLCLEARDEPTHPROC(GLclampd depth);
    public delegate void PFNGLSTENCILMASKPROC(GLuint mask);
    public delegate void PFNGLCOLORMASKPROC(GLboolean red, GLboolean green, GLboolean blue, GLboolean alpha);
    public delegate void PFNGLDEPTHMASKPROC(GLboolean flag);
    public delegate void PFNGLDISABLEPROC(GLenum cap);
    public delegate void PFNGLENABLEPROC(GLenum cap);
    public delegate void PFNGLFINISHPROC();
    public delegate void PFNGLFLUSHPROC();
    public delegate void PFNGLBLENDFUNCPROC(GLenum sfactor, GLenum dfactor);
    public delegate void PFNGLLOGICOPPROC(GLenum opcode);
    public delegate void PFNGLSTENCILFUNCPROC(GLenum func, GLint _ref, GLuint mask);
    public delegate void PFNGLSTENCILOPPROC(GLenum fail, GLenum zfail, GLenum zpass);
    public delegate void PFNGLDEPTHFUNCPROC(GLenum func);
    public delegate void PFNGLPIXELSTOREFPROC(GLenum pname, GLfloat param);
    public delegate void PFNGLPIXELSTOREIPROC(GLenum pname, GLint param);
    public delegate void PFNGLREADBUFFERPROC(GLenum mode);
    public delegate void PFNGLREADPIXELSPROC(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLGETBOOLEANVPROC(GLenum pname, GLboolean[] _params);
    public delegate void PFNGLGETDOUBLEVPROC(GLenum pname, GLdouble[] _params);
    public delegate GLenum PFNGLGETERRORPROC();
    public delegate void PFNGLGETFLOATVPROC(GLenum pname, GLvoid _params);
    public delegate void PFNGLGETINTEGERVPROC(GLenum pname, GLvoid _params);
    public delegate void PFNGLGETTEXIMAGEPROC(GLenum target, GLint level, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLGETTEXPARAMETERFVPROC(GLenum target, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETTEXPARAMETERIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETTEXLEVELPARAMETERFVPROC(GLenum target, GLint level, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETTEXLEVELPARAMETERIVPROC(GLenum target, GLint level, GLenum pname, GLint[] _params);
    public delegate GLboolean PFNGLISENABLEDPROC(GLenum cap);
    public delegate void PFNGLDEPTHRANGEPROC(GLclampd near, GLclampd far);
    public delegate void PFNGLVIEWPORTPROC(GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLDRAWARRAYSPROC(GLenum mode, GLint first, GLsizei count);
    public delegate void PFNGLDRAWELEMENTSPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices);
    public delegate void PFNGLGETPOINTERVPROC(GLenum pname, GLvoid[] _params);
    public delegate void PFNGLPOLYGONOFFSETPROC(GLfloat factor, GLfloat units);
    public delegate void PFNGLCOPYTEXIMAGE1DPROC(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLint border);
    public delegate void PFNGLCOPYTEXIMAGE2DPROC(GLenum target, GLint level, GLenum internalformat, GLint x, GLint y, GLsizei width, GLsizei height, GLint border);
    public delegate void PFNGLCOPYTEXSUBIMAGE1DPROC(GLenum target, GLint level, GLint xoffset, GLint x, GLint y, GLsizei width);
    public delegate void PFNGLCOPYTEXSUBIMAGE2DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXSUBIMAGE1DPROC(GLenum target, GLint level, GLint xoffset, GLsizei width, GLenum format, GLenum type, IntPtr pixels);
    public delegate void PFNGLTEXSUBIMAGE2DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLBINDTEXTUREPROC(GLenum target, GLuint texture);
    public delegate void PFNGLDELETETEXTURESPROC(GLsizei n, GLuint[] textures);
    public delegate void PFNGLGENTEXTURESPROC(GLsizei n, GLuint[] textures);
    public delegate GLboolean PFNGLISTEXTUREPROC(GLuint texture);
    public delegate void PFNGLBLENDCOLORPROC(GLclampf red, GLclampf green, GLclampf blue, GLclampf alpha);
    public delegate void PFNGLBLENDEQUATIONPROC(GLenum mode);
    public delegate void PFNGLDRAWRANGEELEMENTSPROC(GLenum mode, GLuint start, GLuint end, GLsizei count, GLenum type, GLvoid indices);
    public delegate void PFNGLTEXIMAGE3DPROC(GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLTEXSUBIMAGE3DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLCOPYTEXSUBIMAGE3DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLACTIVETEXTUREPROC(GLenum texture);
    public delegate void PFNGLSAMPLECOVERAGEPROC(GLclampf value, GLboolean invert);
    public delegate void PFNGLCOMPRESSEDTEXIMAGE3DPROC(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXIMAGE2DPROC(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXIMAGE1DPROC(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLint border, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE3DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLsizei imageSize,GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE2DPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXSUBIMAGE1DPROC(GLenum target, GLint level, GLint xoffset, GLsizei width, GLenum format, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLGETCOMPRESSEDTEXIMAGEPROC(GLenum target, GLint level, GLvoid img);
    public delegate void PFNGLBLENDFUNCSEPARATEPROC(GLenum sfactorRGB, GLenum dfactorRGB, GLenum sfactorAlpha, GLenum dfactorAlpha);
    public delegate void PFNGLMULTIDRAWARRAYSPROC(GLenum mode, GLint[] first, GLsizei[] count, GLsizei primcount);
    public delegate void PFNGLMULTIDRAWELEMENTSPROC(GLenum mode, GLsizei[] count, GLenum type, GLvoid[] indices, GLsizei primcount);
    public delegate void PFNGLPOINTPARAMETERFPROC(GLenum pname, GLfloat param);
    public delegate void PFNGLPOINTPARAMETERFVPROC(GLenum pname, GLfloat[] _params);
    public delegate void PFNGLPOINTPARAMETERIPROC(GLenum pname, GLint param);
    public delegate void PFNGLPOINTPARAMETERIVPROC(GLenum pname, GLint[] _params);
    public delegate void PFNGLGENQUERIESPROC(GLsizei n, GLuint[] ids);
    public delegate void PFNGLDELETEQUERIESPROC(GLsizei n, GLuint[] ids);
    public delegate GLboolean PFNGLISQUERYPROC(GLuint id);
    public delegate void PFNGLBEGINQUERYPROC(GLenum target, GLuint id);
    public delegate void PFNGLENDQUERYPROC(GLenum target);
    public delegate void PFNGLGETQUERYIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETQUERYOBJECTIVPROC(GLuint id, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETQUERYOBJECTUIVPROC(GLuint id, GLenum pname, GLuint[] _params);
    public delegate void PFNGLBINDBUFFERPROC(GLenum target, GLuint buffer);
    public delegate void PFNGLDELETEBUFFERSPROC(GLsizei n, GLuint[] buffers);
    public delegate void PFNGLGENBUFFERSPROC(GLsizei n, GLuint[] buffers);
    public delegate GLboolean PFNGLISBUFFERPROC(GLuint buffer);
    public delegate void PFNGLBUFFERDATAPROC(GLenum target, GLsizeiptr size, GLvoid data, GLenum usage);
    public delegate void PFNGLBUFFERSUBDATAPROC(GLenum target, GLintptr offset, GLsizeiptr size, GLvoid data);
    public delegate void PFNGLGETBUFFERSUBDATAPROC(GLenum target, GLintptr offset, GLsizeiptr size, GLvoid data);
    public delegate GLboolean PFNGLUNMAPBUFFERPROC(GLenum target);
    public delegate void PFNGLGETBUFFERPARAMETERIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETBUFFERPOINTERVPROC(GLenum target, GLenum pname, GLvoid[] _params);
    public delegate void PFNGLBLENDEQUATIONSEPARATEPROC(GLenum modeRGB, GLenum modeAlpha);
    public delegate void PFNGLDRAWBUFFERSPROC(GLsizei n, GLenum[] bufs);
    public delegate void PFNGLSTENCILOPSEPARATEPROC(GLenum face, GLenum sfail, GLenum dpfail, GLenum dppass);
    public delegate void PFNGLSTENCILFUNCSEPARATEPROC(GLenum face, GLenum func, GLint _ref, GLuint mask);
    public delegate void PFNGLSTENCILMASKSEPARATEPROC(GLenum face, GLuint mask);
    public delegate void PFNGLATTACHSHADERPROC(GLuint program, GLuint shader);
    public delegate void PFNGLBINDATTRIBLOCATIONPROC(GLuint program, GLuint index, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLCOMPILESHADERPROC(GLuint shader);
    public delegate GLuint PFNGLCREATEPROGRAMPROC();
    public delegate GLuint PFNGLCREATESHADERPROC(GLenum type);
    public delegate void PFNGLDELETEPROGRAMPROC(GLuint program);
    public delegate void PFNGLDELETESHADERPROC(GLuint shader);
    public delegate void PFNGLDETACHSHADERPROC(GLuint program, GLuint shader);
    public delegate void PFNGLDISABLEVERTEXATTRIBARRAYPROC(GLuint index);
    public delegate void PFNGLENABLEVERTEXATTRIBARRAYPROC(GLuint index);
    public delegate void PFNGLGETACTIVEATTRIBPROC(GLuint program, GLuint index, GLsizei bufSize, GLsizei[] length, GLint[] size, GLenum[] type, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETACTIVEUNIFORMPROC(GLuint program, GLuint index, GLsizei bufSize, GLsizei[] length, GLint[] size, GLenum[] type, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETATTACHEDSHADERSPROC(GLuint program, GLsizei maxCount, GLsizei[] count, GLuint[] obj);
    public delegate GLint PFNGLGETATTRIBLOCATIONPROC(GLuint program, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETPROGRAMIVPROC(GLuint program, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETPROGRAMINFOLOGPROC(GLuint program, GLsizei bufSize, GLsizei[] length, GLchar[] infoLog);
    public delegate void PFNGLGETSHADERIVPROC(GLuint shader, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETSHADERINFOLOGPROC(GLuint shader, GLsizei bufSize, GLsizei[] length, GLchar[] infoLog);
    public delegate void PFNGLGETSHADERSOURCEPROC(GLuint shader, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string source);
    public delegate GLint PFNGLGETUNIFORMLOCATIONPROC(GLuint program, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETUNIFORMFVPROC(GLuint program, GLint location, GLfloat[] _params);
    public delegate void PFNGLGETUNIFORMIVPROC(GLuint program, GLint location, GLint[] _params);
    public delegate void PFNGLGETVERTEXATTRIBDVPROC(GLuint index, GLenum pname, GLdouble[] _params);
    public delegate void PFNGLGETVERTEXATTRIBFVPROC(GLuint index, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETVERTEXATTRIBIVPROC(GLuint index, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETVERTEXATTRIBPOINTERVPROC(GLuint index, GLenum pname, GLvoid[] pointer);
    public delegate GLboolean PFNGLISPROGRAMPROC(GLuint program);
    public delegate GLboolean PFNGLISSHADERPROC(GLuint shader);
    public delegate void PFNGLLINKPROGRAMPROC(GLuint program);
    public delegate void PFNGLSHADERSOURCEPROC(GLuint shader, GLsizei count, [MarshalAs(UnmanagedType.LPArray)]string[] program, GLint[] length);
    public delegate void PFNGLUSEPROGRAMPROC(GLuint program);
    public delegate void PFNGLUNIFORM1FPROC(GLint location, GLfloat v0);
    public delegate void PFNGLUNIFORM2FPROC(GLint location, GLfloat v0, GLfloat v1);
    public delegate void PFNGLUNIFORM3FPROC(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
    public delegate void PFNGLUNIFORM4FPROC(GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
    public delegate void PFNGLUNIFORM1IPROC(GLint location, GLint v0);
    public delegate void PFNGLUNIFORM2IPROC(GLint location, GLint v0, GLint v1);
    public delegate void PFNGLUNIFORM3IPROC(GLint location, GLint v0, GLint v1, GLint v2);
    public delegate void PFNGLUNIFORM4IPROC(GLint location, GLint v0, GLint v1, GLint v2, GLint v3);
    public delegate void PFNGLUNIFORM1FVPROC(GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLUNIFORM2FVPROC(GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLUNIFORM3FVPROC(GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLUNIFORM4FVPROC(GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLUNIFORM1IVPROC(GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLUNIFORM2IVPROC(GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLUNIFORM3IVPROC(GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLUNIFORM4IVPROC(GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLUNIFORMMATRIX2FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX3FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX4FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLVALIDATEPROGRAMPROC(GLuint program);
    public delegate void PFNGLVERTEXATTRIB1DPROC(GLuint index, GLdouble x);
    public delegate void PFNGLVERTEXATTRIB1DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIB1FPROC(GLuint index, GLfloat x);
    public delegate void PFNGLVERTEXATTRIB1FVPROC(GLuint index, GLfloat[] v);
    public delegate void PFNGLVERTEXATTRIB1SPROC(GLuint index, GLshort x);
    public delegate void PFNGLVERTEXATTRIB1SVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIB2DPROC(GLuint index, GLdouble x, GLdouble y);
    public delegate void PFNGLVERTEXATTRIB2DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIB2FPROC(GLuint index, GLfloat x, GLfloat y);
    public delegate void PFNGLVERTEXATTRIB2FVPROC(GLuint index, GLfloat[] v);
    public delegate void PFNGLVERTEXATTRIB2SPROC(GLuint index, GLshort x, GLshort y);
    public delegate void PFNGLVERTEXATTRIB2SVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIB3DPROC(GLuint index, GLdouble x, GLdouble y, GLdouble z);
    public delegate void PFNGLVERTEXATTRIB3DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIB3FPROC(GLuint index, GLfloat x, GLfloat y, GLfloat z);
    public delegate void PFNGLVERTEXATTRIB3FVPROC(GLuint index, GLfloat[] v);
    public delegate void PFNGLVERTEXATTRIB3SPROC(GLuint index, GLshort x, GLshort y, GLshort z);
    public delegate void PFNGLVERTEXATTRIB3SVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIB4NBVPROC(GLuint index, GLbyte[] v);
    public delegate void PFNGLVERTEXATTRIB4NIVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIB4NSVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIB4NUBPROC(GLuint index, GLubyte x, GLubyte y, GLubyte z, GLubyte w);
    public delegate void PFNGLVERTEXATTRIB4NUBVPROC(GLuint index, GLubyte[] v);
    public delegate void PFNGLVERTEXATTRIB4NUIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIB4NUSVPROC(GLuint index, GLushort[] v);
    public delegate void PFNGLVERTEXATTRIB4BVPROC(GLuint index, GLbyte[] v);
    public delegate void PFNGLVERTEXATTRIB4DPROC(GLuint index, GLdouble x, GLdouble y, GLdouble z, GLdouble w);
    public delegate void PFNGLVERTEXATTRIB4DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIB4FPROC(GLuint index, GLfloat x, GLfloat y, GLfloat z, GLfloat w);
    public delegate void PFNGLVERTEXATTRIB4FVPROC(GLuint index, GLfloat[] v);
    public delegate void PFNGLVERTEXATTRIB4IVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIB4SPROC(GLuint index, GLshort x, GLshort y, GLshort z, GLshort w);
    public delegate void PFNGLVERTEXATTRIB4SVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIB4UBVPROC(GLuint index, GLubyte[] v);
    public delegate void PFNGLVERTEXATTRIB4UIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIB4USVPROC(GLuint index, GLushort[] v);
    public delegate void PFNGLVERTEXATTRIBPOINTERPROC(GLuint index, GLint size, GLenum type, GLboolean normalized, GLsizei stride, GLvoid pointer);
    public delegate void PFNGLUNIFORMMATRIX2X3FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX3X2FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX2X4FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX4X2FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX3X4FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLUNIFORMMATRIX4X3FVPROC(GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLCOLORMASKIPROC(GLuint index, GLboolean r, GLboolean g, GLboolean b, GLboolean a);
    public delegate void PFNGLGETBOOLEANI_VPROC(GLenum target, GLuint index, GLboolean[] data);
    public delegate void PFNGLGETINTEGERI_VPROC(GLenum target, GLuint index, GLint[] data);
    public delegate void PFNGLENABLEIPROC(GLenum target, GLuint index);
    public delegate void PFNGLDISABLEIPROC(GLenum target, GLuint index);
    public delegate GLboolean PFNGLISENABLEDIPROC(GLenum target, GLuint index);
    public delegate void PFNGLBEGINTRANSFORMFEEDBACKPROC(GLenum primitiveMode);
    public delegate void PFNGLENDTRANSFORMFEEDBACKPROC();
    public delegate void PFNGLBINDBUFFERRANGEPROC(GLenum target, GLuint index, GLuint buffer, GLintptr offset, GLsizeiptr size);
    public delegate void PFNGLBINDBUFFERBASEPROC(GLenum target, GLuint index, GLuint buffer);
    public delegate void PFNGLTRANSFORMFEEDBACKVARYINGSPROC(GLuint program, GLsizei count, [MarshalAs(UnmanagedType.LPArray)]string[] varyings, GLenum bufferMode);
    public delegate void PFNGLGETTRANSFORMFEEDBACKVARYINGPROC(GLuint program, GLuint index, GLsizei bufSize, GLsizei[] length, GLsizei[] size, GLenum[] type, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLCLAMPCOLORPROC(GLenum target, GLenum clamp);
    public delegate void PFNGLBEGINCONDITIONALRENDERPROC(GLuint id, GLenum mode);
    public delegate void PFNGLENDCONDITIONALRENDERPROC();
    public delegate void PFNGLVERTEXATTRIBIPOINTERPROC(GLuint index, GLint size, GLenum type, GLsizei stride, GLvoid pointer);
    public delegate void PFNGLGETVERTEXATTRIBIIVPROC(GLuint index, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETVERTEXATTRIBIUIVPROC(GLuint index, GLenum pname, GLuint[] _params);
    public delegate void PFNGLVERTEXATTRIBI1IPROC(GLuint index, GLint x);
    public delegate void PFNGLVERTEXATTRIBI2IPROC(GLuint index, GLint x, GLint y);
    public delegate void PFNGLVERTEXATTRIBI3IPROC(GLuint index, GLint x, GLint y, GLint z);
    public delegate void PFNGLVERTEXATTRIBI4IPROC(GLuint index, GLint x, GLint y, GLint z, GLint w);
    public delegate void PFNGLVERTEXATTRIBI1UIPROC(GLuint index, GLuint x);
    public delegate void PFNGLVERTEXATTRIBI2UIPROC(GLuint index, GLuint x, GLuint y);
    public delegate void PFNGLVERTEXATTRIBI3UIPROC(GLuint index, GLuint x, GLuint y, GLuint z);
    public delegate void PFNGLVERTEXATTRIBI4UIPROC(GLuint index, GLuint x, GLuint y, GLuint z, GLuint w);
    public delegate void PFNGLVERTEXATTRIBI1IVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIBI2IVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIBI3IVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIBI4IVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLVERTEXATTRIBI1UIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIBI2UIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIBI3UIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIBI4UIVPROC(GLuint index, GLuint[] v);
    public delegate void PFNGLVERTEXATTRIBI4BVPROC(GLuint index, GLbyte[] v);
    public delegate void PFNGLVERTEXATTRIBI4SVPROC(GLuint index, GLshort[] v);
    public delegate void PFNGLVERTEXATTRIBI4UBVPROC(GLuint index, GLubyte[] v);
    public delegate void PFNGLVERTEXATTRIBI4USVPROC(GLuint index, GLushort[] v);
    public delegate void PFNGLGETUNIFORMUIVPROC(GLuint program, GLint location, GLuint[] _params);
    public delegate void PFNGLBINDFRAGDATALOCATIONPROC(GLuint program, GLuint color, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate GLint PFNGLGETFRAGDATALOCATIONPROC(GLuint program, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLUNIFORM1UIPROC(GLint location, GLuint v0);
    public delegate void PFNGLUNIFORM2UIPROC(GLint location, GLuint v0, GLuint v1);
    public delegate void PFNGLUNIFORM3UIPROC(GLint location, GLuint v0, GLuint v1, GLuint v2);
    public delegate void PFNGLUNIFORM4UIPROC(GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);
    public delegate void PFNGLUNIFORM1UIVPROC(GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLUNIFORM2UIVPROC(GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLUNIFORM3UIVPROC(GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLUNIFORM4UIVPROC(GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLTEXPARAMETERIIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLTEXPARAMETERIUIVPROC(GLenum target, GLenum pname, GLuint[] _params);
    public delegate void PFNGLGETTEXPARAMETERIIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETTEXPARAMETERIUIVPROC(GLenum target, GLenum pname, GLuint[] _params);
    public delegate void PFNGLCLEARBUFFERIVPROC(GLenum buffer, GLint drawbuffer, GLint[] value);
    public delegate void PFNGLCLEARBUFFERUIVPROC(GLenum buffer, GLint drawbuffer, GLuint[] value);
    public delegate void PFNGLCLEARBUFFERFVPROC(GLenum buffer, GLint drawbuffer, GLfloat[] value);
    public delegate void PFNGLCLEARBUFFERFIPROC(GLenum buffer, GLint drawbuffer, GLfloat depth, GLint stencil);
    public delegate void PFNGLDRAWARRAYSINSTANCEDPROC(GLenum mode, GLint first, GLsizei count, GLsizei primcount);
    public delegate void PFNGLDRAWELEMENTSINSTANCEDPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices, GLsizei primcount);
    public delegate void PFNGLTEXBUFFERPROC(GLenum target, GLenum internalformat, GLuint buffer);
    public delegate void PFNGLPRIMITIVERESTARTINDEXPROC(GLuint index);
    public delegate void PFNGLGETINTEGER64I_VPROC(GLenum target, GLuint index, GLint64[] data);
    public delegate void PFNGLGETBUFFERPARAMETERI64VPROC(GLenum target, GLenum pname, GLint64[] _params);
    public delegate void PFNGLFRAMEBUFFERTEXTUREPROC(GLenum target, GLenum attachment, GLuint texture, GLint level);
    public delegate void PFNGLVERTEXATTRIBDIVISORPROC(GLuint index, GLuint divisor);
    public delegate void PFNGLMINSAMPLESHADINGPROC(GLclampf value);
    public delegate void PFNGLBLENDEQUATIONIPROC(GLuint buf, GLenum mode);
    public delegate void PFNGLBLENDEQUATIONSEPARATEIPROC(GLuint buf, GLenum modeRGB, GLenum modeAlpha);
    public delegate void PFNGLBLENDFUNCIPROC(GLuint buf, GLenum src, GLenum dst);
    public delegate void PFNGLBLENDFUNCSEPARATEIPROC(GLuint buf, GLenum srcRGB, GLenum dstRGB, GLenum srcAlpha, GLenum dstAlpha);
    public delegate GLboolean PFNGLISRENDERBUFFERPROC(GLuint renderbuffer);
    public delegate void PFNGLBINDRENDERBUFFERPROC(GLenum target, GLuint renderbuffer);
    public delegate void PFNGLDELETERENDERBUFFERSPROC(GLsizei n, GLuint[] renderbuffers);
    public delegate void PFNGLGENRENDERBUFFERSPROC(GLsizei n, GLuint[] renderbuffers);
    public delegate void PFNGLRENDERBUFFERSTORAGEPROC(GLenum target, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLGETRENDERBUFFERPARAMETERIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate GLboolean PFNGLISFRAMEBUFFERPROC(GLuint framebuffer);
    public delegate void PFNGLBINDFRAMEBUFFERPROC(GLenum target, GLuint framebuffer);
    public delegate void PFNGLDELETEFRAMEBUFFERSPROC(GLsizei n, GLuint[] framebuffers);
    public delegate void PFNGLGENFRAMEBUFFERSPROC(GLsizei n, GLuint[] framebuffers);
    public delegate GLenum PFNGLCHECKFRAMEBUFFERSTATUSPROC(GLenum target);
    public delegate void PFNGLFRAMEBUFFERTEXTURE1DPROC(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level);
    public delegate void PFNGLFRAMEBUFFERTEXTURE2DPROC(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level);
    public delegate void PFNGLFRAMEBUFFERTEXTURE3DPROC(GLenum target, GLenum attachment, GLenum textarget, GLuint texture, GLint level, GLint zoffset);
    public delegate void PFNGLFRAMEBUFFERRENDERBUFFERPROC(GLenum target, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer);
    public delegate void PFNGLGETFRAMEBUFFERATTACHMENTPARAMETERIVPROC(GLenum target, GLenum attachment, GLenum pname, GLint[] _params);
    public delegate void PFNGLGENERATEMIPMAPPROC(GLenum target);
    public delegate void PFNGLBLITFRAMEBUFFERPROC(GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, GLbitfield mask, GLenum filter);
    public delegate void PFNGLRENDERBUFFERSTORAGEMULTISAMPLEPROC(GLenum target, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLFRAMEBUFFERTEXTURELAYERPROC(GLenum target, GLenum attachment, GLuint texture, GLint level, GLint layer);
    public delegate void PFNGLFLUSHMAPPEDBUFFERRANGEPROC(GLenum target, GLintptr offset, GLsizeiptr length);
    public delegate void PFNGLBINDVERTEXARRAYPROC(GLuint array);
    public delegate void PFNGLDELETEVERTEXARRAYSPROC(GLsizei n, GLuint[] arrays);
    public delegate void PFNGLGENVERTEXARRAYSPROC(GLsizei n, GLuint[] arrays);
    public delegate GLboolean PFNGLISVERTEXARRAYPROC(GLuint array);
    public delegate void PFNGLGETUNIFORMINDICESPROC(GLuint program, GLsizei uniformCount, [MarshalAs(UnmanagedType.LPArray)]string[] uniformNames, GLuint[] uniformIndices);
    public delegate void PFNGLGETACTIVEUNIFORMSIVPROC(GLuint program, GLsizei uniformCount, GLuint[] uniformIndices, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETACTIVEUNIFORMNAMEPROC(GLuint program, GLuint uniformIndex, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string uniformName);
    public delegate GLuint PFNGLGETUNIFORMBLOCKINDEXPROC(GLuint program, [MarshalAs(UnmanagedType.LPStr)]string uniformBlockName);
    public delegate void PFNGLGETACTIVEUNIFORMBLOCKIVPROC(GLuint program, GLuint uniformBlockIndex, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETACTIVEUNIFORMBLOCKNAMEPROC(GLuint program, GLuint uniformBlockIndex, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string uniformBlockName);
    public delegate void PFNGLUNIFORMBLOCKBINDINGPROC(GLuint program, GLuint uniformBlockIndex, GLuint uniformBlockBinding);
    public delegate void PFNGLCOPYBUFFERSUBDATAPROC(GLenum readTarget, GLenum writeTarget, GLintptr readOffset, GLintptr writeOffset, GLsizeiptr size);
    public delegate void PFNGLDRAWELEMENTSBASEVERTEXPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices, GLint basevertex);
    public delegate void PFNGLDRAWRANGEELEMENTSBASEVERTEXPROC(GLenum mode, GLuint start, GLuint end, GLsizei count, GLenum type, GLvoid indices, GLint basevertex);
    public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices, GLsizei primcount, GLint basevertex);
    public delegate void PFNGLMULTIDRAWELEMENTSBASEVERTEXPROC(GLenum mode, GLsizei[] count, GLenum type, GLvoid[] indices, GLsizei primcount, GLint[] basevertex);
    public delegate void PFNGLPROVOKINGVERTEXPROC(GLenum mode);
    public delegate GLsync PFNGLFENCESYNCPROC(GLenum condition, GLbitfield flags);
    public delegate GLboolean PFNGLISSYNCPROC(GLsync sync);
    public delegate void PFNGLDELETESYNCPROC(GLsync sync);
    public delegate GLenum PFNGLCLIENTWAITSYNCPROC(GLsync sync, GLbitfield flags, GLuint64 timeout);
    public delegate void PFNGLWAITSYNCPROC(GLsync sync, GLbitfield flags, GLuint64 timeout);
    public delegate void PFNGLGETINTEGER64VPROC(GLenum pname, GLint64[] _params);
    public delegate void PFNGLGETSYNCIVPROC(GLsync sync, GLenum pname, GLsizei bufSize, GLsizei[] length, GLint[] values);
    public delegate void PFNGLTEXIMAGE2DMULTISAMPLEPROC(GLenum target, GLsizei samples, GLint internalformat, GLsizei width, GLsizei height, GLboolean fixedsamplelocations);
    public delegate void PFNGLTEXIMAGE3DMULTISAMPLEPROC(GLenum target, GLsizei samples, GLint internalformat, GLsizei width, GLsizei height, GLsizei depth, GLboolean fixedsamplelocations);
    public delegate void PFNGLGETMULTISAMPLEFVPROC(GLenum pname, GLuint index, GLfloat[] val);
    public delegate void PFNGLSAMPLEMASKIPROC(GLuint index, GLbitfield mask);
    public delegate void PFNGLBLENDEQUATIONIARBPROC(GLuint buf, GLenum mode);
    public delegate void PFNGLBLENDEQUATIONSEPARATEIARBPROC(GLuint buf, GLenum modeRGB, GLenum modeAlpha);
    public delegate void PFNGLBLENDFUNCIARBPROC(GLuint buf, GLenum src, GLenum dst);
    public delegate void PFNGLBLENDFUNCSEPARATEIARBPROC(GLuint buf, GLenum srcRGB, GLenum dstRGB, GLenum srcAlpha, GLenum dstAlpha);
    public delegate void PFNGLMINSAMPLESHADINGARBPROC(GLclampf value);
    public delegate void PFNGLNAMEDSTRINGARBPROC(GLenum type, GLint namelen, [MarshalAs(UnmanagedType.LPStr)]string name, GLint stringlen, [MarshalAs(UnmanagedType.LPStr)]string _string);
    public delegate void PFNGLDELETENAMEDSTRINGARBPROC(GLint namelen, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLCOMPILESHADERINCLUDEARBPROC(GLuint shader, GLsizei count, [MarshalAs(UnmanagedType.LPStr)]ref string path, GLint[] length);
    public delegate GLboolean PFNGLISNAMEDSTRINGARBPROC(GLint namelen, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETNAMEDSTRINGARBPROC(GLint namelen, [MarshalAs(UnmanagedType.LPStr)]string name, GLsizei bufSize, GLint[] stringlen, [MarshalAs(UnmanagedType.LPStr)]string _string);
    public delegate void PFNGLGETNAMEDSTRINGIVARBPROC(GLint namelen, [MarshalAs(UnmanagedType.LPStr)]string name, GLenum pname, GLint[] _params);
    public delegate void PFNGLBINDFRAGDATALOCATIONINDEXEDPROC(GLuint program, GLuint colorNumber, GLuint index, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate GLint PFNGLGETFRAGDATAINDEXPROC(GLuint program, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGENSAMPLERSPROC(GLsizei count, GLuint[] samplers);
    public delegate void PFNGLDELETESAMPLERSPROC(GLsizei count, GLuint[] samplers);
    public delegate GLboolean PFNGLISSAMPLERPROC(GLuint sampler);
    public delegate void PFNGLBINDSAMPLERPROC(GLuint unit, GLuint sampler);
    public delegate void PFNGLSAMPLERPARAMETERIPROC(GLuint sampler, GLenum pname, GLint param);
    public delegate void PFNGLSAMPLERPARAMETERIVPROC(GLuint sampler, GLenum pname, GLint[] param);
    public delegate void PFNGLSAMPLERPARAMETERFPROC(GLuint sampler, GLenum pname, GLfloat param);
    public delegate void PFNGLSAMPLERPARAMETERFVPROC(GLuint sampler, GLenum pname, GLfloat[] param);
    public delegate void PFNGLSAMPLERPARAMETERIIVPROC(GLuint sampler, GLenum pname, GLint[] param);
    public delegate void PFNGLSAMPLERPARAMETERIUIVPROC(GLuint sampler, GLenum pname, GLuint[] param);
    public delegate void PFNGLGETSAMPLERPARAMETERIVPROC(GLuint sampler, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETSAMPLERPARAMETERIIVPROC(GLuint sampler, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETSAMPLERPARAMETERFVPROC(GLuint sampler, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETSAMPLERPARAMETERIUIVPROC(GLuint sampler, GLenum pname, GLuint[] _params);
    public delegate void PFNGLQUERYCOUNTERPROC(GLuint id, GLenum target);
    public delegate void PFNGLGETQUERYOBJECTI64VPROC(GLuint id, GLenum pname, GLint64[] _params);
    public delegate void PFNGLGETQUERYOBJECTUI64VPROC(GLuint id, GLenum pname, GLuint64[] _params);
    public delegate void PFNGLVERTEXP2UIPROC(GLenum type, GLuint value);
    public delegate void PFNGLVERTEXP2UIVPROC(GLenum type, GLuint[] value);
    public delegate void PFNGLVERTEXP3UIPROC(GLenum type, GLuint value);
    public delegate void PFNGLVERTEXP3UIVPROC(GLenum type, GLuint[] value);
    public delegate void PFNGLVERTEXP4UIPROC(GLenum type, GLuint value);
    public delegate void PFNGLVERTEXP4UIVPROC(GLenum type, GLuint[] value);
    public delegate void PFNGLTEXCOORDP1UIPROC(GLenum type, GLuint coords);
    public delegate void PFNGLTEXCOORDP1UIVPROC(GLenum type, GLuint[] coords);
    public delegate void PFNGLTEXCOORDP2UIPROC(GLenum type, GLuint coords);
    public delegate void PFNGLTEXCOORDP2UIVPROC(GLenum type, GLuint[] coords);
    public delegate void PFNGLTEXCOORDP3UIPROC(GLenum type, GLuint coords);
    public delegate void PFNGLTEXCOORDP3UIVPROC(GLenum type, GLuint[] coords);
    public delegate void PFNGLTEXCOORDP4UIPROC(GLenum type, GLuint coords);
    public delegate void PFNGLTEXCOORDP4UIVPROC(GLenum type, GLuint[] coords);
    public delegate void PFNGLMULTITEXCOORDP1UIPROC(GLenum texture, GLenum type, GLuint coords);
    public delegate void PFNGLMULTITEXCOORDP1UIVPROC(GLenum texture, GLenum type, GLuint[] coords);
    public delegate void PFNGLMULTITEXCOORDP2UIPROC(GLenum texture, GLenum type, GLuint coords);
    public delegate void PFNGLMULTITEXCOORDP2UIVPROC(GLenum texture, GLenum type, GLuint[] coords);
    public delegate void PFNGLMULTITEXCOORDP3UIPROC(GLenum texture, GLenum type, GLuint coords);
    public delegate void PFNGLMULTITEXCOORDP3UIVPROC(GLenum texture, GLenum type, GLuint[] coords);
    public delegate void PFNGLMULTITEXCOORDP4UIPROC(GLenum texture, GLenum type, GLuint coords);
    public delegate void PFNGLMULTITEXCOORDP4UIVPROC(GLenum texture, GLenum type, GLuint[] coords);
    public delegate void PFNGLNORMALP3UIPROC(GLenum type, GLuint coords);
    public delegate void PFNGLNORMALP3UIVPROC(GLenum type, GLuint[] coords);
    public delegate void PFNGLCOLORP3UIPROC(GLenum type, GLuint color);
    public delegate void PFNGLCOLORP3UIVPROC(GLenum type, GLuint[] color);
    public delegate void PFNGLCOLORP4UIPROC(GLenum type, GLuint color);
    public delegate void PFNGLCOLORP4UIVPROC(GLenum type, GLuint[] color);
    public delegate void PFNGLSECONDARYCOLORP3UIPROC(GLenum type, GLuint color);
    public delegate void PFNGLSECONDARYCOLORP3UIVPROC(GLenum type, GLuint[] color);
    public delegate void PFNGLVERTEXATTRIBP1UIPROC(GLuint index, GLenum type, GLboolean normalized, GLuint value);
    public delegate void PFNGLVERTEXATTRIBP1UIVPROC(GLuint index, GLenum type, GLboolean normalized, GLuint[] value);
    public delegate void PFNGLVERTEXATTRIBP2UIPROC(GLuint index, GLenum type, GLboolean normalized, GLuint value);
    public delegate void PFNGLVERTEXATTRIBP2UIVPROC(GLuint index, GLenum type, GLboolean normalized, GLuint[] value);
    public delegate void PFNGLVERTEXATTRIBP3UIPROC(GLuint index, GLenum type, GLboolean normalized, GLuint value);
    public delegate void PFNGLVERTEXATTRIBP3UIVPROC(GLuint index, GLenum type, GLboolean normalized, GLuint[] value);
    public delegate void PFNGLVERTEXATTRIBP4UIPROC(GLuint index, GLenum type, GLboolean normalized, GLuint value);
    public delegate void PFNGLVERTEXATTRIBP4UIVPROC(GLuint index, GLenum type, GLboolean normalized, GLuint[] value);
    public delegate void PFNGLDRAWARRAYSINDIRECTPROC(GLenum mode, GLvoid indirect);
    public delegate void PFNGLDRAWELEMENTSINDIRECTPROC(GLenum mode, GLenum type, GLvoid indirect);
    public delegate void PFNGLUNIFORM1DPROC(GLint location, GLdouble x);
    public delegate void PFNGLUNIFORM2DPROC(GLint location, GLdouble x, GLdouble y);
    public delegate void PFNGLUNIFORM3DPROC(GLint location, GLdouble x, GLdouble y, GLdouble z);
    public delegate void PFNGLUNIFORM4DPROC(GLint location, GLdouble x, GLdouble y, GLdouble z, GLdouble w);
    public delegate void PFNGLUNIFORM1DVPROC(GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLUNIFORM2DVPROC(GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLUNIFORM3DVPROC(GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLUNIFORM4DVPROC(GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX2DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX3DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX4DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX2X3DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX2X4DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX3X2DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX3X4DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX4X2DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLUNIFORMMATRIX4X3DVPROC(GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLGETUNIFORMDVPROC(GLuint program, GLint location, GLdouble[] _params);
    public delegate GLint PFNGLGETSUBROUTINEUNIFORMLOCATIONPROC(GLuint program, GLenum shadertype, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate GLuint PFNGLGETSUBROUTINEINDEXPROC(GLuint program, GLenum shadertype, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETACTIVESUBROUTINEUNIFORMIVPROC(GLuint program, GLenum shadertype, GLuint index, GLenum pname, GLint[] values);
    public delegate void PFNGLGETACTIVESUBROUTINEUNIFORMNAMEPROC(GLuint program, GLenum shadertype, GLuint index, GLsizei bufsize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETACTIVESUBROUTINENAMEPROC(GLuint program, GLenum shadertype, GLuint index, GLsizei bufsize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLUNIFORMSUBROUTINESUIVPROC(GLenum shadertype, GLsizei count, GLuint[] indices);
    public delegate void PFNGLGETUNIFORMSUBROUTINEUIVPROC(GLenum shadertype, GLint location, GLuint[] _params);
    public delegate void PFNGLGETPROGRAMSTAGEIVPROC(GLuint program, GLenum shadertype, GLenum pname, GLint[] values);
    public delegate void PFNGLPATCHPARAMETERIPROC(GLenum pname, GLint value);
    public delegate void PFNGLPATCHPARAMETERFVPROC(GLenum pname, GLfloat[] values);
    public delegate void PFNGLBINDTRANSFORMFEEDBACKPROC(GLenum target, GLuint id);
    public delegate void PFNGLDELETETRANSFORMFEEDBACKSPROC(GLsizei n, GLuint[] ids);
    public delegate void PFNGLGENTRANSFORMFEEDBACKSPROC(GLsizei n, GLuint[] ids);
    public delegate GLboolean PFNGLISTRANSFORMFEEDBACKPROC(GLuint id);
    public delegate void PFNGLPAUSETRANSFORMFEEDBACKPROC();
    public delegate void PFNGLRESUMETRANSFORMFEEDBACKPROC();
    public delegate void PFNGLDRAWTRANSFORMFEEDBACKPROC(GLenum mode, GLuint id);
    public delegate void PFNGLDRAWTRANSFORMFEEDBACKSTREAMPROC(GLenum mode, GLuint id, GLuint stream);
    public delegate void PFNGLBEGINQUERYINDEXEDPROC(GLenum target, GLuint index, GLuint id);
    public delegate void PFNGLENDQUERYINDEXEDPROC(GLenum target, GLuint index);
    public delegate void PFNGLGETQUERYINDEXEDIVPROC(GLenum target, GLuint index, GLenum pname, GLint[] _params);
    public delegate void PFNGLRELEASESHADERCOMPILERPROC();
    public delegate void PFNGLSHADERBINARYPROC(GLsizei count, GLuint[] shaders, GLenum binaryformat, GLvoid binary, GLsizei length);
    public delegate void PFNGLGETSHADERPRECISIONFORMATPROC(GLenum shadertype, GLenum precisiontype, GLint[] range, GLint[] precision);
    public delegate void PFNGLDEPTHRANGEFPROC(GLclampf n, GLclampf f);
    public delegate void PFNGLCLEARDEPTHFPROC(GLclampf d);
    public delegate void PFNGLGETPROGRAMBINARYPROC(GLuint program, GLsizei bufSize, GLsizei[] length, GLenum[] binaryFormat, GLvoid binary);
    public delegate void PFNGLPROGRAMBINARYPROC(GLuint program, GLenum binaryFormat, GLvoid binary, GLsizei length);
    public delegate void PFNGLPROGRAMPARAMETERIPROC(GLuint program, GLenum pname, GLint value);
    public delegate void PFNGLUSEPROGRAMSTAGESPROC(GLuint pipeline, GLbitfield stages, GLuint program);
    public delegate void PFNGLACTIVESHADERPROGRAMPROC(GLuint pipeline, GLuint program);
    public delegate GLuint PFNGLCREATESHADERPROGRAMVPROC(GLenum type, GLsizei count, [MarshalAs(UnmanagedType.LPArray)]string[] strings);
    public delegate void PFNGLBINDPROGRAMPIPELINEPROC(GLuint pipeline);
    public delegate void PFNGLDELETEPROGRAMPIPELINESPROC(GLsizei n, GLuint[] pipelines);
    public delegate void PFNGLGENPROGRAMPIPELINESPROC(GLsizei n, GLuint[] pipelines);
    public delegate GLboolean PFNGLISPROGRAMPIPELINEPROC(GLuint pipeline);
    public delegate void PFNGLGETPROGRAMPIPELINEIVPROC(GLuint pipeline, GLenum pname, GLint[] _params);
    public delegate void PFNGLPROGRAMUNIFORM1IPROC(GLuint program, GLint location, GLint v0);
    public delegate void PFNGLPROGRAMUNIFORM1IVPROC(GLuint program, GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLPROGRAMUNIFORM1FPROC(GLuint program, GLint location, GLfloat v0);
    public delegate void PFNGLPROGRAMUNIFORM1FVPROC(GLuint program, GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORM1DPROC(GLuint program, GLint location, GLdouble v0);
    public delegate void PFNGLPROGRAMUNIFORM1DVPROC(GLuint program, GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORM1UIPROC(GLuint program, GLint location, GLuint v0);
    public delegate void PFNGLPROGRAMUNIFORM1UIVPROC(GLuint program, GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLPROGRAMUNIFORM2IPROC(GLuint program, GLint location, GLint v0, GLint v1);
    public delegate void PFNGLPROGRAMUNIFORM2IVPROC(GLuint program, GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLPROGRAMUNIFORM2FPROC(GLuint program, GLint location, GLfloat v0, GLfloat v1);
    public delegate void PFNGLPROGRAMUNIFORM2FVPROC(GLuint program, GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORM2DPROC(GLuint program, GLint location, GLdouble v0, GLdouble v1);
    public delegate void PFNGLPROGRAMUNIFORM2DVPROC(GLuint program, GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORM2UIPROC(GLuint program, GLint location, GLuint v0, GLuint v1);
    public delegate void PFNGLPROGRAMUNIFORM2UIVPROC(GLuint program, GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLPROGRAMUNIFORM3IPROC(GLuint program, GLint location, GLint v0, GLint v1, GLint v2);
    public delegate void PFNGLPROGRAMUNIFORM3IVPROC(GLuint program, GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLPROGRAMUNIFORM3FPROC(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
    public delegate void PFNGLPROGRAMUNIFORM3FVPROC(GLuint program, GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORM3DPROC(GLuint program, GLint location, GLdouble v0, GLdouble v1, GLdouble v2);
    public delegate void PFNGLPROGRAMUNIFORM3DVPROC(GLuint program, GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORM3UIPROC(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2);
    public delegate void PFNGLPROGRAMUNIFORM3UIVPROC(GLuint program, GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLPROGRAMUNIFORM4IPROC(GLuint program, GLint location, GLint v0, GLint v1, GLint v2, GLint v3);
    public delegate void PFNGLPROGRAMUNIFORM4IVPROC(GLuint program, GLint location, GLsizei count, GLint[] value);
    public delegate void PFNGLPROGRAMUNIFORM4FPROC(GLuint program, GLint location, GLfloat v0, GLfloat v1, GLfloat v2, GLfloat v3);
    public delegate void PFNGLPROGRAMUNIFORM4FVPROC(GLuint program, GLint location, GLsizei count, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORM4DPROC(GLuint program, GLint location, GLdouble v0, GLdouble v1, GLdouble v2, GLdouble v3);
    public delegate void PFNGLPROGRAMUNIFORM4DVPROC(GLuint program, GLint location, GLsizei count, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORM4UIPROC(GLuint program, GLint location, GLuint v0, GLuint v1, GLuint v2, GLuint v3);
    public delegate void PFNGLPROGRAMUNIFORM4UIVPROC(GLuint program, GLint location, GLsizei count, GLuint[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2X3FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3X2FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2X4FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4X2FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3X4FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4X3FVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLfloat[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2X3DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3X2DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX2X4DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4X2DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX3X4DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLPROGRAMUNIFORMMATRIX4X3DVPROC(GLuint program, GLint location, GLsizei count, GLboolean transpose, GLdouble[] value);
    public delegate void PFNGLVALIDATEPROGRAMPIPELINEPROC(GLuint pipeline);
    public delegate void PFNGLGETPROGRAMPIPELINEINFOLOGPROC(GLuint pipeline, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string infoLog);
    public delegate void PFNGLVERTEXATTRIBL1DPROC(GLuint index, GLdouble x);
    public delegate void PFNGLVERTEXATTRIBL2DPROC(GLuint index, GLdouble x, GLdouble y);
    public delegate void PFNGLVERTEXATTRIBL3DPROC(GLuint index, GLdouble x, GLdouble y, GLdouble z);
    public delegate void PFNGLVERTEXATTRIBL4DPROC(GLuint index, GLdouble x, GLdouble y, GLdouble z, GLdouble w);
    public delegate void PFNGLVERTEXATTRIBL1DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIBL2DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIBL3DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIBL4DVPROC(GLuint index, GLdouble[] v);
    public delegate void PFNGLVERTEXATTRIBLPOINTERPROC(GLuint index, GLint size, GLenum type, GLsizei stride, GLvoid pointer);
    public delegate void PFNGLGETVERTEXATTRIBLDVPROC(GLuint index, GLenum pname, GLdouble[] _params);
    public delegate void PFNGLVIEWPORTARRAYVPROC(GLuint first, GLsizei count, GLfloat[] v);
    public delegate void PFNGLVIEWPORTINDEXEDFPROC(GLuint index, GLfloat x, GLfloat y, GLfloat w, GLfloat h);
    public delegate void PFNGLVIEWPORTINDEXEDFVPROC(GLuint index, GLfloat[] v);
    public delegate void PFNGLSCISSORARRAYVPROC(GLuint first, GLsizei count, GLint[] v);
    public delegate void PFNGLSCISSORINDEXEDPROC(GLuint index, GLint left, GLint bottom, GLsizei width, GLsizei height);
    public delegate void PFNGLSCISSORINDEXEDVPROC(GLuint index, GLint[] v);
    public delegate void PFNGLDEPTHRANGEARRAYVPROC(GLuint first, GLsizei count, GLclampd[] v);
    public delegate void PFNGLDEPTHRANGEINDEXEDPROC(GLuint index, GLclampd n, GLclampd f);
    public delegate void PFNGLGETFLOATI_VPROC(GLenum target, GLuint index, GLfloat[] data);
    public delegate void PFNGLGETDOUBLEI_VPROC(GLenum target, GLuint index, GLdouble[] data);
    public delegate GLsync PFNGLCREATESYNCFROMCLEVENTARBPROC(_cl_context context, _cl_event _event, GLbitfield flags);
    public delegate void PFNGLDEBUGMESSAGECONTROLARBPROC(GLenum source, GLenum type, GLenum severity, GLsizei count, GLuint[] ids, GLboolean enabled);
    public delegate void PFNGLDEBUGMESSAGEINSERTARBPROC(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string buf);
    public delegate void PFNGLDEBUGMESSAGECALLBACKARBPROC(GLDEBUGPROCARBP callback, GLvoid userParam);
    public delegate GLuint PFNGLGETDEBUGMESSAGELOGARBPROC(GLuint count, GLsizei bufsize, GLenum[] sources, GLenum[] types, GLuint[] ids, GLenum[] severities, GLsizei[] lengths, [MarshalAs(UnmanagedType.LPStr)]string messageLog);
    public delegate GLenum PFNGLGETGRAPHICSRESETSTATUSARBPROC();
    public delegate void PFNGLGETNMAPDVARBPROC(GLenum target, GLenum query, GLsizei bufSize, GLdouble[] v);
    public delegate void PFNGLGETNMAPFVARBPROC(GLenum target, GLenum query, GLsizei bufSize, GLfloat[] v);
    public delegate void PFNGLGETNMAPIVARBPROC(GLenum target, GLenum query, GLsizei bufSize, GLint[] v);
    public delegate void PFNGLGETNPIXELMAPFVARBPROC(GLenum map, GLsizei bufSize, GLfloat[] values);
    public delegate void PFNGLGETNPIXELMAPUIVARBPROC(GLenum map, GLsizei bufSize, GLuint[] values);
    public delegate void PFNGLGETNPIXELMAPUSVARBPROC(GLenum map, GLsizei bufSize, GLushort[] values);
    public delegate void PFNGLGETNPOLYGONSTIPPLEARBPROC(GLsizei bufSize, GLubyte[] pattern);
    public delegate void PFNGLGETNCOLORTABLEARBPROC(GLenum target, GLenum format, GLenum type, GLsizei bufSize, GLvoid table);
    public delegate void PFNGLGETNCONVOLUTIONFILTERARBPROC(GLenum target, GLenum format, GLenum type, GLsizei bufSize, GLvoid image);
    public delegate void PFNGLGETNSEPARABLEFILTERARBPROC(GLenum target, GLenum format, GLenum type, GLsizei rowBufSize, GLvoid row, GLsizei columnBufSize, GLvoid column, GLvoid span);
    public delegate void PFNGLGETNHISTOGRAMARBPROC(GLenum target, GLboolean reset, GLenum format, GLenum type, GLsizei bufSize, GLvoid values);
    public delegate void PFNGLGETNMINMAXARBPROC(GLenum target, GLboolean reset, GLenum format, GLenum type, GLsizei bufSize, GLvoid values);
    public delegate void PFNGLGETNTEXIMAGEARBPROC(GLenum target, GLint level, GLenum format, GLenum type, GLsizei bufSize, GLvoid img);
    public delegate void PFNGLREADNPIXELSARBPROC(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, GLsizei bufSize, GLvoid data);
    public delegate void PFNGLGETNCOMPRESSEDTEXIMAGEARBPROC(GLenum target, GLint lod, GLsizei bufSize, GLvoid img);
    public delegate void PFNGLGETNUNIFORMFVARBPROC(GLuint program, GLint location, GLsizei bufSize, GLfloat[] _params);
    public delegate void PFNGLGETNUNIFORMIVARBPROC(GLuint program, GLint location, GLsizei bufSize, GLint[] _params);
    public delegate void PFNGLGETNUNIFORMUIVARBPROC(GLuint program, GLint location, GLsizei bufSize, GLuint[] _params);
    public delegate void PFNGLGETNUNIFORMDVARBPROC(GLuint program, GLint location, GLsizei bufSize, GLdouble[] _params);
    public delegate void PFNGLDRAWARRAYSINSTANCEDBASEINSTANCEPROC(GLenum mode, GLint first, GLsizei count, GLsizei primcount, GLuint baseinstance);
    public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEINSTANCEPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices, GLsizei primcount, GLuint baseinstance);
    public delegate void PFNGLDRAWELEMENTSINSTANCEDBASEVERTEXBASEINSTANCEPROC(GLenum mode, GLsizei count, GLenum type, GLvoid indices, GLsizei primcount, GLint basevertex, GLuint baseinstance);
    public delegate void PFNGLDRAWTRANSFORMFEEDBACKINSTANCEDPROC(GLenum mode, GLuint id, GLsizei primcount);
    public delegate void PFNGLDRAWTRANSFORMFEEDBACKSTREAMINSTANCEDPROC(GLenum mode, GLuint id, GLuint stream, GLsizei primcount);
    public delegate void PFNGLGETINTERNALFORMATIVPROC(GLenum target, GLenum internalformat, GLenum pname, GLsizei bufSize, GLint[] _params);
    public delegate void PFNGLGETACTIVEATOMICCOUNTERBUFFERIVPROC(GLuint program, GLuint bufferIndex, GLenum pname, GLint[] _params);
    public delegate void PFNGLBINDIMAGETEXTUREPROC(GLuint unit, GLuint texture, GLint level, GLboolean layered, GLint layer, GLenum access, GLenum format);
    public delegate void PFNGLMEMORYBARRIERPROC(GLbitfield barriers);
    public delegate void PFNGLTEXSTORAGE1DPROC(GLenum target, GLsizei levels, GLenum internalformat, GLsizei width);
    public delegate void PFNGLTEXSTORAGE2DPROC(GLenum target, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXSTORAGE3DPROC(GLenum target, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth);
    public delegate void PFNGLTEXTURESTORAGE1DEXTPROC(GLuint texture, GLenum target, GLsizei levels, GLenum internalformat, GLsizei width);
    public delegate void PFNGLTEXTURESTORAGE2DEXTPROC(GLuint texture, GLenum target, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXTURESTORAGE3DEXTPROC(GLuint texture, GLenum target, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth);
    public delegate void GLDEBUGPROCARB(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string message, GLvoid userParam);
    public delegate GLubyte PFNGLGETSTRINGPROC(GLenum name);
    public delegate GLvoid PFNGLMAPBUFFERPROC(GLenum target, GLenum access);
    public delegate GLvoid PFNGLGETSTRINGIPROC(GLenum name, GLuint index);
    public delegate GLvoid PFNGLMAPBUFFERRANGEPROC(GLenum target, GLintptr offset, GLsizeiptr length, GLbitfield access);
    public delegate void PFNGLCLEARBUFFERDATAPROC(GLenum target, GLenum internalformat, GLenum format, GLenum type, GLvoid data);
    public delegate void PFNGLCLEARBUFFERSUBDATAPROC(GLenum target, GLenum internalformat, GLintptr offset, GLsizeiptr size, GLenum format, GLenum type, GLvoid data);
    public delegate void PFNGLDISPATCHCOMPUTEPROC(GLuint num_groups_x, GLuint num_groups_y, GLuint num_groups_z);
    public delegate void PFNGLDISPATCHCOMPUTEINDIRECTPROC(GLintptr indirect);
    public delegate void PFNGLCOPYIMAGESUBDATAPROC(GLuint srcName, GLenum srcTarget, GLint srcLevel, GLint srcX, GLint srcY, GLint srcZ, GLuint dstName, GLenum dstTarget, GLint dstLevel, GLint dstX, GLint dstY, GLint dstZ, GLsizei srcWidth, GLsizei srcHeight, GLsizei srcDepth);
    public delegate void PFNGLFRAMEBUFFERPARAMETERIPROC(GLenum target, GLenum pname, GLint param);
    public delegate void PFNGLGETFRAMEBUFFERPARAMETERIVPROC(GLenum target, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETINTERNALFORMATI64VPROC(GLenum target, GLenum internalformat, GLenum pname, GLsizei bufSize, GLint64[] _params);
    public delegate void PFNGLINVALIDATETEXSUBIMAGEPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth);
    public delegate void PFNGLINVALIDATETEXIMAGEPROC(GLuint texture, GLint level);
    public delegate void PFNGLINVALIDATEBUFFERSUBDATAPROC(GLuint buffer, GLintptr offset, GLsizeiptr length);
    public delegate void PFNGLINVALIDATEBUFFERDATAPROC(GLuint buffer);
    public delegate void PFNGLINVALIDATEFRAMEBUFFERPROC(GLenum target, GLsizei numAttachments, GLenum[] attachments);
    public delegate void PFNGLINVALIDATESUBFRAMEBUFFERPROC(GLenum target, GLsizei numAttachments, GLenum[] attachments, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLMULTIDRAWARRAYSINDIRECTPROC(GLenum mode, GLvoid indirect, GLsizei drawcount, GLsizei stride);
    public delegate void PFNGLMULTIDRAWELEMENTSINDIRECTPROC(GLenum mode, GLenum type, GLvoid indirect, GLsizei drawcount, GLsizei stride);
    public delegate void PFNGLGETPROGRAMINTERFACEIVPROC(GLuint program, GLenum programInterface, GLenum pname, GLint[] _params);
    public delegate GLuint PFNGLGETPROGRAMRESOURCEINDEXPROC(GLuint program, GLenum programInterface, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETPROGRAMRESOURCENAMEPROC(GLuint program, GLenum programInterface, GLuint index, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLGETPROGRAMRESOURCEIVPROC(GLuint program, GLenum programInterface, GLuint index, GLsizei propCount, GLenum[] props, GLsizei bufSize, GLsizei[] length, GLint[] _params);
    public delegate GLint PFNGLGETPROGRAMRESOURCELOCATIONPROC(GLuint program, GLenum programInterface, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate GLint PFNGLGETPROGRAMRESOURCELOCATIONINDEXPROC(GLuint program, GLenum programInterface, [MarshalAs(UnmanagedType.LPStr)]string name);
    public delegate void PFNGLSHADERSTORAGEBLOCKBINDINGPROC(GLuint program, GLuint storageBlockIndex, GLuint storageBlockBinding);
    public delegate void PFNGLTEXBUFFERRANGEPROC(GLenum target, GLenum internalformat, GLuint buffer, GLintptr offset, GLsizeiptr size);
    public delegate void PFNGLTEXSTORAGE2DMULTISAMPLEPROC(GLenum target, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height, GLboolean fixedsamplelocations);
    public delegate void PFNGLTEXSTORAGE3DMULTISAMPLEPROC(GLenum target, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth, GLboolean fixedsamplelocations);
    public delegate void PFNGLTEXTUREVIEWPROC(GLuint texture, GLenum target, GLuint origtexture, GLenum internalformat, GLuint minlevel, GLuint numlevels, GLuint minlayer, GLuint numlayers);
    public delegate void PFNGLBINDVERTEXBUFFERPROC(GLuint bindingindex, GLuint buffer, GLintptr offset, GLsizei stride);
    public delegate void PFNGLVERTEXATTRIBFORMATPROC(GLuint attribindex, GLint size, GLenum type, GLboolean normalized, GLuint relativeoffset);
    public delegate void PFNGLVERTEXATTRIBIFORMATPROC(GLuint attribindex, GLint size, GLenum type, GLuint relativeoffset);
    public delegate void PFNGLVERTEXATTRIBLFORMATPROC(GLuint attribindex, GLint size, GLenum type, GLuint relativeoffset);
    public delegate void PFNGLVERTEXATTRIBBINDINGPROC(GLuint attribindex, GLuint bindingindex);
    public delegate void PFNGLVERTEXBINDINGDIVISORPROC(GLuint bindingindex, GLuint divisor);
    public delegate void PFNGLDEBUGMESSAGECONTROLPROC(GLenum source, GLenum type, GLenum severity, GLsizei count, GLuint[] ids, GLboolean enabled);
    public delegate void PFNGLDEBUGMESSAGEINSERTPROC(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string buf);
    public delegate void PFNGLDEBUGMESSAGECALLBACKPROC(GLDEBUGPROC callback, GLvoid userParam);
    public delegate GLuint PFNGLGETDEBUGMESSAGELOGPROC(GLuint count, GLsizei bufSize, GLenum[] sources, GLenum[] types, GLuint[] ids, GLenum[] severities, GLsizei[] lengths, [MarshalAs(UnmanagedType.LPStr)]string messageLog);
    public delegate void PFNGLPUSHDEBUGGROUPPROC(GLenum source, GLuint id, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string message);
    public delegate void PFNGLPOPDEBUGGROUPPROC();
    public delegate void PFNGLOBJECTLABELPROC(GLenum identifier, GLuint name, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string label);
    public delegate void PFNGLGETOBJECTLABELPROC(GLenum identifier, GLuint name, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string label);
    public delegate void PFNGLOBJECTPTRLABELPROC(GLvoid ptr, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string label);
    public delegate void PFNGLGETOBJECTPTRLABELPROC(GLvoid ptr, GLsizei bufSize, GLsizei[] length, [MarshalAs(UnmanagedType.LPStr)]string label);
    public delegate void GLDEBUGPROC(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, [MarshalAs(UnmanagedType.LPStr)]string message, GLvoid userParam);
    public delegate void PFNGLBUFFERSTORAGEPROC(GLenum target, GLsizeiptr size, GLvoid data, GLbitfield flags);
    public delegate void PFNGLCLEARTEXIMAGEPROC(GLuint texture, GLint level, GLenum format, GLenum type, GLvoid data);
    public delegate void PFNGLCLEARTEXSUBIMAGEPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLenum type, GLvoid data);
    public delegate void PFNGLBINDBUFFERSBASEPROC(GLenum target, GLuint first, GLsizei count, GLuint[] buffers);
    public delegate void PFNGLBINDBUFFERSRANGEPROC(GLenum target, GLuint first, GLsizei count, GLuint[] buffers, GLintptr[] offsets, GLsizeiptr[] sizes);
    public delegate void PFNGLBINDTEXTURESPROC(GLuint first, GLsizei count, GLuint[] textures);
    public delegate void PFNGLBINDSAMPLERSPROC(GLuint first, GLsizei count, GLuint[] samplers);
    public delegate void PFNGLBINDIMAGETEXTURESPROC(GLuint first, GLsizei count, GLuint[] textures);
    public delegate void PFNGLBINDVERTEXBUFFERSPROC(GLuint first, GLsizei count, GLuint[] buffers, GLintptr[] offsets, GLsizei[] strides);
    public delegate void PFNGLCLIPCONTROLPROC(GLenum origin, GLenum depth);
    public delegate void PFNGLCREATETRANSFORMFEEDBACKSPROC(GLsizei n, GLuint[] ids);
    public delegate void PFNGLTRANSFORMFEEDBACKBUFFERBASEPROC(GLuint xfb, GLuint index, GLuint buffer);
    public delegate void PFNGLTRANSFORMFEEDBACKBUFFERRANGEPROC(GLuint xfb, GLuint index, GLuint buffer, GLintptr offset, GLsizeiptr size);
    public delegate void PFNGLGETTRANSFORMFEEDBACKIVPROC(GLuint xfb, GLenum pname, GLint[] param);
    public delegate void PFNGLGETTRANSFORMFEEDBACKI_VPROC(GLuint xfb, GLenum pname, GLuint index, GLint[] param);
    public delegate void PFNGLGETTRANSFORMFEEDBACKI64_VPROC(GLuint xfb, GLenum pname, GLuint index, GLint64[] param);
    public delegate void PFNGLCREATEBUFFERSPROC(GLsizei n, GLuint[] buffers);
    public delegate void PFNGLNAMEDBUFFERSTORAGEPROC(GLuint buffer, GLsizeiptr size, GLvoid data, GLbitfield flags);
    public delegate void PFNGLNAMEDBUFFERDATAPROC(GLuint buffer, GLsizeiptr size, GLvoid data, GLenum usage);
    public delegate void PFNGLNAMEDBUFFERSUBDATAPROC(GLuint buffer, GLintptr offset, GLsizeiptr size, GLvoid data);
    public delegate void PFNGLCOPYNAMEDBUFFERSUBDATAPROC(GLuint readBuffer, GLuint writeBuffer, GLintptr readOffset, GLintptr writeOffset, GLsizeiptr size);
    public delegate void PFNGLCLEARNAMEDBUFFERDATAPROC(GLuint buffer, GLenum internalformat, GLenum format, GLenum type, GLvoid data);
    public delegate void PFNGLCLEARNAMEDBUFFERSUBDATAPROC(GLuint buffer, GLenum internalformat, GLintptr offset, GLsizeiptr size, GLenum format, GLenum type, GLvoid data);
    public delegate GLvoid PFNGLMAPNAMEDBUFFERPROC(GLuint buffer, GLenum access);
    public delegate GLvoid PFNGLMAPNAMEDBUFFERRANGEPROC(GLuint buffer, GLintptr offset, GLsizeiptr length, GLbitfield access);
    public delegate GLboolean PFNGLUNMAPNAMEDBUFFERPROC(GLuint buffer);
    public delegate void PFNGLFLUSHMAPPEDNAMEDBUFFERRANGEPROC(GLuint buffer, GLintptr offset, GLsizeiptr length);
    public delegate void PFNGLGETNAMEDBUFFERPARAMETERIVPROC(GLuint buffer, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETNAMEDBUFFERPARAMETERI64VPROC(GLuint buffer, GLenum pname, GLint64[] _params);
    public delegate void PFNGLGETNAMEDBUFFERPOINTERVPROC(GLuint buffer, GLenum pname, GLvoid[] _params);
    public delegate void PFNGLGETNAMEDBUFFERSUBDATAPROC(GLuint buffer, GLintptr offset, GLsizeiptr size, GLvoid data);
    public delegate void PFNGLCREATEFRAMEBUFFERSPROC(GLsizei n, GLuint[] framebuffers);
    public delegate void PFNGLNAMEDFRAMEBUFFERRENDERBUFFERPROC(GLuint framebuffer, GLenum attachment, GLenum renderbuffertarget, GLuint renderbuffer);
    public delegate void PFNGLNAMEDFRAMEBUFFERPARAMETERIPROC(GLuint framebuffer, GLenum pname, GLint param);
    public delegate void PFNGLNAMEDFRAMEBUFFERTEXTUREPROC(GLuint framebuffer, GLenum attachment, GLuint texture, GLint level);
    public delegate void PFNGLNAMEDFRAMEBUFFERTEXTURELAYERPROC(GLuint framebuffer, GLenum attachment, GLuint texture, GLint level, GLint layer);
    public delegate void PFNGLNAMEDFRAMEBUFFERDRAWBUFFERPROC(GLuint framebuffer, GLenum buf);
    public delegate void PFNGLNAMEDFRAMEBUFFERDRAWBUFFERSPROC(GLuint framebuffer, GLsizei n, GLenum[] bufs);
    public delegate void PFNGLNAMEDFRAMEBUFFERREADBUFFERPROC(GLuint framebuffer, GLenum src);
    public delegate void PFNGLINVALIDATENAMEDFRAMEBUFFERDATAPROC(GLuint framebuffer, GLsizei numAttachments, GLenum[] attachments);
    public delegate void PFNGLINVALIDATENAMEDFRAMEBUFFERSUBDATAPROC(GLuint framebuffer, GLsizei numAttachments, GLenum[] attachments, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLCLEARNAMEDFRAMEBUFFERIVPROC(GLuint framebuffer, GLenum buffer, GLint drawbuffer, GLint[] value);
    public delegate void PFNGLCLEARNAMEDFRAMEBUFFERUIVPROC(GLuint framebuffer, GLenum buffer, GLint drawbuffer, GLuint[] value);
    public delegate void PFNGLCLEARNAMEDFRAMEBUFFERFVPROC(GLuint framebuffer, GLenum buffer, GLint drawbuffer, GLfloat[] value);
    public delegate void PFNGLCLEARNAMEDFRAMEBUFFERFIPROC(GLuint framebuffer, GLenum buffer, GLint drawbuffer, GLfloat depth, GLint stencil);
    public delegate void PFNGLBLITNAMEDFRAMEBUFFERPROC(GLuint readFramebuffer, GLuint drawFramebuffer, GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, GLbitfield mask, GLenum filter);
    public delegate GLenum PFNGLCHECKNAMEDFRAMEBUFFERSTATUSPROC(GLuint framebuffer, GLenum target);
    public delegate void PFNGLGETNAMEDFRAMEBUFFERPARAMETERIVPROC(GLuint framebuffer, GLenum pname, GLint[] param);
    public delegate void PFNGLGETNAMEDFRAMEBUFFERATTACHMENTPARAMETERIVPROC(GLuint framebuffer, GLenum attachment, GLenum pname, GLint[] _params);
    public delegate void PFNGLCREATERENDERBUFFERSPROC(GLsizei n, GLuint[] renderbuffers);
    public delegate void PFNGLNAMEDRENDERBUFFERSTORAGEPROC(GLuint renderbuffer, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLNAMEDRENDERBUFFERSTORAGEMULTISAMPLEPROC(GLuint renderbuffer, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLGETNAMEDRENDERBUFFERPARAMETERIVPROC(GLuint renderbuffer, GLenum pname, GLint[] _params);
    public delegate void PFNGLCREATETEXTURESPROC(GLenum target, GLsizei n, GLuint[] textures);
    public delegate void PFNGLTEXTUREBUFFERPROC(GLuint texture, GLenum internalformat, GLuint buffer);
    public delegate void PFNGLTEXTUREBUFFERRANGEPROC(GLuint texture, GLenum internalformat, GLuint buffer, GLintptr offset, GLsizeiptr size);
    public delegate void PFNGLTEXTURESTORAGE1DPROC(GLuint texture, GLsizei levels, GLenum internalformat, GLsizei width);
    public delegate void PFNGLTEXTURESTORAGE2DPROC(GLuint texture, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXTURESTORAGE3DPROC(GLuint texture, GLsizei levels, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth);
    public delegate void PFNGLTEXTURESTORAGE2DMULTISAMPLEPROC(GLuint texture, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height, GLboolean fixedsamplelocations);
    public delegate void PFNGLTEXTURESTORAGE3DMULTISAMPLEPROC(GLuint texture, GLsizei samples, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth, GLboolean fixedsamplelocations);
    public delegate void PFNGLTEXTURESUBIMAGE1DPROC(GLuint texture, GLint level, GLint xoffset, GLsizei width, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLTEXTURESUBIMAGE2DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLTEXTURESUBIMAGE3DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLenum type, GLvoid pixels);
    public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE1DPROC(GLuint texture, GLint level, GLint xoffset, GLsizei width, GLenum format, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE2DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOMPRESSEDTEXTURESUBIMAGE3DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLsizei imageSize, GLvoid data);
    public delegate void PFNGLCOPYTEXTURESUBIMAGE1DPROC(GLuint texture, GLint level, GLint xoffset, GLint x, GLint y, GLsizei width);
    public delegate void PFNGLCOPYTEXTURESUBIMAGE2DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLCOPYTEXTURESUBIMAGE3DPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLint x, GLint y, GLsizei width, GLsizei height);
    public delegate void PFNGLTEXTUREPARAMETERFPROC(GLuint texture, GLenum pname, GLfloat param);
    public delegate void PFNGLTEXTUREPARAMETERFVPROC(GLuint texture, GLenum pname, GLfloat[] param);
    public delegate void PFNGLTEXTUREPARAMETERIPROC(GLuint texture, GLenum pname, GLint param);
    public delegate void PFNGLTEXTUREPARAMETERIIVPROC(GLuint texture, GLenum pname, GLint[] _params);
    public delegate void PFNGLTEXTUREPARAMETERIUIVPROC(GLuint texture, GLenum pname, GLuint[] _params);
    public delegate void PFNGLTEXTUREPARAMETERIVPROC(GLuint texture, GLenum pname, GLint[] param);
    public delegate void PFNGLGENERATETEXTUREMIPMAPPROC(GLuint texture);
    public delegate void PFNGLBINDTEXTUREUNITPROC(GLuint unit, GLuint texture);
    public delegate void PFNGLGETTEXTUREIMAGEPROC(GLuint texture, GLint level, GLenum format, GLenum type, GLsizei bufSize, GLvoid pixels);
    public delegate void PFNGLGETCOMPRESSEDTEXTUREIMAGEPROC(GLuint texture, GLint level, GLsizei bufSize, GLvoid pixels);
    public delegate void PFNGLGETTEXTURELEVELPARAMETERFVPROC(GLuint texture, GLint level, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETTEXTURELEVELPARAMETERIVPROC(GLuint texture, GLint level, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETTEXTUREPARAMETERFVPROC(GLuint texture, GLenum pname, GLfloat[] _params);
    public delegate void PFNGLGETTEXTUREPARAMETERIIVPROC(GLuint texture, GLenum pname, GLint[] _params);
    public delegate void PFNGLGETTEXTUREPARAMETERIUIVPROC(GLuint texture, GLenum pname, GLuint[] _params);
    public delegate void PFNGLGETTEXTUREPARAMETERIVPROC(GLuint texture, GLenum pname, GLint[] _params);
    public delegate void PFNGLCREATEVERTEXARRAYSPROC(GLsizei n, GLuint[] arrays);
    public delegate void PFNGLDISABLEVERTEXARRAYATTRIBPROC(GLuint vaobj, GLuint index);
    public delegate void PFNGLENABLEVERTEXARRAYATTRIBPROC(GLuint vaobj, GLuint index);
    public delegate void PFNGLVERTEXARRAYELEMENTBUFFERPROC(GLuint vaobj, GLuint buffer);
    public delegate void PFNGLVERTEXARRAYVERTEXBUFFERPROC(GLuint vaobj, GLuint bindingindex, GLuint buffer, GLintptr offset, GLsizei stride);
    public delegate void PFNGLVERTEXARRAYVERTEXBUFFERSPROC(GLuint vaobj, GLuint first, GLsizei count, GLuint[] buffers, GLintptr[] offsets, GLsizei[] strides);
    public delegate void PFNGLVERTEXARRAYATTRIBBINDINGPROC(GLuint vaobj, GLuint attribindex, GLuint bindingindex);
    public delegate void PFNGLVERTEXARRAYATTRIBFORMATPROC(GLuint vaobj, GLuint attribindex, GLint size, GLenum type, GLboolean normalized, GLuint relativeoffset);
    public delegate void PFNGLVERTEXARRAYATTRIBIFORMATPROC(GLuint vaobj, GLuint attribindex, GLint size, GLenum type, GLuint relativeoffset);
    public delegate void PFNGLVERTEXARRAYATTRIBLFORMATPROC(GLuint vaobj, GLuint attribindex, GLint size, GLenum type, GLuint relativeoffset);
    public delegate void PFNGLVERTEXARRAYBINDINGDIVISORPROC(GLuint vaobj, GLuint bindingindex, GLuint divisor);
    public delegate void PFNGLGETVERTEXARRAYIVPROC(GLuint vaobj, GLenum pname, GLint[] param);
    public delegate void PFNGLGETVERTEXARRAYINDEXEDIVPROC(GLuint vaobj, GLuint index, GLenum pname, GLint[] param);
    public delegate void PFNGLGETVERTEXARRAYINDEXED64IVPROC(GLuint vaobj, GLuint index, GLenum pname, GLint64[] param);
    public delegate void PFNGLCREATESAMPLERSPROC(GLsizei n, GLuint[] samplers);
    public delegate void PFNGLCREATEPROGRAMPIPELINESPROC(GLsizei n, GLuint[] pipelines);
    public delegate void PFNGLCREATEQUERIESPROC(GLenum target, GLsizei n, GLuint[] ids);
    public delegate void PFNGLGETQUERYBUFFEROBJECTI64VPROC(GLuint id, GLuint buffer, GLenum pname, GLintptr offset);
    public delegate void PFNGLGETQUERYBUFFEROBJECTIVPROC(GLuint id, GLuint buffer, GLenum pname, GLintptr offset);
    public delegate void PFNGLGETQUERYBUFFEROBJECTUI64VPROC(GLuint id, GLuint buffer, GLenum pname, GLintptr offset);
    public delegate void PFNGLGETQUERYBUFFEROBJECTUIVPROC(GLuint id, GLuint buffer, GLenum pname, GLintptr offset);
    public delegate void PFNGLMEMORYBARRIERBYREGIONPROC(GLbitfield barriers);
    public delegate void PFNGLGETTEXTURESUBIMAGEPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLenum format, GLenum type, GLsizei bufSize, GLvoid pixels);
    public delegate void PFNGLGETCOMPRESSEDTEXTURESUBIMAGEPROC(GLuint texture, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLsizei bufSize, GLvoid pixels);
    public delegate GLenum PFNGLGETGRAPHICSRESETSTATUSPROC();
    public delegate void PFNGLGETNCOMPRESSEDTEXIMAGEPROC(GLenum target, GLint lod, GLsizei bufSize, GLvoid pixels);
    public delegate void PFNGLGETNTEXIMAGEPROC(GLenum target, GLint level, GLenum format, GLenum type, GLsizei bufSize, GLvoid pixels);
    public delegate void PFNGLGETNUNIFORMDVPROC(GLuint program, GLint location, GLsizei bufSize, GLdouble[] _params);
    public delegate void PFNGLGETNUNIFORMFVPROC(GLuint program, GLint location, GLsizei bufSize, GLfloat[] _params);
    public delegate void PFNGLGETNUNIFORMIVPROC(GLuint program, GLint location, GLsizei bufSize, GLint[] _params);
    public delegate void PFNGLGETNUNIFORMUIVPROC(GLuint program, GLint location, GLsizei bufSize, GLuint[] _params);
    public delegate void PFNGLREADNPIXELSPROC(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, GLsizei bufSize, GLvoid data);
    public delegate void PFNGLTEXTUREBARRIERPROC();
    public delegate GLuint64 PFNGLGETTEXTUREHANDLEARBPROC(GLuint texture);
    public delegate GLuint64 PFNGLGETTEXTURESAMPLERHANDLEARBPROC(GLuint texture, GLuint sampler);
    public delegate void PFNGLMAKETEXTUREHANDLERESIDENTARBPROC(GLuint64 handle);
    public delegate void PFNGLMAKETEXTUREHANDLENONRESIDENTARBPROC(GLuint64 handle);
    public delegate GLuint64 PFNGLGETIMAGEHANDLEARBPROC(GLuint texture, GLint level, GLboolean layered, GLint layer, GLenum format);
    public delegate void PFNGLMAKEIMAGEHANDLERESIDENTARBPROC(GLuint64 handle, GLenum access);
    public delegate void PFNGLMAKEIMAGEHANDLENONRESIDENTARBPROC(GLuint64 handle);
    public delegate void PFNGLUNIFORMHANDLEUI64ARBPROC(GLint location, GLuint64 value);
    public delegate void PFNGLUNIFORMHANDLEUI64VARBPROC(GLint location, GLsizei count, GLuint64[] value);
    public delegate void PFNGLPROGRAMUNIFORMHANDLEUI64ARBPROC(GLuint program, GLint location, GLuint64 value);
    public delegate void PFNGLPROGRAMUNIFORMHANDLEUI64VARBPROC(GLuint program, GLint location, GLsizei count, GLuint64[] values);
    public delegate GLboolean PFNGLISTEXTUREHANDLERESIDENTARBPROC(GLuint64 handle);
    public delegate GLboolean PFNGLISIMAGEHANDLERESIDENTARBPROC(GLuint64 handle);
    public delegate void PFNGLVERTEXATTRIBL1UI64ARBPROC(GLuint index, GLuint64EXT x);
    public delegate void PFNGLVERTEXATTRIBL1UI64VARBPROC(GLuint index, GLuint64EXT[] v);
    public delegate void PFNGLGETVERTEXATTRIBLUI64VARBPROC(GLuint index, GLenum pname, GLuint64EXT[] _params);
    public delegate void PFNGLDISPATCHCOMPUTEGROUPSIZEARBPROC(GLuint num_groups_x, GLuint num_groups_y, GLuint num_groups_z, GLuint group_size_x, GLuint group_size_y, GLuint group_size_z);
    public delegate void PFNGLMULTIDRAWARRAYSINDIRECTCOUNTARBPROC(GLenum mode, GLintptr indirect, GLintptr drawcount, GLsizei maxdrawcount, GLsizei stride);
    public delegate void PFNGLMULTIDRAWELEMENTSINDIRECTCOUNTARBPROC(GLenum mode, GLenum type, GLintptr indirect, GLintptr drawcount, GLsizei maxdrawcount, GLsizei stride);
    public delegate void PFNGLBUFFERPAGECOMMITMENTARBPROC(GLenum target, GLintptr offset, GLsizeiptr size, GLboolean commit);
    public delegate void PFNGLNAMEDBUFFERPAGECOMMITMENTEXTPROC(GLuint buffer, GLintptr offset, GLsizeiptr size, GLboolean commit);
    public delegate void PFNGLNAMEDBUFFERPAGECOMMITMENTARBPROC(GLuint buffer, GLintptr offset, GLsizeiptr size, GLboolean commit);
    public delegate void PFNGLTEXPAGECOMMITMENTARBPROC(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint zoffset, GLsizei width, GLsizei height, GLsizei depth, GLboolean commit);
}