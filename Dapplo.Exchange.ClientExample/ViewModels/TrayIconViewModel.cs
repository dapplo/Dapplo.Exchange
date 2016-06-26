using Caliburn.Micro;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Exchange.ClientExample.Models;
using Dapplo.Log.Facade;
using System.ComponentModel.Composition;
using System.Windows;

namespace Dapplo.Exchange.ClientExample.ViewModels
{
	[Export(typeof(ITrayIconViewModel))]
	public class TrayIconViewModel : Screen, ITrayIconViewModel, IHandle<Microsoft.Exchange.WebServices.Data.EmailMessage>
	{
		private static readonly LogSource Log = new LogSource();

		[Import]
		public IContextMenuTranslations ContextMenuTranslations { get; set; }

		[Import]
		private IEventAggregator EventAggregator { get; set; }

		[Import]
		public ITrayIconManager TrayIconManager { get; set; }

		public void Handle(Microsoft.Exchange.WebServices.Data.EmailMessage message)
		{
			var trayIcon = TrayIconManager.GetTrayIconFor(this);

			// TODO: Create a nice popup, preverably with the avatar of the user
			trayIcon.ShowInfoBalloonTip(message.Sender.Name, message.Subject);
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			var trayIcon = TrayIconManager.GetTrayIconFor(this);
			trayIcon.Show();
			EventAggregator.Subscribe(this);
		}

		public void Exit()
		{
			Log.Debug().WriteLine("Exit");
			Application.Current.Shutdown();
		}
	}
}
