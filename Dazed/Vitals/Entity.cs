using Dazed.Vitals.Paisaje;

namespace Dazed.Vitals;
public abstract class Entity : PaisajeDynamic
{
    protected override void OnCreation()
    {
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