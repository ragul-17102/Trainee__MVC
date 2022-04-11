using System.ComponentModel.DataAnnotations;
namespace Trainee__MVC.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Email { get; set; }

        [Required, MaxLength(20)]
        public string Password { get; set; }

    }
}
