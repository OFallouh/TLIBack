using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;


namespace TLIS_Repository.Base
{
    public interface IRepositoryBase<TEntity, TModel, TKey>
    {
        IQueryable<TEntity> GetAllIncludeMultipleWithCondition(ParameterPagination parameterPagination, List<FilterObjectList> filter, List<FilterObject> Conditions, out int count, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> GetAllIncludeMultiple(ParameterPagination parameterPagination, List<FilterObjectList> filter, out int count, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(ParameterPagination parameterPagination = null, List<FilterObjectList> filter = null);
        IEnumerable<TEntity> GetAll(out int count);
        void UpdateWithHLogic(int? UserId, int HistoryId, int TabelNameId, TEntity OldObject, TEntity NewObject);
        Task<int> AddAsyncWithH(int? UserId, int? SecRecordId, TEntity AddObject);
        void AddRangeWithH(int? UserId, int? SecRecordId, IEnumerable<TEntity> Entities);
        int UpdateWithH(int? UserId, int? SecRecordId, TEntity OldObject, TEntity NewObject);
        IEnumerable<TEntity> GetAllWithoutCount();
        void AddRangeWithHDynamic(int? UserId, int HistoryId, int TabelNameId, IEnumerable<TEntity> Entities);
        void RemoveItemWithHistory(int? UserId, TEntity Entity);
        void RemoveItemWithH(int? UserId, int? SecRecordId, TEntity OldObject);
        void RemoveRangeItemsWithHistory(int? UserId, int? SecRecordId, IEnumerable<TEntity> Entities);
        int GetCount();
        int AddWithH(int? UserId, int? SecRecordId, TEntity AddObject);
        TEntity GetByID(TKey id);
        void AddWithHDynamic(int? UserId, int TabelNameId, TEntity AddObject, int HistoryId);
        IQueryable<TEntity> GetAllAsQueryable(out int count);
        IQueryable<TEntity> GetAllAsQueryable();
        TModel Get(TKey id);
        Task<TModel> GetAsync(TKey id);
        Task<IEnumerable<TModel>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TModel> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TModel> AddModelAsync(TModel item);
        Task AddAsync(TModel item);
        Task AddAsync(TEntity entity);
        void Add(TEntity entity);
        void UpdateSiteWithHistory(int? UserId, TEntity OldObject, TEntity NewObject);
        Task AddRangeAsync(IEnumerable<TModel> items);
        Task AddRangeAsync(IEnumerable<TEntity> items);
        void AddRange(IEnumerable<TEntity> items);
        void AddRang(IEnumerable<TModel> items);
        Task UpdateItem(TModel item);
        void UpdateWithHistory(int? UserId, TEntity OldObject, TEntity NewObject);
        Task AddAsyncWithHistory(int? UserId, TEntity entity);
        void AddWithHistory(int? UserId, TEntity entity);
        Task UpdateItem(TEntity item);
        void Update(TEntity item);
        void UpdateRange(IEnumerable<TEntity> items);
        Task Disable_Enable(int id, bool active, TEntity item);
        Task RemoveItem(TModel item);
        void RemoveItem(TEntity entity);
        Task RemoveRangeItems(IEnumerable<TModel> items);
        void RemoveRangeItems(IEnumerable<TEntity> items);
        void AddRangeWithHistory(int? UserId, IEnumerable<TEntity> Entities);
        Task AddRangeAsyncWithHistory(int? UserId, IEnumerable<TEntity> Entities);
        void RemoveRangeItemsWithHistory(int? UserId, IEnumerable<TEntity> Entities);
        //  IQueryable<TEntity> WhereFilters(List<FilterExperssion> Filter);

        IQueryable<TEntity> WhereFilters(List<FilterExperssionOneValue> Filter);
        //  IQueryable<TEntity> Where(string propertyName, string comparison, string value);
        ICollection<TType> GetWhereAndSelect<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select) where TType : class;
        ICollection<TEntity> GetWhere(Expression<Func<TEntity, bool>> where);
        TEntity GetWhereFirst(Expression<Func<TEntity, bool>> where);
        ICollection<TType> GetSelect<TType>(Expression<Func<TEntity, TType>> select) where TType : class;
        ICollection<TEntity> GetWhereAndInclude(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
        bool Any(Expression<Func<TEntity, bool>> where);
        TType GetWhereSelectFirst<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select) where TType : class;
        IEnumerable<TType> GetIncludeWhereSelect<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select, params Expression<Func<TEntity, object>>[] includes) where TType : class;
        IEnumerable<TEntity> GetIncludeWhere(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
        TEntity GetIncludeWhereFirst(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);
        void RefreshView(string connectionString);
        void AddSiteWithHistory(int? UserId, TEntity entity);
        int AddWithHInsatallation(int? UserId, int? SecRecordId, TEntity AddObject, string? SiteCode);
        Task<int> AddAsyncWithHInstallation(int? UserId, int? SecRecordId, TEntity AddObject, string SiteCode);
        void AddRangeWithHInstallation(int? UserId, int? SecRecordId, IEnumerable<TEntity> Entities, string SiteCode);
        int UpdateWithHInstallation(int? UserId, int? SecRecordId, TEntity OldObject, TEntity NewObject, string SiteCode);
    }
}
