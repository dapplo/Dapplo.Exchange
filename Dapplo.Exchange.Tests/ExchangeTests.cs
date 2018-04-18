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
        private readonly Exchange _exchange;
        public ExchangeTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            var exchange = new Exchange();
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
