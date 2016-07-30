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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using ELF.Core;
using ELF.ViewModels;

namespace ELF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    const string PluginsPath = "Plugins";
    const string WatchDogFilterPattern = "ELF.*Plugin.dll";
    private CompositionContainer container;
    private DirectoryCatalog pluginCatalog;
    private FileSystemWatcher watchDog;

    [Import]
    public ShellViewModel Model
    {
      get; set;
    }

    [Import("ELF.ShellContract")]
    public new Window MainWindow
    {
      get { return base.MainWindow; }
      set { base.MainWindow = value; }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      if (Compose())
        MainWindow.Show();
      else
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);

      if (container != null)
        container.Dispose();
    }

    private bool Compose()
    {
      var catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(SearchAgent).Assembly));
      if (Directory.Exists(PluginsPath)) {
        pluginCatalog = new DirectoryCatalog(PluginsPath);
        watchDog = new FileSystemWatcher(PluginsPath) {
          Filter = WatchDogFilterPattern, 
          EnableRaisingEvents = true, 
          NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.FileName
        };
        watchDog.Changed += WatchDogOnChanged;
        watchDog.Created += WatchDogOnChanged;
        watchDog.Deleted += WatchDogOnChanged;
        catalog.Catalogs.Add(pluginCatalog);
      }

      container = new CompositionContainer(catalog);
      var batch = new CompositionBatch();
      batch.AddPart(this);
      batch.AddExportedValue(container);

      try {
        container.Compose(batch);
      }
      catch (CompositionException compositionException) {
        MessageBox.Show(compositionException.ToString());
        Shutdown(1);
      }
      return true;
    }

    private void WatchDogOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(()=>pluginCatalog.Refresh()));
    }
  }
}
