#region Dapplo 2016-2018 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2018 Dapplo
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

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dapplo.ActiveDirectory;
using Dapplo.ActiveDirectory.Entities;
using Dapplo.Exchange.Entity;
using Dapplo.Log;
using Dapplo.Utils;
using Microsoft.Exchange.WebServices.Data;

#endregion

namespace Dapplo.Exchange
{
    /// <summary>
    ///     This is a wrapper with a lot of convenience for the ExchangeService
    /// TODO: Make interface
    /// TODO: Implement Repository pattern?
    /// </summary>
    public class ExchangeServiceContainer
    {
        private static readonly LogSource Log = new LogSource();
        private readonly bool _allowSelfSignedCertificates;
        private readonly IExchangeSettings _exchangeSettings;

        /// <summary>
        ///     Create an exchange service wrapper with the supplied settings (or null if defaults)
        /// </summary>
        /// <param name="exchangeSettings">null for default</param>
        public ExchangeServiceContainer(IExchangeSettings exchangeSettings = null)
        {
            _exchangeSettings = exchangeSettings ?? new ExchangeSettings();
            _allowSelfSignedCertificates = _exchangeSettings.AllowSelfSignedCertificates;
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
        }

        /// <summary>
        ///     Access the underlying ExchangeService if you want to do something which is not available yet.
        ///     This might be removed as soon as more functionality is available.
        /// </summary>
        public ExchangeService Service { get; private set; }

        /// <summary>
        ///     Initialize the exchange server connection
        /// </summary>
        public ExchangeServiceContainer Initialize()
        {
            Service = new ExchangeService(_exchangeSettings.VersionToUse)
            {
                UseDefaultCredentials = _exchangeSettings.UseDefaultCredentials
            };

            if (!_exchangeSettings.UseDefaultCredentials)
            {
                Log.Debug().WriteLine("Using credentials from the settings: {0}", _exchangeSettings.Username);
                Service.Credentials = new NetworkCredential(_exchangeSettings.Username, _exchangeSettings.Password);
            }

            if (string.IsNullOrEmpty(_exchangeSettings.ExchangeUrl))
            {
                Log.Debug().WriteLine("Trying to resolve the email-address for the current user: {0}", Environment.UserName);
                var emailAddress = Query.ForUser(Environment.UserName).Execute<IAdUser>().FirstOrDefault()?.Email;
                if (emailAddress == null)
                {
                    return this;
                }
                Log.Debug().WriteLine("Found Email-address for the current user: {0}, using auto-discovery.", emailAddress);
                if (_exchangeSettings.AllowRedirectUrl)
                {
                    Service.AutodiscoverUrl(emailAddress, RedirectionUrlValidationCallback);
                }
                else
                {
                    Service.AutodiscoverUrl(emailAddress);
                }
                Log.Debug().WriteLine("Found Url {0}", Service.Url);
            }
            else
            {
                Log.Debug().WriteLine("Using exchange Url from the settings: {0}", _exchangeSettings.ExchangeUrl);
                Service.Url = new Uri(_exchangeSettings.ExchangeUrl);
            }

            return this;
        }

