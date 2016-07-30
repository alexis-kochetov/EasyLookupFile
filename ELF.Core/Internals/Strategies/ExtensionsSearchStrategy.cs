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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using ELF.Contracts;

namespace ELF.Core.Internals.Strategies
{
  internal sealed class ExtensionsSearchStrategy : ISearchStrategy
  {
    private readonly string[] extensions;

    public string[] Extensions
    {
      get {
        Contract.Ensures(Contract.Result<string[]>() != null);
        Contract.Ensures(Contract.Result<string[]>().Length == extensions.Length);

        var result = new string[extensions.Length];
        extensions.CopyTo(result, 0);
        return result;
      }
    }

    public bool Match(FileInfo fileInfo)
    {
      Contract.Assert(extensions != null);
      return extensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(extensions != null);
      Contract.Invariant(extensions.Length > 0);
    }

    public ExtensionsSearchStrategy(string[] extensions)
    {
      Contract.Requires(extensions != null);
      Contract.Requires(extensions.Length > 0);

      this.extensions = extensions;
    }
  }
}