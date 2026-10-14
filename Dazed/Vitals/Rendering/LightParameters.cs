using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Rendering
{
    internal readonly struct LightParameters
    {
        private readonly Dictionary<string, byte> _parameterToIdentifier; 
        private readonly string[] FullIdentifiers;
        internal LightParameters(string[] fullIdentifiers, string[] parameters)
        {
            FullIdentifiers = fullIdentifiers;
            _parameterToIdentifier = new(fullIdentifiers.Length);
            for (byte i = 0; i < fullIdentifiers.Length; i++)
                _parameterToIdentifier.Add(parameters[i], i);
        }
        internal string GetIdentification(string parameter) => FullIdentifiers[_parameterToIdentifier[parameter]]; 
    }
}
