using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;

namespace Ecommerce.Domain.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
    }
}
