using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace JobQuest.Web.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        private string _password;

        public string Password { get; private set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int RoleId { get; set; }

        public string Status { get; set; }

        public Role Role { get; set; }

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

        public void SetPassword(string password)
        {
            _password = BCrypt.Net.BCrypt.HashPassword(password);
            Password = _password;
        }

        public bool CheckPassword(string inputPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, Password);
        }

    }
}
