using System;
using System.Collections.Generic;

namespace EmailIntegrationConsole.Models
{
    /// <summary>
    /// Represents an email message with its metadata and content.
    /// </summary>
    public class EmailMessage
    {
        public string? Id { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? BodyPreview { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public string? FromAddress { get; set; }
        public List<string>? ToAddresses { get; set; } = new List<string>();
        public bool HasAttachments { get; set; }
        public List<EmailAttachment>? Attachments { get; set; } = new List<EmailAttachment>();
    }
}
