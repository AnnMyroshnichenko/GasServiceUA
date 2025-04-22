using GasServiceUA.Models;
using GasServiceUA.UnitOfWork;

namespace GasServiceUA.Repositories
{
    public class BillRepository : RepositoryBase<Bill>
    {
        public BillRepository(IUnitOfWork unitOfwork) : base(unitOfwork) { }
    }
}
