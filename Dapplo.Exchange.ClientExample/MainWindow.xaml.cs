using Microsoft.Exchange.WebServices.Data;
using System;
using System.Windows;
using System.Linq;

namespace Dapplo.Exchange.ClientExample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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

			// Show the upcoming event
			foreach(var appointment in await _exchange.RetrieveAppointmentsAsync(DateTime.Now, DateTime.Now.AddDays(1), 5)) {
				if(appointment.Start < DateTime.Now)
				{
					continue;
				}
				Upcoming.Content = $"Upcoming event: {appointment.Subject}";
            }
			_newMailNotifier = _exchange.CreateEventSubscription((itemEvent) => MessageBox.Show("New emails: " + itemEvent.Count()));
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
