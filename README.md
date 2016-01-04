# Dapplo.Exchange
A simple exchange wrapper which should allow quick access to the calendar and email.

Uses the nuget package "Microsoft.Exchange.WebServices"

Currently the functionality is VERY limited:
* Connection can be made:
 * To a specified Url
 * Connect with specified credentials or default credentials
 * Connect with autodiscover-url by connecting to the Domain-controller (LDAP) to retrieve the email for the current user
* Only a few methods are available:
 * Creating an email object (is quite useless)
 * Retrieve appointments from the calendar
