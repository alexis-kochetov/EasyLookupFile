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
using ELF.Contracts.Internals;
using ELF.Core.Internals.Strategies;
using NUnit.Framework;

namespace ELF.Core.Tests
{
  [TestFixture]
  public class StrategiesTest
  {
    private const string SomeTextFileName = @"..\..\Data\SomeTextFile.txt";

    [Test]
    public void CreationDateTest()
    {
      var fi = new FileInfo(SomeTextFileName);
      Assert.IsTrue(fi.Exists);
      var gt = new CreationDateSearchStrategy(SearchCondition.GreaterThanOrEqual, new DateTime(1990, 1, 1));
      Assert.IsTrue(gt.Match(fi));
      gt = new CreationDateSearchStrategy(SearchCondition.GreaterThanOrEqual, new DateTime(2100, 1, 1));
      Assert.IsFalse(gt.Match(fi));
      var lt = new CreationDateSearchStrategy(SearchCondition.LessThanOrEqual, new DateTime(2100, 1, 1));
      Assert.IsTrue(lt.Match(fi));
      lt = new CreationDateSearchStrategy(SearchCondition.LessThanOrEqual, new DateTime(1900, 1, 1));
      Assert.IsFalse(gt.Match(fi));
    }

    [Test]
    public void ExtensionsTest()
    {
      var fi = new FileInfo(SomeTextFileName);
      Assert.IsTrue(fi.Exists);
      var ex = new ExtensionsSearchStrategy(new[]{".txt"});
      Assert.IsTrue(ex.Match(fi));
      ex = new ExtensionsSearchStrategy(new[] { ".bmp" });
      Assert.IsFalse(ex.Match(fi));
    }

    [Test]
    public void FileAttributesTest()
    {
      var fi = new FileInfo(SomeTextFileName);
      Assert.IsTrue(fi.Exists);
      var attributes = fi.Attributes;
      var at = new FileAttributesMaskSearchStrategy(attributes);
      Assert.IsTrue(at.Match(fi));
      at = new FileAttributesMaskSearchStrategy(FileAttributes.ReadOnly);
      Assert.IsFalse(at.Match(fi));
    }

    [Test]
    public void FileSizeTest()
    {
      var fi = new FileInfo(SomeTextFileName);
      Assert.IsTrue(fi.Exists);
      var gt = new FileSizeSearchStrategy(SearchCondition.GreaterThanOrEqual, 4);
      Assert.IsTrue(gt.Match(fi));
      gt = new FileSizeSearchStrategy(SearchCondition.GreaterThanOrEqual, 20);
      Assert.IsFalse(gt.Match(fi));
      var lt = new FileSizeSearchStrategy(SearchCondition.LessThanOrEqual, 20);
      Assert.IsTrue(lt.Match(fi));
      lt = new FileSizeSearchStrategy(SearchCondition.LessThanOrEqual, 4);
      Assert.IsFalse(lt.Match(fi));
    }
  }
}