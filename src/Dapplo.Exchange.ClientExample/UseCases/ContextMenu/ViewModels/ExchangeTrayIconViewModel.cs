#region Dapplo 2016-2019 - GNU Lesser General Public License

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

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Autofac.Features.AttributeFilters;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.CaliburnMicro.NotifyIconWpf.ViewModels;
using Dapplo.Exchange.ClientExample.Models;
using MahApps.Metro.IconPacks;

#endregion

namespace Dapplo.Exchange.ClientExample.UseCases.ContextMenu.ViewModels
{
    /// <summary>
    /// Defines the tray-icon
    /// </summary>
    public sealed class ExchangeTrayIconViewModel : TrayIconViewModel
    {
        private readonly IEnumerable<Lazy<IMenuItem>> _contextMenuItems;
        private readonly IContextMenuTranslations _contextMenuTranslations;

        /// <summary>
        /// Creates the tray icon
        /// </summary>
        /// <param name="trayIconManager">ITrayIconManager</param>
        /// <param name="contextMenuItems">IEnumerable IMenuItem</param>
        /// <param name="contextMenuTranslations">IContextMenuTranslations</param>
        public ExchangeTrayIconViewModel(
            ITrayIconManager trayIconManager,
            [MetadataFilter("Menu","systemtray")]IEnumerable<Lazy<IMenuItem>> contextMenuItems,
            IContextMenuTranslations contextMenuTranslations) : base(trayIconManager)
        {
            _contextMenuItems = contextMenuItems;
            _contextMenuTranslations = contextMenuTranslations;
        }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            base.OnActivate();

            // Set the title of the icon (the ToolTipText) to our IContextMenuTranslations.Title
            _contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IContextMenuTranslations.Title));

            var items = new List<IMenuItem>();
            // Lazy values
            items.AddRange(_contextMenuItems.Select(lazy => lazy.Value));

            items.Add(new MenuItem
            {
                Style = MenuItemStyles.Separator,
                Id = "Y_Separator"
            });
            ConfigureMenuItems(items);

            // Make sure the margin is set, do this AFTER the icon are set
            items.ApplyIconMargin(new Thickness(2, 2, 2, 2));

            // Set the icon
            SetIcon(new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Email,
                Background = Brushes.Transparent,
                Foreground = Brushes.Black
            });
            Show();
        }
    }
}