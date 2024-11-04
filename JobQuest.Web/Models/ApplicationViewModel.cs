namespace JobQuest.Web.Models
{
    public class ApplicationViewModel
    {
        public int ApplicationId { get; set; } // Assuming you have an Application ID
        public int ApplicantId { get; set; } // Assuming you have a name field
        public string ApplicantName { get; set; } // Assuming you have a name field
        public string Resume { get; set; } // Path or name of the resume file
        public DateTime ApplicationDate { get; set; } // Date of application

        public string Status { get; set; }
    }
}
