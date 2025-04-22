using GasServiceUA.Models;
using GasServiceUA.UnitOfWork;

namespace GasServiceUA.Repositories
{
    public class MeterReadingRepository : RepositoryBase<MeterReading>
    {
        public MeterReadingRepository(IUnitOfWork unitOfwork) : base(unitOfwork) { }
    }
}
