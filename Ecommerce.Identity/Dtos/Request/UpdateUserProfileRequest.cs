using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Identity.Dtos.Request
{
    public class UpdateUserProfileRequest
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public IFormFile? ProfileImage { get; set; }

        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
