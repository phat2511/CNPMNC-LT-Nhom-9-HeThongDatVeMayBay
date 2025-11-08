using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FlightAPI.Services
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        // Constructor nhận đối tượng Cloudinary đã được đăng ký ở Program.cs
        public CloudinaryStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // Thiết lập tham số upload
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "flight_banners", // Tạo thư mục con trên Cloudinary
                DisplayName = fileName,
                Overwrite = false, // Không ghi đè nếu tên bị trùng
                // Tùy chọn thêm: tạo phiên bản ảnh tối ưu hóa ngay sau khi upload
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            // Thực hiện upload
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            // Trả về URL công khai của file
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            // 1. Trích xuất Public ID từ URL
            // Cloudinary URL có dạng: .../vXXXXXX/folder/public_id.extension
            // Ta cần lấy "folder/public_id"
            var uri = new Uri(fileUrl);
            var path = uri.AbsolutePath;

            // Xóa phần mở rộng và dấu gạch chéo đầu/cuối
            var publicIdWithFolder = Path.GetFileNameWithoutExtension(path).Trim('/');

            // Nếu có thư mục con (ví dụ: flight_banners/12345), thì cần phải lấy cả folder + public_id.
            // Phương pháp dưới đây là đơn giản nhất để trích xuất public ID (bao gồm cả folder)

            // Tìm vị trí của folder name (flight_banners) trong path
            var folderIndex = path.IndexOf("/flight_banners/");
            if (folderIndex < 0)
            {
                throw new InvalidOperationException("Could not parse public ID from Cloudinary URL.");
            }

            // Lấy chuỗi bắt đầu từ flight_banners (Public ID)
            var publicId = path.Substring(folderIndex + 1).Split('.')[0];


            // 2. Thiết lập tham số xóa
            var deletionParams = new DeletionParams(publicId);

            // 3. Thực hiện xóa
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Error != null)
            {
                throw new Exception($"Cloudinary deletion failed: {deletionResult.Error.Message}");
            }
        }
    }

    // Class tiện ích để đọc cấu hình từ secrets.json
    public class CloudinarySettings
    {
        public string? CloudName { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    }
}