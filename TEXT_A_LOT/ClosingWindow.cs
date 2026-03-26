using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpFont;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using TEXT_A_LOT;

namespace TEXT_A_LOT
{
    public class ClosingWindow: GameWindow
    {

        Library ft;
        Face face;
        int texture;
        int vao;
        int vbo;
        Shader shader;
        Dictionary<char, Character> Characters = new Dictionary<char, Character>();

        public ClosingWindow(int width, int height, string title) : base(GameWindowSettings.Default,
           new NativeWindowSettings() { Size = (width, height), Title = title })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();
        ft = new Library();
        string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayfairDisplay-VariableFont_wght.ttf");
        face = new Face(ft, fontPath);
            if (face == null)
            {
                Console.WriteLine("ERROR: Face object is null.");
            }
            else
            {
                Console.WriteLine($"Font Loaded: {face.FamilyName}");
                Console.WriteLine($"Glyphs found: {face.GlyphCount}");
            }
face.NewSize();
face.SetPixelSizes(0, 48);
face.LoadChar('B', LoadFlags.Render, LoadTarget.Normal);

if (face.Glyph.Bitmap.Buffer == IntPtr.Zero)
{
    Console.WriteLine("ERROR: Glyph bitmap buffer is empty. Rendering failed.");
}
else
{
    Console.WriteLine($"'A' loaded. Size: {face.Glyph.Bitmap.Width}x{face.Glyph.Bitmap.Rows}, {face.Glyph.Bitmap.ToString()}");
}
GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1); //diable byte alignmwnt restriction

GL.ClearColor(0.1f, .10f, 0.1f, 1.0f);
for (byte c = 0; c < 128; c++)
{
    face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
    //generate texture
    texture = GL.GenTexture();
    GL.BindTexture(TextureTarget.Texture2D, texture);
    int[] swizzle = { (int)All.Red, (int)All.Red, (int)All.Red, (int)All.Red };
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleRgba, swizzle);
    GL.TexImage2D(TextureTarget.Texture2D,
        0,
        PixelInternalFormat.R8,
        face.Glyph.Bitmap.Width,
        face.Glyph.Bitmap.Rows,
        0,
        PixelFormat.Red,
        PixelType.UnsignedByte,
        face.Glyph.Bitmap.Buffer
        );

    //texture options 
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    //store
    Character character = new Character
    {
        TextureID = (int)texture,
        size = new Vector2i(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows),
        bearing = new Vector2i(face.Glyph.BitmapLeft, face.Glyph.BitmapTop),
        advance = (uint)face.Glyph.Advance.X.Value
    };

    Characters.Add((char)c, character);
}
GL.BindTexture(TextureTarget.Texture2D, 0);

shader = new Shader("shader.vert", "shader.frag");

shader.use();


GL.Enable(EnableCap.Blend);
GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, 800.0f, 0.0f, 600.0f, -1.0f, 1.0f);
int projectionLoc = GL.GetUniformLocation(shader.Handle, "projection");
GL.UniformMatrix4(projectionLoc, true, ref projection);

vao = GL.GenVertexArray();
vbo = GL.GenBuffer();

GL.BindVertexArray(vao);
GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, 0, BufferUsageHint.DynamicDraw);
GL.EnableVertexAttribArray(0);
GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
GL.BindVertexArray(0);
           
            


        }

        protected override void OnRenderFrame(FrameEventArgs args)
{
    Vector3 vec = new Vector3(1.0f, 1.0f, 1.0f);
    shader.use();
    GL.Clear(ClearBufferMask.ColorBufferBit);


    RenderText(shader, "save file on exit", 1.0f, 100.0f, 1.5f, vec);




    SwapBuffers();

}

protected override void OnUnload()
{
    base.OnUnload();
    shader.Dispose();

}

protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            

        }



        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            
            
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
        public void RenderText(Shader shader, string text, float x, float y, float scale, Vector3 color)
        {
            //active shader render


            shader.use();

            GL.Uniform3(GL.GetUniformLocation(shader.Handle, "textColor"), color.X, color.Y, color.Z);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(vao);
           



            foreach (char c in text)
            {
                Character ch = Characters[c];
                float xpos = x + ch.bearing.X * scale;
                float ypos = y - (ch.size.Y - ch.bearing.Y) * scale;

                float w = ch.size.X * scale;
                float h = ch.size.Y * scale;
                // update VBO for each character
                float[] vertices =
{
    xpos,     ypos + h,   0.0f, 0.0f,
    xpos,     ypos,       0.0f, 1.0f,
    xpos + w, ypos,       1.0f, 1.0f,

    xpos,     ypos + h,   0.0f, 0.0f,
    xpos + w, ypos,       1.0f, 1.0f,
    xpos + w, ypos + h,   1.0f, 0.0f
};

                //render glph over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                //update context of VBO memory
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, sizeof(float) * vertices.Length, vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                //render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                if (x >= 700f)
                {
                    x = 10;// Reset x
                    y -= 50; // Move down
                    continue;
                }

                //advance cursoe for next glyph
                x += (ch.advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
            }
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }







    }
}
