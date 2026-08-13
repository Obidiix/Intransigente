using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Dazed.Vitals
{
    public class Transform
    {
        public Vector3 Right { get; internal set; }
        public Vector3 Up { get; internal set; }
        public Vector3 Forward { get; internal set; }

        public Matrix4x4 World { get; private set; }
        public Matrix4x4 Local { get; private set; }

        public Transform Parent
        {
            get => parent;
            set
            {
                if (parent != null)
                    parent.RemoveChild(this);
                parent = value;
                if (parent != null)
                    parent.AddChild(this);
                ReloadWorldMatrix();
            }
        }
        private Transform parent;
        private readonly List<Transform> children = new();

        private Vector3 localPosition;
        private Quaternion localRotation;
        private Vector3 scale = Vector3.One;

        public Vector3 LocalPosition
        {
            get => localPosition;
            set
            {
                localPosition = value;
                ReloadWorldMatrix();
            }
        }
        public Vector3 Position
        {
            get => parent == null ? localPosition : parent.Position + localPosition;
            set
            {
                if (parent == null)
                    localPosition = value;
                else
                    localPosition = value - Parent.Position;
                ReloadWorldMatrix();
            }
        }

        public Quaternion Rotation
        {
            get => parent == null ? localRotation : parent.Rotation * localRotation;
            set
            {
                if(parent == null)
                    localRotation = value;
                else
                    localRotation = Quaternion.Inverse(parent.Rotation) * value;
                localRotation = Quaternion.Normalize(localRotation);
                ReloadWorldMatrix();
            }
        }

        private void ReloadWorldMatrix()
        {
            Local = Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(localRotation) * Matrix4x4.CreateTranslation(localPosition);
            World = parent == null ? Local : Local * parent.World;
            Up = World.Up();
            Forward = World.Forward();
            Right = World.Right();

            if (children.Count == 0)
                return;
            foreach (Transform child in children)
                child.ReloadWorldMatrix();
        }

        public void RemoveChild(Transform child)
        {
            children.Remove(child);
        }
        public void AddChild(Transform child)
        {
            children.Add(child);
        }
    }
}
