using Microsoft.AspNetCore.Mvc;
using Web_FIA44_User_verwaltung_Admin_Control.BL;
using Web_FIA44_User_verwaltung_Admin_Control.Helpers;
using Web_FIA44_User_verwaltung_Admin_Control.Models;
using Web_FIA44_User_verwaltung_Admin_Control.ViewModels;

namespace Web_FIA44_User_verwaltung_Admin_Control.Controllers
{
	public class HomeController : Controller
	{
		#region ersten Hashwert generieren ist auskommentiert weil es nur einmal ausgeführt werden muss
		//public HomeController()
		//{
		//	string salt = HashHelper.GenerateSalt(16);
		//	//string salt = "CYUR0PT7nxCY5SWwz7a4XQ==";
		//	string pepper = Environment.GetEnvironmentVariable("Pepper");
		//	string hash = HashHelper.GenerateHash("admin", salt, pepper);


		//}
		#endregion

		#region Dal und Bl
		private readonly IUserService service;

		private string SessionInhalt;
		private int UId;


		public HomeController(IUserService userService)
		{
			// der Service wird initialisiert
			service = userService;
		}
		#endregion

		#region Index
		public IActionResult Index()
		{
			//der User wird überprüft ob er eingeloggt ist oder nicht
			//wenn er eingeloggt ist, wird der Username, die UserId und ob er Admin ist an die View übergeben
			//wenn er nicht eingeloggt ist, wird der Username, die UserId und ob er Admin ist auf null gesetzt
			if (IsUserLoggedIn(out int userId, out string username))
			{
				ViewBag.Username = username;
				ViewBag.UId = userId;
				ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			}
			else
			{
				ViewBag.Username = null;
				ViewBag.UId = null;
				ViewBag.IsAdmin = false;
			}

			//rufe die Index Seite auf
			return View();
		}

		#endregion

		#region Login
		[HttpGet]
		public IActionResult Login()
		{
			//rufe die Login Seite auf
			return View();
		}
		[HttpPost]
		public IActionResult Login(User user)
		{
			//checke die Eingaben des Users und überprüfe ob sie in der Datenbank vorhanden sind
			if (HttpContext.Request.Method == "POST")
			{
				string username = HttpContext.Request.Form["username"];
				string password = HttpContext.Request.Form["password"];
				//überprüfe ob der User in der Datenbank vorhanden ist
				if (service.IsLoginValid(user.Username, user.Password))
				{
					//der User wird aus der Datenbank geholt
					User loggedUser = service.GetUserByUsername(username);
					//der User wird in die Session gespeichert
					HttpContext.Session.SetString("LoggedIn", user.Username);
					HttpContext.Session.SetInt32("LoggedInUserId", loggedUser.UId);
					HttpContext.Session.SetString("IsAdmin", loggedUser.IsAdmin.ToString());
					//Username, UserId und ob der User Admin ist wird an die View übergeben
					ViewBag.Username = user.Username;
					ViewBag.UId = loggedUser.UId;
					ViewBag.IsAdmin = user.IsAdmin;
					//der User wird an die Index Seite weitergeleitet
					return View("Index");
				}
				//wenn der User nicht in der Datenbank vorhanden ist, wird eine Fehlermeldung angezeigt
				else
				{
					ViewBag.Message = "Login failed";
				}
			}
			//und die Login Seite wird erneut aufgerufen
			return View();

		}
		#endregion

		#region Details

		public IActionResult Details(int UId)
		{
			//der User wird überprüft ob er eingeloggt ist oder nicht
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
			//wenn er eingeloggt ist, wird der Username, die UserId und ob er Admin ist an die View übergeben
			ViewBag.Username = loggedInUsername;
			ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			// der User wird aus der Datenbank geholt
			User user = service.GetUser(UId);
			//wenn der User ein Bild hat, wird der Pfad zu dem Bild hinzugefügt
			if (!string.IsNullOrEmpty(user.UserImg))
			{
				user.UserImg = Path.Combine("/images/UserImages", user.UserImg);
			}
			// der User wird an die View übergeben
			return View(user);
		}
		#endregion

