using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO.Compression;
using System.IO;
using BulletSharp;
using Newtonsoft.Json;

namespace EGE.Meshes
{
    public class Mesh
    {
        TriangleMesh CollisionShape;
        Material[] Materials;
        uint[] ElementArrays;
        int[] ElementArraySizes;
        int VertexBuffer;
        int TextureCoordinateBuffer;
        int NormalBuffer;
        public string MeshFolder = "";
        public float Size = 0;
        public string Name { get; set; }
        public bool GenerateCollisionShape { get; set; }

        public Mesh()
        {
            Name = "";
            Materials = new Material[0];
            GenerateCollisionShape = false;
        }

        public Mesh(String name)
        {
            Name = name;
            Materials = new Material[0];
        }

        public void Draw()
        {
            Draw(Color4.Transparent);
        }

        public CollisionShape GetConvexCollisionShape()
        {
            ConvexHullShape convexShape;
            using (var tmpConvexShape = new ConvexTriangleMeshShape(CollisionShape))
            {
                var hull = new ShapeHull(tmpConvexShape);
                hull.BuildHull(tmpConvexShape.Margin);
                convexShape = new ConvexHullShape(hull.Vertices);
            }
            return convexShape;
        }

        public CollisionShape GetCollisionShape()
        {
            return new BvhTriangleMeshShape(CollisionShape, true);
        }

        public void Draw(Color4 color)
        {
            for (int i = 0; i < ElementArraySizes.Length; i++)
            {
                if (Materials[i].Texture != "")
                {
                    Resources.BindTexture(MeshFolder + Materials[i].Texture);
                    Graphics.SetColor(new Color4(0, 0, 0, 255f));
                }
                else if (color != Color4.Transparent && Materials[i].Name == "primary_color")
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    Graphics.SetColor(color);
                    color = Color4.Transparent;
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    Graphics.SetColor(Materials[i].Brush);
                }

                GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.TextureCoordArray);

                // Vertex Array Buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer); //Bind Vertex Buffer
                GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero); // Set vertex pointer

                // Normal buffer
                GL.EnableVertexArrayAttrib(0, 2);
                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

