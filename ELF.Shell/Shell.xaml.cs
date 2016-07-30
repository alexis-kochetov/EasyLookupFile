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
using System.ComponentModel.Composition;
using System.Windows;
using ELF.ViewModels;

namespace ELF
{
  /// <summary>
  /// Interaction logic for Shell.xaml
  /// </summary>
  [Export("ELF.ShellContract", typeof(Window))]
  public partial class Shell : Window
  {
    public ShellViewModel ViewModel { get; private set; }

    protected override void OnClosed(EventArgs e)
    {
      ViewModel.Dispose();
    }

    [ImportingConstructor]
    public Shell([Import(AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]ShellViewModel viewModel)
    {
      ViewModel = viewModel;
      InitializeComponent();
    }
  }
}
