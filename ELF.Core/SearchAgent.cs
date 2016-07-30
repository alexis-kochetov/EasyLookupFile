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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using ELF.Contracts;

namespace ELF.Core
{
  public sealed class SearchAgent
  {
    public static IEnumerable<FileInfo> Search(SearchSettings settings, ISearchStrategy strategy, CancellationToken cancellationToken)
    {
      Contract.Requires(settings != null);
      Contract.Requires(strategy != null);

      var result = EnumerateFiles(settings.Path, settings.DirectoryOption)
        .AsParallel()
        .WithCancellation(cancellationToken)
        .Select(CreateFileInfoSafely)
        .Where(fi => fi != null);
      return strategy.IsEmpty()
        ? result
        : result.Where(strategy.Match);
    }

    private static FileInfo CreateFileInfoSafely(string fileName)
    {
      try {
        return fileName == null 
          ? null 
          : new FileInfo(fileName);
      }
      catch (SecurityException)
      {}
      catch (UnauthorizedAccessException)
      {}
      catch (PathTooLongException)
      {}
      catch (NotSupportedException)
      {}
      return null;
    }

    private static IEnumerable<string> EnumerateFiles(string directory, SearchOption searchOption)
    {
      var files = Enumerable.Empty<string>();
      try {
        files = Directory.EnumerateFiles(directory);
      }
      catch (UnauthorizedAccessException)
      {}
      foreach (var file in files)
        yield return file;
      if (searchOption == SearchOption.AllDirectories) {
        var nestedFiles = Enumerable.Empty<string>();
        try {
          nestedFiles = Directory.EnumerateDirectories(directory).SelectMany(dir => EnumerateFiles(dir, searchOption));
        }
        catch (UnauthorizedAccessException)
        {}
        foreach (var file in nestedFiles)
          yield return file;
      }
    }


    private SearchAgent() 
    {}
  }
}