# Product Requirements Document (PRD)
## Office 365 Email Integration Console Application

### Project Overview
Create a .NET C# console application that integrates with Office 365 using delegated permissions to access user email data through Microsoft Graph API. The application will serve as a prototype for CRM email integration functionality.

### Technical Stack
- **Platform**: .NET 6.0 or higher
- **Language**: C# 
- **Application Type**: Console Application
- **Authentication**: OAuth 2.0 with Delegated Permissions
- **API**: Microsoft Graph API v1.0
- **Authentication Library**: Microsoft.Identity.Client (MSAL)

### Authentication Requirements

#### Azure AD App Registration Configuration
- **Application Type**: Public client (mobile and desktop applications)
- **Redirect URI**: `http://localhost:8080`
- **Required API Permissions** (Microsoft Graph - Delegated):
  - `Mail.Read` - Read user mail
  - `User.Read` - Sign in and read user profile  
  - `offline_access` - Maintain access to data

#### Authentication Flow
- Use **Authorization Code Flow with PKCE**
- Support **silent token acquisition** for subsequent runs
- Handle **token refresh** automatically
- Store tokens securely in user profile directory

### Functional Requirements

#### 1. User Authentication
**Feature**: Login to Office 365
- **Input**: User interaction (no manual credentials)
- **Process**: 
  - Open browser for Microsoft login
  - Handle OAuth callback
  - Acquire and store access/refresh tokens
- **Output**: Successful authentication confirmation
- **Error Handling**: Display clear error messages for failed authentication

#### 2. Folder Listing
**Feature**: List all email folders
- **Input**: Authenticated user session
- **Process**: Call Microsoft Graph API to retrieve folder list
- **Output**: Display numbered list of folders with names and item counts
- **Format**: 
  ```
  1. Inbox (45 items)
  2. Sent Items (123 items)
  3. Drafts (2 items)
  4. Deleted Items (67 items)
  5. Custom Folder (8 items)
  ```

#### 3. Email Listing
**Feature**: List first 100 emails from selected folder
- **Input**: Folder selection (by number from previous list)
- **Process**: Retrieve first 100 emails from selected folder
- **Output**: Display numbered list with email summary
- **Format**:
  ```
  Folder: Inbox
  1. [2024-01-15 09:30] From: john@example.com | Subject: Project Update | Has Attachments: Yes
  2. [2024-01-15 08:45] From: mary@company.com | Subject: Meeting Tomorrow | Has Attachments: No
  3. [2024-01-14 17:22] From: support@service.com | Subject: Ticket #12345 | Has Attachments: Yes
  ```
- **Sorting**: Most recent emails first
- **Required Fields**: Date, From address, Subject, Attachment indicator

#### 4. Email Content Display
**Feature**: Show full content of selected email
- **Input**: Email selection (by number from previous list)
- **Process**: Retrieve complete email details
- **Output**: Display full email information
- **Format**:
  ```
  =================================
  EMAIL DETAILS
  =================================
  From: john@example.com
  To: user@company.com
  Date: 2024-01-15 09:30:45
  Subject: Project Update
  
  Body:
  [Full email body content here]
  
  Attachments: 2 file(s)
  - document.pdf (245 KB)
  - image.png (156 KB)
  =================================
  ```

#### 5. Attachment Listing
**Feature**: Show attachments of selected email
- **Input**: Email with attachments
- **Process**: Retrieve attachment metadata
- **Output**: Display detailed attachment information
- **Format**:
  ```
  =================================
  ATTACHMENTS
  =================================
  Email: Project Update
  
  1. Filename: document.pdf
     Size: 245,760 bytes (240 KB)
     Content Type: application/pdf
     ID: AAMkAGQ...
  
  2. Filename: image.png
     Size: 159,744 bytes (156 KB)
     Content Type: image/png
     ID: AAMkAGQ...
  =================================
  ```

### User Interface Requirements

#### Console Menu Structure
```
=================================
OFFICE 365 EMAIL INTEGRATION
=================================
Status: [Not Authenticated / Authenticated as user@company.com]

1. Login to Office 365
2. List Folders
3. List Emails from Folder
4. Show Email Content
5. Show Email Attachments
6. Logout
7. Exit

Select option (1-7): _
```

#### Navigation Flow
1. **Start** → User must authenticate first
2. **After Login** → All options available
3. **Folder Selection** → Returns to main menu after listing
4. **Email Selection** → Returns to main menu after viewing
5. **Error States** → Return to main menu with error message
6. **Logout** → Clear stored tokens and return to initial state

