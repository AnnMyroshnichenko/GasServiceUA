using GasServiceUA.Models;
using GasServiceUA.UnitOfWork;

namespace GasServiceUA.Repositories
{
    public class TariffRepository : RepositoryBase<Tariff>
    {
        public TariffRepository(IUnitOfWork unitOfwork) : base(unitOfwork) { }
    }
}
