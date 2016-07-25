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

using System.ComponentModel;
using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange
{
	public interface IExchangeSettings
	{
		[DefaultValue(true), Description("Do we allow self-signed certificates")]
		bool AllowSelfSignedCertificates { get; set; }

		[DefaultValue(true), Description("Do we allow a redirect url")]
		bool AllowRedirectUrl { get; set; }

		[DefaultValue(ExchangeVersion.Exchange2010_SP2), Description("Client version of the connection to the exchange server, this should not be higher as the server")]
		ExchangeVersion VersionToUse { get; set; }

		[DefaultValue(true), Description("Should the connection be made with the default credentials")]
		bool UseDefaultCredentials { get; set; }

		[Description("Exchange Url, this is used to connect to the exchange server. If left unset, it will be auto discovered")]
		string ExchangeUrl { get; set; }

		[Description("Username for the connection")]
		string Username { get; set; }

		[Description("Password for the connection")]
		string Password { get; set; }
	}
}