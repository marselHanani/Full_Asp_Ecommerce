using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace Ecommerce.Application.Service
{
    public class FileService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file.Length > 0)
            {
                var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Images", folderName, uniqueName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using (var stream = File.Create(savePath))
                {
                    await file.CopyToAsync(stream);
                }
                return uniqueName;
            }
            throw new Exception("File is empty");
        }
        public void DeleteFile(string fileName, string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","Images", folderName, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
