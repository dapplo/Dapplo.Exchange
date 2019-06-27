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


using System.Collections.Generic;
using System.Windows;
using Dapplo.Config.Ini;
using Dapplo.Windows.User32.Structs;
using Microsoft.Exchange.WebServices.Data;

namespace Dapplo.Exchange.ClientExample.Models.Impl
{
    internal class ExchangeConfig : IniSectionBase<IExchangeConfig>, IExchangeConfig
    {
        public bool AllowSelfSignedCertificates { get; set; }
        public bool AllowRedirectUrl { get; set; }
        public ExchangeVersion VersionToUse { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string ExchangeUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public WindowStartupLocation DefaultWindowStartupLocation { get; set; }
        public bool AreWindowLocationsStored { get; set; }
        public IDictionary<string, WindowPlacement> WindowLocations { get; set; }
        public string Theme { get; set; }
        public string ThemeColor { get; set; }
    }
}
