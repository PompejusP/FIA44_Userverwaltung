using ClassLibrary_FIA44_HashHelper;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Web_FIA44_User_verwaltung_Admin_Control.BL;
using Web_FIA44_User_verwaltung_Admin_Control.DAL;
using Web_FIA44_User_verwaltung_Admin_Control.Helpers;
using Web_FIA44_User_verwaltung_Admin_Control.Models;
using Web_FIA44_User_verwaltung_Admin_Control.ViewModels;

namespace Web_FIA44_User_verwaltung_Admin_Control.Controllers
{
	public class AdminController : Controller
	{
		#region Dal
		private readonly IUserService service;
		

		public AdminController(IUserService userService, IAccessable accessable)
		{
			//Der Konstruktor wird aufgerufen
			service = userService;

		}

		#endregion

		#region AdminIndex alle User anzeigen
		[HttpGet]
		public IActionResult AdminIndex()
		{
			// Wenn der User nicht eingeloggt ist, wird er auf die Login-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId,out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
			// Wenn der User eingeloggt ist, wird er auf die AdminIndex-Seite weitergeleitet
			// und die Session wird übergeben
			// Die Session wird übergeben, damit der User eingeloggt bleibt
			//über ViewBag werden die Daten an die View übergeben damit sie dort angezeigt werden können wer eingeloggt ist und ob der User Admin ist
			ViewBag.Username = loggedInUsername;
			ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//Alle User werden aus der Datenbank geholt
			List<User> users = service.getAllUsers() ?? new List<User>();
			//für jeden User wird überprüft ob das Userbild leer ist
			foreach (var user in users)
			{
				//Wenn das Userbild nicht leer ist, wird es in den Pfad /images/UserImages gespeichert
				if (!string.IsNullOrEmpty(user.UserImg))
				{
					//Das Userbild wird in den Pfad /images/UserImages gespeichert
					user.UserImg = Path.Combine("/images/UserImages", user.UserImg);
				}
			}
			//die User werden an die View übergeben
			return View(users);
		}
		#endregion

		#region newUser
		[HttpGet]
		//HTTPGet Methode um die View newUser anzuzeigen
		public IActionResult newUser()
		{
			// Wenn der User nicht eingeloggt ist, wird er auf die Login-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
            {
                return RedirectToAction("Login");
            }
			// Wenn der User  mit Admin-Status eingeloggt ist, wird er auf die newUser-Seite weitergeleitet und die Session wird übergeben
			ViewBag.Username = loggedInUsername;
            ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//die View newUser wird angezeigt
			return View();

		}
		[HttpPost]
		//HTTPPost Methode um die View newUser anzuzeigen
		public IActionResult newUser(User user)
        {
			//Session Methode um zu checken ob der User eingeloggt ist wenn nicht wird er auf die Login-Seite weitergeleitet
			//ansonsten wird die Session übergeben und er wird auf die newUser-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
            ViewBag.Username = loggedInUsername;
            ViewBag.UId = loggedInUserId;
            ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");

			//ModelStates werden überprüft und Überflüssige ModelStates werden entfernt
			ModelState.Remove("Salt");
            ModelState.Remove("HashedKeyword");
			//Wenn das ModelState gültig ist
			if (ModelState.IsValid)
            {
				//versuche
                try
                {
					//Wenn der Username nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
					if (!service.IsUsernameAvailable(user.Username))
                    {
                        ModelState.AddModelError("Username", "Der Benutzername ist bereits vergeben.");
                        return View(user);
                    }
					//Wenn die Email nicht verfügbar ist, wird eine Fehlermeldung ausgegeben
					if (!service.IsEmailAvailable(user.Email))
					{
						ModelState.AddModelError("Email", "Die Email ist bereits vergeben.");
						return View(user);
					}
					//Wenn das Userbild nicht leer ist, wird es in den Pfad /images/UserImages gespeichert
					if (user.Image != null)
					{
						string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/UserImages");
						user.UserImg = FileUploadHelper.UploadFile(user.Image, uploadFolder);
					}
					//Der User wird in die Datenbank eingefügt
					service.addUser(user);
					//Der neue User bekommt eine neue UserId
					int newUserId = user.UId;
					//Der  User wird nach erfolgreichem hinzufügen zurück auf die AdminIndex-Seite weitergeleitet
					return RedirectToAction("AdminIndex");
                }
				//Fehlermeldung wird nach einem Fehler ausgegeben
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Ein Fehler ist aufgetreten");
					Console.WriteLine(ex.Message);
				}
				
            }
			//Wenn das ModelState nicht gültig ist, wird eine Fehlermeldung ausgegeben
			else
			{
                Console.WriteLine("ModelState ist not valid");

            }
			//Der User wird an die View zurück übergeben
			return View(user);
        }
        #endregion

        #region deleteUser
        public IActionResult deleteUser(int UId)
		{
			//Methode um dem User zu löschen
			service.deleteUser(UId);
			//Der User wird nach dem Löschen auf die AdminIndex-Seite weitergeleitet
			return RedirectToAction("AdminIndex");
		}
		#endregion

