using JobQuest.Web.Models;
using System.Data;

namespace JobQuest.Web.Strategy
{
    public class RoleSearchStrategy : IUserSearchStrategy
    {
        private readonly int _roleId;

        public RoleSearchStrategy(int roleId)
        {
            _roleId = roleId;
        }

        public IQueryable<User> Search(IQueryable<User> users, string searchTerm)
        {
            if (_roleId > 0)
            {
                users = users.Where(u => u.RoleId == _roleId);
            }
            return users;
        }
    }
}
