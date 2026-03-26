using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using StbImageSharp;
using System.IO;

namespace TEXT_A_LOT
{
    // A helper class, much like Shader, meant to simplify loading textures.
    public class Texture
    {
        public readonly int Handle;

        public static Texture LoadFromFile(string path)
        {
            // Generate handle
            int handle = GL.GenTexture();

            // Bind the handle
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

            // OpenGL has it's texture origin in the lower left corner instead of the top left corner,
            // so we tell StbImageSharp to flip the image when loading.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Here we open a stream to the file and pass it to StbImageSharp to load.
            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                if (image != null)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

                }
                else
                {
                    Console.WriteLine("failsed");
                }
            }
         
            return new Texture(handle);
        }

        public Texture(int glHandle)
        {
            Handle = glHandle;
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }


    }
}
