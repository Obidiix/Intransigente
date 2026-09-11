namespace Dazed.Vitals.Content;

public readonly struct IdentifiedAsset
{
    public readonly string AbsolutePath;
    public readonly string RelativePath;
    public readonly string AssetID;

    public IdentifiedAsset(string assetID)
    {
        AssetID = assetID;
        RelativePath = Chest.GetRelativePath(AssetID);
        AbsolutePath = PathUtils.BaseDirectoryCombine(RelativePath);
    }
    
    public override string ToString() => $"Identified Asset:" +
                                         $"ID = {AssetID}" +
                                         $"Path = {RelativePath}";
}