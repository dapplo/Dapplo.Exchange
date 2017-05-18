using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Dapplo.Exchange.ClientExample.Utils;
using Microsoft.Exchange.WebServices.Data;

namespace Dapplo.Exchange.ClientExample.UseCases.Mail.ViewModels
{
    /// <summary>
    /// Defines the view model for an email message
    /// </summary>
    public class NewEmailViewModel : ToastBaseViewModel
    {
        private readonly ExchangeService _exchangeService;
        /// <summary>
        /// The actuall email message
        /// </summary>
        public EmailMessage Email { get; }

#if DEBUG
        /// <summary>
        /// Design-Time constructor
        /// </summary>
        public NewEmailViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            Email = new EmailMessage(new ExchangeService())
            {
                Subject = "Example subject"
            };
        }
#endif
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailMessage">EmailMessage</param>
        /// <param name="exchangeService">ExchangeService for handling certain functions</param>
        public NewEmailViewModel(EmailMessage emailMessage, ExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
            Email = emailMessage;
        }

        /// <summary>
        /// Gets the image in the email
        /// </summary>
        public IEnumerable<ImageContainer> Images
        {
            get
            {
#if DEBUG
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    yield return new ImageContainer();
                    yield return new ImageContainer();
                    yield break;
                }
#endif

                if (!Email.HasAttachments)
                {
                    yield break;
                }

                foreach (var attachment in Email.Attachments)
                {
                    if (!ImageContainer.IsImage(attachment))
                    {
                        continue;
                    }
                    yield return new ImageContainer(attachment);
                }
            }
        }

        /// <summary>
        /// Delete the email
        /// </summary>
        public void Delete()
        {
           // Delete to the deleted items
            _exchangeService.DeleteItems(
                new[] {Email.Id},
                DeleteMode.MoveToDeletedItems,
                SendCancellationsMode.SendToNone,
                AffectedTaskOccurrence.SpecifiedOccurrenceOnly);
        }
    }
}
