using Web_FIA44_User_verwaltung_Admin_Control.Models;

namespace Web_FIA44_User_verwaltung_Admin_Control.DAL
{
	public interface IAccessable
	{
		int InsertUser(User user);

		bool UpdateUser(User user);

		bool DeleteUser(int UId);

		List<User> GetAllUsers();

		User GetUserById(int UId);

		(string, string) GetSaltAndHash(string username);

		bool IsUsernameAvaiable(string username);

		User GetUserByString(string username);

		bool IsEmailAvaiable(string email);

	}
}
