namespace Carter.App.Lib.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MongoDB.Driver;

    /// <summary>
    /// Proxies the MongoDB database to make things like unit testing easier.
    /// </summary>
    /// <typeparam name="TSchema">A MongoDB schema.</typeparam>
    public interface IRepository<TSchema>
    {
        // Query

        /// <summary>
        /// Query for all documents matching a filter per page.
        /// </summary>
        /// <returns>
        /// A page of documents.
        /// </returns>
        /// <param name="query">A filter that includes or excludes documents.</param>
        /// <param name="sorter">A sorter that determines the order documents are returned in.</param>
        /// <param name="page">How many pages to skip, starting at 1.!-- Optional.</param>
        Task<IList<TSchema>> FindAll(FilterDefinition<TSchema> query, SortDefinition<TSchema> sorter, int page = 1);

        /// <summary>
        /// Query for the first document that matches.
        /// </summary>
        /// <returns>
        /// A document.
        /// </returns>
        /// <param name="query">A filter of a specific document to find.</param>
        Task<TSchema> FindOne(FilterDefinition<TSchema> query);

        /// <summary>
        /// Query for the first document by id.
        /// </summary>
        /// <returns>
        /// A document.
        /// </returns>
        /// <param name="id">The id of a document being looked for.</param>
        Task<TSchema> FindById(string id);

        /// <summary>
        /// Counts all documents matching a query.
        /// </summary>
        /// <returns>
        /// A count.
        /// </returns>
        /// <param name="query">A criteria for a document to be counted. Optional.</param>
        Task<long> FindThenCount(FilterDefinition<TSchema> query = null);

        // Mutate

        /// <summary>
        /// Add a document.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="item">A single item to be added.</param>
        Task Add(TSchema item);

        /// <summary>
        /// Update documents.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="filter">A criteria to decide with documents to update.</param>
        /// <param name="update">Applied to each document.</param>
        Task Update(FilterDefinition<TSchema> filter, UpdateDefinition<TSchema> update);

        /// <summary>
        /// Index a collection.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <param name="key">Which key in the documents to index by.</param>
        /// <param name="options">Additional options for the index.</param>
        Task Index(IndexKeysDefinition<TSchema> key, CreateIndexOptions<TSchema> options = null);

        /// <summary>
        /// Configure a collection.
        /// </summary>
        /// <param name="name">Of the collection.</param>
        void Configure(string name);
    }

    /// <summary>
    /// Base implementation of IRepository.
    /// Overrode methods should be sealed to prevent too much nested inheritance.
    /// </summary>
    /// <inheritdoc />
    public abstract class RepositoryBase<TSchema> : IRepository<TSchema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TSchema}"/> class.
        /// </summary>
        /// <param name="db">A database.</param>
        /// <param name="collectionName">A collection name that will be associated with the TSchema.</param>
        public RepositoryBase(IMongoDatabase db, string collectionName)
        {
            // The database needs to be saved for configuring the collection later
            this.Database = db;
            this.Collection = db.GetCollection<TSchema>(collectionName);
        }

        /// <summary>Gets the collection.</summary>
        protected virtual IMongoCollection<TSchema> Collection { get; set; }

        /// <summary>Gets the database. Should only be used for changing collections.</summary>
        protected virtual IMongoDatabase Database { get; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual async Task<TSchema> FindOne(FilterDefinition<TSchema> query)
        {
            return await this.Collection
                .Find(query)
                .FirstOrDefaultAsync();
        }

        // This method is going to be Collection specific,
        // So it will have to be overrode

        /// <inheritdoc />
        public virtual Task<TSchema> FindById(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual async Task<long> FindThenCount(FilterDefinition<TSchema> query = null)
        {
            return await this.Collection.CountDocumentsAsync(query ?? Builders<TSchema>.Filter.Empty);
        }

        /// <inheritdoc />
        public virtual async Task Add(TSchema item)
        {
            await this.Collection.InsertOneAsync(item);
        }

        /// <inheritdoc />
        public virtual async Task Update(
            FilterDefinition<TSchema> filter,
            UpdateDefinition<TSchema> update)
        {
            await this.Collection.UpdateOneAsync(filter, update);
        }

        /// <inheritdoc />
        public virtual async Task Index(
            IndexKeysDefinition<TSchema> key,
            CreateIndexOptions<TSchema> options = null)
        {
            var model = new CreateIndexModel<TSchema>(key, options ?? new CreateIndexOptions());
            await this.Collection.Indexes.CreateOneAsync(model);
        }

        /// <inheritdoc />
        public virtual void Configure(string name)
        {
            this.Collection = this.Database.GetCollection<TSchema>(name);
        }
    }
}