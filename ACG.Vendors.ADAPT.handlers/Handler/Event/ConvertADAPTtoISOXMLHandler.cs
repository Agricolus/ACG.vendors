using System;
using System.Threading;
using System.Threading.Tasks;
using ACG.Common.CQRS.Event;
using MediatR;

namespace ACG.Vendors.ADAPT.handlers.Handler.Event
{
    public class ConvertADAPTtoISOXMLHandler : INotificationHandler<ConvertADAPTtoISOXML>
    {
        public Task Handle(ConvertADAPTtoISOXML notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("yoyoyoy");
        }
    }
}
