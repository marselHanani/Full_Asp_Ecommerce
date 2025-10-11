using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Domain.Entity;
using Ecommerce.Domain.Repository.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repository.classes
{
    public class PaymentRepository(ApplicationDbContext context) : IPaymentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
    }
}
