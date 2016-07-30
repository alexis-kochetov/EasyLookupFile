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
using System.Diagnostics.Contracts;
using Enumerable = System.Linq.Enumerable;

namespace ELF.Contracts
{
  public static class StrategyExtensions
  {
    public static bool IsEmpty(this ISearchStrategy strategy)
    {
      Contract.Requires(strategy != null);
      return strategy == EmptySearchStrategy.Instance;
    }

    public static ISearchStrategy Combine(this ISearchStrategy first, ISearchStrategy second)
    {
      Contract.Requires(first != null);
      Contract.Requires(second != null);

      if (first.IsEmpty())
        return second;
      if (second.IsEmpty())
        return first;

      var firstComposite = first as CompositeSearchStrategy;
      var secondComposite = second as CompositeSearchStrategy;
      if (firstComposite != null && secondComposite != null)
        return new CompositeSearchStrategy(Enumerable.Concat(firstComposite.Strategies, secondComposite.Strategies));
      if (firstComposite != null)
        return new CompositeSearchStrategy(Enumerable.Concat(firstComposite.Strategies, Enumerable.Repeat(second,1)));
      if (secondComposite != null)
        return new CompositeSearchStrategy(Enumerable.Concat(Enumerable.Repeat(first, 1), secondComposite.Strategies));
      return new CompositeSearchStrategy(new[] {first, second});
    }
  }
}