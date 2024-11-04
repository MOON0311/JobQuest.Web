using JobQuest.Web.Data;
using JobQuest.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace JobQuest.Web.Strategy
{
    public class TitleSearchStrategy : IJobPostSearchStrategy
    {
        public IQueryable<JobPost> Search(IQueryable<JobPost> jobPosts, string searchTerm)
        {
            return jobPosts.Where(j => j.Title.Contains(searchTerm));
        }
    }
}
