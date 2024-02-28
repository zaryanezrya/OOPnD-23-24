using System.Collections.Concurrent;

using Hwdtech;
public class ServerThread
{
    private Action _behaviour;
    private BlockingCollection<ICommand> _queue;
    private Thread _thread;
    private bool _stop = false;

    public ServerThread(BlockingCollection<ICommand> queue) {
        _queue = queue;

        _behaviour = () => {
            var cmd = _queue.Take();
            try {
                cmd.Execute();
            } catch(Exception e) {
                IoC.Resolve<ICommand>("ExceptionHandler.Handle", cmd, e).Execute();
            }
        };

        _thread = new Thread(() => {
            while(!_stop) {
                _behaviour();
            }
        });
    }

    internal void Stop() {
        _stop = !_stop;
    }

    internal Action GetBehaviour()
    {
        return _behaviour;
    }

    internal void SetBehaviour(Action newBehaviour) {
        _behaviour = newBehaviour;
    }

    public void Start() 
    {
        _thread.Start();
    }
} 

public class HardStopCommand: ICommand {
    private ServerThread _t;
    public HardStopCommand(ServerThread t) {
        _t = t;
    }

    public void Execute() {
        _t.Stop();
    }
}