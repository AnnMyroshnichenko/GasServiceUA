using GasServiceUA.Models;
using Microsoft.EntityFrameworkCore;

namespace GasServiceUA.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        public void SaveChanges();
    }
}
