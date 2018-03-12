using System.ComponentModel.Composition;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using Dapplo.Exchange.ClientExample.UseCases.Mail.ViewModels;
using Microsoft.Exchange.WebServices.Data;

namespace Dapplo.Exchange.ClientExample.Services
{
    /// <summary>
    /// Handle email messages, and show a notification
    /// </summary>
    [UiStartupAction]
    public class NewEmailHandler : IUiStartupAction, IHandle<EmailMessage>
    {
        private readonly ExchangeService _exchangeService;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Setup the event handling
        /// </summary>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="exchangeService">ExchangeService</param>
        [ImportingConstructor]
        public NewEmailHandler(
            IEventAggregator eventAggregator,
            ExchangeService exchangeService
            )
        {
            _exchangeService = exchangeService;
            _eventAggregator = eventAggregator;
            
        }

        /// <inheritdoc />
        public void Start()
        {
            _eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// Handle the EmailMessage
        /// </summary>
        /// <param name="message"></param>
        public void Handle(EmailMessage message)
        {
            _eventAggregator.PublishOnUIThread(new NewEmailViewModel(message, _exchangeService));
        }

    }
}
