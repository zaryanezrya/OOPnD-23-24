namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
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
}

/// Registry of ServerThread's
/// create q
/// create ST
/// st.Start()