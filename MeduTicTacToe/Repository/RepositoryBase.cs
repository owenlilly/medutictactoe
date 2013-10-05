using System;
using System.Web;
using System.Web.Caching;


namespace MeduTicTacToe.Repository
{
    public class RepositoryBase<T> : IRepository<T>
    {
        protected readonly Cache MemCache;

        public RepositoryBase()
            :this(HttpContext.Current.Cache)
        { }

        public RepositoryBase(Cache cache)
        {
            MemCache = cache;
        }

        public virtual bool Any(string id)
        {
            return MemCache.Get(id) != null;
        }

        public virtual void Save(string id, T data)
        {
            MemCache.Insert(id, data, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 15, 0));
        }

        public virtual void Update(string id, T data)
        {
            Delete(id);
            Save(id, data);
        }

        public virtual T Get(string id)
        {
            return (T)MemCache.Get(id);
        }

        public virtual T Delete(string id)
        {
            return (T)MemCache.Remove(id);
        }
    }
}