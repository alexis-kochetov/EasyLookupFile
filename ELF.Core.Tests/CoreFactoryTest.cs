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
using System.Linq;
using ELF.Contracts;
using ELF.Contracts.Internals;
using ELF.Core.Internals.Strategies;
using NUnit.Framework;

namespace ELF.Core.Tests
{
  [TestFixture]
  public class CoreFactoryTest
  {
    [Test]
    public void EmptyStrategyTest()
    {
      var parameters = SearchParameters.Empty;
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.AreSame(EmptySearchStrategy.Instance, strategy);
    }

    [Test]
    public void ExtensionsStrategyTest()
    {
      var extensions = new []{".bmp", ".txt"};
      var parameters = new SearchParameters {Extensions = extensions};
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.IsInstanceOf<ExtensionsSearchStrategy>(strategy);
      var esStrategy = (ExtensionsSearchStrategy) strategy;
      Assert.AreNotSame(extensions, esStrategy.Extensions);
      Assert.AreEqual(extensions.Length, esStrategy.Extensions.Length);
      Assert.AreEqual(0, extensions.Except(esStrategy.Extensions).Count());
    }

    [Test]
    public void MinCreationDateStrategyTest()
    {
      var creationDate = new DateTime(1900, 1, 1);
      var parameters = new SearchParameters {MinCreationDate = creationDate};
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.IsInstanceOf<CreationDateSearchStrategy>(strategy);
      var cdStrategy = (CreationDateSearchStrategy)strategy;
      Assert.AreEqual(creationDate, cdStrategy.Value);
      Assert.AreEqual(SearchCondition.GreaterThanOrEqual, cdStrategy.Condition);
    }

    [Test]
    public void MaxCreationDateStrategyTest()
    {
      var creationDate = new DateTime(1900, 1, 1);
      var parameters = new SearchParameters { MaxCreationDate = creationDate };
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.IsInstanceOf<CreationDateSearchStrategy>(strategy);
      var cdStrategy = (CreationDateSearchStrategy)strategy;
      Assert.AreEqual(creationDate, cdStrategy.Value);
      Assert.AreEqual(SearchCondition.LessThanOrEqual, cdStrategy.Condition);
    }

    [Test]
    public void CreationDateStrategyTest()
    {
      var minCreationDate = new DateTime(1900, 1, 1);
      var maxCreationDate = new DateTime(2100, 1, 1);
      var parameters = new SearchParameters { MinCreationDate = minCreationDate, MaxCreationDate = maxCreationDate};
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.IsInstanceOf<CompositeSearchStrategy>(strategy);
      var csStrategy = (CompositeSearchStrategy)strategy;
      Assert.AreEqual(2, csStrategy.Strategies.Length);
      Assert.IsInstanceOf<CreationDateSearchStrategy>(csStrategy.Strategies[0]);
      Assert.IsInstanceOf<CreationDateSearchStrategy>(csStrategy.Strategies[1]);
    }

    [Test]
    public void FileAttributesMaskStrategyTest()
    {
      var mask = FileAttributes.Archive | FileAttributes.ReadOnly;
      var parameters = new SearchParameters { FileAttributesMask = mask };
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      Assert.IsNotNull(strategy);
      Assert.IsInstanceOf<FileAttributesMaskSearchStrategy>(strategy);
      var famStrategy = (FileAttributesMaskSearchStrategy)strategy;
      Assert.AreEqual(mask, famStrategy.AttributesMask);
    }
  }
}