using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE.Models
{
    public enum UserRole { Lecturer, Coordinator, Manager, HR }

    public class User
    {
        [Key]
        public int UserID { get; set; }


        [StringLength(50)]
        public string? FirstName { get; set; }


        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public UserRole Role { get; set; }

        public decimal HourlyRate { get; set; }
    }
}
