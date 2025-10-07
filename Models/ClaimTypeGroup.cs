using System.Collections.Generic;

namespace DSD603_BikeShopDB.Models
{
    public class ClaimTypeGroup
    {
        public string ClaimType { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
        public int UsageCount { get; set; }
    }
}