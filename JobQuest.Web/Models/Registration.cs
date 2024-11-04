using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JobQuest.Web.Models
{
    public class Registration
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int RoleId { get; set; }

        public bool ValidateName(string name, string fieldName, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(name) &&
                !Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            {
                modelState.AddModelError(fieldName, $"{fieldName} must contain only letters.");
                return false; // Validation failed
            }
            return true; // Validation passed
        }
    }
}
