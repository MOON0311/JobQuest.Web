using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public class SalaryTypeSearchStrategy : IJobPostSearchStrategy
    {
        //public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string salaryType)
        //{
        //    if (string.IsNullOrEmpty(salaryType))
        //    {
        //        return jobPosts;
        //    }

        //    return jobPosts.Where(j => j.SalaryType.Equals(salaryType, StringComparison.OrdinalIgnoreCase));
        //}

        private readonly string _salaryType;

        public SalaryTypeSearchStrategy(string salaryType)
        {
            _salaryType = salaryType;
        }

        public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm)
        {
            return jobPosts.Where(j => j.SalaryType == _salaryType);
        }
    }
}
