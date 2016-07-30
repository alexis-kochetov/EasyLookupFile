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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace ELF.Contracts
{
  public sealed class CompositeSearchStrategy : ISearchStrategy
  {
    private readonly ISearchStrategy[] strategies;

    public ISearchStrategy[] Strategies
    {
      get
      {
        Contract.Ensures(Contract.Result<ISearchStrategy[]>() != null);
        Contract.Ensures(Contract.Result<ISearchStrategy[]>().Length == strategies.Length);
        
        var result = new ISearchStrategy[strategies.Length];
        strategies.CopyTo(result, 0);
        return result;
      }
    }

    public bool Match(FileInfo fileInfo)
    {
      return strategies
        .Aggregate(true, (r, s) => r && s.Match(fileInfo));
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(strategies != null);
      Contract.Invariant(strategies.Length > 0);
    }

    public CompositeSearchStrategy(IEnumerable<ISearchStrategy> strategies)
    {
      Contract.Requires(strategies != null);
      Contract.Requires(strategies.ToArray().Length > 0);
      Contract.Ensures(this.strategies != null);
      Contract.Ensures(this.strategies.Length > 0);

      this.strategies = strategies.ToArray();
    }
  }
}