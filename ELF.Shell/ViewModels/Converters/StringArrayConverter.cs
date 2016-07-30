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
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ELF.ViewModels
{
  [ValueConversion(typeof(string[]), typeof(string))]
  public sealed class StringArrayConverter : IValueConverter
  {
    private const string Separator = "; ";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var array = (string[]) value;
      return string.Join(Separator, array);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var strValue = value as string;
      if (string.IsNullOrWhiteSpace(strValue))
        return new string[0];
      return strValue.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .ToArray();
    }
  }
}