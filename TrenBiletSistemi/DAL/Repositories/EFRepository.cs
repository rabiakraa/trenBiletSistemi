using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// EntityFramework için hazırlıyor olduğumuz bu repositoriyi daha önceden tasarladığımız generic repositorimiz olan IRepository arayüzünü implemente ederek tasarladık.
    /// Bu şekilde tasarlamamızın ana sebebi ise veritabanına independent(bağımsız) bir durumda kalabilmek. Örneğin MongoDB için ise ilgili provider'ı aracılığı ile MongoDBRepository tasarlayabiliriz.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public EFRepository(Context dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("DbContext null olamaz.");

            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public void AddRange(List<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).SingleOrDefault();
        }


        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }


        public void Delete(T entity)
        {
            if (entity.GetType().GetProperty("SilindiMi") != null)
            {
                entity.GetType().GetProperty("SilindiMi").SetValue(entity, true);
                T _entity = entity;
                this.Update(_entity);
            }
        }

        public bool Delete(int id)
        {
            var entity = GetById(id);
              if (entity == null) return false;
            else
            {
                Delete(entity);
                return true;
            }
        }

      
    }
}
