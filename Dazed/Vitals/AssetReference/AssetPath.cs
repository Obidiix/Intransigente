namespace Dazed.Vitals.AssetReference
{
    public class AssetPath : IExposedElement
    {
        public readonly string AssetRelativePath;

        public override int GetHashCode() => AssetRelativePath.GetHashCode();

        public AssetPath(string fieldName, Type classType)
        {
            string id = ExposedData.ConstructID(fieldName, classType);
            ExposedData.RetrieveAndCast(id, out AssetRelativePath);
        }
        public AssetPath(string assetRelativePath) => AssetRelativePath = assetRelativePath;

        public static implicit operator string(AssetPath path) => path.AssetRelativePath;
    }
}
