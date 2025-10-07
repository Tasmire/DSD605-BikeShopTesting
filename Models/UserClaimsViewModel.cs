using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DSD603_BikeShopDB.Models
{
    public class UserClaimsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public bool HasClaims => Claims.Any();
        public int ClaimCount => Claims.Count;
    }
}

