using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ELF.Core.Tests
{
  [TestFixture]
  [Explicit]
  public class EnumerateFilesTest
  {

    [Test]
    public void CustomIteratorTest()
    {
      foreach (var file in EnumerateFiles("D:\\", SearchOption.AllDirectories)) {
        
      }
    }

    private static IEnumerable<string> EnumerateFiles(string directory, SearchOption searchOption)
    {
      var files = Enumerable.Empty<string>();
      try {
        files = Directory.EnumerateFiles(directory);
      }
      catch(UnauthorizedAccessException)
      {}
      foreach (var file in files)
        yield return file;
      if (searchOption == SearchOption.AllDirectories) {
        var nestedFiles = Enumerable.Empty<string>();
        try {
          nestedFiles = Directory.EnumerateDirectories(directory).SelectMany(dir => EnumerateFiles(dir, searchOption));
        }
        catch (UnauthorizedAccessException)
        {
        }
        foreach (var file in nestedFiles)
          yield return file;
      }
    }
  }
}