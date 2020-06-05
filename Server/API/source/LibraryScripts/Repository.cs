namespace Carter.App.Lib.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MongoDB.Driver;

    public interface IRepository<TSchema>
    {
        // Query
        Task<IList<TSchema>> FindAll(FilterDefinition<TSchema> query, SortDefinition<TSchema> sorter);

        Task<TSchema> FindOne(FilterDefinition<TSchema> query);

        Task<TSchema> FindById(string id);

        // Mutate
        Task Add(TSchema item);

        Task Delete(TSchema item);

        Task Update(TSchema item);

        Task Index(CreateIndexModel<TSchema> index);
    }

    public abstract class RepositoryBase<TSchema> : IRepository<TSchema>
    {
        public RepositoryBase(IMongoDatabase db, string collectionName)
        {
            this.Collection = db.GetCollection<TSchema>(collectionName);
        }

        protected virtual IMongoCollection<TSchema> Collection { get; set; }

        public virtual Task<IList<TSchema>> FindAll(FilterDefinition<TSchema> query, SortDefinition<TSchema> sorter)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TSchema> FindOne(FilterDefinition<TSchema> query)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TSchema> FindById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual Task Add(TSchema item)
        {
            throw new NotImplementedException();
        }

        public virtual Task Delete(TSchema item)
        {
            throw new NotImplementedException();
        }

        public virtual Task Update(TSchema item)
        {
            throw new NotImplementedException();
        }

        public virtual Task Index(CreateIndexModel<TSchema> index)
        {
            throw new NotImplementedException();
        }
    }
}