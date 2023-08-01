using System;
using System.Collections.Generic;
using System.Linq;
using KUSYS.Model.Base;

namespace KUSYS.Data.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> ListQueryable { get; }
        IQueryable<T> ListQueryableNoTracking { get; }
        Task<T> GetById(object id);
        Task<DbOperationResult> Insert(T entity);
        Task<DbOperationResult> Insert(IEnumerable<T> entities);
        Task<DbOperationResult> Update(T entity);
        Task<DbOperationResult> Update(IEnumerable<T> entities);
        Task<DbOperationResult> Delete(T entity);
        Task<DbOperationResult> Delete(IEnumerable<T> entities);
        void Detach(T entity);
    }
}