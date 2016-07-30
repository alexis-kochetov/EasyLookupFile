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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using ELF.Commands;
using ELF.Contracts;
using ELF.Core;
using ELF.Views;

namespace ELF.ViewModels
{
  [Export]
  public sealed class ShellViewModel : ViewModel, IDataErrorInfo, IDisposable
  {
    private bool isSearchRunning;
    [Import]
    private Lazy<BasicView> corePlugin;
    [ImportMany("ELF.PluginContract", AllowRecomposition = true)] 
    public ObservableCollection<Lazy<IPlugin, IPluginMetaData>> Plugins { get; private set;}
    private Lazy<IPlugin, IPluginMetaData> selectedPlugin;
    private RelayCommand searchCommand;
    private RelayCommand stopCommand;
    private IDisposable searchHandle;
    private int currentProgress;
    private string path;
    private bool processRecursively;
    private string status;

    public const int MinimumProgress = 0;
    public const int MaximumProgress = 100;

    public string Path
    {
      get { return path; }
      set {
        if (path == value)
          return;
        path = value;
        OnPropertyChanged("Path");
      }
    }

    public bool ProcessRecursively
    {
      get { return processRecursively; }
      set {
        if (processRecursively == value)
          return;
        processRecursively = value;
        OnPropertyChanged("ProcessRecursively");
      }
    }

    public bool IsSearchRunning
    {
      get { return isSearchRunning; }
      private set {
        if (isSearchRunning == value) 
          return;
        isSearchRunning = value;
        OnPropertyChanged("IsSearchRunning");
      }
    }

    public int CurrentProgress
    {
      get { return currentProgress;  }
      private set
      {
        if (currentProgress == value)
          return;
        if (value >= MaximumProgress)
          value = MinimumProgress;
        currentProgress = value;
        OnPropertyChanged("CurrentProgress");
      }
    }

    public string Status
    {
      get { return status; }
      private set
      {
        if (status == value)
          return;
        status = value;
        OnPropertyChanged("Status");
      }
    }

    public BasicView BasicPlugin
    {
      get
      {
        if (selectedPlugin == null)
          return null;
        var searchParameters = SelectedPlugin.Value.Parameters;
        var basicView = corePlugin.Value;
        searchParameters.Extensions = SelectedPlugin.Metadata.Extensions;
        basicView.Parameters = null;
        basicView.Parameters = searchParameters;
        if (selectedPlugin.Metadata.Type == "Core")
          return null;
        return basicView;
      }
    }

    public Lazy<IPlugin, IPluginMetaData> SelectedPlugin
    {
      get { return selectedPlugin; }
      set {
        if (selectedPlugin == value)
          return;
        selectedPlugin = null;
        OnPropertyChanged("BasicPlugin");
        OnPropertyChanged("SelectedPlugin");
        selectedPlugin = value;
        OnPropertyChanged("SelectedPlugin");
        OnPropertyChanged("BasicPlugin");
      }
    }

    public ObservableCollection<FileInfo> FoundFiles { get; private set; }

    public ICommand SearchCommand
    {
      get { return searchCommand ?? (searchCommand = new RelayCommand(Search, CanSearch)); }
    }

    public ICommand StopCommand
    {
      get { return stopCommand ?? (stopCommand = new RelayCommand(StopSearch, CanStopSearch)); }
    }

    private bool CanStopSearch(object parameter)
    {
      return IsSearchRunning;
    }

    private void StopSearch(object parameter)
    {
      OnCompleted();
    }

    private bool CanSearch(object parameter)
    {
      return !IsSearchRunning && SelectedPlugin != null && string.IsNullOrEmpty(Error);
    }

    private void Search(object sender)
    {
      IsSearchRunning = true;
      FoundFiles.Clear();
      CurrentProgress = 0;
      var searchSettings = new SearchSettings(Path, ProcessRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
      var plugin = selectedPlugin.Value;
      var factory = plugin.StrategyFactory;
      var parameters = plugin.Parameters;
      var strategy = factory.CreateStrategy(parameters);
      if (SelectedPlugin.Metadata.Type != "Core")
        strategy = corePlugin.Value.StrategyFactory.CreateStrategy(parameters).Combine(strategy);
      var cts = new CancellationTokenSource();
      var lease = SearchAgent.Search(searchSettings, strategy, cts.Token)
        .ToObservable()
        .Buffer(TimeSpan.FromSeconds(1), 40)
        .ObserveOn(SynchronizationContext.Current)
        .Subscribe(OnNextChunk, OnError, OnCompleted);
      searchHandle = new CompositeDisposable(new CancellationDisposable(cts), lease);
    }

    private void OnNextChunk(IList<FileInfo> chunk)
    {
      foreach (var fileInfo in chunk)
        FoundFiles.Add(fileInfo);
      CurrentProgress++;
//      var count = FoundFiles.Count;
//      if (count >= 9000) // It was a joke :)
//        StopSearch(null);
      ReportStatus();
    }

    private void OnError(Exception ex)
    {
      CurrentProgress = 0;
      IsSearchRunning = false;
      searchHandle.Dispose();
      ReportStatus(ex);
    }

    private void OnCompleted()
    {
      Reset();
      ReportStatus();
    }

    private void Reset()
    {
      CurrentProgress = 0;
      IsSearchRunning = false;
      if (searchHandle != null)
        searchHandle.Dispose();
    }

    private void ReportStatus(Exception exception = null)
    {
      if (exception != null) {
        var aggregate = exception as AggregateException;
        Status = string.Format("Error: {0}", aggregate != null ? aggregate.InnerExceptions[0].Message : exception.Message);
        return;
      }
      var count = FoundFiles.Count;
//      var result = count >= 9000
//        ? "There are over 9000 files found."
//        : string.Format("{0} file{1} found.", count, count > 0 ? "s" : "");
      var result = string.Format("{0} file{1} found.", count, count > 0 ? "s" : "");
      Status = IsSearchRunning 
        ? result 
        : result + " Search completed.";
    }

    public string this[string columnName]
    {
      get
      {
        var result = string.Empty;
        switch (columnName) {
          case "Path":
            result = ValidatePath();
            break;
        }
        return result;
      }
    }

    private string ValidatePath()
    {
      if (string.IsNullOrWhiteSpace(Path))
        return "Directory path needs to be entered";
      if (!Directory.Exists(Path))
        return "There is no such directory.";
      return string.Empty;
    }

    public string Error
    {
      get { return ValidatePath(); }
    }


    public void Dispose()
    {
      if (searchHandle != null)
        searchHandle.Dispose();
    }

    public ShellViewModel()
    {
      IsSearchRunning = false;
      Path = Environment.CurrentDirectory;
      FoundFiles = new ObservableCollection<FileInfo>();
    }
  }
}