### Technical Implementation Requirements

#### 1. Project Structure
```
EmailIntegrationConsole/
├── Program.cs                 // Main entry point
├── Services/
│   ├── AuthenticationService.cs
│   ├── GraphService.cs
│   └── TokenCacheService.cs
├── Models/
│   ├── EmailMessage.cs
│   ├── Folder.cs
│   └── Attachment.cs
├── Utilities/
│   ├── ConsoleHelper.cs
│   └── ConfigurationHelper.cs
└── appsettings.json
```

#### 2. Configuration (appsettings.json)
```json
{
  "AzureAd": {
    "ClientId": "your-client-id",
    "TenantId": "common",
    "RedirectUri": "http://localhost:8080",
    "Scopes": [
      "https://graph.microsoft.com/Mail.Read",
      "https://graph.microsoft.com/User.Read"
    ]
  },
  "GraphApi": {
    "BaseUrl": "https://graph.microsoft.com/v1.0",
    "MaxRetries": 3,
    "RetryDelay": 1000
  }
}
```

#### 3. Required NuGet Packages
- `Microsoft.Identity.Client` (latest stable)
- `Microsoft.Graph` (latest stable)
- `Microsoft.Graph.Auth` (latest stable)
- `Microsoft.Extensions.Configuration` (for settings)
- `Microsoft.Extensions.Configuration.Json`
- `Newtonsoft.Json` (for JSON handling)

#### 4. Error Handling Requirements
- **Network Errors**: Retry mechanism with exponential backoff
- **Authentication Errors**: Clear error messages and re-authentication prompt
- **API Rate Limiting**: Respect Microsoft Graph throttling
- **Invalid Input**: Validate user selections and provide helpful feedback
- **Token Expiration**: Automatic refresh with fallback to re-authentication

#### 5. Security Requirements
- **Token Storage**: Use MSAL token cache in user profile directory
- **No Hardcoded Secrets**: Client ID only (public client)
- **Secure Communication**: HTTPS only for all API calls
- **Token Scope**: Minimal required permissions only

### Data Models

#### EmailMessage Model
```csharp
public class EmailMessage
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string BodyPreview { get; set; }
    public DateTime ReceivedDateTime { get; set; }
    public string FromAddress { get; set; }
    public List<string> ToAddresses { get; set; }
    public bool HasAttachments { get; set; }
    public List<EmailAttachment> Attachments { get; set; }
}
```

#### Folder Model
```csharp
public class Folder
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public int TotalItemCount { get; set; }
    public int UnreadItemCount { get; set; }
}
```

#### EmailAttachment Model
```csharp
public class EmailAttachment
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public bool IsInline { get; set; }
}
```

### Performance Requirements
- **Startup Time**: Application should start within 3 seconds
- **Authentication**: Login flow should complete within 30 seconds
- **API Calls**: Each operation should complete within 10 seconds
- **Memory Usage**: Maximum 100MB RAM usage
- **Token Refresh**: Should be transparent to user (< 2 seconds)

### Testing Requirements
- **Unit Tests**: Core services should have 80%+ code coverage
- **Integration Tests**: Test with real Microsoft Graph API
- **Error Scenarios**: Test all error conditions
- **Authentication Flow**: Test complete OAuth flow
- **Token Management**: Test token refresh and expiration scenarios

### Success Criteria
1. **Authentication**: User can successfully log in with Office 365 account
2. **Folder Access**: Application displays all accessible email folders
3. **Email Retrieval**: Can retrieve and display first 100 emails from any folder
4. **Content Display**: Full email content is properly formatted and displayed
5. **Attachment Handling**: Attachment metadata is correctly retrieved and displayed
6. **Error Resilience**: Application handles network issues and API errors gracefully
7. **User Experience**: Intuitive console interface with clear navigation

### Future Considerations
- **Email Search**: Add search functionality by subject, sender, or content
- **Attachment Download**: Allow downloading attachments to local storage
- **Multiple Accounts**: Support for multiple Office 365 accounts
- **Configuration UI**: Simple configuration interface for Azure AD settings
- **Logging**: Comprehensive logging for debugging and monitoring

### Constraints and Limitations
- **Console Only**: No graphical user interface required
- **Read-Only**: No email modification or sending capabilities
- **Public Client**: Cannot store client secrets securely
- **Single User**: Designed for individual user authentication only
- **Windows/Mac/Linux**: Must work cross-platform where .NET is supported