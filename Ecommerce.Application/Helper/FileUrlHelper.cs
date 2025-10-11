using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Application.Helper
{
    public class FileUrlHelper
    {
        public string GetImageUrl(string fileName, string folderName, HttpRequest request)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            return $"{request.Scheme}://{request.Host}/Images/{folderName}/{fileName}";
        }
    }
}
