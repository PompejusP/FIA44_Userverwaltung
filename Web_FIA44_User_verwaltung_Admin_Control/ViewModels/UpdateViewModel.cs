using System.ComponentModel.DataAnnotations;

namespace Web_FIA44_User_verwaltung_Admin_Control.ViewModels
{
    public class UpdateViewModel
    {
        public int UId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; } // Optional

        [Display(Name = "Geburtsdatum")]
        public DateTime Birthday { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Nutzerbild")]
        public string? UserImg { get; set; }

        [Display(Name = "Admin?")]
        public bool IsAdmin { get; set; }

        public IFormFile? Image { get; set; } // Optional
    }
}
