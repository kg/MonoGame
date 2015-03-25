// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class Shader
    {
        // The shader handle.
        private uint _shaderHandle = 0;

        // We keep this around for recompiling on context lost and debugging.
        private string _glslCode;

        private struct Attribute
        {
            public VertexElementUsage usage;
            public int index;
            public string name;
            public int location;
        }

        private Attribute[] _attributes;

        private void PlatformConstruct(BinaryReader reader, bool isVertexShader, byte[] shaderBytecode)
        {
            _glslCode = System.Text.Encoding.ASCII.GetString(shaderBytecode);

            HashKey = MonoGame.Utilities.Hash.ComputeHash(shaderBytecode);

            var attributeCount = (int)reader.ReadByte();
            _attributes = new Attribute[attributeCount];
            for (var a = 0; a < attributeCount; a++)
            {
                _attributes[a].name = reader.ReadString();
                _attributes[a].usage = (VertexElementUsage)reader.ReadByte();
                _attributes[a].index = reader.ReadByte();
                reader.ReadInt16(); //format, unused
            }
        }

        internal uint GetShaderHandle()
        {
            // If the shader has already been created then return it.
            if (_shaderHandle != 0)
                return _shaderHandle;
            
            _shaderHandle = GraphicsDevice.GLDevice.glCreateShader(Stage == ShaderStage.Vertex ? OpenGLDevice.GLenum.GL_VERTEX_SHADER : OpenGLDevice.GLenum.GL_FRAGMENT_SHADER);
            GraphicsDevice.GLDevice.glShaderSource(_shaderHandle, 1, new string[] { _glslCode }, new int[] { _glslCode.Length });
            GraphicsDevice.GLDevice.glCompileShader(_shaderHandle);

            var compiled = 0;
            GraphicsDevice.GLDevice.glGetShaderiv(_shaderHandle, OpenGLDevice.GLenum.GL_COMPILE_STATUS, out compiled);
            if (compiled == 0)
            {
                //Console.WriteLine(
                //    GraphicsDevice.GLDevice.glGetShaderInfoLog(_shaderHandle)
                //);

                if (GraphicsDevice.GLDevice.glIsShader(_shaderHandle))
                {
                    GraphicsDevice.GLDevice.glDeleteShader(_shaderHandle);
                }
                _shaderHandle = 0;

                throw new InvalidOperationException("Shader Compilation Failed");
            }

            return _shaderHandle;
        }

        internal void GetVertexAttributeLocations(uint program)
        {
            for (int i = 0; i < _attributes.Length; ++i)
            {
                _attributes[i].location = GraphicsDevice.GLDevice.glGetAttribLocation(program, _attributes[i].name);
            }
        }

        internal int GetAttribLocation(VertexElementUsage usage, int index)
        {
            for (int i = 0; i < _attributes.Length; ++i)
            {
                if ((_attributes[i].usage == usage) && (_attributes[i].index == index))
                    return _attributes[i].location;
            }
            return -1;
        }

        internal void ApplySamplerTextureUnits(uint program)
        {
            // Assign the texture unit index to the sampler uniforms.
            foreach (var sampler in Samplers)
            {
                var loc = GraphicsDevice.GLDevice.glGetUniformLocation(program, sampler.name);
                if (loc != -1)
                {
                    GraphicsDevice.GLDevice.glUniform1i(loc, sampler.textureSlot);
                }
            }
        }

        private void PlatformGraphicsDeviceResetting()
        {
            if (_shaderHandle != 0)
            {
                if (GraphicsDevice.GLDevice.glIsShader(_shaderHandle))
                {
                    GraphicsDevice.GLDevice.glDeleteShader(_shaderHandle);
                }
                _shaderHandle = 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GraphicsDevice.AddDisposeAction(() =>
                {
                    if (_shaderHandle != 0)
                    {
                        if (GraphicsDevice.GLDevice.glIsShader(_shaderHandle))
                        {
                            GraphicsDevice.GLDevice.glDeleteShader(_shaderHandle);
                        }
                        _shaderHandle = 0;
                    }
                });
            }

            base.Dispose(disposing);
        }
    }
}
