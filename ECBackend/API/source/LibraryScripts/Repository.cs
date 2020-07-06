namespace Carter.App.Lib.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MongoDB.Driver;

    public interface IRepository<TSchema>
    {
        // Query
        Task<IList<TSchema>> FindAll(FilterDefinition<TSchema> query, SortDefinition<TSchema> sorter, int page = 1);

        Task<TSchema> FindOne(FilterDefinition<TSchema> query);

        Task<TSchema> FindById(string id);

        Task<long> FindThenCount(FilterDefinition<TSchema> query = null);

        // Mutate
        Task Add(TSchema item);

        Task Update(FilterDefinition<TSchema> filter, UpdateDefinition<TSchema> update);

        Task Index(IndexKeysDefinition<TSchema> key, CreateIndexOptions<TSchema> options = null);

        void Configure(string name);
    }

    public abstract class RepositoryBase<TSchema> : IRepository<TSchema>
    {
        public RepositoryBase(IMongoDatabase db, string collectionName)
        {
            // The database needs to be saved for configuring the collection later
            this.Database = db;
            this.Collection = db.GetCollection<TSchema>(collectionName);
        }

        protected virtual IMongoCollection<TSchema> Collection { get; set; }

        protected virtual IMongoDatabase Database { get; set; }

        public virtual async Task<IList<TSchema>> FindAll(
            FilterDefinition<TSchema> filter,
            SortDefinition<TSchema> sorter,
            int page = 1)
        {
            return await this.Collection
                .Find(filter)
                .Sort(sorter)
                .ToListAsync();
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

        public virtual async Task<long> FindThenCount(FilterDefinition<TSchema> query = null)
        {
            return await this.Collection.CountDocumentsAsync(query ?? Builders<TSchema>.Filter.Empty);
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

        public virtual void Configure(string name)
        {
            this.Collection = this.Database.GetCollection<TSchema>(name);
        }
    }
}