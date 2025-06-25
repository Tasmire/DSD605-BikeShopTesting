using System.ComponentModel.DataAnnotations;

namespace DSD603_Bike_Shop.Models
{
    public class Staff
    {
        [Display(Name = "Staff ID")]
        public Guid StaffId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
