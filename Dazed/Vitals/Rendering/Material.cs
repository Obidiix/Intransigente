using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    public class Material
    {
        private readonly Material? _parent;
        public bool IsVariant => _parent != null;

        private readonly int _uniqueIndex;
        private static int CurrentAssignedIndex = 0;

        private readonly Shader _materialShader;
        private readonly Dictionary<string, int> _keyToIndex = [];
        private readonly List<MaterialParameter> _params = [];
        //private readonly List<string> _keys = [];
        //private readonly List<object> _values = [];

        internal event Action<int>? OnParameterChange;

        private static readonly Dictionary<Shader, int> _indexes = [];
        internal static readonly List<Material> _instancedMaterials = [];

        private static int _usedMaterial;
        internal static int UsedMaterialIndex => _usedMaterial;

        public static Material GetMaterialInstance(Shader matShader, bool forceNewInstance = false)
        {
            if (!forceNewInstance)
                if (_indexes.TryGetValue(matShader, out var index))
                    return _instancedMaterials[index];
            Material? m = null;
            m = new Material(matShader);
            _indexes.Add(matShader, _instancedMaterials.Count);
            _instancedMaterials.Add(m);
            return m;
        }

        public static Material GetMaterialVariant(Material mat)
        {
            Material variant = new(mat);
            mat.OnParameterChange += variant.UpdateFromParent;
            return variant;
        }

        internal void UpdateFromParent(int index)
        {
            if (!IsVariant)
            {
                Scriptorium.Warn("A material base instance should not be updating from a parent, as it shouldn't have one");
                return;
            }
            _params[index].SetValue(_parent._params[index].Value);
        }

        internal void UseMaterial()
        {
            _usedMaterial = _uniqueIndex;
            UpdateShaderParameters();
        }

        public void SetFloat(string name, float value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }

        public void SetVector2(string name, Vector2 value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }

        public void SetVector3(string name, Vector3 value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }
        public void SetVector4(string name, Vector4 value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }
        public void SetVector4(string name, Color4 value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }
        public void SetMatrix4(string name, Matrix4 value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }
        public void SetBool(string name, bool value)
        {
            try
            {
                SetValue(name, value);
                OnParameterChange?.Invoke(ParameterLocation(name));
            }
            catch (Exception ex)
            {
                Scriptorium.LogException(ex);
            }
        }

        private void UpdateShaderParameters()
        {
            for(int i = 0; i < _params.Count; i++)
            {
                if (!_params[i].Changed)
                    continue;
                _materialShader.SetInterpretedValue(_params[i].Name, _params[i].Value);
                _params[i].UnflagChange();
            }
        }

        private int ParameterLocation(string name) => _keyToIndex[name];
        private void SetValue<T>(string name, T value) where T : struct => _params[ParameterLocation(name)].SetValue(value);

        private Material(Shader shader)
        {
            _uniqueIndex = CurrentAssignedIndex++;
            _materialShader = shader;
            GetUniformsFromShader();
        }

        private void GetUniformsFromShader()
        {
            string[] keys = _materialShader.GetUniformsKeys();
            //_keys.Capacity = keys.Length;
            //_values.Capacity = keys.Length;
            _params.Capacity = keys.Length;
            for(int i = 0; i < keys.Length; i++)
            {
                _params.Add(new(keys[i], default));
                _keyToIndex.Add(keys[i], i);
            }
        }

        internal int GetAttribLocation(string v) => _materialShader.GetAttribLocation(v);

        //For variants
        private Material(Material mat)
        {
            _uniqueIndex = CurrentAssignedIndex++;
            _parent = mat;
            _materialShader = mat._materialShader;
            for (int i = 0; i < mat._params.Count; i++)
            {
                _keyToIndex.Add(mat._params[i].Name, i);
                _params.Add(new(mat._params[i].Name, mat._params[i].Value));
            }
        }
    }
}
