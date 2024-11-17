using System.ComponentModel.DataAnnotations;

namespace Web_FIA44_User_verwaltung_Admin_Control.Models
{
	public class User
	{
		[Display(Name = "UserId")]
		public int UId { get; set; }
		[Display(Name = "Username")]
		public string Username { get; set; }
		public string HashedKeyword { get; set; }
		public string Salt { get; set; }
		[Display(Name = "Geburtsdatum")]
		public DateTime Birthday { get; set; }
		public string Email { get; set; }
		[Display(Name = "Nutzerbild")]
		public string? UserImg { get; set; }
		[Display(Name = "Admin?")]
		public bool IsAdmin { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public IFormFile? Image { get; set; }
	}
}
