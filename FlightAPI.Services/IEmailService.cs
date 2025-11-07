using System.Threading.Tasks; // (Nhớ "dùng" (using) cái này)
using Task = System.Threading.Tasks.Task;
namespace FlightAPI.Services
{
    public interface IEmailService
    {
        // "Nhiệm vụ": Gửi mail, nhận 'mail đích', 'tiêu đề', 'ruột' (HTML)
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}