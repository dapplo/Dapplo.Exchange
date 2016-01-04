﻿using System.Windows;

namespace Dapplo.Exchange.ClientExample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Exchange _exchange;
        public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
        }

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			_exchange = new Exchange();
			await _exchange.InitializeAsync();
			Fields.IsEnabled = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var email = _exchange.CreateEmailMessage();
			email.ToRecipients.Add(To.Text);
			email.Subject = Subject.Text;
			email.Body = new Microsoft.Exchange.WebServices.Data.MessageBody(Microsoft.Exchange.WebServices.Data.BodyType.Text, Body.Text);
			email.Send();
		}
	}
}
