using Dazed.Vitals;
using System;
using System.Collections.Generic;
using System.Text;
using Dazed.Vitals.Paisaje.Entities;

namespace Dazed.Game
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
