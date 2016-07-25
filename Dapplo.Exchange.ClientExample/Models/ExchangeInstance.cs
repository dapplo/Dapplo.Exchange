#region Dapplo 2016 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016 Dapplo
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
using System.Threading;
using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Log.Facade;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

#endregion

namespace Dapplo.Exchange.ClientExample.Models
{
	[StartupAction(StartupOrder = (int) CaliburnStartOrder.TrayIcons + 10), ShutdownAction]
	public class ExchangeInstance : IStartupAction, IShutdownAction
	{
		private static readonly LogSource Log = new LogSource();
		private IDisposable _eventSubscription;
		private Exchange _exchange;

		[Import]
		private IEventAggregator EventAggregator { get; set; }

		/// <summary>
		///     Action to process a new email
		/// </summary>
		public Action<EmailMessage> NewEmailAction { get; set; }


		public async Task ShutdownAsync(CancellationToken token = default(CancellationToken))
		{
			_eventSubscription?.Dispose();
			await Task.Yield();
		}

		public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			_exchange = new Exchange();
			await _exchange.InitializeAsync().ConfigureAwait(false);

			_eventSubscription = _exchange.CreateEventSubscription(notificationEvents =>
			{
				foreach (var notificationEvent in notificationEvents)
				{
					switch (notificationEvent.EventType)
					{
						case EventType.NewMail:
							if (NewEmailAction != null)
							{
								var itemEvent = (ItemEvent) notificationEvent;
								var id = new ItemId(itemEvent.ItemId.UniqueId);
								try
								{
									// Get Email
									var email = EmailMessage.Bind(_exchange.Service, id);
									// Call action
									NewEmailAction(email);
								}
								catch (Exception ex)
								{
									Log.Error().WriteLine(ex);
								}
							}
							break;
						default:
							Log.Verbose().WriteLine("Ignoring event {0}", notificationEvent.EventType);
							break;
					}
				}
			});

			NewEmailAction = email => { EventAggregator.PublishOnUIThread(email); };
		}
	}
}