
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpFont;
using StbImageSharp;
using System.ComponentModel;
using System.Text;
using TEXT_A_LOT;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace TEXT_A_LOT

{
    class Text : GameWindow
    {
        Library ft;
        Face face;
        int texture;
        int vao;
        int vbo;
        Shader shader;
        string a;
        StringBuilder ag = new StringBuilder();
        Vector2i mousePos;

        Dictionary<char, Character> Characters = new Dictionary<char, Character>();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();
        public Text(int width, int height, string title) : base(GameWindowSettings.Default,
                       new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            
           
        }
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
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1); //disable byte alignmwnt restriction

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
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4,0  , BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false,4 *sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // In your GameWindow subclass constructor or Load event:
             



        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Vector3 vec = new Vector3(1.0f, 1.0f, 1.0f);
            shader.use();
            GL.Clear(ClearBufferMask.ColorBufferBit);

          
            RenderText(shader, "", 10.0f, 550.0f, .50f, vec);
            
            
             

            SwapBuffers();
            
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
           

        }

        protected override void OnUnload()
        {
            base.OnUnload();
            shader.Dispose();
            
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
          
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyDown(Keys.Backspace))
            {
                if (ag.Length >= 1)
                {
                    ag.Length--;
                    if (KeyboardState.IsKeyReleased(Keys.Backspace))
                    {

                    }
                }
            }
            
            PointToClient(mousePos);
           


        }



        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            char keySymbol = (char)e.Unicode;
            ag.Append(keySymbol);
          
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            SaveSystem s = new SaveSystem();
            string f_name = "bb";
            string b = ag.ToString();
            s.SaveDataAsync(f_name, b);

            using (ClosingWindow close = new ClosingWindow(300, 100, "CLOSE"))
            {
                close.CenterWindow();
                close.MaximumSize = (300, 100);
                close.Run();
                Vector3 vec = new Vector3(1.0f, 1.0f, 1.0f);

            }

            


        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
                

        
    }

    struct Character
    {
        public int TextureID; // ID handle of the glyph texture
        public Vector2i size; // size of glyph
        public Vector2i bearing; // offset from baseline to left/top of glyph
        public uint advance; // offset to advance to next glyph
    };
    

}