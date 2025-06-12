using System.Collections.Concurrent;
using MemoriaPiDataCore.Models;

namespace MemoriaPiWeb.Services
{
    public class ActiveUserStore
    {
        // Ein thread-sicherer Speicher für Benutzer, die gerade online sind.
        // Key: UserId (string), Value: ApplicationUser-Objekt
        private readonly ConcurrentDictionary<string, ApplicationUser> _users = new();

        /// <summary>
        /// Fügt einen Benutzer zur Liste der aktiven Benutzer hinzu.
        /// </summary>
        public void AddUser(ApplicationUser user)
        {
            // Fügt den Benutzer hinzu oder aktualisiert ihn, falls er bereits vorhanden ist.
            _users.AddOrUpdate(user.Id, user, (key, existingUser) => user);
        }

        /// <summary>
        /// Entfernt einen Benutzer aus der Liste der aktiven Benutzer.
        /// </summary>
        public void RemoveUser(string userId)
        {
            _users.TryRemove(userId, out _);
        }

        /// <summary>
        /// Gibt eine Liste aller derzeit aktiven Benutzer zurück.
        /// </summary>
        public List<ApplicationUser> GetActiveUsers()
        {
            return _users.Values.OrderBy(u => u.UserName).ToList();
        }
    }
}