using Dazed.Vitals.Paisaje;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Entities
{
    /// <summary>
    /// Exposes the entity to the level editor
    /// </summary>
    public class EntityAttribute : PaisajeElementAttribute
    {
        public EntityAttribute(string elementName) : base(elementName, typeof(Entity))
        {
        }
    }
}
