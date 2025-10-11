using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entity
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string? WebhookSecret { get; set; }
    }
}
