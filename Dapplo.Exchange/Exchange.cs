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

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.Exchange
{
	/// <summary>
	/// This is a wrapper with a lot of convenience for the ExchangeService
	/// </summary>
	public class Exchange
	{
		private Microsoft.Exchange.WebServices.Data.ExchangeService _exchangeService;
		private IExchangeSettings _exchangeSettings;
		private static bool _allowSelfSignedCertificates = true;

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
		/// A simple helper to create an email message for the exchange service
		/// </summary>
		/// <returns>EmailMessage</returns>
		public Microsoft.Exchange.WebServices.Data.EmailMessage CreateEmailMessage()
		{
			var emailMessage = new Microsoft.Exchange.WebServices.Data.EmailMessage(_exchangeService);
			return emailMessage;
		}

		/// <summary>
		/// Initialize the exchange server connection
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <returns>Task</returns>
		public async Task InitializeAsync(CancellationToken token = default(CancellationToken))
		{
			await Task.Run(() =>
			{
				_exchangeService = new Microsoft.Exchange.WebServices.Data.ExchangeService(_exchangeSettings.VersionToUse);

				// Remove this if things work
				_exchangeService.TraceEnabled = true;
				_exchangeService.TraceFlags = Microsoft.Exchange.WebServices.Data.TraceFlags.All;


				_exchangeService.UseDefaultCredentials = _exchangeSettings.UseDefaultCredentials;
				if (!_exchangeSettings.UseDefaultCredentials)
				{
					_exchangeService.Credentials = new NetworkCredential(_exchangeSettings.Username, _exchangeSettings.Password);
                }

				if (string.IsNullOrEmpty(_exchangeSettings.ExchangeUrl))
				{
					var filter = LdapAccess.CreateFindUserFilter(Environment.UserName);
					var result = LdapAccess.FindAll(Environment.UserDomainName, filter);

					foreach (var userProperties in result)
					{
						if (userProperties.ContainsKey("mail"))
						{
							if (_exchangeSettings.AllowRedirectUrl)
							{
								_exchangeService.AutodiscoverUrl(userProperties["mail"].ToString(), RedirectionUrlValidationCallback);
							}
							else
							{
								_exchangeService.AutodiscoverUrl(userProperties["mail"].ToString());
							}
							
							break;
						}
					}
				}
				else
				{
					_exchangeService.Url = new Uri(_exchangeSettings.ExchangeUrl);
                }

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
