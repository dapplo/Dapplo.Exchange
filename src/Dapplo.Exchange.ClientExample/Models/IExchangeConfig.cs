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

using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.Config.Ini;

namespace Dapplo.Exchange.ClientExample.Models
{
	/// <summary>
	/// Container for the exchange settings
	/// </summary>
	[IniSection("Exchange")]
	public interface IExchangeConfig : IExchangeSettings, IUiConfiguration, IMetroUiConfiguration
    {
	}
}