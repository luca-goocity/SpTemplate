using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Linq;
using SpRepo.Abstraction;
using SpRepo.Models.Entities;

namespace SpRepo.Implementation;

public sealed class AtlasRepository<T> : IAtlasRepository<T> where T : BaseEntity
{
    private readonly IClientSessionHandle clientSession;

    private readonly string collectionName;

    private readonly IMongoDatabase database;

    public AtlasRepository(string connectionString, bool logQueryOnConsole = false, string? collectionName = null)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrlBuilder.ToMongoUrl());

        if (logQueryOnConsole)
        {
            settings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e => { Debug.WriteLine($"{e.CommandName} - {e.Command}"); });
            };
        }

        var client = new MongoAtlasClient(settings);
        
        this.clientSession = client.StartSession();
        
        var databaseName = MongoUrl.Create(connectionString).DatabaseName;
        this.database = client.GetDatabase(databaseName);
        
        this.collectionName = collectionName ?? typeof(T).Name;
    }

    private IMongoCollection<T> _collection => database.GetCollection<T>(collectionName);
    private IMongoQueryable<T> _collectionQueryable => _collection.AsQueryable();


    public T? GetLastAdded() => _collectionQueryable.OrderByDescending(xx => xx.Id).FirstOrDefault();

    public IMongoQueryable<T> GetAllAsQueryable() => _collectionQueryable;

    public IMongoQueryable<T> FindAllAsQueryable(Expression<Func<T, bool>> filter) => _collectionQueryable.Where(filter);

    public Task<List<T>> GetAllAsync(CancellationToken cancellation = default) =>
        FindAllAsync(s => true, cancellation);

    public List<T> GetAll(CancellationToken cancellation = default) =>
        FindAll(s => true, cancellation);


    public Task<T?> FindByIdAsync(string id, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, Builders<T>.Filter.Eq(x => x.Id, id))
                   .SingleOrDefaultAsync(cancellation) as Task<T?>;


    public T? FindById(string id, CancellationToken cancellation = default) =>
        (FindAll(Builders<T>.Filter.Eq(x => x.Id, id), cancellation)).SingleOrDefault();

    public Task<List<T>> FindAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .ToListAsync(cancellation);

    public List<T> FindAll(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .ToList(cancellation);


    public Task<List<T>> FindAllAsync(FilterDefinition<T> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .ToListAsync(cancellation);

    public List<T> FindAll(FilterDefinition<T> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .ToList(cancellation);

    public Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .SingleOrDefaultAsync(cancellation) as Task<T?>;

    public T? Find(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.Find(clientSession, filter)
                   .SingleOrDefault(cancellation);

    public Task<bool> Exists(string id, CancellationToken cancellation = default) =>
        Exists(s => s.Id == id, cancellation);

    public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        0 < await CountAsync(filter, cancellation);

    public long Count(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.CountDocuments(clientSession, filter, cancellationToken: cancellation);

    public Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default) =>
        _collection.CountDocumentsAsync(clientSession, filter, cancellationToken: cancellation);


    public void CopyFrom(string fromCollection, CancellationToken cancellation = default, RenameCollectionOptions? options = null, string? newCollectionName = null)
    {
        Drop(cancellation);
        database.RenameCollection(fromCollection, string.IsNullOrEmpty(newCollectionName) ? collectionName : newCollectionName, options, cancellation);
    }

    public void Drop(CancellationToken cancellation = default, string? newcollectionName = null)
    {
        database.DropCollection(string.IsNullOrEmpty(newcollectionName) ? collectionName : newcollectionName, cancellation);
    }

    public void Add(T t, CancellationToken cancellationToken = default, InsertOneOptions? options = null) => _collection.InsertOne(clientSession, t, options, cancellationToken);

    public Task AddAsync(T t, CancellationToken cancellationToken = default, InsertOneOptions? options = null) => _collection.InsertOneAsync(clientSession, t, options, cancellationToken);

    public void AddAll(IEnumerable<T> t, CancellationToken cancellationToken = default, InsertManyOptions? options = null) => _collection.InsertMany(clientSession, t, options, cancellationToken);

    public Task AddAllAsync(IEnumerable<T> t, CancellationToken cancellationToken = default, InsertManyOptions? options = null) => _collection.InsertManyAsync(clientSession, t, options, cancellationToken);

    public T? AddOrUpdate(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default)
    {
        var resultOk = _collection.ReplaceOne(clientSession, condition, entity, new ReplaceOptions
        {
            IsUpsert = true
        }, cancellationToken);

        return resultOk.IsAcknowledged ? entity : null;
    }

    public async Task<T?> AddOrUpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var resultOk = await _collection.ReplaceOneAsync(clientSession, condition, entity, new ReplaceOptions
            {
                IsUpsert = true
            }, cancellationToken);

            return resultOk.IsAcknowledged ? entity : null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public long BulkAddOrUpdate(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null)
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var record in records)
        {
            if (record == null) continue;
            var upsertOne = new ReplaceOneModel<T>(condition, record)
            {
                IsUpsert = true
            };

            bulkOps.Add(upsertOne);
        }

        options ??= new BulkWriteOptions
        {
            BypassDocumentValidation = false,
            IsOrdered = false
        };

        return _collection.BulkWrite(clientSession, bulkOps, options, cancellationToken).InsertedCount;
    }

    public async Task<long> BulkAddOrUpdateAsync(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null)
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var record in records)
        {
            if (record == null) continue;
            var upsertOne = new ReplaceOneModel<T>(condition, record)
            {
                IsUpsert = true
            };

            bulkOps.Add(upsertOne);
        }

        options ??= new BulkWriteOptions
        {
            BypassDocumentValidation = false,
            IsOrdered = false
        };

        var result = await _collection.BulkWriteAsync(clientSession, bulkOps, options, cancellationToken);

        return result.InsertedCount;
    }

    public long BulkInsert(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null)
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var record in records)
        {
            if (record == null) continue;
            var upsertOne = new ReplaceOneModel<T>(condition, record)
            {
                IsUpsert = false
            };

            bulkOps.Add(upsertOne);
        }

        options ??= new BulkWriteOptions
        {
            BypassDocumentValidation = false,
            IsOrdered = false
        };

        return _collection.BulkWrite(clientSession, bulkOps, options, cancellationToken).InsertedCount;
    }

    public async Task<long> BulkInsertAsync(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null)
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var record in records)
        {
            if (record == null) continue;
            var upsertOne = new ReplaceOneModel<T>(condition, record)
            {
                IsUpsert = false
            };

            bulkOps.Add(upsertOne);
        }

        options ??= new BulkWriteOptions
        {
            BypassDocumentValidation = false,
            IsOrdered = false
        };

        var result = await _collection.BulkWriteAsync(clientSession, bulkOps, options, cancellationToken);

        return result.InsertedCount;
    }

    public bool Update(T entity, string key, CancellationToken cancellationToken = default)
    {
        var result = _collection.ReplaceOne(clientSession, x => x.Id == key, entity, new ReplaceOptions
        {
            IsUpsert = false
        }, cancellationToken);

        return result.IsAcknowledged;
    }

    public bool UpdateAll(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default)
    {
        var result = _collection.UpdateMany(clientSession, filter, updateDefinition, cancellationToken: cancellationToken);

        return result.IsAcknowledged;
    }

    public async Task<bool> UpdateAllAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default)
    {
        var result = await _collection.UpdateManyAsync(clientSession, filter, updateDefinition, cancellationToken: cancellationToken);

        return result.IsAcknowledged;
    }

    public async Task<T?> UpdateAsync(T entity, string key, CancellationToken cancellationToken = default)
    {
        var resultOk = await _collection.ReplaceOneAsync(clientSession, x => x.Id == key, entity, new ReplaceOptions
        {
            IsUpsert = false
        }, cancellationToken);

        return (resultOk.IsAcknowledged) ? entity : null;
    }

    public async Task<T?> UpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default)
    {
        var resultOk = await _collection.ReplaceOneAsync(clientSession, condition, entity, new ReplaceOptions
        {
            IsUpsert = false
        }, cancellationToken);

        return (resultOk.IsAcknowledged) ? entity : null;
    }


    public bool UpdateFields(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default)
    {
        //example usage
        //var upd = Builders<CandeleDoc>.Update.Set(r => r.PrezzoApertura, cc.PrezzoApertura)
        //    .Set(r => r.Volumi, cc.pre)
        //    .Set(r => r.PrezzoChiusura, cc.PrezzoChiusura)
        //    .Set(r => r.PrezzoMinimo, cc.PrezzoMinimo)
        //    .Set(r => r.PrezzoMassimo, cc.PrezzoMassimo);

        var result = _collection.UpdateMany(clientSession, condition, updateDefinition, new UpdateOptions
        {
            IsUpsert = false
        });
        return result.IsAcknowledged;
    }


    public async Task<bool> UpdateFieldsAsync(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default)
    {
        //example usage
        //var upd = Builders<CandeleDoc>.Update.Set(r => r.PrezzoApertura, cc.PrezzoApertura)
        //    .Set(r => r.Volumi, cc.pre)
        //    .Set(r => r.PrezzoChiusura, cc.PrezzoChiusura)
        //    .Set(r => r.PrezzoMinimo, cc.PrezzoMinimo)
        //    .Set(r => r.PrezzoMassimo, cc.PrezzoMassimo);

        var result = await _collection.UpdateManyAsync(clientSession, condition, updateDefinition, new UpdateOptions
        {
            IsUpsert = false
        }, cancellationToken);

        return result.IsAcknowledged;
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(key, cancellationToken);
        return result.IsAcknowledged;
    }
    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> key, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteManyAsync(key, cancellationToken);
        return result.IsAcknowledged;
    }
}