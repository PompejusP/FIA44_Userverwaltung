using Microsoft.Data.SqlClient;
using Web_FIA44_User_verwaltung_Admin_Control.Models;

namespace Web_FIA44_User_verwaltung_Admin_Control.DAL
{
	public class SqlDal : IAccessable
	{
		#region Sql Connection
		//connectionString zum Verbinden mit der Datenbank
		private readonly string connectionString;
		//Konstruktor
		public SqlDal(string connString)
		{
			//connectionString wird übergeben
			connectionString = connString;
		}

		#endregion

		#region User löschen
		public bool DeleteUser(int UId)
		{
			//SQL-Abfrage um einen User aus der Datenbank zu löschen
			string deleteUser = "DELETE FROM [User] WHERE UId = @UId";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand deleteCmd = new SqlCommand(deleteUser, connection);
				//Parameter hinzufügen
				deleteCmd.Parameters.AddWithValue("@UId", UId);
				//Verbindung öffnen
				connection.Open();
				//Anzahl der betroffenen Zeilen
				//affectedRows = 1, wenn ein User gelöscht wurde
				int affectedRows = deleteCmd.ExecuteNonQuery();
				//Verbindung schließen
				connection.Close();
				//True zurückgeben, wenn ein User gelöscht wurde
				return affectedRows == 1;
			}
		}

		#endregion

