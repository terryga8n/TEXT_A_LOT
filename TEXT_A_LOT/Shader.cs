
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TEXT_A_LOT
{
    public class Shader
    {
        public int Handle;


        public Shader(string vertexPath, string fragmentPath)
        {
            int VertexShader;
            int FragmentShader;
            //Load  the source code from the individual shader files
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);
            //generate shaders and bind to the source 
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            //complile and check for errors
            GL.CompileShader(VertexShader);
            GL.CompileShader(FragmentShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);

            if (success == 0)
            {
                string infolog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infolog);
            }

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out  success);

            if (success == 0)
            {
                string infolog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infolog);
            }

            //linking shaders into a program
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int successf);
            if (successf == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            //detach individual shader fragments as they are now part of the shader prgrm
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);


        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak, did you forget  to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
};