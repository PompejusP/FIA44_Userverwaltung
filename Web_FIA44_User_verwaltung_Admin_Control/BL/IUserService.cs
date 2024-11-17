using Web_FIA44_User_verwaltung_Admin_Control.Models;

namespace Web_FIA44_User_verwaltung_Admin_Control.BL
{
	public interface IUserService
	{
		User GetUserByUsername(string username);
		User GetUser(int UId);
		bool IsLoginValid(string username, string kennwort);
		void addUser(User user);
		void updateUser(User user);
		void deleteUser(int UId);
		List<User> getAllUsers();

		bool IsUsernameAvailable(string username);
		void RegisterUser(User user);

		bool IsEmailAvailable(string email);
	}
}
