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
using System.ComponentModel.Composition;

namespace ELF.Contracts
{
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class Plugin : ExportAttribute, IPluginMetaData
  {
    public string Type { get; private set; }
    public string Description { get; private set; }
    public string[] Extensions { get; set; }
    
    public Plugin(string type, string description) : base(typeof(IPlugin))
    {
      Type = type;
      Description = description;
      Extensions = new string[0];
    }
  }
}