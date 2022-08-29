using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ManageMoneyServer.Models
{
    public class User : IdentityUser
    {
        public List<Portfolio> Portfolios { get; set; }
    }
}