		#region Alle User anzeigen
		//Alle User anzeigen
		public List<User> GetAllUsers()
		{
			//SQL-Abfrage um alle User aus der Datenbank zu holen
			string GetAllUsers = "SELECT * FROM [User]";

			//Verbindung zur Datenbank 
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand getallcmd = new SqlCommand(GetAllUsers, connection);
				//Verbindung öffnen
				connection.Open();
				//SqlDataReader um die Daten aus der Datenbank zu lesen
				using (SqlDataReader reader = getallcmd.ExecuteReader())
				{
					//Liste um die User zu speichern
					List<User> users = new List<User>();
					//Solange Daten vorhanden sind
					while (reader.Read())
					{
						//User Objekt erstellen und mit den Daten aus der Datenbank füllen
						User user = new User();
						user.UId = (int)reader["UId"];
						user.Username = reader["Username"].ToString();
						user.HashedKeyword = reader["HashedKeyword"].ToString();
						user.Salt = reader["Salt"].ToString();
						user.Birthday = (DateTime)reader["Birthday"];
						user.Email = reader["Email"].ToString();
						user.UserImg = reader["UserImg"].ToString();
						user.IsAdmin = (bool)reader["IsAdmin"];

						//User Objekt zur Liste hinzufügen
						users.Add(user);
					}
					//Verbindung schließen
					connection.Close();
					//Liste zurückgeben
					return users;
				}
			}
		}
		#endregion

		#region Einzelnen User anzeigen
		//Einzelnen User anzeigen
		public User GetUserById(int UId)
		{
			//SQL-Abfrage um einen User aus der Datenbank zu holen
			string GetUserById = "SELECT * FROM [User] WHERE UId = @UId";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand getbyidcmd = new SqlCommand(GetUserById, connection);
				//Parameter hinzufügen
				getbyidcmd.Parameters.AddWithValue("@UId", UId);
				//Verbindung öffnen
				connection.Open();
				//SqlDataReader um die Daten aus der Datenbank zu lesen
				SqlDataReader reader = getbyidcmd.ExecuteReader();
				//User Objekt erstellen
				User user = new User();
				//Daten aus der Datenbank in das User Objekt schreiben
				if (reader.Read())
				{
					//User Objekt mit den Daten aus der Datenbank füllen
					user.UId = (int)reader["UId"];
					user.Username = reader["Username"].ToString();
					user.HashedKeyword = reader["HashedKeyword"].ToString();
					user.Salt = reader["Salt"].ToString();
					user.Birthday = (DateTime)reader["Birthday"];
					user.Email = reader["Email"].ToString();
					user.UserImg = reader["UserImg"].ToString();
					user.IsAdmin = (bool)reader["IsAdmin"];
				}
				//Verbindung schließen
				connection.Close();
				//User Objekt zurückgeben
				return user;
			}
		}
		#endregion

		#region User hinzufügen
		public int InsertUser(User user)
		{
			//SQL-Abfrage um einen neuen User in die Datenbank hinzuzufügen
			string InsertUser = "INSERT INTO [User] (Username,HashedKeyword,Salt,Birthday,Email,UserImg,IsAdmin) " +
				"output inserted.UId VALUES (@Username, @HashedKeyword, @Salt, @Birthday, @Email, @UserImg, @IsAdmin)";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand insertcmd = new SqlCommand(InsertUser, connection);

				//Parameter hinzufügen
				insertcmd.Parameters.AddWithValue("@Username", user.Username);
				insertcmd.Parameters.AddWithValue("@HashedKeyword", user.HashedKeyword);
				insertcmd.Parameters.AddWithValue("@Salt", user.Salt);
				insertcmd.Parameters.AddWithValue("@Birthday", user.Birthday);
				insertcmd.Parameters.AddWithValue("@Email", user.Email);
				insertcmd.Parameters.AddWithValue("@UserImg", (object)user.UserImg ?? DBNull.Value);
				insertcmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
				//Verbindung öffnen
				connection.Open();
				//Neue UId des Users
				int newUId = (int)insertcmd.ExecuteScalar();
				//Verbindung schließen
				connection.Close();
				//Neue UId zurückgeben
				return newUId;
			}

		}
		#endregion

		#region User aktualisieren
		public bool UpdateUser(User user)
		{
			//SQL-Abfrage um einen User in der Datenbank zu aktualisieren
			string UpdateUser = "UPDATE [User] SET Username=@Username, HashedKeyword=@HashedKeyword, Salt=@Salt, Birthday=@Birthday, Email=@Email, UserImg=@UserImg, IsAdmin=@IsAdmin WHERE UId=@UId";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand updateCmd = new SqlCommand(UpdateUser, connection);

				//Parameter hinzufügen
				updateCmd.Parameters.AddWithValue("@UId", user.UId);
				updateCmd.Parameters.AddWithValue("@Username", user.Username);
				updateCmd.Parameters.AddWithValue("@HashedKeyword", user.HashedKeyword);
				updateCmd.Parameters.AddWithValue("@Salt", user.Salt);
				updateCmd.Parameters.AddWithValue("@Birthday", user.Birthday);
				updateCmd.Parameters.AddWithValue("@Email", user.Email);
				updateCmd.Parameters.AddWithValue("@UserImg", (object)user.UserImg ?? DBNull.Value);
				updateCmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);

				//Verbindung öffnen
				connection.Open();

				//Anzahl der betroffenen Zeilen
				int affectedRows = updateCmd.ExecuteNonQuery();

				//Verbindung schließen
				connection.Close();

				//True zurückgeben, wenn ein User aktualisiert wurde
				return affectedRows == 1;
			}
		}
		#endregion

		#region Salt und Hash abrufen
		public (string, string) GetSaltAndHash(string username)
		{
			//SQL-Abfrage um Salt und Hash eines Users aus der Datenbank zu holen
			string query = "SELECT Salt, HashedKeyword FROM [User] WHERE username = @Username";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand cmd = new SqlCommand(query, connection);
				//Parameter hinzufügen
				cmd.Parameters.AddWithValue("@Username", username);
				//Verbindung öffnen
				connection.Open();
				//SqlDataReader um die Daten aus der Datenbank zu lesen
				SqlDataReader reader = cmd.ExecuteReader();
				//wenn Daten vorhanden sind
				if (reader.Read())
				{
					//Salt und Hash aus der Datenbank holen
					string salt = reader["Salt"].ToString();
					string hash = reader["HashedKeyword"].ToString();
					return (salt, hash);
				}
				//ansonsten Exception werfen 
				else
				{
					throw new Exception("User not found.");
				}
			}

		}
		#endregion

		#region überprüfen ob Username verfügbar ist ist
		public bool IsUsernameAvaiable(string username)
		{
			//SQL-Abfrage um zu überprüfen ob der Username verfügbar ist
			string query = "SELECT COUNT(*) FROM [User] WHERE Username = @Username";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand cmd = new SqlCommand(query, connection);
				//Parameter hinzufügen
				cmd.Parameters.AddWithValue("@Username", username);
				//Verbindung öffnen
				connection.Open();
				//Anzahl der betroffenen Zeilen
				int count = (int)cmd.ExecuteScalar();
				//Verbindung schließen
				connection.Close();
				//True zurückgeben, wenn der Username verfügbar ist bzw wenn count = 0 ist
				return count == 0;
			}

		}
		#endregion

		#region User nach Username abrufen
		public User GetUserByString(string username)
		{
			//SQL-Abfrage um einen User über den Username aus der Datenbank zu holen
			string GetUserByString = "SELECT * FROM [User] WHERE Username = @Username";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand getbystringcmd = new SqlCommand(GetUserByString, connection);
				//Parameter hinzufügen
				getbystringcmd.Parameters.AddWithValue("@Username", username);
				//Verbindung öffnen
				connection.Open();
				//SqlDataReader um die Daten aus der Datenbank zu lesen
				SqlDataReader reader = getbystringcmd.ExecuteReader();
				//User Objekt erstellen
				User user = new User();
				//wenn Daten vorhanden sind dann User Objekt mit den Daten aus der Datenbank füllen
				if (reader.Read())
				{
					//User Objekt mit den Daten aus der Datenbank füllen
					user.UId = (int)reader["UId"];
					user.Username = reader["Username"].ToString();
					user.HashedKeyword = reader["HashedKeyword"].ToString();
					user.Salt = reader["Salt"].ToString();
					user.Birthday = (DateTime)reader["Birthday"];
					user.Email = reader["Email"].ToString();
					user.UserImg = reader["UserImg"].ToString();
					user.IsAdmin = (bool)reader["IsAdmin"];
				}
				//Verbindung schließen
				connection.Close();
				//User Objekt zurückgeben
				return user;
			}
		}


		#endregion

		#region Überprüfen ob die Email schon vergeben ist
		public bool IsEmailAvaiable(string email)
		{
			string query = "SELECT COUNT(*) FROM [User] WHERE Email = @Email";
			//Verbindung zur Datenbank
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//SqlCommand um die SQL-Abfrage auszuführen
				SqlCommand cmd = new SqlCommand(query, connection);
				//Parameter hinzufügen
				cmd.Parameters.AddWithValue("@Email", email);
				//Verbindung öffnen
				connection.Open();
				//Anzahl der betroffenen Zeilen
				int count = (int)cmd.ExecuteScalar();
				//Verbindung schließen
				connection.Close();
				//True zurückgeben, wenn die Email verfügbar ist bzw wenn count = 0 ist ansonsten false
				return count == 0;
			}
		}


		#endregion
	}

}



