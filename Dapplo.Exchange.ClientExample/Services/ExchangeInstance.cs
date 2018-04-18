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

using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Log;
using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange.ClientExample.Services
{
    /// <summary>
    /// Starts the connection to exchange
    /// </summary>
    [StartupAction(StartupOrder = (int) CaliburnStartOrder.TrayIcons + 10), ShutdownAction]
    public class ExchangeInstance : IStartupAction, IShutdownAction
    {
        private static readonly LogSource Log = new LogSource();
        private IDisposable _eventSubscription;
        private Exchange _exchange = new Exchange();

        [Import]
        private IEventAggregator EventAggregator { get; set; }

        [Import]
        private IServiceExporter ServiceExporter { get; set; }

        /// <inheritdoc />
        public void Shutdown()
        {
            _eventSubscription?.Dispose();
        }

        /// <inheritdoc />
        public void Start()
        {
            _exchange = new Exchange();
            _exchange.Initialize();

            ServiceExporter.Export(_exchange.Service);

            _eventSubscription = _exchange.Observe(WellKnownFolderName.Inbox, EventType.NewMail).Subscribe(notificationEvent =>
            {
                var itemEvent = (ItemEvent) notificationEvent;
                var id = new ItemId(itemEvent.ItemId.UniqueId);
                try
                {
                    // Get Email
                    var email = EmailMessage.Bind(_exchange.Service, id);
                    // Call action
                    EventAggregator.BeginPublishOnUIThread(email);
                }
                catch (Exception ex)
                {
                    Log.Error().WriteLine(ex);
                }
            });
        }
    }
}