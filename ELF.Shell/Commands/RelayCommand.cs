// Copyright (C) 2011 Alexis Kochetov.
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// Created by: Alexis Kochetov
// Created:    2011.03.15
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace ELF.Commands
{
  public class RelayCommand : ICommand
  {
    private readonly Action<object> execute;
    private readonly Func<object,bool> canExecute;

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return canExecute == null ? true : canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      execute(parameter);
    }

    public RelayCommand(Action<object> execute, Func<object,bool> canExecute = null)
    {
      Contract.Requires(execute != null);

      this.execute = execute;
      this.canExecute = canExecute;
    }

  }
}