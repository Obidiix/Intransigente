namespace Dazed.Vitals.Paisaje;

[AttributeUsage(AttributeTargets.Class)]
public class BrushElementAttribute(string elementIdentifier) : PaisajeElementAttribute(elementIdentifier, typeof(PaisajeDynamic))
{
}