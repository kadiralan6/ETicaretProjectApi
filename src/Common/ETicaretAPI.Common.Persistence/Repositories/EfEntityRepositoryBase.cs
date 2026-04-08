using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Domain.Entities;
using ETicaretAPI.Common.Domain.Interfaces;
using ETicaretAPI.Common.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ETicaretAPI.Common.Persistence.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity<int>, new()
        where TContext : DbContext
    {
        protected readonly TContext _context;

        public EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }

        #region Synchronous Methods
        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }


            return query.ToList();
        }

        public IEnumerable<TEntity> GetAllWithAsNoTracking(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToList();
        }

        /// <summary>
        /// Gets all entities from the database.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns>A list of entities</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<TEntity> GetAllWithSelector(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null)
        {
            var result = predicate == null
                ? _context.Set<TEntity>().Select(selector).ToList()
                : _context.Set<TEntity>().Where(predicate).Select(selector).ToList();

            return result;
        }

        public IEnumerable<TEntity> GetAllAsNoTrackingWithSelector(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null)
        {
            var result = predicate == null
                ? _context.Set<TEntity>().AsNoTracking().Select(selector).ToList()
                : _context.Set<TEntity>().AsNoTracking().Where(predicate).Select(selector).ToList();

            return result;
        }

        /// <summary>
        /// Gets all entities from the database with pagination.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <param name="selector">An optional list of related entities to selector.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of entities per page.</param>
        /// <returns>A list of entities.</returns>
        public IPagedResult<TEntity> GetAllWithPagination(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null, IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (selector != null)
            {
                query = query.Select(selector);
            }

            var rowCount = query.Count();

            query = query.AddOrder(orders);

            var result = query.GetPaged(page, pageSize, rowCount);

            return result;
        }

        public IPagedResult<TEntity> GetAllAsNoTrackingWithPagination(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null, IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (selector != null)
            {
                query = query.Select(selector);
            }

            var rowCount = query.Count();

            query = query.AddOrder(orders);

            var result = query.GetPaged(page, pageSize, rowCount);

            return result;
        }

        /// <summary>
        /// Gets a single entity from the database based on the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the entity.</param>
        public TEntity? Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public TEntity? GetWithAsNoTracking(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Gets a single entity from the database based on the provided predicate and selector.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TEntity? GetWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null)
        {
            var result = predicate == null
                ? _context.Set<TEntity>().Select(selector).FirstOrDefault()
                : _context.Set<TEntity>().Where(predicate).Select(selector).FirstOrDefault();

            return result;
        }

        public TEntity? GetAsNoTrackingWithSelector(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null)
        {
            var result = predicate == null
                ? _context.Set<TEntity>().AsNoTracking().Select(selector).FirstOrDefault()
                : _context.Set<TEntity>().AsNoTracking().Where(predicate).Select(selector).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public TEntity Add(TEntity entity)
        {
            var addedEntity = _context.Set<TEntity>().Add(entity);

            return addedEntity.Entity;
        }

        /// <summary>
        /// Adds a range of entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A list of added entities.</returns>
        public List<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            var entitiesList = entities.ToList();

            // ID'leri set et
            foreach (var entity in entitiesList.Where(e => e.Id == 0))
            {
                entity.Id = 0;
            }

            _context.Set<TEntity>().AddRange(entitiesList);

            return entitiesList;
        }

        /// <summary>
        /// Updates an entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public TEntity Update(TEntity entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.CurrentValues["ModifiedAt"] = DateTime.UtcNow;
            updatedEntity.State = EntityState.Modified;


            return updatedEntity.Entity;
        }

        /// <summary>
        /// Updates a range of entities in the database.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>A list of updated entities.</returns>
        public List<TEntity> UpdateRange(IEnumerable<TEntity> entities)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var updatedEntity = _context.Entry(entity);
                updatedEntity.CurrentValues["ModifiedAt"] = DateTime.UtcNow;
                updatedEntity.State = EntityState.Modified;
            }

            return entitiesList;
        }

        /// <summary>
        /// Hard deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public bool HardDelete(TEntity entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;


            return true;
        }

        /// <summary>
        /// Hard deletes a range of entities from the database.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <returns>A list of booleans indicating the success of each deletion.</returns>
        public List<bool> HardDeleteRange(IEnumerable<TEntity> entities)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var deletedEntity = _context.Entry(entity);
                deletedEntity.State = EntityState.Deleted;
            }


            return entitiesList.Select(e => true).ToList();
        }

        /// <summary>
        /// Soft deletes an entity from the database by marking it as deleted.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <returns>True if the soft deletion was successful, otherwise false.</returns>
        public bool SoftDelete(TEntity entity)
        {
            var entry = _context.Entry(entity);

            // Only mark IsDeleted and DeletedAt as modified
            // Other changed properties on the entity are already tracked by EF Core
            // and will appear in audit log
            entry.Property("IsDeleted").CurrentValue = true;
            entry.Property("IsDeleted").IsModified = true;

            entry.Property("DeletedAt").CurrentValue = DateTime.UtcNow;
            entry.Property("DeletedAt").IsModified = true;


            return true;
        }

        /// <summary>
        /// Soft deletes a range of entities from the database by marking them as deleted.
        /// </summary>
        /// <param name="entities">The entities to soft delete.</param>
        /// <returns>A list of booleans indicating the success of each soft deletion.</returns>
        public List<bool> SoftDeleteRange(IEnumerable<TEntity> entities)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var entry = _context.Entry(entity);

                // Only mark IsDeleted and DeletedAt as modified
                // Other changed properties on the entity are already tracked by EF Core
                // and will appear in audit log
                entry.Property("IsDeleted").CurrentValue = true;
                entry.Property("IsDeleted").IsModified = true;

                entry.Property("DeletedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("DeletedAt").IsModified = true;
            }

            return entitiesList.Select(e => true).ToList();
        }

        /// <summary>
        /// Gets a list of entities using a stored procedure.
        /// </summary>
        /// <param name="prosedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters to pass to the stored procedure.</param>
        /// <returns>A list of entities.</returns>
        public List<TEntity> GetListWithStoredProsedure(string prosedureName, object[] parameters = null)
        {
            var result = _context.Set<TEntity>().FromSqlRaw(prosedureName, parameters).ToList();

            return result;
        }

        /// <summary>
        /// Executes a stored procedure.
        /// </summary>
        /// <param name="prosedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters to pass to the stored procedure.</param>
        /// <returns>True if the execution was successful, otherwise false.</returns>
        public bool ExecuteStoredProcedure(string prosedureName, object[] parameters = null)
        {
            var result = _context.Database.ExecuteSqlRaw(prosedureName, parameters);

            return result > 0;
        }

        /// <summary>
        /// Counts the number of entities in the database.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <returns>The count of entities.</returns>
        public int Count(Expression<Func<TEntity, bool>>? predicate = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.Count();
        }

        /// <summary>
        /// Calculates the average of a numeric property for the entities in the database.
        /// </summary>
        /// <param name="columnName">The name of the column to calculate the average for.</param>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <returns>The average value.</returns>
        public decimal Average(string columnName, Expression<Func<TEntity, bool>>? predicate = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var propertyInfo = typeof(TEntity).GetProperty(columnName);
            var result = query.Select(e => propertyInfo.GetValue(e, null)).ToList();

            return result.Average(e => Convert.ToDecimal(e));
        }

        #endregion

        #region Async Methods
        /// <summary>
        /// Asynchronously gets all entities from the database.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <returns>A list of entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllWithAsNoTrackingAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }


            return await query.ToListAsync(cancellationToken);
        }



        /// <summary>
        /// Asynchronously gets all entities from the database with a selector.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <param name="selector">An optional selector to project the entities.</param>
        /// <returns>A list of entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAllWithSelectorAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default)
        {
            var result = predicate == null
                ? await _context.Set<TEntity>().Select(selector).ToListAsync(cancellationToken)
                : await _context.Set<TEntity>().Where(predicate).Select(selector).ToListAsync(cancellationToken);



            return result;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsNoTrackingWithSelectorAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default)
        {
            var result = predicate == null
                ? await _context.Set<TEntity>().AsNoTracking().Select(selector).ToListAsync(cancellationToken)
                : await _context.Set<TEntity>().AsNoTracking().Where(predicate).Select(selector).ToListAsync(cancellationToken);


            return result;
        }

        /// <summary>
        /// Asynchronously gets a single entity from the database based on the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the entity.</param>
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }



            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> GetWithAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>>[]? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets all entities from the database with pagination.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <param name="selector">An optional list of related entities to selector.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of entities per page.</param>
        /// <returns>A list of entities.</returns>
        public async Task<IPagedResult<TEntity>> GetAllWithPaginationAsync(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null, IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (selector != null)
            {
                query = query.Select(selector);
            }

            var rowCount = await query.CountAsync(cancellationToken);

            query = query.AddOrder(orders);

            var result = await query.GetPagedAsync(page, pageSize, rowCount);

            return result;
        }

        public async Task<IPagedResult<TEntity>> GetAllAsNoTrackingWithPaginationAsync(int page = 1, int pageSize = 10, Expression<Func<TEntity, bool>> predicate = null, IEnumerable<(Expression<Func<TEntity, object>> orderSelector, bool orderAsc)> orders = null, Expression<Func<TEntity, TEntity>> selector = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (selector != null)
            {
                query = query.Select(selector);
            }

            var rowCount = await query.CountAsync(cancellationToken);

            query = query.AddOrder(orders);

            var result = await query.GetPagedAsync(page, pageSize, rowCount);

            return result;
        }

        /// <summary>
        /// Asynchronously gets a single entity from the database based on the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter the entity.</param>
        /// <param name="includeProperties">Optional properties to include in the query.</param>
        /// <returns>The first entity that matches the predicate, or null if no entity matches.</returns>
        public async Task<TEntity?> GetWithSelectorAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default)
        {
            var result = selector == null
                ? await _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken)
                : await _context.Set<TEntity>().Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);


            return result;
        }

        public async Task<TEntity?> GetAsNoTrackingWithSelectorAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>>? selector = null, CancellationToken cancellationToken = default)
        {
            var result = selector == null
                ? await _context.Set<TEntity>().AsNoTracking().Where(predicate).FirstOrDefaultAsync(cancellationToken)
                : await _context.Set<TEntity>().AsNoTracking().Where(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Asynchronously adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var addedEntity = await _context.Set<TEntity>().AddAsync(entity, cancellationToken);


            return addedEntity.Entity;
        }

        /// <summary>
        /// Asynchronously adds a range of entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A list of added entities.</returns>
        public async Task<List<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var entitiesList = entities.ToList();

            // Ensure all entities have valid IDs
            foreach (var entity in entitiesList.Where(e => e.Id.Equals(default(int))))
            {
                entity.Id = 0;
            }

            await _context.Set<TEntity>().AddRangeAsync(entitiesList, cancellationToken);

            return entitiesList;
        }

        /// <summary>
        /// Asynchronously updates an entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public async Task<TEntity> UpdateAsync(TEntity? entity, CancellationToken cancellationToken = default)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.CurrentValues["ModifiedAt"] = DateTime.UtcNow;
            updatedEntity.State = EntityState.Modified;


            return updatedEntity.Entity;
        }

        /// <summary>
        /// Asynchronously updates a range of entities in the database.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <returns>A list of updated entities.</returns>
        public async Task<List<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var updatedEntity = _context.Entry(entity);
                updatedEntity.CurrentValues["ModifiedAt"] = DateTime.UtcNow;
                updatedEntity.State = EntityState.Modified;
            }


            return entitiesList;
        }

        /// <summary>
        /// Asynchronously hard deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> HardDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;


            return true;
        }

        /// <summary>
        /// Asynchronously hard deletes a range of entities from the database.
        /// <summary>
        /// <param name="entities">The entities to delete.</param>
        /// <returns>A list of booleans indicating the success of each deletion.</returns>
        public async Task<List<bool>> HardDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var deletedEntity = _context.Entry(entity);
                deletedEntity.State = EntityState.Deleted;
            }


            return entitiesList.Select(e => true).ToList();
        }

        /// <summary>
        /// Asynchronously soft deletes an entity from the database by marking it as deleted.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        /// <returns>True if the soft deletion was successful, otherwise false.</returns>
        public async Task<bool> SoftDeleteAsync(TEntity? entity, CancellationToken cancellationToken = default)
        {
            var entry = _context.Entry(entity);

            // Only mark IsDeleted and DeletedAt as modified
            // Other changed properties on the entity are already tracked by EF Core
            // and will appear in audit log
            entry.Property("IsDeleted").CurrentValue = true;
            entry.Property("IsDeleted").IsModified = true;

            entry.Property("DeletedAt").CurrentValue = DateTime.UtcNow;
            entry.Property("DeletedAt").IsModified = true;


            return true;
        }

        /// <summary>
        /// Asynchronously soft deletes a range of entities from the database by marking them as deleted.
        /// </summary>
        /// <param name="entities">The entities to soft delete.</param>
        /// <returns>A list of booleans indicating the success of each soft deletion.</returns>
        public async Task<List<bool>> SoftDeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var entitiesList = entities.ToList();

            foreach (var entity in entitiesList)
            {
                var entry = _context.Entry(entity);

                // Only mark IsDeleted and DeletedAt as modified
                // Other changed properties on the entity are already tracked by EF Core
                // and will appear in audit log
                entry.Property("IsDeleted").CurrentValue = true;
                entry.Property("IsDeleted").IsModified = true;

                entry.Property("DeletedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("DeletedAt").IsModified = true;
            }

            return entitiesList.Select(e => true).ToList();
        }

        /// <summary>
        /// Asynchronously gets a list of entities using a stored procedure.
        /// </summary>
        /// <param name="prosedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters to pass to the stored procedure.</param>
        /// <returns>A list of entities.</returns>
        public async Task<List<TEntity>> GetListWithStoredProsedureAsync(string prosedureName, object[] parameters = null, CancellationToken cancellationToken = default)
        {
            var result = await _context.Set<TEntity>().FromSqlRaw(prosedureName, parameters).ToListAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Asynchronously executes a stored procedure.
        /// </summary>
        /// <param name="prosedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters to pass to the stored procedure.</param>
        /// <returns>True if the execution was successful, otherwise false.</returns>
        public async Task<bool> ExecuteStoredProcedureAsync(string prosedureName, object[] parameters = null, CancellationToken cancellationToken = default)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(prosedureName, parameters, cancellationToken);


            return result > 0;
        }

        /// <summary>
        /// Asynchronously counts the number of entities in the database.
        /// </summary>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <returns>The count of entities.</returns>
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously calculates the average of a numeric property for the entities in the database.
        /// </summary>
        /// <param name="columnName">The name of the column to calculate the average for.</param>
        /// <param name="predicate">An optional predicate to filter the entities.</param>
        /// <returns>The average value.</returns>
        public async Task<decimal> AverageAsync(string columnName, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var propertyInfo = typeof(TEntity).GetProperty(columnName);
            var result = await query.Select(e => propertyInfo.GetValue(e, null)).ToListAsync(cancellationToken);

            return result.Count > 0 ? result.Average(e => Convert.ToDecimal(e)) : 0;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Entity'nin sadece belirtilen property'lerini günceller (Partial Update).
        /// Diğer property'ler veritabanında değiştirilmez. ModifiedAt otomatik ayarlanır.
        /// </summary>
        public async Task<TEntity> UpdateFieldAsync(TEntity entity, Expression<Func<TEntity, object>>[] propertiesToUpdate, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (propertiesToUpdate == null || propertiesToUpdate.Length == 0)
                throw new ArgumentException("At least one property must be specified.");

            // Property adlarını çıkar
            var propertyNames = propertiesToUpdate.Select(GetPropertyNameFromExpression).ToHashSet();
            propertyNames.Add(nameof(IEntity<int>.ModifiedAt));

            var dbEntry = _context.Entry(entity);

            // ModifiedAt'i güncelle
            dbEntry.CurrentValues[nameof(IEntity<int>.ModifiedAt)] = DateTime.UtcNow;

            // State'i Modified yap - bu tüm property'leri modified yapar ve değişiklikleri korur
            dbEntry.State = EntityState.Modified;

            // Güncellenmemesi gereken property'leri IsModified = false yap
            foreach (var property in dbEntry.Properties)
            {
                if (!propertyNames.Contains(property.Metadata.Name))
                {
                    property.IsModified = false;
                }
            }

            return dbEntry.Entity;
        }

        /// <summary>
        /// Birden fazla entity'nin sadece belirtilen property'lerini toplu günceller (Bulk Partial Update).
        /// Diğer property'ler veritabanında değiştirilmez. ModifiedAt otomatik ayarlanır.s
        /// </summary>
        public async Task<List<TEntity>> UpdateFieldRangeAsync(
            IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>>[] propertiesToUpdate,
            CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();

            if (!entitiesList.Any())
                return entitiesList;

            if (propertiesToUpdate == null || propertiesToUpdate.Length == 0)
                throw new ArgumentException("At least one property must be specified.");

            // Property adlarını bir kere çıkar
            var propertyNames = propertiesToUpdate.Select(GetPropertyNameFromExpression).ToHashSet();
            propertyNames.Add(nameof(IEntity<int>.ModifiedAt));

            foreach (var entity in entitiesList)
            {
                var dbEntry = _context.Entry(entity);

                // ModifiedAt'i güncelle
                dbEntry.CurrentValues[nameof(IEntity<int>.ModifiedAt)] = DateTime.UtcNow;

                // State'i Modified yap - bu tüm property'leri modified yapar ve değişiklikleri korur
                dbEntry.State = EntityState.Modified;

                // Güncellenmemesi gereken property'leri IsModified = false yap
                foreach (var property in dbEntry.Properties)
                {
                    if (!propertyNames.Contains(property.Metadata.Name))
                    {
                        property.IsModified = false;
                    }
                }
            }

            return entitiesList;
        }

        /// <summary>
        /// Expression'dan property adını çıkarır. Boxing (Convert) durumlarını da handle eder.
        /// </summary>
        private static string GetPropertyNameFromExpression<T>(Expression<Func<T, object>> expression)
        {
            Expression body = expression.Body;

            // UnaryExpression ise (boxing için Convert kullanılıyor), içindeki operand'ı al
            if (body is UnaryExpression unaryExpression)
            {
                body = unaryExpression.Operand;
            }

            if (body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException($"Expression '{expression}' does not refer to a property.");
        }
        #endregion
    }
}
