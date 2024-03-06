using Moq;
using Movable;
using Spacebattle;
using TechTalk.SpecFlow;
using vectr;

namespace SpaceBattle.Lib.Test;

[Binding]
public class MoveTest
{
    private readonly Mock<IMovable> _movable;
    private Action commandExecutionLambda;
    public MoveTest()
    {
        _movable = new Mock<IMovable>();
        commandExecutionLambda = () => { };
    }

    [When("происходит прямолинейное равномерное движение без деформации")]
    public void CalculatedTheMovement()
    {
        var mc = new MoveCommand(_movable.Object);
        commandExecutionLambda = () => mc.Execute();
    }

    [Given(@"космический корабль находится в точке пространства с координатами \((.*), (.*)\)")]
    public void GivenThePosition(int p0, int p1)
    {
        _movable.SetupGet(m => m.Location).Returns(new Vector(p0, p1));
    }

    [Given(@"имеет мгновенную скорость \((.*), (.*)\)")]
    public void GivenSpeed(int p0, int p1)
    {
        _movable.SetupGet(m => m.Velosity).Returns(new Vector(p0, p1));
    }

    [Obsolete]
    [Given("изменить положение в пространстве космического корабля невозможно")]
    public void NotSetPosition()
    {
        _movable.SetupSet(m => m.Location).Throws<Exception>();
    }

    [Given("космический корабль, положение в пространстве которого невозможно определить")]
    public void NotFindPosition()
    {
        _movable.SetupGet(m => m.Location).Throws<Exception>();
    }

    [Given("скорость корабля определить невозможно")]
    public void NotSetSpeed()
    {
        _movable.SetupGet(m => m.Velosity).Throws<Exception>();
    }

    [Then(@"космический корабль перемещается в точку пространства с координатами \((.*), (.*)\)")]
    public void MovingToAPoint(int p0, int p1)
    {
        commandExecutionLambda();
        _movable.VerifySet(m => m.Location = new Vector(p0, p1), Times.Once);
    }

    [Then(@"возникает ошибка Exception")]
    public void AppearExeption()
    {
        Assert.Throws<Exception>(() => commandExecutionLambda());
    }
}

