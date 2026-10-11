using Assimp;
using Dazed.Vitals.Exceptions;

namespace Dazed.Vitals.Content
{
    public static class Chest
    {
        internal static AssimpContext AssimpContext { get; private set; } = null!;

        static Chest()
        {
            AssimpContext = new();
        }
    }
}
