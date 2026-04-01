using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpFont;
using StbImageSharp;
using System.ComponentModel;
using System.Text;
using TEXT_A_LOT;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using NfdSharp;
using NfdExt;
using OpenTK.Windowing.Common.Input;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace TEXT_A_LOT
{
    partial class Text
    {
        public void RenderText(Shader shader, string text, float x, float y, float scale, Vector3 color)
        {
            //active shader render


            shader.use();

            GL.Uniform3(GL.GetUniformLocation(shader.Handle, "textColor"), color.X, color.Y, color.Z);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(vao);
            text = ag.ToString();



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
