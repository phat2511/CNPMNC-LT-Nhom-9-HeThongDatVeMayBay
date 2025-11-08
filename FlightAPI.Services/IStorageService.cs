using System.IO;
using System.Threading.Tasks;

namespace FlightAPI.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Tải lên một file từ Stream và trả về URL công khai.
        /// </summary>
        /// <param name="fileStream">Dữ liệu file dưới dạng Stream.</param>
        /// <param name="fileName">Tên gốc của file (ví dụ: banner.jpg).</param>
        /// <param name="contentType">Loại file (ví dụ: image/jpeg).</param>
        /// <returns>URL công khai của file đã tải lên.</returns>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Xóa một file khỏi dịch vụ lưu trữ.
        /// </summary>
        /// <param name="fileUrl">URL công khai hoặc Public ID của file cần xóa.</param>
        Task DeleteFileAsync(string fileUrl);
    }
}