using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public class DescriptionSearchStrategy : IJobPostSearchStrategy
    {
        public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm)
        {
            return jobPosts.Where(j => j.Description.Contains(searchTerm));
        }
    }
}
