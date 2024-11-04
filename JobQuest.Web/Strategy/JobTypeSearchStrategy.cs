using JobQuest.Web.Data;
using JobQuest.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace JobQuest.Web.Strategy
{
    public class JobTypeSearchStrategy : IJobPostSearchStrategy
    {
        private readonly string _jobType;

        public JobTypeSearchStrategy(string jobType)
        {
            _jobType = jobType;
        }

        public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm)
        {
            return jobPosts.Where(j => j.JobType == _jobType);
        }
    }
}
