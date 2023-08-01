using Microsoft.EntityFrameworkCore;
using KUSYS.Model.Base;

namespace KUSYS.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private DbSet<T> _objectSet;
        private bool _disposed;
        protected readonly KUSYSDbContext context;

        protected virtual DbSet<T> Entities => _objectSet ??= context.Set<T>();

        public Repository(KUSYSDbContext context)
        {
            this.context = context;
            _objectSet = this.context.Set<T>();
        }

        public IQueryable<T> ListQueryable => Entities;

        public IQueryable<T> ListQueryableNoTracking => Entities.AsNoTracking();

        public async Task<DbOperationResult> Delete(T entity)
        {
            try
            {
                Entities.Remove(entity);
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veri Silindi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<DbOperationResult> Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                return new DbOperationResult(false, "Liste boþ gönderilemez");

            try
            {
                Entities.RemoveRange(entities);
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veriler Silindi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<T> GetById(object id)
        {
            return await _objectSet.FindAsync(id);
        }

        public async Task<DbOperationResult> Insert(T entity)
        {
            if (entity == null)
                return new DbOperationResult(false, "Boþ veri kaydedilemez");

            try
            {
                Entities.Add(entity);
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veri Eklendi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<DbOperationResult> Insert(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return new DbOperationResult(false, "Boþ veri kaydedilemez");

            try
            {
                Entities.AddRange(entities);
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veriler Eklendi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<DbOperationResult> Update(T entity)
        {
            if (entity == null)
                return new DbOperationResult(false, "Boþ veri güncellenemez");

            try
            {
                Entities.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veri Güncellendi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }
        }

        public async Task<DbOperationResult> Update(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                return new DbOperationResult(false, "Boþ veri güncellenemez");

            try
            {
                Entities.UpdateRange(entities);
                await context.SaveChangesAsync();
                return new DbOperationResult(true, "Veriler Güncellendi");
            }
            catch (Exception ex)
            {
                return new DbOperationResult(false, ex.Message);
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Detach(T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }
    }
}
