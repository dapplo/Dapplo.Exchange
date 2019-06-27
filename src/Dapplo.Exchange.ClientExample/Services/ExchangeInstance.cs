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
    [Service(nameof(ExchangeInstance), nameof(CaliburnServices.CaliburnMicroBootstrapper))]
    public class ExchangeInstance : IStartup, IShutdown
    {
        private readonly IEventAggregator _eventAggregator;
        private static readonly LogSource Log = new LogSource();
        private IDisposable _eventSubscription;
        private readonly ExchangeServiceContainer _exchangeServiceContainer;

        /// <summary>
        /// Constructor for the
        /// </summary>
        /// <param name="exchangeServiceContainer">ExchangeServiceContainer</param>
        /// <param name="eventAggregator">eventAggregator</param>
        public ExchangeInstance(
            ExchangeServiceContainer exchangeServiceContainer,
            IEventAggregator eventAggregator)
        {
            _exchangeServiceContainer = exchangeServiceContainer;
            _eventAggregator = eventAggregator;
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _eventSubscription?.Dispose();
        }

        /// <inheritdoc />
        public void Startup()
        {
            _eventSubscription = _exchangeServiceContainer.Observe(WellKnownFolderName.Inbox, EventType.NewMail).Subscribe(notificationEvent =>
            {
                var itemEvent = (ItemEvent) notificationEvent;
                var id = new ItemId(itemEvent.ItemId.UniqueId);
                try
                {
                    // Get Email
                    var email = EmailMessage.Bind(_exchangeServiceContainer.Service, id);
                    // Call action
                    _eventAggregator.BeginPublishOnUIThread(email);
                }
                catch (Exception ex)
                {
                    Log.Error().WriteLine(ex);
                }
            });
        }
    }
}