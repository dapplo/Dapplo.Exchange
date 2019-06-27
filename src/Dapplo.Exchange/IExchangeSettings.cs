// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2019 Dapplo
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

#region Usings

using System.ComponentModel;
using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange
{
	/// <summary>
	/// Various settings for the Exchange communication
	/// </summary>
	public interface IExchangeSettings
	{
        /// <summary>
        /// Allow that the HTTPS connection is made with a self signed certificate
        /// </summary>
        [DefaultValue(true), Description("Do we allow self-signed certificates")]
		bool AllowSelfSignedCertificates { get; set; }

        /// <summary>
        /// Allow redirects to be followed
        /// </summary>
        [DefaultValue(true), Description("Do we allow a redirect url")]
		bool AllowRedirectUrl { get; set; }

        /// <summary>
        /// What version of exchange needs to be used
        /// </summary>
		[DefaultValue(ExchangeVersion.Exchange2010_SP2), Description("Client version of the connection to the exchange server, this should not be higher as the server")]
		ExchangeVersion VersionToUse { get; set; }

        /// <summary>
        /// If set to true, use the default credentials
        /// </summary>
		[DefaultValue(true), Description("Should the connection be made with the default credentials")]
		bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Url to the exchange server
        /// </summary>
        [Description("Exchange Url, this is used to connect to the exchange server. If left unset, it will be auto discovered")]
		string ExchangeUrl { get; set; }

        /// <summary>
        /// Username which the connection uses
        /// </summary>
        [Description("Username for the connection")]
		string Username { get; set; }

        /// <summary>
        /// Password for the connection
        /// </summary>
        [Description("Password for the connection")]
		string Password { get; set; }
	}
}