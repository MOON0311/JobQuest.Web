using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public interface IUserSearchStrategy
    {
        IQueryable<User> Search(IQueryable<User> users, string searchTerm);
    }
}
