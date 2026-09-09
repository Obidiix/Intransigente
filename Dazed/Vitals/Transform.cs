using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals
{
    public class Transform
    {
        public Vector3 Right { get; internal set; }
        public Vector3 Up { get; internal set; }
        public Vector3 Forward { get; internal set; }

        public Matrix4 World { get; private set; }
        public Matrix4 Local { get; private set; }

        public Transform? Parent
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
        private Transform? parent;
        private readonly List<Transform> children = new();

        private Vector3 _localPosition;
        private Quaternion _localRotation;
        private Vector3 _scale = Vector3.One;

        public event Action<Matrix4> TransformationChange;

        public Vector3 LocalPosition
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                UpdateTranslation();
            }
        }
        public Vector3 Position
        {
            get => parent == null ? _localPosition : parent.Position + _localPosition;
            set
            {
                if (parent == null)
                    _localPosition = value;
                else
                    _localPosition = value - parent.Position;
                UpdateTranslation();
            }
        }

        public Quaternion Rotation
        {
            get => parent == null ? _localRotation : parent.Rotation * _localRotation;
            set
            {
                if(parent == null)
                    _localRotation = value;
                else
                    _localRotation = Quaternion.Invert(parent.Rotation) * value;
                _localRotation = Quaternion.Normalize(_localRotation);
                UpdateRotation();
            }
        }


        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateScale();
            }
        }
        private Matrix4 _scaleM, _rotationM, _translationM;

        private void ReloadWorldMatrix()
        {
            Local = _scaleM * _rotationM * _translationM;
            World = parent == null ? Local : Local * parent.World;
            Up = World.Up();
            Forward = World.Forward();
            Right = World.Right();

            TransformationChange?.Invoke(World);
            if (children.Count == 0)
                return;
            foreach (Transform child in children)
                child.ReloadWorldMatrix();
        }
        /// <summary>
        /// Moves on the global axis
        /// </summary>
        /// <param name="movement"></param>
        public void MoveGlobal(Vector3 movement)
        {
            movement = Vector3.Normalize(movement);
            MoveInternal(movement);
        }
        /// <summary>
        /// Moves on the local axis
        /// </summary>
        /// <param name="movement"></param>
        public void MoveLocal(Vector3 movement)
        {
            Vector3 transformedMovement =
                movement.X * Right +
                movement.Y * Up +
                movement.Z * Forward;
            transformedMovement = Vector3.Normalize(transformedMovement);
            MoveInternal(transformedMovement);
        }

        private void MoveInternal(Vector3 move)
        {
            _localPosition += move;
            UpdateTranslation();
        }

        private void UpdateTranslation()
        {
            _translationM = Matrix4.CreateTranslation(_localPosition);
            ReloadWorldMatrix();
        }
        private void UpdateRotation()
        {
            _rotationM = Matrix4.CreateFromQuaternion(_localRotation);
            ReloadWorldMatrix();
        }
        private void UpdateScale()
        {
            _scaleM = Matrix4.CreateScale(_scale);
            ReloadWorldMatrix();
        }

        public void TranslateX(float amount, bool localAxis = true)
        {
            Vector3 xTranslation = localAxis ? amount * Right : amount * Vector3.UnitX;
            _localPosition += xTranslation;
            UpdateTranslation();
        }
        public void TranslateY(float amount, bool localAxis = true)
        {
            Vector3 yTranslation = localAxis ? amount * Up : amount * Vector3.UnitY;
            _localPosition += yTranslation;
            UpdateTranslation();
        }
        public void TranslateZ(float amount, bool localAxis = true)
        {
            Vector3 zTranslation = localAxis ? amount * Forward : amount * Vector3.UnitZ;
            _localPosition += zTranslation;
            UpdateTranslation();
        }

        public void RemoveChild(Transform child)
        {
            children.Remove(child);
        }
        public void AddChild(Transform child)
        {
            children.Add(child);
        }

        public Transform()
        {
            _translationM = Matrix4.CreateTranslation(_localPosition);
            _localRotation = Quaternion.Identity;
            _rotationM = Matrix4.CreateFromQuaternion(_localRotation);
            _scaleM = Matrix4.CreateScale(_scale);
            World = Matrix4.Identity;
            ReloadWorldMatrix();
        }
    }
}