		#region updateUser
		[HttpGet]
		//HTTPGet Methode um die View Update anzuzeigen
		public IActionResult Update(int UId)
		{
			//Session Methode um zu checken ob der User eingeloggt ist wenn nicht wird er auf die Login-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
			{
				return RedirectToAction("Login");
			}
			//Übergabe der Session an die View damit der User eingeloggt bleibt und in der Navbar angezigt wird wer eingeloggt ist
			ViewBag.Username = loggedInUsername;
			ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//der User wird mit seiner UserId aus der Datenbank geholt
			User user = service.GetUser(UId);
			//Wenn der User nicht vorhanden ist, wird eine Fehlermeldung ausgegeben
			if (user == null)
			{
				return NotFound();
			}

			// Der User wird in ein UpdateViewModel umgewandelt und an die View übergeben
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
		//HTTPPost Methode um die View Update anzuzeigen
		public IActionResult Update(UpdateViewModel model)
        {
			//Session Methode um zu checken ob der User eingeloggt ist wenn nicht wird er auf die Login-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
            {
                return RedirectToAction("Login");
            }
			//Übergabe der Session an die View damit der User eingeloggt bleibt und in der Navbar angezigt wird wer eingeloggt ist
			ViewBag.Username = loggedInUsername;
            ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//Wenn das ModelState gültig ist
			if (ModelState.IsValid)
            {
				//versuche
				try
				{
					//der User wird mit seiner UserId aus der Datenbank geholt
					User currentUser = service.GetUser(model.UId);
					//Wenn der User nicht vorhanden ist, wird eine Fehlermeldung ausgegeben
					if (currentUser == null)
                    {
                        return NotFound();
                    }
					//wenn der Username nicht mit dem aktuellen Username übereinstimmt und der Username bereits vergeben ist, wird eine Fehlermeldung ausgegeben
					//und zuzrück auf die Update-Seite weitergeleitet
					if (currentUser.Username != model.Username && !service.IsUsernameAvailable(model.Username))
                    {
                        ModelState.AddModelError("Username", "Der Benutzername ist bereits vergeben.");
                        return View(model);
                    }
					//wenn die Email nicht mit der aktuellen Email übereinstimmt und die Email bereits vergeben ist, wird eine Fehlermeldung ausgegeben
					if (currentUser.Email != model.Email && !service.IsEmailAvailable(model.Email))
					{
						ModelState.AddModelError("Email", "Die Email ist bereits vergeben.");
						return View(model);
					}
					//Wenn das Userbild nicht leer ist, wird es in den Pfad /images/UserImages gespeichert
					if ( model.Image != null)
                    {
                        string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/UserImages");
						//wenn der User ein neues Bild hat, wird das alte Bild gelöscht
						if (!string.IsNullOrEmpty(model.UserImg))
						{
							string oldImagePath = Path.Combine(uploadFolder, model.UserImg);
							if (System.IO.File.Exists(oldImagePath))
							{
								System.IO.File.Delete(oldImagePath);
							}
						}
                        model.UserImg = FileUploadHelper.UploadFile(model.Image, uploadFolder);
                    }
					//wenn das Userbild leer ist, wird das alte Bild übernommen
					else
					{
						model.UserImg = currentUser.UserImg;
					}
					//Der User wird in die Datenbank eingefügt 
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
					//Der User wird in die Datenbank eingefügt
					service.updateUser(user);
					//Der User wird nach dem Aktualisieren auf die AdminIndex-Seite weitergeleitet
					return RedirectToAction("AdminIndex");
                }
				//Fehlermeldung wird nach einem Fehler ausgegeben
				catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ein Fehler ist aufgetreten: " + ex.Message);
                    Console.WriteLine(ex.Message);
                }
            }
			//Wenn das ModelState nicht gültig ist, wird eine Fehlermeldung ausgegeben
			else
			{
                Console.WriteLine("ModelState is not valid");
            }
			//Der User wird an die View zurück übergeben
			return View(model);
        }
		#endregion

		#region Details
		//Detailseite um die Details eines Users anzuzeigen
		public IActionResult Details(int UId)
		{
			//Session Methode um zu checken ob der User eingeloggt ist wenn nicht wird er auf die Login-Seite weitergeleitet
			if (!IsUserLoggedIn(out int loggedInUserId, out string loggedInUsername))
            {
                return RedirectToAction("Login");
            }
			//Übergabe der Session an die View damit der User eingeloggt bleibt und in der Navbar angezigt wird wer eingeloggt ist
			ViewBag.Username = loggedInUsername;
            ViewBag.UId = loggedInUserId;
			ViewBag.IsAdmin = bool.Parse(HttpContext.Session.GetString("IsAdmin") ?? "false");
			//der User wird mit seiner UserId aus der Datenbank geholt
			User user = service.GetUser(UId);
			//wenn der user ein Bild hat, wird es in den Pfad /images/UserImages gespeichert
			if (!string.IsNullOrEmpty(user.UserImg))
            {
				user.UserImg = Path.Combine("/images/UserImages", user.UserImg);
			}
			// der User wird an die View übergeben
			return View(user);
		}
		#endregion

		#region Ist der User eingeloggt?
		private bool IsUserLoggedIn(out int userId, out string username)
		{
			//Session Methode um zu checken ob der User eingeloggt ist
			//wenn der User eingeloggt ist, wird die Session übergeben
			//userId und username werden übergeben
			userId = HttpContext.Session.GetInt32("LoggedInUserId") ?? 0;
            username = HttpContext.Session.GetString("LoggedIn");

			//wenn die userId nicht 0 ist, ist der User eingeloggt
			return userId != 0;

        }
        #endregion
    }
}
