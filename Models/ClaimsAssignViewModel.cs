using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DSD603_BikeShopDB.Models
{
    public class ClaimsAssignViewModel
    {
        [Required(ErrorMessage = "Please select a user")]
        public string SelectedUserName { get; set; } = string.Empty;

        public SelectList Users { get; set; } = new(Enumerable.Empty<SelectListItem>());

        [Required(ErrorMessage = "Claim type is required")]
        public string ClaimType { get; set; } = string.Empty;

        public string? ClaimValue { get; set; }

        public string? StatusMessage { get; set; }
    }
}