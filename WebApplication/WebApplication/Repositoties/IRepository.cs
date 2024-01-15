using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WebApplication.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> Add(TEntity obj);
        Task<TEntity> GetById(string id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Update(string id, TEntity obj);
        Task<bool> Remove(string id);
        Task<IEnumerable<TEntity>> Filter(object obj);
    }
}
