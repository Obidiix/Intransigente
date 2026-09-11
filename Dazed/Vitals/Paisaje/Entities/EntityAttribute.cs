namespace Dazed.Vitals.Paisaje.Entities
{
    /// <summary>
    /// Exposes the entity to the level editor
    /// </summary>
    public class EntityAttribute(string elementIdentifier) : PaisajeElementAttribute(elementIdentifier, typeof(Entity));
}
