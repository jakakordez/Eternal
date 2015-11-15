using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace EGE.Meshes
{
    public class Mesh
    {
        Material[] Materials;
        uint[] ElementArrays;
        int[] ElementArraySizes;
        int VertexBuffer;
        int TextureCoordinateBuffer;
        public Vector3 Location { get;set; }
        public string ObjectFileName { get; set; }

        public Mesh()
        {
            ObjectFileName = "";
            Materials = new Material[0];
            Location = new Vector3();
        }

        public void Draw()
        {
            for (int i = 0; i < ElementArraySizes.Length; i++)
            {
                if (Materials[i].Brush== Color4.Black) System.Diagnostics.Debugger.Break(); ;
                if (Materials[i].Texture != "")
                {
                    GL.Color4(Color.White);
                    Tools.TextureManager.BindTexture(Materials[i].Texture);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.Color4(Materials[i].Brush);
                }

                GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                // Vertex Array Buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer); //Bind Array Buffer
                                                                       // Set the Pointer to the current bound array describing how the data ia stored
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                // Enable the client state so it will use this array buffer pointer
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArrays[i]);

                switch (Settings.CurrentDrawingMode)
                {
                    case Settings.DrawingModes.Wireframe:
                        GL.DrawElements(PrimitiveType.LineStrip, ElementArraySizes[i], DrawElementsType.UnsignedInt, IntPtr.Zero);
                        break;
                    case Settings.DrawingModes.Full:
                        GL.DrawElements(PrimitiveType.Triangles, ElementArraySizes[i], DrawElementsType.UnsignedInt, IntPtr.Zero);
                        break;
                    case Settings.DrawingModes.Textured:
                        GL.BindBuffer(BufferTarget.ArrayBuffer, TextureCoordinateBuffer);
                        // Set the Pointer to the current bound array describing how the data ia stored
                        GL.TexCoordPointer(2, TexCoordPointerType.Float, 8, IntPtr.Zero);
                        // Enable the client state so it will use this array buffer pointer
                        GL.EnableClientState(ArrayCap.TextureCoordArray);

                        GL.DrawElements(PrimitiveType.Triangles, ElementArraySizes[i], DrawElementsType.UnsignedInt, IntPtr.Zero);
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                        break;
                }
                // Restore the state
                GL.PopClientAttrib();
            }
        }

        public void Load(Vector3[] Vertices, int[] Indicies, string TextureName, Vector2[] TextureCoordinates)
        {
            Materials = new Material[] { new Material("texture") { Texture = TextureName } };
            VertexBuffer = AddVertexBuffer(Vertices);
            TextureCoordinateBuffer = AddTextureCoordsBuffer(TextureCoordinates);
            ElementArrays = new uint[1];
            GL.GenBuffers(1, ElementArrays);
            ElementArraySizes = new int[] { FillIndexBuffer(Indicies, ElementArrays[0]) };
        }

        public void LoadOBJ()
        {
            string name = ObjectFileName;
            Face[] Faces = new Face[0];
            Vector3[] OriginalVertices = new Vector3[0];
            Vector2[] OriginalTextureCoordinates = new Vector2[0];
            Vector3[] SortedVertices = new Vector3[0];
            Vector2[] SortedTextureCoordinates = new Vector2[0];

            string[] file = Encoding.Default.GetString(Tools.ResourceManager.GetResource(name)).Replace("\r", "").Split('\n');
            int currentMaterial = 0;
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] != "" && file[i][0] != '#')
                {
                    string[] line = file[i].Split(' ');
                    switch (line[0])
                    {
                        case "mtllib":
                            LoadMTL(line[1]);
                            break;
                        case "usemtl":
                            for (int j = 0; j < Materials.Length; j++)
                            {
                                if (Materials[j].Name == line[1])
                                {
                                    currentMaterial = j;
                                    break;
                                }
                            }
                            break;
                        case "v":
                            Misc.Push<Vector3>(new Vector3(Misc.toFloat(line[1]), Misc.toFloat(line[2]), Misc.toFloat(line[3])), ref OriginalVertices);
                            break;
                        case "f":
                            Face f = new Face();
                            f.mtl = currentMaterial;
                            for (int j = 1; j < line.Length; j++)
                            {
                                if (line[j].Contains('/'))
                                {
                                    string[] fac = line[j].Split('/');
                                    int v = Misc.Push<Vector3>(OriginalVertices[Misc.toInt(fac[0]) - 1], ref SortedVertices);
                                    int t = Misc.Push<Vector2>(OriginalTextureCoordinates[Misc.toInt(fac[1]) - 1], ref SortedTextureCoordinates);
                                    Misc.Push<int>(v, ref f.vertices);
                                }
                            }
                            Misc.Push<Face>(f, ref Faces);
                            break;
                        case "vt":
                            Misc.Push<Vector2>(new Vector2(Misc.toFloat(line[2]), Misc.toFloat(line[1])), ref OriginalTextureCoordinates);
                            break;
                    }
                }
            }

            Array.Sort<Face>(Faces, delegate (Face x, Face y) { return x.mtl.CompareTo(y.mtl); });
            currentMaterial = Faces[0].mtl;
            //  Misc.Push<Material>(Materials[currentMaterial].Brush, ref Materials);
            ElementArrays = new uint[Materials.Length];
            GL.GenBuffers(Materials.Length, ElementArrays);
            int[] currentElements = new int[0];
            ElementArraySizes = new int[0];
            int set = 0;
            for (int i = 0; i < Faces.Length; i++)
            {
                Misc.Push<int>(Faces[i].vertices, ref currentElements);
                if (Faces.Length - 1 == i || currentMaterial != Faces[i + 1].mtl)
                {
                    Misc.Push<int>(FillIndexBuffer(currentElements, ElementArrays[set]), ref ElementArraySizes);
                    if (Faces.Length - 1 > i)
                    {
                        set++;
                        currentMaterial = Faces[i + 1].mtl;
                        Misc.Push<Material>(Materials[currentMaterial], ref Materials);
                        currentElements = new int[0];
                    }
                }

            }
            VertexBuffer = AddVertexBuffer(SortedVertices);
            TextureCoordinateBuffer = AddTextureCoordsBuffer(SortedTextureCoordinates);
        }

        private void LoadMTL(string name)
        {
            string[] file = Encoding.Default.GetString(Tools.ResourceManager.GetResource(name)).Replace("\r", "").Split('\n');
            Material currentMaterial = null;
            for (int i = 0; i < file.Length; i++)
            {
                string[] line = file[i].Split(' ');
                switch (line[0])
                {
                    case "newmtl":
                        if (currentMaterial != null) Misc.Push<Material>(currentMaterial, ref Materials);
                        currentMaterial = new Material(line[1]);
                        break;
                    case "Kd":
                        if (currentMaterial != null)
                        {
                            currentMaterial.Brush.R = Misc.toFloat(line[1]);
                            currentMaterial.Brush.G = Misc.toFloat(line[2]);
                            currentMaterial.Brush.B = Misc.toFloat(line[3]);
                        }
                        break;
                    case "map_Kd":
                        if (currentMaterial != null)
                        {
                            currentMaterial.Texture = Misc.pathName(line[1]);
                        }
                        break;
                }
            }
            if (currentMaterial != null) Misc.Push<Material>(currentMaterial, ref Materials);
        }

        public static int AddVertexBuffer(Vector3[] Vertices)
        {
            int VertexBuffer = GL.GenBuffer();
            long bufferSize;
            // Vertex Array Buffer
            {
                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);

                // Send data to buffer
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vector3.SizeInBytes), Vertices, BufferUsageHint.StaticDraw);

                // Validate that the buffer is the correct size

                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (Vertices.Length * Vector3.SizeInBytes != bufferSize)
                    throw new ApplicationException("Vertex array not uploaded correctly");

                // Clear the buffer Binding
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            return VertexBuffer;
        }

        public static int AddTextureCoordsBuffer(Vector2[] TextureCoordinates)
        {
            int TextureCoordinateBuffer = GL.GenBuffer();
            long bufferSize;
            {
                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, TextureCoordinateBuffer);

                // Send data to buffer
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TextureCoordinates.Length * 8), TextureCoordinates, BufferUsageHint.StaticDraw);

                // Validate that the buffer is the correct size
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (TextureCoordinates.Length * 8 != bufferSize)
                    throw new ApplicationException("TexCoord array not uploaded correctly");

                // Clear the buffer Binding
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            return TextureCoordinateBuffer;
        }

        static int FillIndexBuffer(int[] Indicies, uint Buffer)
        {
            long bufferSize;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indicies.Length * sizeof(int)), Indicies, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (Indicies.Length * sizeof(int) != bufferSize)
                throw new ApplicationException("Element array not uploaded correctly");
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            return Indicies.Length;
        }
    }
    public class Face
    {
        public int[] vertices;
        public int[] textureCoordinates;
        public int mtl;
        public Face(int[] v)
        {
            vertices = v;
        }
        public Face()
        {
            vertices = new int[0];
            textureCoordinates = new int[0];
        }
    }

    class Material
    {
        public Color4 Brush;
        public string Name;
        public string Texture;
        public Material(string name)
        {
            Name = name;
            Brush = new Color4();
            Brush.A = 255;
            Texture = "";
        }
    }
}
