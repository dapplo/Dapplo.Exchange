using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.Exchange.ClientExample.Models;
using MahApps.Metro.IconPacks;

namespace Dapplo.Exchange.ClientExample.UseCases.ContextMenu
{
    /// <summary>
    ///     This will add an extry for the exit to the context menu
    /// </summary>
    [Export("contextmenu", typeof(IMenuItem))]
    public sealed class ExitMenuItem : ClickableMenuItem
    {
        /// <summary>
        /// Configure the exit menu item
        /// </summary>
        /// <param name="contextMenuTranslations"></param>
        [ImportingConstructor]
        public ExitMenuItem(IContextMenuTranslations contextMenuTranslations)
        {
            // automatically update the DisplayName
            contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IContextMenuTranslations.Exit));
            Id = "Z_Exit";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Close,
            };
            ClickAction = clickedItem =>
            {
                Application.Current.Shutdown();
            };
            this.ApplyIconForegroundColor(Brushes.DarkRed);
        }
    }
}
