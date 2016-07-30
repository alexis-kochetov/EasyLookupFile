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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace ELF.ViewModels
{
  [ValueConversion(typeof(FileAttributes?), typeof(ObservableCollection<FileAttributeItem>))]
  public sealed class FileAttributesConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var attributes = (FileAttributes?) value;
      var result = new ObservableCollection<FileAttributeItem> {
        new FileAttributeItem {Flag = FileAttributes.Archive, Title = "Archive"},
        new FileAttributeItem {Flag = FileAttributes.Compressed, Title = "Compressed"},
        new FileAttributeItem {Flag = FileAttributes.Encrypted, Title = "Encrypted"},
        new FileAttributeItem {Flag = FileAttributes.Hidden, Title = "Hidden"},
        new FileAttributeItem {Flag = FileAttributes.ReadOnly, Title = "Readonly"},
        new FileAttributeItem {Flag = FileAttributes.System, Title = "System"},
        new FileAttributeItem {Flag = FileAttributes.Temporary, Title = "Temporary"}
      };
      if (!attributes.HasValue)
        return result;
      foreach (var item in result.Where(item => attributes.Value.HasFlag(item.Flag)))
        item.IsSelected = true;
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var collection = value as ObservableCollection<FileAttributeItem>;
      if (collection == null)
        return default(FileAttributes?);
      var result = collection
        .Where(i => i.IsSelected)
        .Aggregate((FileAttributes) 0, (r, i) => r | i.Flag);
      return ((int)result) == 0 ? 
        default(FileAttributes?) : 
        result;
    }
  }
}