        /// <summary>
        ///     Use this to get notified of events, this uses StreamingNotifications
        /// </summary>
        /// <param name="wellKnownFolderName">WellKnownFolderName to look to, Inbox if none is specified</param>
        /// <param name="eventTypes">params EventType, if nothing specified than EventType.NewMail is taken</param>
        /// <returns>IObservable which publishes NotificationEvent</returns>
        public IObservable<NotificationEvent> Observe(WellKnownFolderName wellKnownFolderName = WellKnownFolderName.Inbox, params EventType[] eventTypes)
        {
            if (eventTypes == null || eventTypes.Length == 0)
            {
                eventTypes = new[] { EventType.NewMail };
            }

            return Observable.Create<NotificationEvent>(
                observer =>
                {
                    try
                    {
                        var streamingSubscription = Service.SubscribeToStreamingNotifications(new FolderId[] { wellKnownFolderName }, eventTypes);

                        var connection = new StreamingSubscriptionConnection(Service, 1);
                        connection.AddSubscription(streamingSubscription);
                        connection.OnNotificationEvent += (sender, notificationEventArgs) =>
                        {
                            foreach (var notificationEvent in notificationEventArgs.Events)
                            {
                                observer.OnNext(notificationEvent);
                            }
                        };
                        // Handle errors
                        connection.OnSubscriptionError += (sender, subscriptionErrorEventArgs) => observer.OnError(subscriptionErrorEventArgs.Exception);

                        // Use this to 
                        bool disposedConnection = false;

                        // As the time to live is maximum 30 minutes, the connection is closed by the server
                        connection.OnDisconnect += (sender, subscriptionErrorEventArgs) =>
                        {
                            if (subscriptionErrorEventArgs.Exception != null || disposedConnection)
                            {
                                return;
                            }
                            // Connection closed by server, just reconnect
                            // See: https://msdn.microsoft.com/en-us/library/office/hh312849.aspx
                            // "This behavior is expected and is not an error condition"
                            connection.Open();
                        };

                        // Start the connection
                        connection.Open();

                        // Return a disposable which disposed the connection and unsubscribes the subscription
                        return SimpleDisposable.Create(() =>
                        {
                            disposedConnection = true;
                            connection.Dispose();
                            streamingSubscription.Unsubscribe();
                        });
                    }
                    catch (Win32Exception e)
                    {
                        observer.OnError(e);
                        return Disposable.Empty;
                    }
                });
        }

