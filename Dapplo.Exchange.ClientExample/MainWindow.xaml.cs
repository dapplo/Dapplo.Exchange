/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.Exchange.

	Dapplo.Exchange is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.Exchange is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.Exchange.  If not, see <http://www.gnu.org/licenses/>.
 */

 using Microsoft.Exchange.WebServices.Data;
using System;
using System.Windows;
using System.Linq;
using System.IO;
using Dapplo.LogFacade;

namespace Dapplo.Exchange.ClientExample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly LogSource Log = new LogSource();
		private Exchange _exchange;
		private IDisposable _newMailNotifier;

        public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
			Closing += MainWindow_Closing;
        }

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_newMailNotifier?.Dispose();
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			_exchange = new Exchange();
			await _exchange.InitializeAsync();
			Fields.IsEnabled = true;

			_newMailNotifier = _exchange.CreateEventSubscription((itemEvent) => MessageBox.Show("New emails: " + itemEvent.Count()));

			// Show the upcoming event
			var appointments = await _exchange.RetrieveAppointmentsAsync(DateTime.Now, DateTime.Now.AddDays(1), 5);

			var first = appointments.Where(x => x.Start < DateTime.Now).OrderBy(x => x.Start).FirstOrDefault();
			if (first != null)
			{
				Upcoming.Content = $"Upcoming event: {first.Subject}";
			}

			var contacts = await _exchange.RetrieveContactsAsync();
			foreach(var contact in contacts)
			{
				var photoAttachment = contact.GetContactPictureAttachment();
				if (photoAttachment == null)
				{
					Log.Info().WriteLine("No photo for {1}, {0}", contact.GivenName, contact.Surname);
					continue;
				}
				var filename = $@"c:\localdata\{contact.GivenName}_{contact.Surname}{photoAttachment.Name}";
				if (!File.Exists(filename))
				{
					Log.Info().WriteLine("Writing photo for {1}, {0} to: {2}", contact.GivenName, contact.Surname, filename);
					File.WriteAllBytes(filename, photoAttachment.Content);
				}
				else
				{
					Log.Info().WriteLine("Already write photo for {1}, {0}", contact.GivenName, contact.Surname);
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var email = new EmailMessage(_exchange.Service);
			email.ToRecipients.Add(To.Text);
			email.Subject = Subject.Text;
			email.Body = new MessageBody(BodyType.Text, Body.Text);
			email.Send();
		}
	}
}
