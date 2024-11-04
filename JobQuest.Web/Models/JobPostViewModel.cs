using System.ComponentModel.DataAnnotations;

namespace JobQuest.Web.Models
{
    public class JobPostViewModel
    {
        public int JobPostId { get; set; }

        public string Title { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string JobType { get; set; }

        public string Location { get; set; }

        public int Salary { get; set; }

        public string SalaryType { get; set; }

        public DateTime PostedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
