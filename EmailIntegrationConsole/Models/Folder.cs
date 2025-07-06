using System;

namespace EmailIntegrationConsole.Models
{
    /// <summary>
    /// Represents an email folder with its metadata.
    /// </summary>
    public class Folder
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public int TotalItemCount { get; set; }
        public int UnreadItemCount { get; set; }
    }
}
