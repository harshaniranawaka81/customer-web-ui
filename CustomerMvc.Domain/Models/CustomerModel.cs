using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomerMvc.Domain.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the Name.")]
        [StringLength(50, ErrorMessage = "Name cannot be more than 50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the Email.")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the Address.")]
        [StringLength(200, ErrorMessage = "Address cannot be more than 200 characters")]
        public string Address { get; set; } = string.Empty;
    }
}