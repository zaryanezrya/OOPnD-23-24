using Hwdtech;

namespace SpaceBattle.Lib;

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
        _action();
    }
}

public class SimpleMacroCommand : ICommand
{
    private readonly IEnumerable<ICommand> _commands;
    public SimpleMacroCommand(IEnumerable<ICommand> commands) {
        _commands = commands;
    }
    public void Execute()
    {
        foreach (var cmd in _commands) {
            cmd.Execute();

        }
    }
}