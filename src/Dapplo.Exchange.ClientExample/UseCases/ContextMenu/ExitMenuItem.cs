// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2019 Dapplo
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

using System.Windows;
using System.Windows.Media;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Exchange.ClientExample.Models;
using MahApps.Metro.IconPacks;

namespace Dapplo.Exchange.ClientExample.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the exit to the context menu
    /// </summary>
    [Menu("systemtray")]
    public sealed class ExitMenuItem : ClickableMenuItem
    {
        /// <summary>
        /// Configure the exit menu item
        /// </summary>
        /// <param name="contextMenuTranslations"></param>
        public ExitMenuItem(IContextMenuTranslations contextMenuTranslations)
        {
            // automatically update the DisplayName
            contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IContextMenuTranslations.Exit));
            Id = "Z_Exit";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Close,
            };
            ClickAction = clickedItem =>
            {
                Application.Current.Shutdown();
            };
            this.ApplyIconForegroundColor(Brushes.DarkRed);
        }
    }
}
