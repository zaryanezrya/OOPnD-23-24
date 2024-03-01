using System.Collections.Concurrent;
using Hwdtech;

public class ServerThread{
    private BlockingCollection<ICommand> _q;
    private bool _stop = false;
    private Action _behaviour;
    private Thread _t;

    public ServerThread(BlockingCollection<ICommand> queue){
        _q = queue;
        
        _behaviour = () => {
            var cmd = _q.Take();
            try{
                cmd.Execute();
            }
            catch (Exception e){
                IoC.Resolve<ICommand>("Exception.Handler", e, cmd).Execute();
            }
        };
        
        _t = new Thread(
            () => {
                while(!_stop){
                    _behaviour();
                }
            }
        );
    }

    public void Start(){
        _t.Start();
    }

    internal void Stop(){
        _stop = true;
    } 

    internal void UpdateBehaviour(Action new_behaviour){
        _behaviour = new_behaviour;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(Thread))
        {
            return _t == (Thread)obj;
        }

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return false;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        // throw new System.NotImplementedException();
        return base.GetHashCode();
    }
}

public class HardStopCommand : ICommand
{
    private ServerThread _t;
    public HardStopCommand(ServerThread t) {
        _t = t;
    }
    public void Execute()
    {
        if (_t.Equals(Thread.CurrentThread)) {
            _t.Stop();
        } else {
            throw new Exception("WRONG!");
        }
    }
}