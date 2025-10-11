using System;
using System.Collections.Generic;

namespace Ecommerce.Application.Dtos.Response
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public string? AuthorName { get; set; }
        public string? ProductName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ReplyResponse> Comments { get; set; } = new List<ReplyResponse>();
    }
}
