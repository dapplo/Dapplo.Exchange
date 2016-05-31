using Dapplo.Config.Language;
using System.ComponentModel;

namespace Dapplo.Exchange.ClientExample.Models
{
	[Language("ContextMenu")]
	public interface IContextMenuTranslations : ILanguage, INotifyPropertyChanged
	{
		[DefaultValue("Exit")]
		string Exit { get; }
	}
}