                // Texture buffer
                GL.EnableVertexArrayAttrib(0, 1);
                GL.BindBuffer(BufferTarget.ArrayBuffer, TextureCoordinateBuffer); // Bind texture buffer
                //GL.TexCoordPointer(2, TexCoordPointerType.Float, 8, IntPtr.Zero); // Set texture pointer
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);

                // Index buffer
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArrays[i]); // Bind Index buffer

                GL.DrawElements(PrimitiveType.Triangles, ElementArraySizes[i], DrawElementsType.UnsignedInt, IntPtr.Zero);
                GL.BindTexture(TextureTarget.Texture2D, 0); // Reset texture

                // Restore the state
                GL.PopClientAttrib();
            }
        }

        public void Load(Vector3[] Vertices, int[] Indices, string TextureName, Vector2[] TextureCoordinates)
        {
            Materials = new Material[] { new Material("texture") { Texture = TextureName } };
            VertexBuffer = AddVertexBuffer(Vertices);
            TextureCoordinateBuffer = AddTextureCoordsBuffer(TextureCoordinates);
            ElementArrays = new uint[1];
            GL.GenBuffers(1, ElementArrays);
            ElementArraySizes = new int[] { FillIndexBuffer(Indices, ElementArrays[0]) };
            CollisionShape = new TriangleMesh();

            if (GenerateCollisionShape)
            {
                TriangleMesh mesh = new TriangleMesh();
                for (int i = 0; i < Indices.Length; i += 3)
                {
                    CollisionShape.AddTriangle(Vertices[Indices[i]], Vertices[Indices[i + 1]], Vertices[Indices[i + 2]]);
                }
            }
        }

        public void LoadMesh(ZipArchive MeshArchive)
        {
            CollisionShape = new TriangleMesh();

            ZipArchiveEntry entry = MeshArchive.GetEntry("Descriptor.json");
            JsonConvert.PopulateObject(Misc.StreamToString(entry.Open()), this, Global.SerializerSettings);
            using (Stream str = MeshArchive.GetEntry("Materials.mtl").Open()) LoadMTL(Misc.StreamToString(str).Replace("\r", "").Split('\n'));
            entry = MeshArchive.GetEntry("Vertices.raw");
            Stream s = entry.Open();
            Vector3[] Vertices = new Vector3[entry.Length / 12];
            for (int i = 0; i < entry.Length / 12; i++)
            {
                Vertices[i] = new Vector3();
                byte[] input = new byte[12];
                s.Read(input, 0, 12);
                Vertices[i].X = BitConverter.ToSingle(input, 0);
                Vertices[i].Y = BitConverter.ToSingle(input, 4);
                Vertices[i].Z = BitConverter.ToSingle(input, 8);
                if (Vertices[i].Length > Size) Size = Vertices[i].Length;
            }
            VertexBuffer = AddVertexBuffer(Vertices);

            ElementArrays = new uint[MeshArchive.Entries.Where(z => new Regex(@"\d+.raw").IsMatch(z.Name)).Count()];
            GL.GenBuffers(ElementArrays.Length, ElementArrays);
            ElementArraySizes = new int[ElementArrays.Length];

            foreach (var item in MeshArchive.Entries)
            {
                s = item.Open();
                if (item.Name == "Vertices.raw" || item.Name == "Descriptor.json" || item.Name == "Materials.mtl") continue;
                else if (item.Name == "TextureCoordinates.raw")
                {
                    Vector2[] TextureCoordinates = new Vector2[item.Length / 8];
                    for (int i = 0; i < TextureCoordinates.Length; i++)
                    {
                        TextureCoordinates[i] = new Vector2();
                        byte[] input = new byte[8];
                        s.Read(input, 0, 8);
                        TextureCoordinates[i].X = BitConverter.ToSingle(input, 0);
                        TextureCoordinates[i].Y = BitConverter.ToSingle(input, 4);
                    }
                    TextureCoordinateBuffer = AddTextureCoordsBuffer(TextureCoordinates);
                }
                else if (item.Name == "Normals.raw")
                {
                    Vector3[] Normals = new Vector3[item.Length/12];
                    for (int i = 0; i < Normals.Length; i++)
                    {
                        Normals[i] = new Vector3();
                        byte[] input = new byte[12];
                        s.Read(input, 0, 12);
                        Normals[i].X = BitConverter.ToSingle(input, 0);
                        Normals[i].Y = BitConverter.ToSingle(input, 4);
                        Normals[i].Z = BitConverter.ToSingle(input, 8);
                    }
                    NormalBuffer = AddNormalsBuffer(Normals);
                }
                else
                {
                    int mat = Convert.ToInt32(item.Name.Split('.')[0]);
                    byte[] bytes = new byte[item.Length];
                    s.Read(bytes, 0, (int)item.Length);
                    int[] Indices = new int[item.Length / 4];
                    Buffer.BlockCopy(bytes, 0, Indices, 0, (int)item.Length);
                    for (int i = 0; i < Indices.Length; i+=3)
                    {
                        CollisionShape.AddTriangle(Vertices[Indices[i]], Vertices[Indices[i+1]], Vertices[Indices[i+2]]);
                    }
                    ElementArraySizes[mat] = FillIndexBuffer(Indices, ElementArrays[mat]);
                }
                s.Close();
            }
        }

        public void BuildFromFile(Stream objStream, Stream mtlStream, Resources.ProgressReport progressReporter)
        {
            ZipArchive archive = ZipFile.Open(Resources.ArchivePath, ZipArchiveMode.Update, Global.Encoding);
            ZipArchive meshArchive = new ZipArchive(archive.CreateEntry(Name).Open(), ZipArchiveMode.Update);
            progressReporter(this, 90, "Importing MTL file ...");

            string mtlFile = Misc.StreamToString(mtlStream);
            using (Stream s = meshArchive.CreateEntry("Materials.mtl").Open()) s.Write(Global.Encoding.GetBytes(mtlFile), 0, Global.Encoding.GetByteCount(mtlFile));
            LoadMTL(mtlFile.Replace("\r", "").Split('\n'));

            BuildOBJ(objStream, meshArchive, progressReporter);
            meshArchive.Dispose();
            archive.Dispose();
            progressReporter(this, 100, "Done ...");
        }

        public void BuildOBJ(Stream objStream, ZipArchive meshArchive, Resources.ProgressReport progressReporter)
        {
            List<Face> Faces = new List<Face>();
            List<Vector3> OriginalVertices = new List<Vector3>();
            List<Vector3> OriginalNormals = new List<Vector3>();
            List<Vector2> OriginalTextureCoordinates = new List<Vector2>();
            List<Vector3> SortedVertices = new List<Vector3>();
            List<Vector3> SortedNormals = new List<Vector3>();
            List<Vector2> SortedTextureCoordinates = new List<Vector2>();

            string[] file = Misc.StreamToString(objStream).Replace("\r", "").Split('\n');
            int currentMaterial = 0;
            for (int i = 0; i < file.Length; i++)
            {
                progressReporter(this, i*100 /file.Length, "Loading OBJ file ...");
                if (file[i] != "" && file[i][0] != '#')
                {
                    string[] line = file[i].Split(' ');
                    switch (line[0])
                    {
                        case "mtllib":
                            //LoadMTL(line[1]);
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
                            OriginalVertices.Add(new Vector3(Misc.toFloat(line[1]), Misc.toFloat(line[2]), Misc.toFloat(line[3])));
                            break;
                        case "f":
                            Face f = new Face();
                            f.mtl = currentMaterial;
                            for (int j = 1; j < line.Length; j++)
                            {
                                if (line[j].Contains('/'))
                                {
                                    string[] fac = line[j].Split('/');
                                    SortedVertices.Add(OriginalVertices[Misc.toInt(fac[0]) - 1]);
                                    SortedTextureCoordinates.Add(OriginalTextureCoordinates[Misc.toInt(fac[1]) - 1]);
                                    SortedNormals.Add(OriginalNormals[Misc.toInt(fac[2]) - 1]);
                                    int v = SortedVertices.Count - 1; // Index of last vertex
                                    Misc.Push<int>(v, ref f.vertices);
                                }
                            }
                            Faces.Add(f);
                            break;
                        case "vt":
                            OriginalTextureCoordinates.Add(new Vector2(Misc.toFloat(line[2]), Misc.toFloat(line[1])));
                            break;
                        case "vn":
                            OriginalNormals.Add(new Vector3(new Vector3(Misc.toFloat(line[1]), Misc.toFloat(line[2]), Misc.toFloat(line[3]))));
                            break;
                    }
                }
            }

            Faces.Sort(delegate (Face x, Face y) { return x.mtl.CompareTo(y.mtl); });
            currentMaterial = Faces[0].mtl;
            ElementArrays = new uint[Materials.Length];
            int[] currentElements = new int[0];
            int set = 0;

            Stream s = meshArchive.CreateEntry("Descriptor.json").Open();
            byte[] entryBytes = Global.Encoding.GetBytes(JsonConvert.SerializeObject(this, Global.SerializerSettings));
            s.Write(entryBytes, 0, entryBytes.Length);
            s.Close();

            for (int i = 0; i < Faces.Count; i++)
            {
                progressReporter(this, i*100/Faces.Count, "Exporting faces ...");
                Misc.Push<int>(Faces[i].vertices, ref currentElements);
                if (Faces.Count - 1 == i || currentMaterial != Faces[i + 1].mtl)
                {
                    using (Stream str = meshArchive.CreateEntry(set + ".raw").Open()) { 
                        for (int j = 0; j < currentElements.Length; j++)
                        {
                            byte[] output = BitConverter.GetBytes(currentElements[j]);
                            str.Write(output, 0, output.Length);
                        }
                    }
                    if (Faces.Count - 1 > i)
                    {
                        set++;
                        currentMaterial = Faces[i + 1].mtl;
                        currentElements = new int[0];
                    }
                }

            }

            using (Stream str = meshArchive.CreateEntry("Vertices.raw").Open())
            {
                foreach (var item in SortedVertices)
                {
                    progressReporter(this, 25, "Exporting vertices ...");
                    byte[] Output = new byte[4 * 3];
                    Buffer.BlockCopy(BitConverter.GetBytes(item.X), 0, Output, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(item.Y), 0, Output, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(item.Z), 0, Output, 8, 4);
                    str.Write(Output, 0, 12);
                }
            }
            using (Stream str = meshArchive.CreateEntry("Normals.raw").Open())
            {
                foreach (var item in SortedNormals)
                {
                    progressReporter(this, 50, "Exporting normals ...");
                    byte[] Output = new byte[4 * 3];
                    Buffer.BlockCopy(BitConverter.GetBytes(item.X), 0, Output, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(item.Y), 0, Output, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(item.Z), 0, Output, 8, 4);
                    str.Write(Output, 0, 12);
                }
            }
            using (Stream str = meshArchive.CreateEntry("TextureCoordinates.raw").Open())
            {
                foreach (var item in SortedTextureCoordinates)
                {
                    progressReporter(this, 75, "Exporting texture coordinates ...");
                    byte[] Output = new byte[4 * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(item.X), 0, Output, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(item.Y), 0, Output, 4, 4);
                    str.Write(Output, 0, 8);
                }
            }       
        }

        public void LoadMTL(string[] file)
        {
            Materials = new Material[0];
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
                    case "d":
                        currentMaterial.Brush.A = Misc.toFloat(line[1]);
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

        public static int AddNormalsBuffer(Vector3[] normals)
        {
            int NormalsBuffer = GL.GenBuffer();
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalsBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Length * 12), normals, BufferUsageHint.StaticDraw);
                long bufferSize;
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (normals.Length * Vector3.SizeInBytes != bufferSize)
                    throw new ApplicationException("Normals array not uploaded correctly");
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            return NormalsBuffer;
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
