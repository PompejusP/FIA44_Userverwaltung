using System.ComponentModel.DataAnnotations;

namespace Web_FIA44_User_verwaltung_Admin_Control.ViewModels
{
	public class RegisterViewModel
	{

		[Display(Name = "Username")]
		public string Username { get; set; }

		[Display(Name = "Geburtsdatum")]
		public DateTime Birthday { get; set; }
		public string Email { get; set; }
		[Display(Name = "Nutzerbild")]
		public string? UserImg { get; set; }

		[DataType(DataType.Password)]
		public string Password { get; set; }

		public IFormFile? Image { get; set; } // Optional
	}
}