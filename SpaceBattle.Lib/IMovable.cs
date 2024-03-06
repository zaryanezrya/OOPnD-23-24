namespace Movable;
using vectr;

public interface IMovable
{
    Vector Location { get; set; }
    Vector Velosity { get; }
}
