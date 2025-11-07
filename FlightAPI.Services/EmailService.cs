using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks; // <--- "BÙA" (ALIAS) NÊN "Ở" (BE) "SAU" (AFTER) "THẰNG" NÀY
using Task = System.Threading.Tasks.Task; // <--- "DÁN" (PASTE) "HÀNG" (LINE) NÀY VÀO
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;

namespace FlightAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger; // (Thêm "loa" (logger) để "báo" (report) lỗi)

        // (Sửa "Bàn giao" (Constructor) - "nhận" (inject) 2 "món")
        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger; // (Gán "loa")

            // "Lắp Ổ Khóa" (Như cũ)
            var apiKey = _config["Brevo:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Brevo ApiKey bị 'trống' trong appsettings.");
            }
            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                // "Chỉ" (Only) "Thêm" (Add) "nếu" (if) "chưa" (not) "có" (exist)
                Configuration.Default.ApiKey.Add("api-key", apiKey);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var apiInstance = new TransactionalEmailsApi();

                var fromEmail = _config["Brevo:FromEmail"];
                var fromName = _config["Brevo:FromName"];
                var sender = new SendSmtpEmailSender(fromName, fromEmail);

                var to = new List<SendSmtpEmailTo>
                {
                    new SendSmtpEmailTo(toEmail)
                };

                var sendSmtpEmail = new SendSmtpEmail(
                    sender: sender,
                    to: to,
                    htmlContent: htmlContent,
                    subject: subject
                );

                await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            }
            catch (Exception ex)
            {
                // (Nếu "bưu tá" Brevo "chết" (fail) -> "la làng" (log) lỗi
                //  để "não" AuthService "biết" (know) mà "in" (print) ra "Tủ" (console))
                _logger.LogError(ex, "LỖI BƯU TÁ (Brevo): Gửi đến {Email} thất bại.", toEmail);
                throw; // (Ném lỗi "ra ngoài" cho "não" Auth "bắt")
            }
        }
    }
}