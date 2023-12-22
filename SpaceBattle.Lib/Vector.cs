namespace _Vector;
public class Vector
{
    private readonly int[] _values;
    public int Size => _values.Length;

    public Vector(params int[] values)
    {
        if (values.Length == 0)
        {
            throw new ArgumentException();
        }

        _values = values;
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
        if (v1.Size != v2.Size)
        {
            throw new System.ArgumentException();
        }

        return !(v1 == v2);
    }

    public static bool operator ==(Vector v1, Vector v2)
    {
        if (v1.Size != v2.Size)
        {
            throw new System.ArgumentException();
        }

        return v1.Equals(v2);
    }

    public static Vector operator +(Vector v1, Vector v2)
    {

        if (v1.Size != v2.Size)
        {
            throw new System.ArgumentException();
        }

        return new Vector(v1._values.Zip(v2._values, (a, b) => a + b).ToArray());
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector vector && _values.SequenceEqual(vector._values);
    }
    public override int GetHashCode()
    {
        return _values.GetHashCode();
    }
}