		#region Update
		[HttpGet]
		public IActionResult Update(int UId)
		{
			//der User wird überprüft ob er eingeloggt ist oder nicht wenn nicht wird er auf die Login Seite weitergeleitet 
			//wenn er eingeloggt ist, wird der Username, die UserId und ob er Admin ist an die View übergeben
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
			ViewBag.Username = loggedInUsername;
			ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//der User wird mit seiner UserId aus der Datenbank geholt
			User user = service.GetUser(UId);
			//wenn kein User gefunden wird, wird eine Fehlermeldung angezeigt
			if (user == null)
			{
				return NotFound();
			}

			// Der User wird in ein UpdateViewModel umgewandelt
			var model = new UpdateViewModel
			{
				UId = user.UId,
				Username = user.Username,
				Birthday = user.Birthday,
				Email = user.Email,
				UserImg = user.UserImg,
				IsAdmin = user.IsAdmin,

			};

			//der User wird an die View übergeben
			return View(model);
		}
		[HttpPost]
		public IActionResult Update(UpdateViewModel model)
		{
			//der User wird überprüft ob er eingeloggt ist oder nicht wenn nicht wird er auf die Login Seite weitergeleitet
			//wenn er eingeloggt ist, wird der Username, die UserId und ob er Admin ist an die View übergeben
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
			ViewBag.Username = loggedInUsername;
			ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//es wird überprüft ob die Eingaben des Users korrekt sind
			if (ModelState.IsValid)
			{
				// versuche den User zu updaten
				try
				{
					//der User wird aus der Datenbank geholt
					User currentUser = service.GetUser(model.UId);
					//wenn kein User gefunden wird, wird eine Fehlermeldung angezeigt
					if (currentUser == null)
					{
						return NotFound();
					}
					if (!IsUsernameValid(currentUser, model.Username) || !IsEmailValid(currentUser, model.Email))
					{
						return View(model);
					}
					model.UserImg = ProcessUserImage(model, currentUser);
					//der User wird in ein User Objekt umgewandelt
					User user = new User
					{
						UId = model.UId,
						Username = model.Username,
						Password = model.Password, // Optional
						Birthday = model.Birthday,
						Email = model.Email,
						UserImg = model.UserImg,
						IsAdmin = model.IsAdmin
					};
					//der User wird geupdatet
					service.updateUser(user);
					//nach abschluss wird der User an die Details Seite weitergeleitet
					return RedirectToAction("Index");
				}
				//wenn ein Fehler auftritt wird eine Fehlermeldung angezeigt
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Ein Fehler ist aufgetreten");
					Console.WriteLine(ex.Message);
				}
			}
			//wenn die Eingaben des Users nicht korrekt sind, wird die Update Seite erneut aufgerufen
			else
			{
				Console.WriteLine("ModelState is not valid");
			}
			//der User wird an die View übergeben
			return View(model);
		}
		#endregion

		#region Logout
		public IActionResult Logout()
		{
			//Session wird gelöscht
			HttpContext.Session.Remove("LoggedIn");
			HttpContext.Session.Remove("LoggedInUserId");

			//der User wird an die Login Seite weitergeleitet			//es wird auf die Index Seite weitergeleitet
			return RedirectToAction("Index");
		}
		#endregion

