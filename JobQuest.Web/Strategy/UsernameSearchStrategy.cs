using JobQuest.Web.Models;

namespace JobQuest.Web.Strategy
{
    public class UsernameSearchStrategy : IUserSearchStrategy
    {
        private readonly string _searchTerm;

        public UsernameSearchStrategy(string searchTerm)
        {
            _searchTerm = searchTerm;
        }

        public IQueryable<User> Search(IQueryable<User> users, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.UserName.Contains(searchTerm));
            }
            return users;
        }
    }
}
