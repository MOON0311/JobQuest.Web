using JobQuest.Web.Models;

namespace JobQuest.Web.Factory
{
    public static class JobPostFactory
    {
        public static JobPost CreateJobPost(JobPost viewModel, int userId)
        {
            JobPost jobPost = null;

            switch (viewModel.JobType)
            {
                case "FullTime":
                    jobPost = new FullTimeJobPost();
                    break;
                case "PartTime":
                    jobPost = new PartTimeJobPost();
                    break;
                case "Freelancer":
                    jobPost = new FreelancerJobPost();
                    break;
                default:
                    throw new ArgumentException("Invalid job type");
            }

            jobPost.Title = viewModel.Title;
            jobPost.CompanyName = viewModel.CompanyName;
            jobPost.Description = viewModel.Description;
            jobPost.Location = viewModel.Location;
            jobPost.Salary = viewModel.Salary;
            jobPost.SalaryType = viewModel.SalaryType;
            jobPost.PostedDate = DateTime.Now;
            jobPost.UserId = userId;
            jobPost.IsActive = true;

            return jobPost;
        }
    }
}
