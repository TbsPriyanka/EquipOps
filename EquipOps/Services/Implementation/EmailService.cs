using EquipOps.API.Services.Interface;
using EquipOps.BAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace EquipOps.API.Services.Implementation
{
    public class EmailService (ILogger<EmailService> _logger, IConfiguration _config) : IEmailService
    {
        public async Task SendForgotPasswordEmailAsync(string email, string userName, string resetLink)
        {
            var subject = "🔐 Reset Your Password – Login Engine";

            var body = $@"
<table width='100%' cellspacing='0' cellpadding='0' style='font-family: Arial, sans-serif;'>
    <tr>
        <td style='padding: 20px; background-color: #f7f7f7;'>
            <table width='600' align='center' cellpadding='0' cellspacing='0' style='background: #ffffff; border-radius: 8px; padding: 20px;'>
                <tr>
                    <td style='font-size: 18px; font-weight: bold; color: #333;'>Hi {userName},</td>
                </tr>
                <tr>
                    <td style='padding-top: 15px; font-size: 15px; color: #555;'>
                        You recently requested to reset your password for your Login Engine account.
                        Click the button below to reset it.
                    </td>
                </tr>

                <tr>
                    <td style='padding-top: 25px; text-align: center;'>
                        <a href='{resetLink}' 
                           style='background-color: #4f46e5;
                                  padding: 12px 20px;
                                  border-radius: 6px;
                                  color: #ffffff;
                                  text-decoration: none;
                                  font-size: 16px;
                                  font-weight: bold;'>
                            Reset Password
                        </a>
                    </td>
                </tr>

                <tr>
                    <td style='padding-top: 20px; font-size: 14px; color: #777;'>
                        If the button doesn’t work, copy and paste this link into your browser:<br/>
                        <span style='color: #4f46e5;'>{resetLink}</span>
                    </td>
                </tr>

                <tr>
                    <td style='padding-top: 15px; font-size: 14px; color: #777;'>
                        This link is valid for <strong>15 minutes</strong>.  
                        If you didn’t request a password reset, you can safely ignore this email.
                    </td>
                </tr>

                <tr>
                    <td style='padding-top: 30px; font-size: 15px; color: #555;'>
                        Best Regards,<br/>
                        <strong>Team Support</strong><br/>
                        AI Recruiter Portal
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
";

            await SendEmailAsync(email, subject, body, true);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPortString = _config["Smtp:Port"];
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Password"];
            var fromMail = _config["Smtp:From"];
            var enableSslString = _config["Smtp:EnableSsl"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortString) ||
                string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromMail))
            {
                _logger.LogError("SMTP configuration is missing.");
                return;
            }

            if (!int.TryParse(smtpPortString, out int smtpPort))
            {
                _logger.LogError("Invalid SMTP port: {Port}", smtpPortString);
                return;
            }

            bool enableSsl = bool.TryParse(enableSslString, out var ssl) && ssl;

            using var client = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);

            _logger.LogInformation("{Subject} email sent to {Email}", subject, toEmail);
        }
    }
}
