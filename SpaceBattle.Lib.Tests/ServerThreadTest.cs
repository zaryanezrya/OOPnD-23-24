namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<ICommand>(
            "Scopes.Current.Set", 
            IoC.Resolve<object>("Scopes.New", 
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", "Server.Commands.HardStop", (object[] args) =>
        {
            return new ActionCommand(() =>
            {
                new HardStopCommand((ServerThread)args[0]).Execute();
                new ActionCommand((Action)args[1]).Execute();
            });
        }).Execute();
    }

    [Fact]
    public void HardStopCommandShouldStopServer()
    {

        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange
        var q = new BlockingCollection<ICommand>(100);
        var t = new ServerThread(q);

        var hs = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, () => { mre.Set(); });


        q.Add(new ActionCommand(() => { }));
        q.Add(new ActionCommand(() => { Thread.Sleep(3000); }));
        q.Add(hs);
        q.Add(new ActionCommand(() => { }));

        // Act
        t.Start();
        mre.WaitOne();

        // Assert
        Assert.Single(q);
    }

    [Fact]
    public void AnExceptionSholdNotStopServerThread()
    {
        // AAA
        //Arrange
        var mre = new ManualResetEvent(false);
        // Create Queue
        var q = new BlockingCollection<ICommand>(100);
        // Create ST
        var t = new ServerThread(q);
        // Fill queue:
        //  command
        //  command with Excetion
        //  command 
        //  HardStop
        //  command
        var usualCommand = new Mock<ICommand>();
        usualCommand.Setup(m => m.Execute()).Verifiable();

        var exceptionCommand = new Mock<ICommand>();
        exceptionCommand.Setup(m=> m.Execute()).Throws<Exception>().Verifiable();

        var hardStopCommand = IoC.Resolve<ICommand>("Server.Commands.HardStop", t, ()=> {mre.Set();});

        q.Add(IoC.Resolve<ICommand>(
                "Scopes.Current.Set", 
                IoC.Resolve<object>("Scopes.New", 
                    IoC.Resolve<object>("Scopes.Root")
                ))
        );

        var exceptionHandler = new Mock<ICommand>();
        exceptionHandler.Setup(m=> m.Execute()).Verifiable();


        q.Add (
            IoC.Resolve<ICommand>("IoC.Register", 
                "ExceptionHandler.Handle", (object[] args) => exceptionHandler.Object
        ));
        q.Add(usualCommand.Object);
        q.Add(exceptionCommand.Object);
        q.Add(usualCommand.Object);
        q.Add(hardStopCommand);
        q.Add(usualCommand.Object);

        //Act
        t.Start();
        mre.WaitOne();

        //Assert()
        //check handler
        exceptionHandler.Verify(m=>m.Execute(), Times.Once());
        // commnd's call count
        usualCommand.Verify(m=>m.Execute(), Times.Exactly(2));
        Assert.Single(q);

    }

}

/// Registry of ServerThread's
/// create q
/// create ST
/// st.Start()
/// 

// root -> UnitTestScope 
//     \-> ServerThreadScope

// Dictionaty<int, ServerThread> threadRegistry;
// IoC.Resolve<ICommand>("IoC.Register", "ServerThread.Registry", () => threadRegistry).Execute()

// Serverthread.Registry.Get(id)
// Serverthread.Registry.Add(id, Serverthread)

// Serverthread.Queue.Registry.Get(id)
// Serverthread.Queue.Registry.Add(id, BlockingCollection)
