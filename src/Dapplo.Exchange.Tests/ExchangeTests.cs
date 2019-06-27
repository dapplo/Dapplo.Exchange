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

using System.Reactive.Linq;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Microsoft.Exchange.WebServices.Data;
using Xunit;
using Xunit.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace Dapplo.Exchange.Tests
{
    public class ExchangeTests
    {
        private static readonly LogSource Log = new LogSource();
        private readonly ExchangeServiceContainer _exchange;
        public ExchangeTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            var exchange = new ExchangeServiceContainer();
            exchange.Initialize();
            _exchange = exchange;
        }

        //[Fact]
        public async Task TestListEmails()
        {
            var emails = await _exchange.RetrieveMailsAsync(WellKnownFolderName.Inbox, 10).ConfigureAwait(true);
            Assert.True(emails.Count > 0);
            foreach (var emailMessage in emails)
            {
                Log.Debug().WriteLine(emailMessage.Subject);
            }
        }

        //[Fact]
        public async Task TestObserve()
        {
            await _exchange.Observe().FirstAsync();
        }
    }
}
