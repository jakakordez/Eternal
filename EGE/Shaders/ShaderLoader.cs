using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace EGE.Shaders
{
    class ShaderLoader
    {
        public static int LoadShaders(string VertexShaderSource, string FragmentShaderSource)
        {
            // Create the shaders
            int VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            int FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
            // Read the Vertex Shader code from the file
	        int[] Result = new int[1];
            string InfoLog;
            string VertexShaderCode = (string)VertexShaderSource.Clone();
            string FragmentShaderCode = (string)FragmentShaderSource.Clone();

            // Compile Vertex Shader
            GL.ShaderSource(VertexShaderID, VertexShaderCode);
            GL.CompileShader(VertexShaderID);

            // Check Vertex Shader
            GL.GetShader(VertexShaderID, ShaderParameter.CompileStatus, Result);
            InfoLog = GL.GetShaderInfoLog(VertexShaderID);
            if (Result[0] == 0){
                System.Diagnostics.Debugger.Log(0, "Vertex shader compiler", InfoLog);
                System.Diagnostics.Debugger.Break();
            }

            // Compile Fragment Shader
            GL.ShaderSource(FragmentShaderID, FragmentShaderCode);
            GL.CompileShader(FragmentShaderID);

            // Check Fragment Shader
            GL.GetShader(FragmentShaderID, ShaderParameter.CompileStatus, Result);
            InfoLog = GL.GetShaderInfoLog(FragmentShaderID);
            GL.GetShaderInfoLog(FragmentShaderID, out InfoLog);
            if (Result[0] == 0)
            {
                System.Diagnostics.Debugger.Log(0, "Fragment shader compiler", InfoLog);
                System.Diagnostics.Debugger.Break();
            }

            // Link the program
            int ProgramID = GL.CreateProgram();
            GL.AttachShader(ProgramID, VertexShaderID);
            GL.AttachShader(ProgramID, FragmentShaderID);
            GL.LinkProgram(ProgramID);

            // Check the program
            GL.GetProgram(ProgramID, GetProgramParameterName.LinkStatus, Result);
            InfoLog = GL.GetProgramInfoLog(ProgramID);
            if (Result[0] == 0)
            {
                System.Diagnostics.Debugger.Log(0, "Shader linker", InfoLog);
                System.Diagnostics.Debugger.Break();
            }
            GL.DetachShader(ProgramID, VertexShaderID);
            GL.DetachShader(ProgramID, FragmentShaderID);

            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(FragmentShaderID);
	        return ProgramID;
        }
    }
}
