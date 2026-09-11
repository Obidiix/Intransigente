using Dazed.Vitals.Paisaje;

namespace Dazed.Vitals;
public abstract class Entity : PaisajeElement
{
    public Transform? Transform { get; private set; }

    protected override void OnCreation()
    {
        Transform = new Transform();
        Phone.SubscribeToUpdate(Update);
        Phone.SubscribeToDrawCall(Draw);
    }

    protected override void OnDestruction()
    {
        Phone.UnSubscribeToUpdate(Update);
        Phone.UnSubscribeToDrawCall(Draw);
    }

    protected abstract void Update();
    protected abstract void Draw();
}