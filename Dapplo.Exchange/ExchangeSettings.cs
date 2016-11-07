﻿#region Dapplo 2016 - GNU Lesser General Public License

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

using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange
{
	/// <summary>
	/// Implementation of the IExchangeSettings, would not be needed if using Dapplo.InterfaceImpl
	/// </summary>
	public class ExchangeSettings : IExchangeSettings
	{
		public bool AllowSelfSignedCertificates { get; set; } = true;

		public bool AllowRedirectUrl { get; set; } = true;

		public string ExchangeUrl { get; set; }

		public bool UseDefaultCredentials { get; set; } = true;

		public string Username { get; set; }

		public string Password { get; set; }

		public ExchangeVersion VersionToUse { get; set; } = ExchangeVersion.Exchange2010_SP2;
	}
}