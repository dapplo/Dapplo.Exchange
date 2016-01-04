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
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Linq;

namespace Dapplo.Exchange
{
	/// <summary>
	/// This is a wrapper with a lot of convenience for the ExchangeService
	/// </summary>
	public class Exchange
	{
		private IExchangeSettings _exchangeSettings;
		private static bool _allowSelfSignedCertificates = true;

		/// <summary>
		/// Access the underlying ExchangeService if you want to do something which is not available yet.
		/// This might be removed as soon as more functionality is available.
		/// </summary>
		public ExchangeService Service
		{
			get;
			private set;
		}

		/// <summary>
		/// Create an exchange service wrapper with the supplied settings (or null if defaults)
		/// </summary>
		/// <param name="exchangeSettings">null for default</param>
		public Exchange(IExchangeSettings exchangeSettings = null)
		{
			_exchangeSettings = exchangeSettings ?? new ExchangeSettings();
			_allowSelfSignedCertificates = _exchangeSettings.AllowSelfSignedCertificates;
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
		}

		/// <summary>
		/// Use this to get notified of new emails
		/// </summary>
		/// <param name="notifyAction">the action to call when</param>
		/// <param name="pullInterval">the intervall to pull, 30 seconds when nothing is specified</param>
		/// <param name="wellKnownFolderName">WellKnownFolderName, Inbox if null</param>
		/// <returns>a disposable, when disposed the pulling is stopped</returns>
		public IDisposable CreateNewMailNotify(Action<IEnumerable<ItemEvent>> notifyAction, TimeSpan pullInterval = default(TimeSpan), WellKnownFolderName wellKnownFolderName = WellKnownFolderName.Inbox)
		{
			if (pullInterval.TotalMilliseconds == 0)
			{
				pullInterval = TimeSpan.FromSeconds(30);
            }
			var subscription = Service.SubscribeToPullNotifications(new FolderId[] { new FolderId(wellKnownFolderName) }, 5, null, EventType.NewMail);
			var timer = new System.Timers.Timer
			{
				Interval = pullInterval.TotalMilliseconds,
				AutoReset = true
			};
			timer.Elapsed += (source, elapsedEventArgs) =>
			{
				var itemEvents = subscription.GetEvents().ItemEvents;
				if (itemEvents.Any())
				{
					notifyAction(itemEvents);
				}
			};
			timer.Start();

			// Return a disposable which stops the timer and unsubscribes the subscription
			return Disposable.Create(() =>
			{
				if (timer.Enabled)
				{
					timer.Stop();
				}
				timer.Dispose();
				subscription.Unsubscribe();
			});
		}

		/// <summary>
		/// Initialize the exchange server connection
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <returns>Task</returns>
		public async System.Threading.Tasks.Task InitializeAsync(CancellationToken token = default(CancellationToken))
		{
			await System.Threading.Tasks.Task.Run(() =>
			{
				Service = new ExchangeService(_exchangeSettings.VersionToUse);

				// Remove this if things work
				Service.TraceEnabled = true;
				Service.TraceFlags = TraceFlags.All;


				Service.UseDefaultCredentials = _exchangeSettings.UseDefaultCredentials;
				if (!_exchangeSettings.UseDefaultCredentials)
				{
					Service.Credentials = new NetworkCredential(_exchangeSettings.Username, _exchangeSettings.Password);
                }

				if (string.IsNullOrEmpty(_exchangeSettings.ExchangeUrl))
				{
					var filter = LdapAccess.CreateFindUserFilter(Environment.UserName);
					var result = LdapAccess.FindAll(filter);

					foreach (var userProperties in result)
					{
						if (userProperties.ContainsKey("mail"))
						{
							if (_exchangeSettings.AllowRedirectUrl)
							{
								Service.AutodiscoverUrl(userProperties["mail"].ToString(), RedirectionUrlValidationCallback);
							}
							else
							{
								Service.AutodiscoverUrl(userProperties["mail"].ToString());
							}
							
							break;
						}
					}
				}
				else
				{
					Service.Url = new Uri(_exchangeSettings.ExchangeUrl);
                }

			}, token);
		}

		/// <summary>
		/// Retrieve appointment items from the exchange service
		/// </summary>
		/// <returns>FindItemsResults with appointment</returns>
		public async System.Threading.Tasks.Task<FindItemsResults<Appointment>> RetrieveAppointmentsAsync(DateTime startDate, DateTime endDate, int maxItems = 20, CancellationToken token = default(CancellationToken))
		{
			return await System.Threading.Tasks.Task.Run(() =>
			{
				// Initialize the calendar folder object with only the folder ID. 
				var calendar = CalendarFolder.Bind(Service, WellKnownFolderName.Calendar, new PropertySet());
				// Set the start and end time and number of appointments to retrieve.
				var calendarView = new CalendarView(startDate, endDate, maxItems);

				// Limit the properties returned to the appointment's subject, start time, and end time.
				calendarView.PropertySet = new PropertySet(ItemSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);

				// Retrieve a collection of appointments by using the calendar view.
				return calendar.FindAppointments(calendarView);
			}, token);
        }

		#region Validation
		/// <summary>
		/// This method validates the redirect Url, this is only called when a redirection is used
		/// </summary>
		/// <param name="redirectionUrl"></param>
		/// <returns>true if the redirect url is okay</returns>
		private static bool RedirectionUrlValidationCallback(string redirectionUrl)
		{
			// The default for the validation callback is to reject the URL.
			bool result = false;

			var redirectionUri = new Uri(redirectionUrl);

			// Validate the contents of the redirection URL. In this simple validation
			// callback, the redirection URL is considered valid if it is using HTTPS
			// to encrypt the authentication credentials. 
			if (redirectionUri.Scheme == "https")
			{
				result = true;
			}
			return result;
		}

		/// <summary>
		/// This is needed for allowing the self-signed certificates
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns>bool if the certificate is allowed</returns>
		private static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}

			// If there are errors in the certificate chain, look at each error to determine the cause.
			if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
			{
				if (chain != null && chain.ChainStatus != null)
				{
					foreach (X509ChainStatus status in chain.ChainStatus)
					{
						if ((certificate.Subject == certificate.Issuer) && (status.Status == X509ChainStatusFlags.UntrustedRoot))
						{
							if (_allowSelfSignedCertificates)
							{
								// Self-signed certificates with an untrusted root are valid. 
								continue;
							}
							return false;
						}
						else
						{
							if (status.Status != X509ChainStatusFlags.NoError)
							{
								// If there are any other errors in the certificate chain, the certificate is invalid,
								// so the method returns false.
								return false;
							}
						}
					}
				}

				// When processing reaches this line, the only errors in the certificate chain are 
				// untrusted root errors for self-signed certificates. These certificates are valid
				// for default Exchange server installations, so return true.
				return true;
			}
			else
			{
				// In all other cases, return false.
				return false;
			}
		}
		#endregion
	}
}
