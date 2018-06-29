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
    [Menu("systemtray")]
    public sealed class TitleMenuItem : MenuItem
    {
        /// <summary>
        /// Configure the title menu item
        /// </summary>
        /// <param name="contextMenuTranslations"></param>
        public TitleMenuItem(IContextMenuTranslations contextMenuTranslations)
        {
            // automatically update the DisplayName
            contextMenuTranslations.CreateDisplayNameBinding(this, nameof(IContextMenuTranslations.Title));
            Id = "A_Title";
            Style = MenuItemStyles.Title;

            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Email,
                Background = Brushes.Transparent,
                Foreground = Brushes.Black
            };
            this.ApplyIconForegroundColor(Brushes.DarkRed);
        }
    }
}
