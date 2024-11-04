using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public class SalarySearchStrategy : IJobPostSearchStrategy
    {
        private readonly int? minSalary;
        private readonly int? maxSalary;

        public SalarySearchStrategy(int? minSalary, int? maxSalary)
        {
            this.minSalary = minSalary;
            this.maxSalary = maxSalary;
        }

        public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm)
        {
            // Filter job posts based on salary range
            if (minSalary.HasValue)
            {
                jobPosts = jobPosts.Where(j => j.Salary >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                jobPosts = jobPosts.Where(j => j.Salary <= maxSalary.Value);
            }

            return jobPosts;
        }
    }
}
