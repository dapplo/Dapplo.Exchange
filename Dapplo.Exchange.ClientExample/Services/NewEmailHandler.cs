using Caliburn.Micro;
using Dapplo.CaliburnMicro;
using Dapplo.Exchange.ClientExample.UseCases.Mail.ViewModels;
using Microsoft.Exchange.WebServices.Data;

namespace Dapplo.Exchange.ClientExample.Services
{
    /// <summary>
    /// Handle email messages, and show a notification
    /// </summary>
    public class NewEmailHandler : IUiStartup, IHandle<EmailMessage>
    {
        private readonly ExchangeServiceContainer _exchangeServiceContainer;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Setup the event handling
        /// </summary>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="exchangeServiceContainer">ExchangeService</param>
        public NewEmailHandler(
            IEventAggregator eventAggregator,
            ExchangeServiceContainer exchangeServiceContainer
        )
        {
            _exchangeServiceContainer = exchangeServiceContainer;
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Handle the EmailMessage
        /// </summary>
        /// <param name="message"></param>
        public void Handle(EmailMessage message)
        {
            _eventAggregator.PublishOnUIThread(new NewEmailViewModel(message, _exchangeServiceContainer.Service));
        }

        /// <inheritdoc />
        public void Start()
        {
            _eventAggregator.Subscribe(this);
        }
    }
}
