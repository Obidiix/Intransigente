using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Reflection.Metadata;

namespace Dazed.Vitals.Rendering
{
    public class Shader : IDisposable
    {
        int _handle;
        bool _isDisposed;
        private static event Action ReloadAllShaders;

        private string _pathFrag, _pathVert;
        private readonly Dictionary<string, int> _uniformLocations = new();
        
        public static Shader GetShaderInstance(string path, bool pathIsAbsolute = false)
        {
            string vertexPath = path + PathUtils.VertexShaderFileExtension;
            string fragmentPath = path + PathUtils.FragmentShaderFileExtension;
            if (!pathIsAbsolute)
            {
                vertexPath = Path.Combine(PathUtils.ShadersDirectory, vertexPath);
                fragmentPath = Path.Combine(PathUtils.ShadersDirectory, fragmentPath);
            }
            return GetShaderInstanceCommon(vertexPath, fragmentPath);
        }

        public static Shader GetShaderInstance(string vertPath, string fragPath, bool pathIsAbsolute = false)
        {
            if (!pathIsAbsolute)
            {
                vertPath = Path.Combine(PathUtils.ShadersDirectory, vertPath);
                fragPath = Path.Combine(PathUtils.ShadersDirectory, fragPath);
            }
            return GetShaderInstanceCommon(vertPath, fragPath);
        }

        private static Shader GetShaderInstanceCommon(string vertexPath, string fragmentPath)
        {
            if (!FilesExists(vertexPath, fragmentPath))
            {
                Scriptorium.Error($"One of the following paths do not exist: <{vertexPath}> || <{fragmentPath}>");
                throw new Exception();
            }
            Shader s = new(vertexPath, fragmentPath);
            return s;
        }

        private Shader(string vertPath, string fragPath)
        {
            InitializeWithPaths(vertPath, fragPath);
            ReloadAllShaders += ReloadShader;
        }

        internal string[] GetUniformsKeys()
        {
            return _uniformLocations.Keys.ToArray();
        }

        public static void ReloadShaders() => ReloadAllShaders?.Invoke();

        private void ReloadShader() => InitializeWithPaths(_pathVert, _pathFrag);

        private void InitializeWithPaths(string vertPath, string fragPath)
        {
            //Assuming that if one is empty the other also is
            if (string.IsNullOrEmpty(_pathFrag))
            {
                _pathVert = vertPath;
                _pathFrag = fragPath;
            }
            string VertexShaderSource = File.ReadAllText(vertPath);
            string FragmentShaderSource = File.ReadAllText(fragPath);

            int VertexShader;
            int FragmentShader;

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            if (!CompileShader(VertexShader))
            {
                if (_handle == 0)
                    throw new Exception("Check console for more information");
                return;
            }

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            if (!CompileShader(FragmentShader))
            {
                if (_handle == 0)
                    throw new Exception("Check console for more information");
                return;
            }

            if (_handle != 0)
                GL.DeleteProgram(_handle);
            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, VertexShader);
            GL.AttachShader(_handle, FragmentShader);

            LinkProgram(_handle);

            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations.Clear();
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

        private static bool CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                Scriptorium.LogException(new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}"));
                return false;
            }
            return true;
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

        public void Use() => GL.UseProgram(_handle);

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(_handle, attribName);

        public void Dispose()
        {
            if (!_isDisposed)
                GL.DeleteProgram(_handle);
            _isDisposed = true;

            ReloadAllShaders -= ReloadShader;
            GC.SuppressFinalize(this);
        }

        internal void SetFloat(string name, float value)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], value);
        }

        /// <summary>
        /// Sets a matrix uniform without needing to reference it
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        internal void SetMatrix4NR(string name, Matrix4 data)
        {
            Use();
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        internal void SetMatrix4(string name, ref Matrix4 data)
        {
            Use();
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        internal void SetVector3(string name, Vector3 vector3)
        {
            Use();
            GL.Uniform3(_uniformLocations[name], vector3);
        }

        internal void SetVector2(string name, Vector2 vector2)
        {
            Use();
            GL.Uniform2(_uniformLocations[name], vector2);
        }

        internal void SetVector4(string name, Vector4 vector4)
        {
            Use();
            GL.Uniform4(_uniformLocations[name], vector4);
        }

        internal void SetVector4(string name, Color4 color4)
        {
            Use();
            GL.Uniform4(_uniformLocations[name], color4);
        }

        internal void SetBool(string name, bool value)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], value ? 1 : 0);
        }

        internal void SetInterpretedValue(string name, object value)
        {
            //Might need to replace this with a quick lookup dictionary that directly gives the required
            //function for the type
            Type type = value.GetType();
            if (type == typeof(float))
                SetFloat(name, (float)value);
            else if (type == typeof(Vector2))
                SetVector2(name, (Vector2)value);
            else if (type == typeof(Vector3))
                SetVector3(name, (Vector3)value);
            else if (type == typeof(Vector4))
                SetVector4(name, (Vector4)value);
            else if (type == typeof(Color4))
                SetVector4(name, (Color4)value);
            else if (type == typeof(Color4))
                SetVector4(name, (Color4)value);
            else if (type == typeof(Matrix4))
                SetMatrix4NR(name, (Matrix4)value);
        }

        ~Shader()
        {
            if (!_isDisposed)
                Scriptorium.Warn("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
}
