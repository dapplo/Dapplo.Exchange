using Dapplo.Config.Ini;

namespace Dapplo.Exchange.ClientExample.Models
{
	[IniSection("Exchange")]
	public interface IExchangeConfig : IExchangeSettings
	{
	}
}
