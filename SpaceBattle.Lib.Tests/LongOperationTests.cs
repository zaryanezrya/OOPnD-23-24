namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Moq;

using SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;

public class QueueTests
{
    [Fact]
    public void QueueTest1()
    {

        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();

        qMock.Setup(q => q.Take()).Returns(()=> qReal.Dequeue());

        var cmd = new Mock<Lib.ICommand>();
        qReal.Enqueue(cmd.Object);

        Assert.Equal(cmd.Object, qMock.Object.Take());
    }

    [Fact]
    public void QueueTest2()
    {
        var qBC = new BlockingCollection<Lib.ICommand>();

        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();

        qMock.Setup(q => q.Take()).Returns(()=> qReal.Dequeue());
        qMock.Setup(q => q.Add(It.IsAny<Lib.ICommand>())).Callback(
            (Lib.ICommand cmd) => qReal.Enqueue(cmd));

        var cmd = new Mock<Lib.ICommand>();
        //qReal.Enqueue(cmd.Object);

        qMock.Object.Add(cmd.Object);

        Assert.Equal(cmd.Object, qMock.Object.Take());
    }
}

public interface IUObject {
    object GetProperty(string key);
    void SetProperty(string key, object value);
}

public  class MovableAdapter: IMovable {
    private readonly IUObject _obj;
    public MovableAdapter(IUObject obj){
        _obj = obj;
    }
    public int[] Position {
        get => (int[])_obj.GetProperty("Position");
        set => _obj.SetProperty("Position", value);
    }
    public int[] Velocity => (int[])_obj.GetProperty("Velocity");
}



public class IoCExamplesTest
{
    public IoCExamplesTest() {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    [Fact]
    public void IoCExample1()
    {


        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "my_first_dependecy", 
            (object[] args) => {
                return "Hello!";
            }
        ).Execute();

        var str = IoC.Resolve<string>("my_first_dependecy");

        Assert.Equal(str, "Hello!");
    }

    [Fact]
    public void IoCExample2(){
        var qReal = new Queue<Lib.ICommand>();
        var qMock = new Mock<IQueue>();

        qMock.Setup(q => q.Take()).Returns(()=> qReal.Dequeue());
        qMock.Setup(q => q.Add(It.IsAny<Lib.ICommand>())).Callback(
            (Lib.ICommand cmd) => qReal.Enqueue(cmd));

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "Game.Queue", 
            (object[] args) => {
                return qMock.Object;
            }
        ).Execute();

        var cmd = new Mock<Lib.ICommand>();
        qReal.Enqueue(cmd.Object);

        var cmdFromQ = IoC.Resolve<IQueue>("Game.Queue").Take();

        Assert.Equal(cmd.Object, cmdFromQ);
    }
}


public 
class ActionCommand : Lib.ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
// new ActionCommand(() => {});

/*

IStartOrder {
    target: IUObject,
    name: "OperationName",
    initialValues {
        "Velocity": [1, 1]
    }
}

1. Начало ДО
StartLongOperation(IStartOrder order) {
    1. Resolve "Game.Object.SetValues"(initialValues)
    2. var operation = Resolve "Game.Operation.{order.name}"(order.target)
    3. target.SetProperty("order.name", operation)
    4. Resolve "Game.Queue.Add"(operation)
}


IStopOrder {
    target: IUObject,
    name: "OperationName",
    initialValues {
        "Velocity": [0, 0]
    }
}
2. Конец ДО

StopLongOperation(IStopOrder order) {
    1. Resolve "Game.Object.SetValues"(initialValues)
    2. order.target.SetProperty(order.name, EmptyCommand)
}

q = [  c c c c c c c c c ]

*/
