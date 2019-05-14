using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IRepository<T> where T:class
    {
        IQueryable<T> GetAll();     //Select * işlemi
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate); // select * from where ...
        T GetById(int id); //select * from where id=...
        T Get(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        void AddRange(List<T> entities);

    }
}
