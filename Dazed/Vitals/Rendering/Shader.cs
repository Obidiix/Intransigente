using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Reflection.Metadata;

namespace Dazed.Vitals.Rendering
{
    public class Shader : IDisposable
    {
        int _handle;
        bool _isDisposed;

        private readonly Dictionary<string, int> _uniformLocations = new();
        public Shader(string shaderPath, bool pathIsAbsolute = false)
        {
            string vertexPath = shaderPath + PathUtils.VertexShaderFileExtension;
            string fragmentPath = shaderPath + PathUtils.FragmentShaderFileExtension;
            if (!pathIsAbsolute)
            {
                vertexPath = Path.Combine(PathUtils.ShadersDirectory, vertexPath);
                fragmentPath = Path.Combine(PathUtils.ShadersDirectory, fragmentPath);
            }
            if (!FilesExists(vertexPath, fragmentPath))
                return;
            InitializeWithPaths(vertexPath, fragmentPath);
        }

        public Shader(string vertPath, string fragPath, bool pathIsAbsolute = false)
        {
            if (!pathIsAbsolute)
            {
                vertPath = Path.Combine(PathUtils.ShadersDirectory, vertPath);
                fragPath = Path.Combine(PathUtils.ShadersDirectory, fragPath);
            }
            if (!FilesExists(vertPath, fragPath))
                return;
            InitializeWithPaths(vertPath, fragPath);
        }

        private void InitializeWithPaths(string vertPath, string fragPath)
        {
            string VertexShaderSource = File.ReadAllText(vertPath);
            string FragmentShaderSource = File.ReadAllText(fragPath);

            int VertexShader;
            int FragmentShader;

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            CompileShader(VertexShader);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            CompileShader(FragmentShader);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, VertexShader);
            GL.AttachShader(_handle, FragmentShader);

            LinkProgram(_handle);

            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);


            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(_handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(_handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }

            GL.DetachShader(_handle, VertexShader);
            GL.DetachShader(_handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        private static bool FilesExists(string vertPath, string fragPath)
        {
            if(!File.Exists(vertPath) || !File.Exists(fragPath))
            {
                Scriptorium.Warn("Path for vertex / fragment shader does not exists");
                Scriptorium.Warn($"Vertex path: {vertPath}");
                Scriptorium.Warn($"Fragment path: {fragPath}");
                return false;
            }

            return true;
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(_handle, attribName);

        public void Dispose()
        {
            if (!_isDisposed)
                GL.DeleteProgram(_handle);
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        internal void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(_handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        internal void SetVector3(string name, Vector3 vector3)
        {
            GL.UseProgram(_handle);
            GL.Uniform3(_uniformLocations[name], vector3);
        }

        ~Shader()
        {
            if (!_isDisposed)
                Scriptorium.Warn("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
}
