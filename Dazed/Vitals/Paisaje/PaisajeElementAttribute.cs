using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Paisaje
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PaisajeElementAttribute : Attribute
    {
        public readonly string ElementName;
        public readonly Type ElementType;

        public PaisajeElementAttribute(string elementName, Type elementType)
        {
            ElementName = elementName;
            ElementType = elementType;
        }
    }
}
