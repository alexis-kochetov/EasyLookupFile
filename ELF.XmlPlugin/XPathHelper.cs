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
using System.Xml;
using System.Xml.XPath;

namespace ELF.XmlPlugin
{
  public static class XPathHelper
  {
    [Pure]
    public static XPathExpression Compile(string xPathPattern)
    {
      Contract.Requires(!string.IsNullOrEmpty(xPathPattern));
      Contract.Ensures(Contract.Result<XPathExpression>() != null);

      var document = new XmlDocument();
      var navigator = document.CreateNavigator();
      Contract.Assert(navigator != null);
      var result = navigator.Compile(xPathPattern);
      Contract.Assume(result != null);
      return result;
    }

    [Pure]
    public static bool XPathPatternIsValid(string xPathPattern)
    {
      Contract.Requires(!string.IsNullOrEmpty(xPathPattern));

      try {
        Compile(xPathPattern);
        return true;
      }
      catch (XPathException) {
        return false;
      }
    }
  }
}