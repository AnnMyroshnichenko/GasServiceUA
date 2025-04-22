using GasServiceUA.Data;
using GasServiceUA.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GasServiceUA.Repositories
{
    public abstract class RepositoryBase<T> : ControllerBase, IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected DbSet<T> dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public RepositoryBase(IUnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            dbSet = _unitOfWork.Context.Set<T>();
        }

        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public T GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public T Create(T entity)
        {
            dbSet.Add(entity);
            _unitOfWork.SaveChanges();
            return entity;
        }

        public IActionResult Update(int id, T entity)
        {
            var existingOrder = dbSet.Find(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            _unitOfWork.Context.Entry(existingOrder).CurrentValues.SetValues(entity);

            try
            {
                _unitOfWork.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        public IActionResult Delete(int id)
        {
            var data = dbSet.Find(id);
            if (data == null)
            {
                return NotFound();
            }

            dbSet.Remove(data);
            _unitOfWork.SaveChanges();
            return NoContent();
        }
    }
}
