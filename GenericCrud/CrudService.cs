using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GenericCrud
{
    public class CrudService<TEntity, TDto, TKey> : ICrudService<TEntity, TDto, TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public CrudService(DbContext DbContext, IMapper mapper)
        {
            _dbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<TDto?> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.Id.Equals(id))
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync(filter: null, cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            filter ??= x => true;
            IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                .AsNoTracking()
            .Where(filter);

            return await query
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetAsync(IEnumerable<Expression<Func<TEntity, bool>>>? filters = null, CancellationToken cancellationToken = default)
        {
            filters ??= [];
            IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                .AsNoTracking();

            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }

            return await query
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(Expression<Func<TEntity, TSort>>? sortBy = null, bool ascending = true, CancellationToken cancellationToken = default)
        {
            return await GetOrderedAsync(filter: null, sortBy, ascending, cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, TSort>>? sortBy = null, bool ascending = true, CancellationToken cancellationToken = default)
        {
            filter ??= x => true;
            IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(filter);

            if (sortBy != null)
            {
                if (ascending)
                    query = query.OrderBy(sortBy);
                else
                    query = query.OrderByDescending(sortBy);
            }

            return await query
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TDto>> GetOrderedAsync<TSort>(IEnumerable<Expression<Func<TEntity, bool>>>? filters = null, Expression<Func<TEntity, TSort>>? sortBy = null, bool ascending = true, CancellationToken cancellationToken = default)
        {
            filters ??= [];
            IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                .AsNoTracking();

            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }

            if (sortBy != null)
            {
                if (ascending)
                    query = query.OrderBy(sortBy);
                else
                    query = query.OrderByDescending(sortBy);
            }

            return await query
              .ProjectTo<TDto>(_mapper.ConfigurationProvider)
              .ToListAsync(cancellationToken);
        }

        public async Task<TKey> CreateAsync(TDto dto)
        {
            TEntity entity = _mapper.Map<TEntity>(dto);
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task DeleteAsync(TKey id)
        {
            TEntity entity = await _dbContext.Set<TEntity>()
                .SingleOrDefaultAsync(e => e.Id.Equals(id))
                ?? throw new ObjectNotFoundException();

            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TKey id, TDto dto)
        {
            TEntity? entity = await _dbContext
                .Set<TEntity>()
                .Where(e => e.Id.Equals(id))
                .SingleOrDefaultAsync()
                ?? throw new ObjectNotFoundException();

            _mapper.Map(dto, entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}