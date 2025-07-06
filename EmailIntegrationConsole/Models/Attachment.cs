using System;

namespace EmailIntegrationConsole.Models
{
    /// <summary>
    /// Represents an email attachment with its metadata.
    /// </summary>
    public class EmailAttachment
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public long Size { get; set; }
        public bool IsInline { get; set; }
    }
}
