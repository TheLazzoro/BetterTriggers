using System;
using System.Globalization;
using System.IO;

namespace GUI.Utility
{
    internal static class ErrorLog
    {
        public static string Directory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Better Triggers", "errors");

        /// <summary>
        /// Writes the exception to %LocalAppData%/Better Triggers/errors.
        /// </summary>
        /// <returns>Path of the written file, or null if it could not be written.</returns>
        public static string? Write(Exception ex, string context = "")
        {
            try
            {
                System.IO.Directory.CreateDirectory(Directory);
                string fileName = "Error_" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff", CultureInfo.InvariantCulture) + ".txt";
                string path = Path.Combine(Directory, fileName);
                File.WriteAllText(path, $"{context}{Environment.NewLine}{ex}");
                return path;
            }
            catch
            {
                return null;
            }
        }
    }
}
