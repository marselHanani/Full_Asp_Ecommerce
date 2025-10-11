using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Identity.Data;
using Ecommerce.Identity.Entities;
using Ecommerce.Identity.Service.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Identity.Service.classes
{
    public class InformationUserService(AppIdentityDbContext context) : IInformationUserService
    {
        private readonly AppIdentityDbContext _context = context;

        public async Task<string?> GetUserNameByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
            return user?.FullName;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found");
            return user;
        }
    }
}
