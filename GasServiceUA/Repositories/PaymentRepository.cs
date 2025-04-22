using GasServiceUA.Models;
using GasServiceUA.UnitOfWork;

namespace GasServiceUA.Repositories
{
    public class PaymentRepository : RepositoryBase<Payment>
    {
        public PaymentRepository(IUnitOfWork unitOfwork) : base(unitOfwork) { }
    }
}
