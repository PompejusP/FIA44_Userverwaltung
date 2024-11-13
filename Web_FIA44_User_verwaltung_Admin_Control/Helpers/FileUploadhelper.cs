using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Web_FIA44_User_verwaltung_Admin_Control.Helpers
{
    /// <summary>
    /// Diese Klasse enthält Hilfsmethoden für den Datei-Upload.
    /// </summary>
    public static class FileUploadHelper
    {
        /// <summary>
        /// Lädt eine Datei hoch und speichert sie im angegebenen Verzeichnis.
        /// </summary>
        /// <param name="upload">Die hochzuladende Datei.</param>
        /// <param name="uploadFolder">Das Verzeichnis, in dem die Datei gespeichert werden soll.</param>
        /// <returns>Der relative Pfad zur gespeicherten Datei.</returns>
        public static string UploadFile(IFormFile upload, string uploadFolder)
        {
            // Überprüfen, ob die Datei null oder leer ist
            if (upload == null || upload.Length == 0)
            {
                return null;
            }

            // Erzeuge eine neue Guid und verwende sie als Dateinamen
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            // Datei im angegebenen Verzeichnis speichern
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                upload.CopyTo(stream);
            }

            // Rückgabe des relativen Pfads zur gespeicherten Datei
            return  fileName;
        }
    }
}
