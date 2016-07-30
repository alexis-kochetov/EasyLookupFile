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
using System.IO;
using System.Reflection;

namespace ELF
{
  class Program
  {
    [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
    [STAThread]
    static void Main(string[] args)
    {
      var startupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var cachePath = Path.Combine(startupPath, "__cache");
      var pluginsPath = Path.Combine(startupPath, "Plugins");
      var configFile = Path.Combine(startupPath, "ELF.Shell.exe.config");
      var assemblyPath = Path.Combine(startupPath, "ELF.Shell.exe");

      var setup = new AppDomainSetup {
        ApplicationName = "ELF.Shell",
        ShadowCopyDirectories = string.Join(";", pluginsPath, startupPath),
        ShadowCopyFiles = "true",
        CachePath = cachePath,
        ConfigurationFile = configFile
      };

      var domain = AppDomain.CreateDomain("ELF.Shell", AppDomain.CurrentDomain.Evidence, setup);
      try {
        domain.ExecuteAssembly(assemblyPath);
        AppDomain.Unload(domain);
        Directory.Delete(cachePath, true);
      }
      catch
      {}
    }
  }
}
