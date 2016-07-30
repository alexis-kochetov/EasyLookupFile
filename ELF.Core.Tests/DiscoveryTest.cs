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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using ELF.Contracts;
using NUnit.Framework;

namespace ELF.Core.Tests
{
  [TestFixture]
  public class DiscoveryTest
  {
    [Import]
    private ISearchStrategyFactory factory;

    [TestFixtureSetUp]
    public void TesFixtureSetUp()
    {
      var catalog = new AssemblyCatalog(typeof(CoreSearchStrategyFactory).Assembly);
      var container = new CompositionContainer(catalog);
      container.SatisfyImportsOnce(this);
    }

    [Test]
    public void MainTest()
    {
      Assert.IsNotNull(factory);
      var parameters = SearchParameters.Empty;
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.AreSame(EmptySearchStrategy.Instance, strategy);
    }
  }
}