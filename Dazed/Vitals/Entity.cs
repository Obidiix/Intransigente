using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals
{
    public abstract class Entity
    {
        public string Name => _uniqueName;
        private readonly string _uniqueName;

        public Transform Transform => _transform;
        private readonly Transform _transform;

        public Entity(string entityUniqueName, Transform? entityTransform = null)
        {
            _uniqueName = entityUniqueName;
            _transform = entityTransform;
            _transform ??= new();
        }

        internal abstract void LogicUpdate();
        internal abstract void DrawUpdate();

        internal virtual void OnDestroy() { }
    }
}
