using System;

namespace Ecommerce.Application.Dtos.Response
{
    public class ReplyResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string? AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ReplyResponse> Replies { get; set; } = new List<ReplyResponse>();
    }
}