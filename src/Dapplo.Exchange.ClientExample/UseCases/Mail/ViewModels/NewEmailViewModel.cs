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
        /// The actual email message
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