        /// <summary>
        /// Retrieve Appointment
        /// </summary>
        /// <param name="limit">int default is 500</param>
        /// <returns>IList of Appointment</returns>
        public Task<IList<Appointment>> RetrieveAppointmentsAsync(int limit = 20)
        {
            var tcs = new TaskCompletionSource<IList<Appointment>>();

            try
            {
                var properties = new PropertySet(ItemSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, new ExtendedPropertyDefinition(0x10f4, MapiPropertyType.Boolean));
                Service.BeginSyncFolderItems(asyncResult =>
                {
                    try
                    {
                        var result = Service.EndSyncFolderItems(asyncResult);
                        tcs.TrySetResult(result.Select(itemChange => itemChange.Item).OfType<Appointment>().ToList());
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, null, WellKnownFolderName.Calendar, properties, null, limit, SyncFolderItemsScope.NormalItems, null);
            }
            catch
            {
                tcs.TrySetResult(Enumerable.Empty<Appointment>().ToList());
                throw;
            }

            return tcs.Task;
        }

        /// <summary>
        ///     Retrieve appointment items from the exchange service
        /// </summary>
        /// <returns>IEnumerable with appointment</returns>
        public IEnumerable<Appointment> RetrieveAppointments(DateTime startDate, DateTime endDate, int maxItems = 20)
        {
            // Limit the properties returned to the appointment's subject, start time, and end time.
            var propertySet = new PropertySet(ItemSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End);
            // Initialize the calendar folder object with only the folder ID. 
            var calendar = CalendarFolder.Bind(Service, WellKnownFolderName.Calendar, propertySet);
            // Set the start and end time and number of appointments to retrieve.
            var calendarView = new CalendarView(startDate, endDate, maxItems);
            // Retrieve a collection of appointments by using the calendar view.
            return calendar.FindAppointments(calendarView);
        }

        /// <summary>
        ///     Retrieve contact items from the exchange service
        /// </summary>
        /// <returns>FindItemsResults with Item</returns>
        public IEnumerable<Contact> RetrieveContacts(int limit = 100)
        {
            // Limit the properties returned
            // Initialize the calendar folder object with only the folder ID. 
            var contactsFolder = ContactsFolder.Bind(Service, WellKnownFolderName.Contacts);
            // Set the amount of contacts to return
            var itemView = new ItemView(limit);

            // For later
            var propertySet = new PropertySet(ItemSchema.Attachments, ContactSchema.GivenName, ContactSchema.Surname, ContactSchema.EmailAddresses);

            // Retrieve a collection of items by using the itemview.
            var itemsList = contactsFolder.FindItems(itemView);

            return itemsList.OfType<Contact>().Select(item =>
            {
                var contact = Contact.Bind(Service, item.Id, propertySet);
                foreach (var fileAttachment in contact.Attachments.OfType<FileAttachment>())
                {
                    if (!fileAttachment.IsContactPhoto)
                    {
                        continue;
                    }
                    Log.Verbose().WriteLine("Loading contact photo attachment for {1}, {0}", contact.GivenName, contact.Surname);
                    // Load the attachment to access the content.
                    fileAttachment.Load();
                }
                return contact;
            });
        }


        /// <summary>
        /// Retrieve email messages
        /// </summary>
        /// <param name="wellKnownFolderName">WellKnownFolderName</param>
        /// <param name="limit">int default is 500</param>
        /// <returns>IList of EmailMessage</returns>
        public Task<IList<EmailMessage>> RetrieveMailsAsync(WellKnownFolderName wellKnownFolderName = WellKnownFolderName.Inbox, int limit = 500)
        {
            return RetrieveMailsAsync(new FolderId(wellKnownFolderName), limit);
        }

        /// <summary>
        /// Retrieve email messages
        /// </summary>
        /// <param name="folderId">FolderId</param>
        /// <param name="limit">int default is 500</param>
        /// <returns>IList of EmailMessage</returns>
        public Task<IList<EmailMessage>> RetrieveMailsAsync(FolderId folderId, int limit = 500)
        {
            var tcs = new TaskCompletionSource<IList<EmailMessage>>();

            try
            {
                var properties = new PropertySet(BasePropertySet.IdOnly,
                    EmailMessageSchema.From, ItemSchema.Subject, ItemSchema.HasAttachments, ItemSchema.DateTimeReceived,
                    new ExtendedPropertyDefinition(0x10f4, MapiPropertyType.Boolean));
                Service.BeginSyncFolderItems(asyncResult =>
                {
                    try
                    {
                        var result = Service.EndSyncFolderItems(asyncResult);
                        tcs.TrySetResult(result.Select(itemChange => itemChange.Item).OfType<EmailMessage>().ToList());
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }, null, folderId, properties, null, limit, SyncFolderItemsScope.NormalItems, null);
            }
            catch
            {
                tcs.TrySetResult(Enumerable.Empty<EmailMessage>().ToList());
                throw;
            }

            return tcs.Task;
        }

        /// <summary>
        ///     Retrieve MeetingRequest from the exchange service Inbox
        /// </summary>
        /// <returns>MeetingRequests</returns>
        public IEnumerable<MeetingRequest> RetrieveMeetingRequests(int maxItems = 20)
        {
            // Limit the properties returned
            // Initialize the calendar folder object with only the folder ID. 
            var mailFolder = Folder.Bind(Service, WellKnownFolderName.Inbox);
            // Set the amount of contacts to return
            var itemView = new ItemView(maxItems);

            // Retrieve a collection of items by using the itemview.
            var itemsList = mailFolder.FindItems(itemView);

            // What properties we want to know
            //var propertySet = new PropertySet(MeetingRequestSchema.MyResponseType, MeetingRequestSchema.Location, MeetingMessageSchema.ResponseType);

            return itemsList.OfType<MeetingRequest>();
        }

        #region Validation

        /// <summary>
        ///     This method validates the redirect Url, this is only called when a redirection is used
        /// </summary>
        /// <param name="redirectionUrl"></param>
        /// <returns>true if the redirect url is okay</returns>
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            var result = false;

            var redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        ///     This is needed for allowing the self-signed certificates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns>bool if the certificate is allowed</returns>
        private bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == 0)
            {
                // In all other cases, return false.
                return false;
            }
            if (chain?.ChainStatus == null)
            {
                return true;
            }
            foreach (var status in chain.ChainStatus)
            {
                if (certificate.Subject == certificate.Issuer && status.Status == X509ChainStatusFlags.UntrustedRoot)
                {
                    if (_allowSelfSignedCertificates)
                    {
                        // Self-signed certificates with an untrusted root are valid. 
                        continue;
                    }
                    return false;
                }
                if (status.Status != X509ChainStatusFlags.NoError)
                {
                    // If there are any other errors in the certificate chain, the certificate is invalid,
                    // so the method returns false.
                    return false;
                }
            }

            // When processing reaches this line, the only errors in the certificate chain are 
            // untrusted root errors for self-signed certificates. These certificates are valid
            // for default Exchange server installations, so return true.
            return true;
        }

        #endregion
    }
}