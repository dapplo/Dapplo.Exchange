using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using System.ComponentModel.Composition;

namespace Dapplo.Exchange.ClientExample.ViewModels
{
	[Export(typeof(IShell))]
	public class ExchangeClientViewModel : Conductor<IScreen>.Collection.OneActive, IShell
	{
	}
}
