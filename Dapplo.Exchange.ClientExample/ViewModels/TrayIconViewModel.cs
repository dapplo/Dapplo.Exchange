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

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Exchange.ClientExample.Models;
using Dapplo.Log.Facade;
using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange.ClientExample.ViewModels
{
	[Export(typeof(ITrayIconViewModel))]
	public class TrayIconViewModel : Screen, ITrayIconViewModel, IHandle<EmailMessage>
	{
		private static readonly LogSource Log = new LogSource();

		[Import]
		public IContextMenuTranslations ContextMenuTranslations { get; set; }

		[Import]
		private IEventAggregator EventAggregator { get; set; }

		[Import]
		public ITrayIconManager TrayIconManager { get; set; }

		public void Handle(EmailMessage message)
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
			Dapplication.Current.Shutdown();
		}
	}
}