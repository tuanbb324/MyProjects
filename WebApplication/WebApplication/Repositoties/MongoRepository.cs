using MongoDB.Driver;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace WebApplication.Repositories
{
    public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<TEntity> DbSet;

        protected MongoRepository(IMongoContext context)
        {
            Database = context.Database;
            DbSet = Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual async Task<TEntity> Add(TEntity obj)
        {
            await DbSet.InsertOneAsync(obj);
            return obj;
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            var data = await DbSet.Find(FilterId(id)).SingleOrDefaultAsync();
            return data;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public async virtual Task<TEntity> Update(string id, TEntity obj)
        {
            await DbSet.ReplaceOneAsync(FilterId(id), obj);
            return obj;
        }

        public async virtual Task<bool> Remove(string id)
        {
            var result = await DbSet.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }
        public virtual async Task<IEnumerable<TEntity>> Filter(object obj)
        {
            var filterDefinition = FilterAny(obj);
            var result = await DbSet.FindAsync(filterDefinition);
            return result.ToList();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        private static FilterDefinition<TEntity> FilterId(string key)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", key);
            return filter;
        }
        private static FilterDefinition<TEntity> FilterAny(object item)
        {
            var filter = Builders<TEntity>.Filter.Empty;
            foreach (PropertyInfo p in item.GetType().GetProperties())
            {
                object _val = p.GetValue(item, null);
                string _name = p.Name;
                if (_val != null)
                {
                    if (_val.GetType() == typeof(string[]))
                    {
                        string _tempVal = string.Join(";", (string[])_val);
                        filter &= Builders<TEntity>.Filter.Eq(_name, _tempVal);
                    }
                    else
                    {
                        filter &= Builders<TEntity>.Filter.Eq(_name, _val);
                    }
                }
            }

            return filter;
        }
    }
}
