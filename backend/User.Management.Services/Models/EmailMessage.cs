using MimeKit;

namespace User.Management.Service.Models
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            //To.AddRange(to.Select( x=> new MailboxAddress("email", x)));
            To.AddRange(to.Select(x => new MailboxAddress(string.Empty, x)));
            Subject = subject;
            Content = content;
        }

    }
}