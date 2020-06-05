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

        Task Update(FilterDefinition<TSchema> filter, UpdateDefinition<TSchema> update);

        Task Index(IndexKeysDefinition<TSchema> key, CreateIndexOptions<TSchema> options = null);
    }

    public abstract class RepositoryBase<TSchema> : IRepository<TSchema>
    {
        public RepositoryBase(IMongoDatabase db, string collectionName)
        {
            this.Collection = db.GetCollection<TSchema>(collectionName);
        }

        protected virtual IMongoCollection<TSchema> Collection { get; set; }

        public virtual async Task<IList<TSchema>> FindAll(
            FilterDefinition<TSchema> filter,
            SortDefinition<TSchema> sorter = null)
        {
            if (sorter == null)
            {
                return await this.Collection
                    .Find(filter)
                    .Sort(sorter)
                    .ToListAsync();
            }
            else
            {
                return await this.Collection
                    .Find(filter)
                    .ToListAsync();
            }
        }

        public virtual async Task<TSchema> FindOne(FilterDefinition<TSchema> query)
        {
            return await this.Collection
                .Find(query)
                .FirstOrDefaultAsync();
        }

        // This method is going to be Collection specific,
        // So it will have to overrode
        public virtual Task<TSchema> FindById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task Add(TSchema item)
        {
            await this.Collection.InsertOneAsync(item);
        }

        public virtual async Task Update(
            FilterDefinition<TSchema> filter,
            UpdateDefinition<TSchema> update)
        {
            await this.Collection.UpdateOneAsync(filter, update);
        }

        public virtual async Task Index(
            IndexKeysDefinition<TSchema> key,
            CreateIndexOptions<TSchema> options = null)
        {
            var model = new CreateIndexModel<TSchema>(key, options ?? new CreateIndexOptions());
            await this.Collection.Indexes.CreateOneAsync(model);
        }
    }
}