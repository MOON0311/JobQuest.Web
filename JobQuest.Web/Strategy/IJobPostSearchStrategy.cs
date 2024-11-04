using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public interface IJobPostSearchStrategy
    {
        IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm);
    }
}
