using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Paisaje
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PaisajeElementAttribute(string elementIdentifier, Type elementType) : Attribute
    {
        public readonly string ElementIdentifier = elementIdentifier;
        public readonly Type ElementType = elementType;
    }
}