		#region Registrieren
		[HttpGet]
		public IActionResult Register()
		{
			//rufe die Register Seite auf
			return View();
		}
		[HttpPost]
		public IActionResult Register(RegisterViewModel model)
		{
			//es wird überprüft ob die Eingaben des Users korrekt sind
			if (ModelState.IsValid)
			{
				//versuche den User zu registrieren
				try
				{
					if (!IsUsernameAvailable(model.Username) || !IsEmailAvailable(model.Email))
					{
						return View(model);
					}
					model.UserImg = UploadImage(model);
					//der User wird in ein User Objekt umgewandelt
					User user = new User
					{
						Username = model.Username,
						Password = model.Password,
						Birthday = model.Birthday,
						Email = model.Email,
						UserImg = model.UserImg,

					};
					//der User wird hinzugefügt
					service.addUser(user);
					//nach abschluss wird der User an die Login Seite weitergeleitet
					return RedirectToAction("Login");
				}
				//wenn ein Fehler auftritt wird eine Fehlermeldung angezeigt
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Ein Fehler ist aufgetreten");
					Console.WriteLine(ex.Message);
				}
			}
			//wenn die Eingaben des Users nicht korrekt sind, wird die Register Seite erneut aufgerufen
			else
			{
				Console.WriteLine("ModelState is not valid");
			}
			//der User wird an die View übergeben
			return View(model);
		}
		#endregion

		#region besondere Methoden zur Prüfung von verschiedenen gegebenheiten

		#region Ist der User eingeloggt?
		private bool IsUserLoggedIn(out int userId, out string username)
		{
			//der User wird überprüft ob er eingeloggt ist oder nicht
			//wenn er eingeloggt ist , wird der Usernameund  die UserId an die View übergeben
			userId = HttpContext.Session.GetInt32("LoggedInUserId") ?? 0;
			username = HttpContext.Session.GetString("LoggedIn");

			//wenn die UserId nicht 0 ist, ist der User eingeloggt
			return userId != 0;

		}
		#endregion

		#region ist der username verfügbar?
		//die Methode IsUsernameValid überprüft ob der Username verfügbar ist bei der Update Methode
		private bool IsUsernameValid(User currentUser, string newusername)
		{
			//Wenn der USername nicht dem aktuellen Usernamen entspricht und der Username nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
			if (currentUser.Username != newusername && !service.IsUsernameAvailable(newusername))
			{
				ModelState.AddModelError("Username", "Der Benutzername ist bereits vergeben.");
				return false;
			}
			return true;
		}
		//die Methode IsUsernameAvailable überprüft ob der Username verfügbar ist bei der newUser Methode
		private bool IsUsernameAvailable(string username)
		{
			//Wenn der Username nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
			if (!service.IsUsernameAvailable(username))
			{
				ModelState.AddModelError("Username", "Der Benutzername ist bereits vergeben.");
				return false;
			}
			return true;
		}
		#endregion

		#region ist die Email verfügbar?
		//die Methode IsEmailValid überprüft ob die Email verfügbar ist bei der Update Methode
		private bool IsEmailValid(User currentUser, string newEmail)
		{
			//Wenn die neu eingetragene Email nicht der aktuellen Email entspricht und die Email nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
			if (currentUser.Email != newEmail && !service.IsEmailAvailable(newEmail))
			{
				ModelState.AddModelError("Email", "Die Email ist bereits vergeben.");
				return false;
			}
			return true;
		}
		//die Methode IsEmailAvailable überprüft ob die Email verfügbar ist bei der newUser Methode
		private bool IsEmailAvailable(string Email)
		{
			//Wenn die Email nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
			if (!service.IsEmailAvailable(Email))
			{
				ModelState.AddModelError("Email", "Die Email ist bereits vergeben.");
				return false;
			}
			return true;
		}
		#endregion

		#region Bild upload  optionen
		//Methode um das Userbild zu updaten
		private string ProcessUserImage(UpdateViewModel model, User currentUser)
		{
			//Wenn das Bild Model nicht leer ist, wird das Bild in den Pfad /images/UserImages gespeichert
			if (model.Image != null)
			{
				string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/UserImages");
				//Wenn das Userbildfeld nicht leer ist, wird das alte Bild gelöscht und das neue Bild hochgeladen
				if (!string.IsNullOrEmpty(model.UserImg))
				{
					string oldImagePath = Path.Combine(uploadFolder, model.UserImg);
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}
				//Das Bild wird in den Pfad /images/UserImages gespeichert
				return FileUploadHelper.UploadFile(model.Image, uploadFolder);
			}

			//der currentUser wird zurückgegeben
			return currentUser.UserImg;
		}
		//Methode um das Userbild  einzufügen bei einem neuen User
		private string UploadImage(RegisterViewModel model)
		{
			//Wenn das Bild Model nicht leer ist, wird das Bild in den Pfad /images/UserImages gespeichert
			if (model.Image != null)
			{
				string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/UserImages");
				//Das Bild wird in den Pfad /images/UserImages gespeichert
				return FileUploadHelper.UploadFile(model.Image, uploadFolder);
			}
			else //wenn kein Bild hochgeladen wird, wird ein Standardbild verwendet
			{
				return "standard.jpg";
			}
		}

		#endregion

		#endregion

	}
}
