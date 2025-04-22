using GasServiceUA.Models;
using GasServiceUA.UnitOfWork;

namespace GasServiceUA.Repositories
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
