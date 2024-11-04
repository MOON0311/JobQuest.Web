using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JobQuest.Web.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }

        public int UserId { get; set; }

        public int JobPostId { get; set; }

        public string? ResumeFilePath { get; set; }

        public DateTime ApplicationDate { get; set; } // When the application was submitted

        public string Status { get; set; } // Status of the application (e.g., Applied, Interviewed)

    }
}
