using Microsoft.AspNetCore.Identity;

namespace DSD603_BikeShopDB.Models
{
    public class RolesIndexViewModel
    {
        public List<IdentityRole> Roles { get; set; }
        public List<UserRolesViewModel> UserRoles { get; set; }
    }

}
