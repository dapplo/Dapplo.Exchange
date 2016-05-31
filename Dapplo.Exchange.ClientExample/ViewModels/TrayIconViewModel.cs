using Caliburn.Micro;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Exchange.ClientExample.Models;
using Dapplo.LogFacade;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
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

		// TODO: Remove "hack" for the shutdown/exit
		[Import]
		private IBootstrapper Bootstrapper { get; set; }

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

		public async Task Exit()
		{
			Log.Debug().WriteLine("Exit");

			// TODO: Remove "hack" for the shutdown/exit
			await Bootstrapper.StopAsync(CancellationToken.None);
			Application.Current.Shutdown();
		}
	}
}
