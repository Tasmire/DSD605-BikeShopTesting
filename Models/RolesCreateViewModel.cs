using System.ComponentModel.DataAnnotations;

namespace DSD603_BikeShopDB.Models
{
    public class RolesCreateViewModel
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
    }
}
