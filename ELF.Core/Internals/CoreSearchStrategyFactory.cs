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
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using ELF.Contracts.Internals;
using ELF.Core.Internals.Strategies;

namespace ELF.Contracts
{
  [PartCreationPolicy(CreationPolicy.Shared)]
  [Export("ELF.Core.CoreSearchStrategyFactory", typeof(ISearchStrategyFactory))]
  internal sealed class CoreSearchStrategyFactory : ISearchStrategyFactory
  {
    [ContractVerification(false)]
    public ISearchStrategy CreateStrategy(SearchParameters parameters)
    {
      var strategies = new List<ISearchStrategy>();
      if (parameters.Extensions.Length > 0)
        strategies.Add(new ExtensionsSearchStrategy(parameters.Extensions));
      if (parameters.MinSize.HasValue)
        strategies.Add(new FileSizeSearchStrategy(SearchCondition.GreaterThanOrEqual, parameters.MinSize.Value));
      if (parameters.MaxSize.HasValue)
        strategies.Add(new FileSizeSearchStrategy(SearchCondition.LessThanOrEqual, parameters.MaxSize.Value));
      if (parameters.MinCreationDate.HasValue)
        strategies.Add(new CreationDateSearchStrategy(SearchCondition.GreaterThanOrEqual, parameters.MinCreationDate.Value));
      if (parameters.MaxCreationDate.HasValue)
        strategies.Add(new CreationDateSearchStrategy(SearchCondition.LessThanOrEqual, parameters.MaxCreationDate.Value));
      if (parameters.FileAttributesMask.HasValue)
        strategies.Add(new FileAttributesMaskSearchStrategy(parameters.FileAttributesMask.Value));
      
      if (strategies.Count == 0)
        return EmptySearchStrategy.Instance;
      return strategies.Count == 1 
        ? strategies[0] 
        : new CompositeSearchStrategy(strategies);
    }
  }
}