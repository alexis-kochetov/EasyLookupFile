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
using System.Diagnostics.Contracts;
using System.IO;

namespace ELF.Contracts
{
  public class SearchParameters
  {
    private static readonly SearchParameters empty = new SearchParameters();

    public static SearchParameters Empty
    {
      get { return empty; }
    }

    public string[] Extensions { [Pure]get; set; }
    public long? MinSize { [Pure]get; set; }
    public long? MaxSize { [Pure]get; set; }
    public DateTime? MinCreationDate { [Pure]get; set; }
    public DateTime? MaxCreationDate { [Pure]get; set; }
    public FileAttributes? FileAttributesMask { [Pure]get; set; }
    
    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
      Contract.Invariant(!MinSize.HasValue || MinSize >= 0);
      Contract.Invariant(!MaxSize.HasValue || MaxSize >= 0);
      Contract.Invariant(!MinSize.HasValue || !MaxSize.HasValue || (MinSize.Value <= MaxSize.Value));
      Contract.Invariant(!MinCreationDate.HasValue || !MaxCreationDate.HasValue || (MinCreationDate.Value <= MaxCreationDate.Value));
      Contract.Invariant(!FileAttributesMask.HasValue || SearchHelper.FileAttributesMaskIsValid(FileAttributesMask.Value));
      Contract.Invariant(Extensions != null);
    }

    public SearchParameters()
    {
      Contract.Assume(!MinSize.HasValue);
      Contract.Assume(!MaxSize.HasValue);
      Contract.Assume(!MinCreationDate.HasValue);
      Contract.Assume(!MaxCreationDate.HasValue);
      Contract.Assume(!FileAttributesMask.HasValue);
      Extensions = SearchHelper.EmptyExtensions;
      Contract.Assume(Extensions != null && Extensions.Length == 0);
    }
  }
}