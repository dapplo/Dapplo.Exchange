using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.LogFacade;
using Dapplo.Utils;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.Exchange.ClientExample.Models
{
	[StartupAction(StartupOrder = ((int)CaliburnStartOrder.TrayIcons + 10)), ShutdownAction]
	public class ExchangeInstance : IStartupAction, IShutdownAction
	{
		private static readonly LogSource Log = new LogSource();
		private Exchange _exchange;
		private IDisposable _eventSubscription;

		[Import]
		private IEventAggregator EventAggregator { get; set; }

		public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			_exchange = new Exchange();
			await _exchange.InitializeAsync();

			_eventSubscription = _exchange.CreateEventSubscription((notificationEvents) =>
			{
				foreach(var notificationEvent in notificationEvents)
				{
					switch(notificationEvent.EventType)
					{
						case Microsoft.Exchange.WebServices.Data.EventType.NewMail:
							if (NewEmailAction != null)
							{
								var itemEvent = (Microsoft.Exchange.WebServices.Data.ItemEvent)notificationEvent;
								var id = new Microsoft.Exchange.WebServices.Data.ItemId(itemEvent.ItemId.UniqueId);
								try
								{
									// Get Email
									var email = Microsoft.Exchange.WebServices.Data.EmailMessage.Bind(_exchange.Service, id);
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

			NewEmailAction = (email) =>
			{
				EventAggregator.PublishOnUIThread(email);
			};
		}


		public Task ShutdownAsync(CancellationToken token = default(CancellationToken))
		{
			_eventSubscription?.Dispose();
			return Task.FromResult(true);
		}

		/// <summary>
		/// Action to process a new email
		/// </summary>
		public Action<Microsoft.Exchange.WebServices.Data.EmailMessage> NewEmailAction { get; set; }
	}
}
