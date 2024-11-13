using ClassLibrary_FIA44_HashHelper;
using Web_FIA44_User_verwaltung_Admin_Control.DAL;
using Web_FIA44_User_verwaltung_Admin_Control.Models;

namespace Web_FIA44_User_verwaltung_Admin_Control.BL
{
	public class BusiLay : IUserService
	{
		private readonly IAccessable dal;

		public BusiLay(IAccessable accessable)
		{
			dal = accessable;
		}
		#region User nach dem Username suchen
		public User GetUserByUsername(string username)
		{
			//dal.GetUserByString(username) der Methode GetUserByString wird der Username übergeben
			return dal.GetUserByString(username);
		}
		#endregion

		#region User nach der Id suchen
		public User GetUser(int UId)
		{
			//dal.GetUserById(UId) der Methode GetUserById wird die Id übergeben und der User wird zurückgegeben
			return dal.GetUserById(UId);
		}
		#endregion

		#region Überprüfen ob der Login gültig ist
		public bool IsLoginValid(string username, string kennwort)
		{

			//GetSaltAndHash gibt ein Tuple zurück mit Salt und Hash
			(string salt, string hash) = dal.GetSaltAndHash(username);
			string pepper = Environment.GetEnvironmentVariable("Pepper");
			string hashedPassword = HashHelper.GenerateHash(kennwort, salt, pepper);
			//Überprüfen ob das gehashte Passwort mit dem in der Datenbank übereinstimmt
			if (hashedPassword == hash)
			{
				//Wenn ja wird true zurückgegeben
				return true;
			}
			else
			{
				//Wenn nein wird false zurückgegeben
				return false;
			}

		}
		#endregion

		#region User hinzufügen
		public void addUser(User user)
		{
			//Salt wird generiert
			string generatedSalt = HashHelper.GenerateSalt(16);
			//Pepper wird aus der Umgebungsvariable geholt
			string pepper = Environment.GetEnvironmentVariable("Pepper");
			//Das Passwort wird gehasht
			string hashedPassword = HashHelper.GenerateHash(user.Password, generatedSalt, pepper);
			//Ein neuer User wird erstellt
			User newUser = new User
			{
				Username = user.Username,
				HashedKeyword = hashedPassword,
				Salt = generatedSalt,
				Birthday = user.Birthday,
				Email = user.Email,
				UserImg = user.UserImg,
				IsAdmin = user.IsAdmin
			};
			//versuchen den User in die Datenbank einzufügen
			try
			{
				dal.InsertUser(newUser);
			}
			//Falls ein Fehler auftritt wird eine Fehlermeldung ausgegeben
			catch (Exception ex)
			{
				Console.WriteLine($"Error inserting user: {ex.Message}");
					}
		}
		#endregion

		#region User aktualisieren
		public void updateUser(User user)
		{
			//Den aktuellen User holen
			User currentUser = dal.GetUserByString(user.Username);
			//Überprüfen ob das Passwort geändert wurde
			if (!string.IsNullOrEmpty(user.Password))
			{
				//wenn ja dann generiere ein neues Salt
				string generatedSalt = HashHelper.GenerateSalt(16);
				//hole das Pepper aus der Umgebungsvariable
				string pepper = Environment.GetEnvironmentVariable("Pepper");
				//hash das neue Passwort
				string hashedPassword = HashHelper.GenerateHash(user.Password, generatedSalt, pepper);

				//setze das Salt und das gehashte Passwort
				currentUser.HashedKeyword = hashedPassword;
				currentUser.Salt = generatedSalt;
			}
			//Setze die anderen Werte
			currentUser.Birthday = user.Birthday;
			currentUser.Email = user.Email;
			currentUser.UserImg = user.UserImg;
			currentUser.IsAdmin = user.IsAdmin;
			//Aktualisiere den User in der Datenbank
			dal.UpdateUser(currentUser);
		}
		
		#endregion

		#region Alle User holen

		public List<User> getAllUsers()
		{
			//Hole alle User aus der Datenbank
			return dal.GetAllUsers();
		}
		#endregion

		#region User löschen
		public void deleteUser(int UId)
		{
			//Lösche den User aus der Datenbank
			dal.DeleteUser(UId);
		}
		#endregion

		#region User registrieren
		public void RegisterUser(User user)
		{
			//Salt wird generiert
			string generatedSalt = HashHelper.GenerateSalt(16);
			//Pepper wird aus der Umgebungsvariable geholt
			string pepper = Environment.GetEnvironmentVariable("Pepper");
			//Das Passwort wird gehasht
			string hashedPassword = HashHelper.GenerateHash(user.Password, generatedSalt, pepper);
			//Ein neuer User wird erstellt
			User newUser = new User
			{
				Username = user.Username,
				HashedKeyword = hashedPassword,
				Salt = generatedSalt,
				Birthday = user.Birthday,
				Email = user.Email,
				UserImg = user.UserImg,
				//Admin ist standardmäßig false und wird hier nicht gesetzt dies kann nur ein Admin nach der Registrierung machen
			};
			//Der User wird in die Datenbank eingefügt
			dal.InsertUser(newUser);
		}
        #endregion

        #region Überprüfen ob der Username verfügbar ist
        public bool IsUsernameAvailable(string username)
        {
			//Überprüfen ob der Username verfügbar ist
			return dal.IsUsernameAvaiable(username);
        }
		#endregion

		#region Überprüfen ob die Email nicht vergeben ist
		public bool IsEmailAvailable(string email)
		{
			//Überprüfen ob die Email verfügbar ist
			return dal.IsEmailAvaiable(email);
		}
		#endregion


	}
}
