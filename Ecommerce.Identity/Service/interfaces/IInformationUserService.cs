using Ecommerce.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Service.interfaces
{
    public interface IInformationUserService
    {
        Task<string?> GetUserNameByIdAsync(Guid userId);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
    }
}
