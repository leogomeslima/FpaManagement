using System.Net;
using System.Net.Mail;
using FpaManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FpaManagement.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            var message = CreateMailMessage(to, subject, body, isHtml);

            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, string mimeType, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = CreateSmtpClient();
            var message = CreateMailMessage(to, subject, body, true);

            var stream = new MemoryStream(attachment);
            var attachmentItem = new Attachment(stream, attachmentName, mimeType);
            message.Attachments.Add(attachmentItem);

            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email with attachment sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
            throw;
        }
    }

    public async Task SendBulkEmailsAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default)
    {
        var tasks = recipients.Select(to => SendEmailAsync(to, subject, body, true, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private SmtpClient CreateSmtpClient()
    {
        var host = _configuration["Email:Host"] ?? "smtp.gmail.com";
        var port = int.Parse(_configuration["Email:Port"] ?? "587");
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

        var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
                ? new NetworkCredential(username, password)
                : CredentialCache.DefaultNetworkCredentials
        };

        return client;
    }

    private MailMessage CreateMailMessage(string to, string subject, string body, bool isHtml)
    {
        var fromEmail = _configuration["Email:From"] ?? "noreply@fpamanagement.com";
        var fromName = _configuration["Email:FromName"] ?? "FPA Management";

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        message.To.Add(to);

        return message;
    }
}
