using Dazed.Vitals;
using Dazed.Vitals.Paisaje.Entities;

namespace Intransigente
{
    [Entity(PlayerEntityName)]
    public class PlayerEntity : Entity
    {
        public const string PlayerEntityName = "player";

        protected override void Draw()
        {
            Scriptorium.Message("Player update");
        }

        protected override void Update()
        {
            Scriptorium.Message("Player draw");
        }
    }
}
