using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.Application.Dtos.Response
{
    public class ProductImageResponse
    {
        [JsonIgnore]
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsMain { get; set; }
    }
}
