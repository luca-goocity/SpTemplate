using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SpRepo.Models.Entities;

namespace SpRepo.Abstraction;

public interface IRepository<T> where T : BaseEntity
{
    T? GetLastAdded();
    
    IMongoQueryable<T> GetAllAsQueryable();

    IMongoQueryable<T> FindAllAsQueryable(Expression<Func<T, bool>> filter);

    Task<List<T>> GetAllAsync(CancellationToken cancellation = default);

    List<T> GetAll(CancellationToken cancellation = default);

    Task<T?> FindByIdAsync(string id, CancellationToken cancellation = default);

    T? FindById(string id, CancellationToken cancellation = default);

    Task<List<T>> FindAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    Task<List<T>> FindAllAsync(FilterDefinition<T> filter, CancellationToken cancellation = default);

    List<T> FindAll(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    List<T> FindAll(FilterDefinition<T> filter, CancellationToken cancellation = default);

    Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    T? Find(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    Task<bool> Exists(string id, CancellationToken cancellation = default);

    Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    long Count(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellation = default);

    void CopyFrom(string fromCollection, CancellationToken cancellation = default, RenameCollectionOptions? options = null, string? newCollectionName = null);

    void Drop(CancellationToken cancellation = default, string? newcollectionName = null);

    void Add(T t, CancellationToken cancellationToken = default, InsertOneOptions? options = null);

    Task AddAsync(T t, CancellationToken cancellationToken = default, InsertOneOptions? options = null);

    void AddAll(IEnumerable<T> t, CancellationToken cancellationToken = default, InsertManyOptions? options = null);

    Task AddAllAsync(IEnumerable<T> t, CancellationToken cancellationToken = default, InsertManyOptions? options = null);

    T? AddOrUpdate(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default);

    Task<T?> AddOrUpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default);

    long BulkAddOrUpdate(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null);

    Task<long> BulkAddOrUpdateAsync(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null);

    long BulkInsert(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null);

    Task<long> BulkInsertAsync(Expression<Func<T, bool>> condition, IEnumerable<T?> records, CancellationToken cancellationToken = default, BulkWriteOptions? options = null);

    bool Update(T updated, string key, CancellationToken cancellationToken = default);

    bool UpdateAll(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default);

    Task<bool> UpdateAllAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default);

    Task<T?> UpdateAsync(T updated, string key, CancellationToken cancellationToken = default);

    Task<T?> UpdateAsync(Expression<Func<T, bool>> condition, T entity, CancellationToken cancellationToken = default);

    bool UpdateFields(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default);

    Task<bool> UpdateFieldsAsync(Expression<Func<T, bool>> condition, UpdateDefinition<T> updateDefinition, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default);
}