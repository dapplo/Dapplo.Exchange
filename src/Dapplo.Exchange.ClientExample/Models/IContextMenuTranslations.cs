#region Dapplo 2016-2018 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2018 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.Exchange
// 
// Dapplo.Exchange is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.Exchange is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.Exchange. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#endregion

#region Usings

using System.ComponentModel;
using Dapplo.Language;

#endregion

namespace Dapplo.Exchange.ClientExample.Models
{
    /// <summary>
    /// Translations for the context menu
    /// </summary>
    [Language("ContextMenu")]
    public interface IContextMenuTranslations : ILanguage, INotifyPropertyChanged
    {
        /// <summary>
        /// Used for the exit menu item
        /// </summary>
        [DefaultValue("Exit")]
        string Exit { get; }

        /// <summary>
        /// Used for the menu item title
        /// </summary>
        [DefaultValue("Email Client")]
        string Title { get; }
    }
}