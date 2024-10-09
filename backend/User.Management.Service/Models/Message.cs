using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace User.Management.Service.Models
{
    internal class Message
    {
        public List<MailboxAddress> RecipientAddresses { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(IEnumerable<string> RecipientAddresses, string subject, string content)
        {
            RecipientAddresses = new List<MailboxAddress>();
            RecipientAddresses.AddRange(recipients.Select( x=> new MailboxAddress(x)));
            Subject = subject;
            Content = content;
        }

    }
}