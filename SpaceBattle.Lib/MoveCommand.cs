﻿using Command;
using Movable;

namespace SpaceBattle;

public class MoveCommand : ICommand
{
    private readonly IMovable movable;

    public MoveCommand(IMovable movable)
    {
        this.movable = movable;
    }

    public void Execute()
    {
        movable.Location += movable.Velosity;
    }
}
