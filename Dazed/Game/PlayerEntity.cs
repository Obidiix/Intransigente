using Dazed.Vitals;
using Dazed.Vitals.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Game
{
    [Entity(PlayerEntityName)]
    public class PlayerEntity : Entity
    {
        public const string PlayerEntityName = "player";

        public PlayerEntity(Transform? entityTransform = null) : base(PlayerEntityName, entityTransform)
        {
        }

        internal override void DrawUpdate()
        {
            Scriptorium.Message("Player update");
        }

        internal override void LogicUpdate()
        {
            Scriptorium.Message("Player draw");
        }
    }
}
