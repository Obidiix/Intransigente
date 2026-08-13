using Assimp;

namespace Dazed.Vitals.Rendering
{
    public class Bone
    {
        private Matrix4x4 _boneTransformation;
        private readonly VertexWeight[] _vertexWeights;
        public Bone(Assimp.Bone source)
        {
            _boneTransformation = source.OffsetMatrix;
            _vertexWeights = [.. source.VertexWeights];
        }
    }
}
