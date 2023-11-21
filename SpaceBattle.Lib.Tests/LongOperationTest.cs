using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ExampleTests
{

    [Fact]
    public void IQueueTestExample()
    {
        var qReal = new Queue<ICommand>();

        var qMock = new Mock<IQueue>();
        qMock.Setup(q => q.Take()).Returns(()=>qReal.Dequeue());

        var cmd = new Mock<ICommand>();

        qReal.Enqueue(cmd.Object);

        Assert.Equal(cmd.Object, qMock.Object.Take());
    }
}

public class ExampleIoCTests
{
    public ExampleIoCTests() {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    [Fact]
    public void IoCRegister()
    {
        var cmd = new Mock<ICommand>().Object;

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "my_first_dependency", 
            (object[] args)=> {
                return cmd;
            }
        ).Execute();

        var cmdFromIoC = IoC.Resolve<ICommand>("my_first_dependency");

        Assert.Equal(cmdFromIoC, cmd);        
    }

    [Fact]
    void IoCIQueue() {
        var qReal = new Queue<ICommand>();
        var qMock = new Mock<IQueue>();
        qMock.Setup(q => q.Take()).Returns(()=>qReal.Dequeue());
        qMock.Setup(q => q.Add(It.IsAny<ICommand>())).Callback(
            (ICommand cmd) => {
                qReal.Enqueue(cmd);
            }
        );

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register", 
            "Game.Queue", 
            (object[] args)=> {
                return qMock.Object;
            }
        ).Execute();

        var cmd = new Mock<ICommand>();

        IoC.Resolve<IQueue>("Game.Queue").Add(cmd.Object);
        
        var cmdFromIoC = IoC.Resolve<IQueue>("Game.Queue").Take();

        Assert.Equal(cmd.Object, cmdFromIoC);
    }

    [Fact]
    void ActionCommandTests() {
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Game.Commands.EmptyCommand",
            (object[] args) => {
                return new ActionCommand(()=>{});
            }
        );
    }
}

internal class ActionCommand : ICommand
{
    private Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
        _action();
    }
}

/* 2 лаба
1.
    Приказ:
        1. Таргет
        2. Свойства {"имя свойства": "значение", ...}

    StartCommand(Приказ) {
        1. Присвоение свойств.
        2. cmd = Конструивание длительной операции (IoC.Resolve<ICommand>("Game.Operation.Move"))
        3. Закинуть длительную операцию в таргет
        4. cmd закинуть в очередь.
    }

2.
    StopCommand(Приказ)

*/
