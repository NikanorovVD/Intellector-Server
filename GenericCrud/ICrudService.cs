using System.Linq.Expressions;

namespace GenericCrud
{
    public interface ICrudService<TEntity, TDto, TKey>
    {
        public Task<TDto?> GetAsync(TKey id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TDto>> GetAsync(CancellationToken cancellationToken = default);
        public Task<IEnumerable<TDto>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = default,
            CancellationToken cancellationToken = default);
        public Task<IEnumerable<TDto>> GetAsync(
           IEnumerable<Expression<Func<TEntity, bool>>>? filters = default,
           CancellationToken cancellationToken = default);

        public Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(
            Expression<Func<TEntity, TSort>>? sortBy = default,
            bool ascending = true,
            CancellationToken cancellationToken = default);

        public Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(
            Expression<Func<TEntity, bool>>? filter = default,
            Expression<Func<TEntity, TSort>>? sortBy = default,
            bool ascending = true,
            CancellationToken cancellationToken = default);

        public Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(
            IEnumerable<Expression<Func<TEntity, bool>>>? filters = default,
            Expression<Func<TEntity, TSort>>? sortBy = default,
            bool ascending = true,
            CancellationToken cancellationToken = default);

        public Task<TKey> CreateAsync(TDto dto);
        public Task UpdateAsync(TKey id, TDto dto);
        public Task DeleteAsync(TKey id);
    }
}