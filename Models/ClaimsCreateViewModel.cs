using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DSD603_BikeShopDB.Models
{
    public class ClaimsCreateViewModel
    {
        // User selection
        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "User")]
        public string SelectedUserName { get; set; } = string.Empty;
        public SelectList Users { get; set; } = new(Enumerable.Empty<SelectListItem>());

        // Claim type and value
        [Required(ErrorMessage = "Claim type is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Claim type must be between 1 and 100 characters")]
        [Display(Name = "Claim Type")]
        public string ClaimType { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Claim value cannot exceed 255 characters")]
        [Display(Name = "Claim Value")]
        public string? ClaimValue { get; set; }

        // Status message for feedback
        public string? StatusMessage { get; set; }

        public int TotalClaimTypes { get; set; }
        public int TotalClaims { get; set; }
        public int UsersWithClaims { get; set; }
        public List<ClaimTypeGroup> ClaimTypeGroups { get; set; } = new();
        public List<UserClaimDetail> AllUserClaims { get; set; } = new();
    }
}