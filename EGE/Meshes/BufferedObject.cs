using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace EGE
{
    class BufferedObject
    {
        int VertexBuffer, ElementArray, TextureCoordinateBuffer, ElementArraySize;

        public void Load(Vector3[] Vertices, int[] indicies, Vector2[] TextureCoordinates)
        {
            VertexBuffer = GL.GenBuffer();
            ElementArray = GL.GenBuffer();
            TextureCoordinateBuffer = GL.GenBuffer();
            ElementArraySize = indicies.Length;
            long bufferSize;
                
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArray);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicies.Length * sizeof(int)), indicies, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (indicies.Length * sizeof(int) != bufferSize)
                throw new ApplicationException("Element array not uploaded correctly");
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

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
        }

        public void Draw()
        {
            switch (Settings.CurrentDrawingMode)
            {
                case Settings.DrawingModes.Wireframe:
                    GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, TextureCoordinateBuffer);
                    // Set the Pointer to the current bound array describing how the data ia stored
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 8, IntPtr.Zero);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.TextureCoordArray);

                    // Vertex Array Buffer
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer); //Bind Array Buffer
                                                                           // Set the Pointer to the current bound array describing how the data ia stored
                    GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArray);
                    GL.DrawElements(PrimitiveType.LineStrip, ElementArraySize, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    // Restore the state
                    GL.PopClientAttrib();
                    break;
                case Settings.DrawingModes.Full:
                    break;
                case Settings.DrawingModes.Textured:
                    GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, TextureCoordinateBuffer);
                    // Set the Pointer to the current bound array describing how the data ia stored
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 8, IntPtr.Zero);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.TextureCoordArray);

                    // Vertex Array Buffer
                    GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer); //Bind Array Buffer
                                                                           // Set the Pointer to the current bound array describing how the data ia stored
                    GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementArray);
                    GL.DrawElements(PrimitiveType.Triangles, ElementArraySize, DrawElementsType.UnsignedInt, IntPtr.Zero);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    // Restore the state
                    GL.PopClientAttrib();
                    break;
                default:
                    break;
            }
            
        }
    }
}
