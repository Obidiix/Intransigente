using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal sealed class MaterialParameter
    {
        private bool _changed;
        private Type? _type;
        private readonly string _name;
        private object? _value;

        public bool Changed => _changed;
        public Type? Type => _type;
        public string Name => _name;
        public object? Value => _value;

        public MaterialParameter(string name, object value)
        {
            _name = name;
            if(value == null)
                return;
            _value = value;
            _type = value.GetType();
        }

        public void SetValue<T>(T value) where T : struct
        {
            if (_type == null)
                _type = value.GetType();
            if (value.GetType() != _type)
                throw new InvalidOperationException();
            if (value.Equals(_value))
                return;
            _changed = true;
            _value = value;
        }
        public void SetValue(object? value)
        {
            if (value == null)
                return;
            if (_type == null)
                _type = value.GetType();
            if (value.GetType() != _type)
                throw new InvalidOperationException();
            if (value == _value)
                return;
            _changed = true;
            _value = value;
        }
        public void UnflagChange() => _changed = false;
    }
}
