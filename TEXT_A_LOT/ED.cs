using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace TEXT_A_LOT
{
    public class ED : GameWindow
    {

        Shader shader;
        Stopwatch timer = new Stopwatch(){};

        float[] vertices =
        {
            0.5f, 0.5f, 0.0f,//top-right Vertex
            0.5f, -0.5f, 0.0f,//Bottom right vertex
            -0.5f,  -0.5f, 0.0f,  //bottom left
            -0.5f,  0.5f, 0.0f,  //topleft

        };

        uint[] indicies =
        {
            0,1,3,  // first triangle
            1,2,3 // second triangle
        };
        int VertexBufferObject;
        int EBO;

        int VertexArrayObject;


        public ED(int width, int height, string title) : base(GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title })
        { }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

          


            //copy the vertext data into the buffer's memory
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Length * sizeof(uint), indicies, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");
            shader.use();
            timer = new Stopwatch();
            timer.Start();

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.use();
//update uniform color double
        double timeValue = timer.Elapsed.TotalSeconds;
            float greenvalue = (float)Math.Sin(timeValue) / 2.0f + 0.9f;
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "uniColor");
            GL.Uniform4(vertexColorLocation, 0.0f, greenvalue, 0.0f, 1.0f);
            
            GL.BindVertexArray(VertexArrayObject);

            GL.DrawElements(PrimitiveType.Triangles, indicies.Length, DrawElementsType.UnsignedInt, 0);


            SwapBuffers();

        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            shader.Dispose();
        }

    }
}
