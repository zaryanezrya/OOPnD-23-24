using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ServerThreadTest
{
    public ServerThreadTest() {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", 
                IoC.Resolve<object>("Scopes.Root")            
            )
        ).Execute();

        IoC.Resolve<ICommand>("IoC.Register", 
            "Hard Stop The Thread", 
            (object[] args) => {
                return new ActionCommand(()=>{
                    new HardStopCommand((ServerThread)args[0]).Execute();
                    new ActionCommand((Action)args[1]).Execute();
                });
            }
        ).Execute();
    }

    [Fact]
    public void HardStopCommandShouldStopServerThread() {
        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange
        // new Queue
        var q = new BlockingCollection<ICommand>(10);
        // new St
        var st = new ServerThread(q);

        var cmd = new Mock<ICommand>();
        cmd.Setup(m=>m.Execute()).Verifiable();

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", st, ()=>{mre.Set();});

        // Fill Queue:
        //   1. cmd
        //   2. hs 
        //   3. cmd
        q.Add(cmd.Object);
        q.Add(hs);
        q.Add(cmd.Object);


        // Act
        // st.Start
        st.Start();
        // Thread.Sleep(10000);
        mre.WaitOne();

        // Assert
        // q.count == 1
        Assert.Single(q);
        // cmd.Verify(m=>m.Execute(), Times.Once)
        cmd.Verify(m=>m.Execute(), Times.Once());
    }

    [Fact]
    public void ExceptionCommandShouldNotStopServerThread() {
        // AAA
        // Arrange

        

        var mre = new ManualResetEvent(false);
        // AAA
        // Arrange
        // new Queue
        var q = new BlockingCollection<ICommand>(10);
        // new St
        var st = new ServerThread(q);

        var cmd = new Mock<ICommand>();
        cmd.Setup(m=>m.Execute()).Verifiable();

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", st, ()=>{mre.Set();});

        var handleCommand = new Mock<ICommand>();
        handleCommand.Setup(m=>m.Execute()).Verifiable();
        // IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler", (object[] args)=> handleCommand.Object).Execute();


        // Fill Queue:
        // [cmd, cmdE, cmd, HS, cmd]

        var cmdE = new Mock<ICommand>();
        cmdE.Setup(m=>m.Execute()).Throws<Exception>().Verifiable();

        q.Add(
            IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", 
                    IoC.Resolve<object>("Scopes.Root")            
                )
            )
        );
        q.Add(IoC.Resolve<ICommand>("IoC.Register", "Exception.Handler", (object[] args)=> handleCommand.Object));
        q.Add(cmd.Object);
        q.Add(cmdE.Object);
        q.Add(cmd.Object);
        q.Add(hs);
        q.Add(cmd.Object);


        // Act
        // st.Start
        st.Start();
        // Thread.Sleep(10000);
        mre.WaitOne();

        // Assert
        // q.count == 1
        Assert.Single(q);
        // cmd.Verify(m=>m.Execute(), Times.Once)
        cmd.Verify(m=>m.Execute(), Times.Exactly(2));
        handleCommand.Verify(m=>m.Execute(), Times.Once());

    }
}

// Root -> T1Scope
//      -> T2Scope
//      -> T3Scope
