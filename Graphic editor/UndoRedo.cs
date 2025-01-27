﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Graphic_editor
{
    class UndoRedo
    {
        public Stack<ICommand> undoCommands = new Stack<ICommand>();
        public Stack<ICommand> redoCommands = new Stack<ICommand>();

        public Canvas cavas { get; set; }

        public void Undo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (undoCommands.Count != 0)
                {
                    ICommand command = undoCommands.Pop();
                    command.UnExecute();
                    redoCommands.Push(command);
                }
            }
        }

        public void Redo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (redoCommands.Count != 0)
                {
                    ICommand command = redoCommands.Pop();
                    command.Execute();
                    undoCommands.Push(command);
                }
            }
        }

        public void AddComand(ICommand command)
        {
            undoCommands.Push(command);
            redoCommands.Clear();
        }
    }
}
