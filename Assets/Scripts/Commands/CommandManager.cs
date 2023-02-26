using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager
{
    private Stack<ICommand> undoStack;
    private Stack<ICommand> redoStack;

    public CommandManager()
    {
        undoStack = new Stack<ICommand>();
        redoStack = new Stack<ICommand>();
    }

    public void AddCommand(ICommand command)
    {
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }
    }

    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }

    public bool CanRedo()
    {
        return redoStack.Count > 0;
    }
}