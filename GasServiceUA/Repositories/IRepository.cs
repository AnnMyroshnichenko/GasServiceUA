using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GasServiceUA.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        T GetByID(object id);

        T Create(T entity);

        IActionResult Update(int id, T entity);

        IActionResult Delete(int id);
    }
}
