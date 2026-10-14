using OpenTK.Mathematics;

namespace Dazed.Vitals.Rendering;

public struct Color
{
    public static Color Red => new(1, 0, 0);
    public static Color Green => new(0, 1, 0);
    public static Color Blue => new(0, 0, 1);
    public static Color Yellow => new(1, 1, 0);
    public static Color LightCyan => new(0.878f, 1, 1);
    public static Color White => new(1, 1, 1);
    public static Color Black => new(0, 0, 0);
    
    public float R
    {
        get => _r;
        set => _r = value;
    }
    private float _r;

    public float G
    {
        get => _g;
        set => _g = value;
    }
    private float _g;

    public float B
    {
        get => _b;
        set => _b = value;
    }
    private float _b;

    public float A
    {
        get => _a;
        set => _a = value;
    }
    private float _a;

    public Color(float r, float g, float b)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = 1;
    }
    public Color(float r, float g, float b, float a)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }
    
    public static implicit operator Color4(Color c) =>  new Color4(c.R, c.G, c.B, c.A);
    public static implicit operator Color(Color4 c) =>  new Color(c.R, c.G, c.B, c.A);
    public static implicit operator System.Numerics.Vector3(Color c) => new(c.R, c.G, c.B);

    public Vector4 ToVector4()  => new Vector4(_r, _g, _b, _a);
    public Vector3 ToVector3() => new Vector3(R, G, B);
}