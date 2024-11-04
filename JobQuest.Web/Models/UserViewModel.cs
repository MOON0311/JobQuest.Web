namespace JobQuest.Web.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Status { get; set; }
        public string RoleName { get; set; }
    }

}
