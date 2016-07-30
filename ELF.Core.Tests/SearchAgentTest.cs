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
using System.Threading;
using ELF.Contracts;
using NUnit.Framework;

namespace ELF.Core.Tests
{
  [TestFixture]
  public class SearchAgentTest
  {
    private const string TestDataPath = @"..\..\Data";
    private static readonly string[] fileNames = new[]{"SomeImageFile.bmp", "SomeTextFile.txt", "SomeXMLFile.xml"};
    private static readonly string[] nestedFileNames = new[]{"NestedImageFile.bmp", "NestedTextFile.txt", "NestedXMLFile.xml"};

    [Test]
    public void SearchFilesTest()
    {
      var settings = new SearchSettings(TestDataPath, SearchOption.TopDirectoryOnly);
      var parameters = SearchParameters.Empty;
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      var result = SearchAgent.Search(settings, strategy, CancellationToken.None);
      var list = result.ToList();
      Assert.AreEqual(3, list.Count);
      Assert.AreEqual(0, list.Select(fi => fi.Name).Except(fileNames).Count());
    }

    [Test]
    public void SearchNestedFilesTest()
    {
      var settings = new SearchSettings(TestDataPath, SearchOption.AllDirectories);
      var parameters = SearchParameters.Empty;
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      var result = SearchAgent.Search(settings, strategy, CancellationToken.None);
      var list = result.ToList();
      Assert.AreEqual(6, list.Count);
      Assert.AreEqual(0, list.Select(fi => fi.Name).Except(fileNames).Except(nestedFileNames).Count());
    }

    [Test]
    public void FilterByExtensionTest()
    {
      var settings = new SearchSettings(TestDataPath, SearchOption.AllDirectories);
      var parameters = new SearchParameters { Extensions = new[] { ".bmp", ".xml" } };
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      var result = SearchAgent.Search(settings, strategy, CancellationToken.None);
      var list = result.ToList();
      Assert.AreEqual(4, list.Count);
      Assert.IsTrue(list.All(fi => 
        fi.Name.EndsWith(".bmp",StringComparison.InvariantCultureIgnoreCase) ||
        fi.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase)));
    }

    [Test]
    public void CompositeParametersSearchTest()
    {
      var settings = new SearchSettings(TestDataPath, SearchOption.AllDirectories);
      var parameters = new SearchParameters {Extensions = new[] {".txt", ".xml"}, MinSize = 20, MaxSize = 100};
      var factory = new CoreSearchStrategyFactory();
      var strategy = factory.CreateStrategy(parameters);
      var result = SearchAgent.Search(settings, strategy, CancellationToken.None);
      var list = result.ToList();
      Assert.AreEqual(2, list.Count);
      Assert.IsTrue(list.All(fi => fi.Name.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase)));
    }
  }
}