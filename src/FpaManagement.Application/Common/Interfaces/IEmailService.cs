namespace FpaManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, string mimeType, CancellationToken cancellationToken = default);
    Task SendBulkEmailsAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default);
}
