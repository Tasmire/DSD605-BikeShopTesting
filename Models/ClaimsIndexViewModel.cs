using System.Collections.Generic;

namespace DSD603_BikeShopDB.Models
{
    public class ClaimsIndexViewModel
    {
        public List<UserClaimsViewModel> UserClaims { get; set; } = new();
        public List<string> Users { get; set; } = new();
        public string? StatusMessage { get; set; }
    }
}