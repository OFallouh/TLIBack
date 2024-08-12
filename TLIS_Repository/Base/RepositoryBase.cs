using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using TLIS_DAL.Models;
using System.Reflection;
using static TLIS_Repository.Helpers.Constants;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static Dapper.SqlMapper;
using Castle.Components.DictionaryAdapter;

namespace TLIS_Repository.Base
{
    public class RepositoryBase<TEntity, TModel, TKey> : IRepositoryBase<TEntity, TModel, TKey> where TEntity : class
    {
        ApplicationDbContext _context;
        IMapper _mapper;
        DbSet<TEntity> dataTable
        {
            get
            {
                return _context.Set<TEntity>();
            }
        }



        public RepositoryBase(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }




        public IQueryable<TEntity> GetAllIncludeMultiple(ParameterPagination parameterPagination,
            List<FilterObjectList> filter, out int count, params Expression<Func<TEntity, object>>[] includes)
        {

            IQueryable<TEntity> query = null;//= dataTable.AsQueryable();
            count = 0;
            if (filter == null)
                filter = new List<FilterObjectList>();
            try
            {
                //try {
                //    var s = WhereFiltersAllType(new List<FilterObject> { new FilterObject("Deleted", false) }).FirstOrDefault();
                //    filter.Add(new FilterObjectList("Deleted", new List<object> { false }));
                //        }
                //catch { }

                if (filter != null && filter.Count > 0 && parameterPagination != null)
                {
                    var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                    count = dataTable.Where(expression).Count();
                    query = dataTable.Where(expression).Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                   .Take(parameterPagination.PageSize).AsQueryable();
                }
                else
                {

                    if (filter != null && filter.Count > 0)
                    {
                        var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                        query = dataTable.Where(expression).AsQueryable();
                    }
                    if (query != null)
                        count = query.ToList().Count;
                    else
                        count = dataTable.Count();
                    if (parameterPagination != null)
                    {
                        query = dataTable.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                       .Take(parameterPagination.PageSize).AsQueryable();
                    }
                    if (includes != null)
                    {
                        query = includes.Aggregate(dataTable.AsQueryable(),
                                  (current, include) => current.Include(include)).AsQueryable();
                    }
                }


            }
            catch (Exception)
            {

            }
            if (query == null)
                query = dataTable.AsQueryable();
            return query;
        }

        public IQueryable<TEntity> GetAllIncludeMultipleWithCondition(ParameterPagination parameterPagination, List<FilterObjectList> filter, List<FilterObject> Conditions, out int count, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = null;//= dataTable.AsQueryable();
            if (Conditions == null)
                Conditions = new List<FilterObject>();
            // try
            // {
            //     var s = WhereFiltersAllType(new List<FilterObject> { new FilterObject("Deleted", false) }).FirstOrDefault();
            //     Conditions.Add(new FilterObject("Deleted", false));

            // }
            //catch { }
            if (filter != null)
            {
                FilterObject test = Conditions.FirstOrDefault();
                FilterObjectList y = _mapper.Map<FilterObjectList>(test);
                IList<FilterObjectList> x = _mapper.Map<IList<FilterObjectList>>(Conditions);
                filter.AddRange(_mapper.Map<IEnumerable<FilterObjectList>>(x));
            }
            count = 0;
            try
            {
                if (filter != null && filter.Count > 0 && parameterPagination != null)
                {
                    var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                    count = dataTable.Where(expression).Count();
                    query = dataTable.Where(expression).Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                   .Take(parameterPagination.PageSize).AsQueryable();
                }
                else if (Conditions != null && Conditions.Count > 0 && parameterPagination != null)
                {
                    var expression = ExpressionUtils.BuildPredicate<TEntity>(Conditions);
                    count = dataTable.Where(expression).Count();
                    query = dataTable.Where(expression).Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                   .Take(parameterPagination.PageSize).AsQueryable();
                }
                else
                {

                    if (filter != null && filter.Count > 0)
                    {
                        var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                        query = dataTable.Where(expression).AsQueryable();
                    }
                    if (Conditions != null && Conditions.Count > 0)
                    {
                        var expression = ExpressionUtils.BuildPredicate<TEntity>(Conditions);
                        query = dataTable.Where(expression).AsQueryable();
                    }
                    if (query != null)
                        count = query.ToList().Count;
                    if (parameterPagination != null)
                    {
                        query = dataTable.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                       .Take(parameterPagination.PageSize).AsQueryable();
                    }
                }
                if (includes != null)
                {
                    query = includes.Aggregate(query,
                              (current, include) => current.Include(include)).AsQueryable();
                }
            }
            catch (Exception e)
            {

            }
            if (query == null)
                query = dataTable.AsQueryable();
            return query;
        }

        public int GetCount()
        {
            return dataTable.Count();
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(ParameterPagination parameterPagination = null,
            List<FilterObjectList> filter = null)
        {

            var source = dataTable.AsNoTracking().AsQueryable();
            if (filter != null && parameterPagination != null && filter.Count!=0)
            {
                var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                source = source.Where(expression).Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                               .Take(parameterPagination.PageSize);
            }
            else if (filter != null && filter.Count != 0)
            {
                var expression = ExpressionUtils.BuildPredicate<TEntity>(filter);
                source = source.Where(expression).AsQueryable();
            }
            else if (parameterPagination != null)
            {
                source = source.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                               .Take(parameterPagination.PageSize);
            }
            return await source.ToListAsync();
        }


        public virtual IEnumerable<TEntity> GetAll(out int count)
        {
            //var query = WhereFiltersAllType(new List<FilterObject> { new FilterObject("Deleted", false) });
            var res = dataTable.ToList();
            count = res.Count;
            return res;
        }

        public virtual IEnumerable<TEntity> GetAllWithoutCount()
        {
            //var query = WhereFiltersAllType(new List<FilterObject> { new FilterObject("Deleted", false) });
            var res = dataTable.ToList();
            return res;
        }

        public IQueryable<TEntity> GetAllAsQueryable(out int count)
        {
            count = dataTable.Count();
            return dataTable.AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsQueryable()
        {
            var data = dataTable.AsNoTracking().AsQueryable();

            return data;
        }

        public virtual async Task<TModel> GetAsync(TKey id)
        {

            var res = await dataTable.FindAsync(id);
            return _mapper.Map<TModel>(res);

        }

        public virtual TEntity GetByID(TKey id)
        {
            return dataTable.Find(id);
        }

        public virtual TModel Get(TKey id)
        {

            var res = dataTable.Find(id);
            return _mapper.Map<TModel>(res);

        }
        public async Task<IEnumerable<TModel>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var res = await dataTable.AsNoTracking().Where(predicate).ToListAsync();
            return _mapper.Map<IEnumerable<TModel>>(res);
        }

        public async Task<TModel> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var res = await dataTable.AsNoTracking().SingleOrDefaultAsync(predicate);
            return _mapper.Map<TModel>(res);
        }

        public virtual async Task AddAsync(TModel item)
        {
            var entity = _mapper.Map<TEntity>(item);
            await dataTable.AddAsync(entity);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await dataTable.AddAsync(entity);

        }

        public virtual void Add(TEntity entity)
        {
            dataTable.Add(entity);
        }

        public virtual async Task<TModel> AddModelAsync(TModel item)
        {
            var entity = _mapper.Map<TEntity>(item);
            await dataTable.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TModel>(entity);

        }
        public virtual async Task AddRangeAsync(IEnumerable<TModel> items)
        {
            var entities = _mapper.Map<IEnumerable<TEntity>>(items);
            await dataTable.AddRangeAsync(entities);
        }
        public void AddRange(IEnumerable<TEntity> items)
        {
            dataTable.AddRange(items);
        }
        public void AddRang(IEnumerable<TModel> items)
        {
            var entities = _mapper.Map<IEnumerable<TEntity>>(items);
            dataTable.AddRange(entities);
        }

        public virtual void AddWithHistory(TEntity entity, int? UserId)
        {
            dataTable.Add(entity);
            _context.SaveChanges();

            if (UserId != null)
            {
                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);

                TLItablesNames TableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x => x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id,
                    PreviousHistoryId = null,
                    RecordId = entityId.ToString(),
                    TablesNameId = TableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
            }
        }
        public virtual Task UpdateItem(TModel item)
        {
            var entity = _mapper.Map<TEntity>(item);
            return Task.Run(() => dataTable.Update(entity));
        }

        public virtual Task UpdateItem(TEntity item)
        {
            _context.Entry(item).State = EntityState.Detached;
            //var entity = _mapper.Map<TEntity>(item); 
            return Task.Run(() => dataTable.Update(item));

        }

        public virtual void Update(TEntity item)
        {


            _context.Entry<TEntity>(item).State = EntityState.Detached;
            //var entity = _mapper.Map<TEntity>(item);

            dataTable.Update(item);

        }

        public virtual void UpdateRange(IEnumerable<TEntity> items)
        {
            //var entity = _mapper.Map<TEntity>(item);
            dataTable.UpdateRange(items);

        }

        public virtual Task Disable_Enable(int id, bool active, TEntity item)
        {

            return Task.Run(() => dataTable.Update(item));

        }

        public virtual Task RemoveItem(TModel item)
        {
            var entity = _mapper.Map<TEntity>(item);
            return Task.Run(() => dataTable.Remove(entity));
        }

        public virtual void RemoveItem(TEntity item)
        {
            dataTable.Remove(item);
        }

        public virtual Task RemoveRangeItems(IEnumerable<TModel> items)
        {
            var entities = _mapper.Map<IEnumerable<TEntity>>(items);
            return Task.Run(() => dataTable.RemoveRange(entities));
        }

        public virtual void RemoveRangeItems(IEnumerable<TEntity> items)
        {
            var entities = _mapper.Map<IEnumerable<TEntity>>(items);
            dataTable.RemoveRange(entities);
        }

        public IQueryable<TEntity> WhereFilters(List<FilterExperssionOneValue> Filter)
        {
            var source = dataTable.AsQueryable();
            var expression = ExpressionUtils.BuildPredicate<TEntity>(Filter);
            return source.Where(expression);
        }
        //public IQueryable<TEntity> Where(string propertyName, string comparison, string value)
        //{
        //    var source = dataTable.AsQueryable();
        //    var expression = ExpressionUtils.BuildPredicate<TEntity>(propertyName, comparison, value);
        //    return source.Where(expression);
        //}


        public async Task AddRangeAsync(IEnumerable<TEntity> items)
        {
            await dataTable.AddRangeAsync(items);
        }

        //public IQueryable<TEntity> WhereFiltersAllType(List<FilterObject> Filter)
        //{
        //    var source = dataTable.AsQueryable();
        //    var expression = ExpressionUtils.BuildPredicate<TEntity>(Filter);
        //    return source.Where(expression);
        //}

        public ICollection<TEntity> GetWhereAndInclude(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
        {

            IQueryable<TEntity> query = dataTable.Where(where).AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }
            return query.ToList();

        }
        public ICollection<TType> GetWhereAndSelect<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select) where TType : class
        {
            return dataTable.Where(where).Select(select).ToList();
        }
        public ICollection<TEntity> GetWhere(Expression<Func<TEntity, bool>> where)
        {
            return dataTable.Where(where).ToList();
        }

        public TEntity GetWhereFirst(Expression<Func<TEntity, bool>> where)
        {
            return dataTable.Where(where).FirstOrDefault();
        }
        public TType GetWhereSelectFirst<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select) where TType : class
        {
            return dataTable.Where(where).Select(select).FirstOrDefault();
        }
        public bool Any(Expression<Func<TEntity, bool>> where)
        {
            return dataTable.Any(where);
        }
        public ICollection<TType> GetSelect<TType>(Expression<Func<TEntity, TType>> select) where TType : class
        {
            return dataTable.Select(select).ToList();

        }

        public IEnumerable<TEntity> GetAll(ParameterPagination parameterPagination, Expression<Func<TEntity, bool>> where, out int count, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = null;
            if (where != null && parameterPagination != null)
            {
                count = dataTable.Where(where).Count();
                query = dataTable.Where(where).Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                          .Take(parameterPagination.PageSize).AsQueryable();
            }
            else if (where != null)
            {
                count = dataTable.Where(where).Count();
                query = dataTable.Where(where).AsQueryable();
            }
            else
            {
                count = dataTable.Count();
                query = dataTable.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                           .Take(parameterPagination.PageSize).AsQueryable();
            }

            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }
            return query.ToList();
        }

        public IEnumerable<TType> GetIncludeWhereSelect<TType>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TType>> select, params Expression<Func<TEntity, object>>[] includes) where TType : class
        {
            return includes.Aggregate(dataTable.AsQueryable(),
                          (current, include) => current.Include(include)).Where(where).Select(select).ToList();


        }
        public IEnumerable<TEntity> GetIncludeWhere(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
        {
            var test = includes.Aggregate(dataTable.AsQueryable(),
                          (current, include) => current.Include(include)).Where(where).ToList();
            return test;

        }


        public TEntity GetIncludeWhereFirst(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
        {
            return includes.Aggregate(dataTable.AsQueryable(),
                          (current, include) => current.Include(include)).Where(where).FirstOrDefault();
        }

        #region History Functions..
        // Add One Item
        public virtual void AddWithHistory(int? UserId, TEntity entity)
        {
            //if (UserId == null)
            //    UserId = _context.TLIuser.FirstOrDefault(x => x.UserName == "AdminSy").Id;
            dataTable.Add(entity);
            _context.SaveChanges();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());
                
                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = null,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();
            }
        }


        //-----------------------------------------------------------------------------------
        public virtual void AddWithH(int? UserId, int? SecRecordId, TEntity AddObject)
        {

            dataTable.Add(AddObject);
            _context.SaveChanges();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == AddObject.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                int entityId = (int)AddObject.GetType().GetProperty("Id").GetValue(AddObject, null);
                string entityIdString = entityId.ToString();

                TLIhistory AddTablesHistory = new TLIhistory
                {
                    HistoryTypeId = HistoryTypeId,
                    RecordId = entityIdString,
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };

                _context.TLIhistory.Add(AddTablesHistory);
                _context.SaveChanges();

                List<PropertyInfo> Attributes = AddObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDet> ListOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object NewAttributeValue = Attribute.GetValue(AddObject, null);
                    if (SecRecordId != null)
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = SecRecordId.ToString(),
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }
                    else
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = entityIdString,
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }
                        
                }

                _context.TLIhistoryDet.AddRange(ListOfHistoryDetailsToAdd);
                _context.SaveChanges();
            }
        }
        public virtual async Task AddAsyncWithH(int? UserId, int? SecRecordId, TEntity AddObject)
        {

            await dataTable.AddAsync(AddObject);
            await _context.SaveChangesAsync();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == AddObject.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                int entityId = (int)AddObject.GetType().GetProperty("Id").GetValue(AddObject, null);
                string entityIdString = entityId.ToString();
                TLIhistory AddTablesHistory = new TLIhistory
                {
                    HistoryTypeId = HistoryTypeId,
                    RecordId = entityIdString,
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };

                await _context.TLIhistory.AddAsync(AddTablesHistory);
                await _context.SaveChangesAsync();

                List<PropertyInfo> Attributes = AddObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDet> ListOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object NewAttributeValue = Attribute.GetValue(AddObject, null);
                    if (SecRecordId != null)
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = SecRecordId.ToString(),
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }
                    else
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = entityIdString,
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }

                }

                await _context.TLIhistoryDet.AddRangeAsync(ListOfHistoryDetailsToAdd);
                await _context.SaveChangesAsync();
            }
        }
        public void AddRangeWithHistory(int? UserId, int? SecRecordId, IEnumerable<TEntity> Entities)
        {
            foreach (TEntity Entity in Entities)
            {
                AddWithH(UserId, SecRecordId, Entity);
            }
            _context.SaveChanges();
        }
        public virtual void UpdateWithH(int? UserId, int? SecRecordId, TEntity OldObject, TEntity NewObject)
        {
          
            TEntity entity = _mapper.Map<TEntity>(NewObject);
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Edit.ToString().ToLower()).Id;

                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);
                string entityIdString = entityId.ToString();


                TLItablesHistory PreviousHistory = _context.TLItablesHistory.OrderBy(x => x.Date).LastOrDefault(x => x.RecordId == entityId.ToString() &&
                    x.TablesNameId == EntityTableNameModel.Id);

                int? PreviousHistoryId = null;
                if (PreviousHistory != null)
                    PreviousHistoryId = PreviousHistory.Id;

                TLIhistory AddTablesHistory = new TLIhistory
                {
                    HistoryTypeId = HistoryTypeId,
                    RecordId = entityIdString,
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };

                _context.TLIhistory.Add(AddTablesHistory);
                _context.SaveChanges();

                List<PropertyInfo> Attributes = OldObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDet> ListOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object OldAttributeValue = Attribute.GetValue(OldObject, null);
                    object NewAttributeValue = Attribute.GetValue(NewObject, null);

                    if (((OldAttributeValue != null && NewAttributeValue != null) ? OldAttributeValue.ToString() == NewAttributeValue.ToString() : false) ||
                        (OldAttributeValue == null && NewAttributeValue == null))
                        continue;
                    if (SecRecordId != null)
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = SecRecordId.ToString(),
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            OldValue = OldAttributeValue != null ? OldAttributeValue.ToString() : null,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }
                    else
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = entityIdString,
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            OldValue = OldAttributeValue != null ? OldAttributeValue.ToString() : null,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }

                }
                _context.TLIhistoryDet.AddRange(ListOfHistoryDetailsToAdd);
                _context.SaveChanges();
            }
            OldObject = NewObject;
            _context.Entry<TEntity>(OldObject).State = EntityState.Detached;
            dataTable.Update(OldObject);
            _context.SaveChanges();
        }
        public virtual void RemoveItemWithH(int? UserId, int? SecRecordId, TEntity OldObject)
        {
            dataTable.Remove(OldObject);
            _context.SaveChanges();

            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == OldObject.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Delete.ToString().ToLower()).Id;

                int entityId = (int)OldObject.GetType().GetProperty("Id").GetValue(OldObject, null);
                string entityIdString = entityId.ToString();
                TLIhistory AddTablesHistory = new TLIhistory
                {
                    HistoryTypeId = HistoryTypeId,
                    RecordId = entityIdString,
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };

                _context.TLIhistory.Add(AddTablesHistory);
                _context.SaveChanges();

                List<PropertyInfo> Attributes = OldObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDet> ListOfHistoryDetailsToAdd = new List<TLIhistoryDet>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object NewAttributeValue = Attribute.GetValue(OldObject, null);
                    if (SecRecordId != null)
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = SecRecordId.ToString(),
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }
                    else
                    {
                        TLIhistoryDet HistoryDetails = new TLIhistoryDet
                        {
                            TablesNameId = EntityTableNameModel.Id,
                            RecordId = entityIdString,
                            HistoryId = AddTablesHistory.Id,
                            AttributeName = Attribute.Name,
                            NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                            AttributeType = AttributeType.Static
                        };

                        ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                    }

                }

                _context.TLIhistoryDet.AddRange(ListOfHistoryDetailsToAdd);
                _context.SaveChanges();
            }
        }
        public virtual void RemoveRangeItemsWithHistory(int? UserId, int? SecRecordId, IEnumerable<TEntity> Entities)
        {
            foreach (TEntity Entity in Entities)
            {
                RemoveItemWithH(UserId, SecRecordId, Entity);
            }
            _context.SaveChanges();
        }

        // ---------------------------------------------------------------------------------------




        public virtual void AddSiteWithHistory(int? UserId, TEntity entity)
        {
            //if (UserId == null)
            //    UserId = _context.TLIuser.FirstOrDefault(x => x.UserName == "AdminSy").Id;
            dataTable.Add(entity);
            _context.SaveChanges();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                string entityId = (string)entity.GetType().GetProperty("SiteCode").GetValue(entity, null);

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = null,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();
            }
        }
        public virtual async Task AddAsyncWithHistory(int? UserId, TEntity entity)
        {
            await dataTable.AddAsync(entity);
            await _context.SaveChangesAsync();
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Add.ToString().ToLower()).Id;

                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);

                await _context.TLItablesHistory.AddAsync(new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = null,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                });
                await _context.SaveChangesAsync();
            }
        }

        // Add Range
        public void AddRangeWithHistory(int? UserId, IEnumerable<TEntity> Entities)
        {
            foreach (TEntity Entity in Entities)
            {
                AddWithHistory(UserId, Entity);
            }
            _context.SaveChanges();
        }
        public async Task AddRangeAsyncWithHistory(int? UserId, IEnumerable<TEntity> Entities)
        {
            foreach (TEntity Entity in Entities)
            {
                await AddAsyncWithHistory(UserId, Entity);
            }
            await _context.SaveChangesAsync();
        }
        public Response<string> CheckSpaces(int UserId, string SiteCode, string TableName, int LibraryId, float SpaceInstallation, string Cabinet)
        {
            try
            {
                TLIsite tLIsite = new TLIsite();
                TLIsite Site = new TLIsite();
                TLIsite OldValueSite =new TLIsite();
                TEntity Oldentity;

                OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                tLIsite = OldValueSite;
                Oldentity = _mapper.Map<TEntity>(tLIsite);
                Site = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                if (SpaceInstallation != 0)
                {
                    var space = Site.ReservedSpace + SpaceInstallation;
                    if (space <= Site.RentedSpace)
                    {
                        var NewSite = new TLIsite
                        {
                            SiteCode= Site.SiteCode,
                            SiteName= Site.SiteName,
                            LocationHieght= Site.LocationHieght,
                            LocationType= Site.LocationType,
                            Latitude= Site.Latitude,
                            Longitude= Site.Longitude,
                            siteStatusId= Site.siteStatusId,
                            RentedSpace= Site.RentedSpace,
                            SiteVisiteDate= Site.SiteVisiteDate,
                            Zone= Site.Zone,
                            SubArea = Site.SubArea,
                            RegionCode= Site.RegionCode,
                            STATUS_DATE= Site.STATUS_DATE,
                            CREATE_DATE= Site.CREATE_DATE,
                            ReservedSpace = space,
                            AreaId= Site.AreaId,
                        
                        };

                        TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                        UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else
                {
                    if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                    {
                        var civilWithoutLegSpaceLibrary = _context.TLIcivilWithoutLegLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilWithoutLegSpaceLibrary != null)
                        {
                           
                            OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                            tLIsite = OldValueSite;
                            Oldentity = _mapper.Map<TEntity>(tLIsite);
                            Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                            var space = Site.ReservedSpace + civilWithoutLegSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();  

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var civilWithLegSpaceLibrary = _context.TLIcivilWithLegLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilWithLegSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilWithLegSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var civilNonSteelLibrary = _context.TLIcivilNonSteelLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilNonSteelLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilNonSteelLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Power")
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var cabinetPowerLibrarySpaceLibrary = _context.TLIcabinetPowerLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetPowerLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetPowerLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }

                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else if (cabinetPowerLibrarySpaceLibrary.Depth != 0 && cabinetPowerLibrarySpaceLibrary.Width != 0)
                        {
                            OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                            tLIsite = OldValueSite;
                            Oldentity = _mapper.Map<TEntity>(tLIsite);
                            Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                            var lengh = cabinetPowerLibrarySpaceLibrary.Depth;
                            var Width = cabinetPowerLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = result,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Telecom")
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var cabinetSpaceLibrary = _context.TLIcabinetTelecomLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else if (cabinetSpaceLibrary.Depth != 0 && cabinetSpaceLibrary.Width != 0)
                        {
                            OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                            tLIsite = OldValueSite;
                            Oldentity = _mapper.Map<TEntity>(tLIsite);
                            Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                            var lengh = cabinetSpaceLibrary.Depth;
                            var Width = cabinetSpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = result,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIgenerator.ToString() == TableName)
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var generatorLibrarySpaceLibrary = _context.TLIgeneratorLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (generatorLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + generatorLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges(); 

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }

                        else if (generatorLibrarySpaceLibrary.Length != 0 && generatorLibrarySpaceLibrary.Width != 0)
                        {
                            OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                            tLIsite = OldValueSite;
                            Oldentity = _mapper.Map<TEntity>(tLIsite);
                            Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                            var lengh = generatorLibrarySpaceLibrary.Length;
                            var Width = generatorLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = result,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges(); 

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIsolar.ToString() == TableName)
                    {
                        OldValueSite = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode == SiteCode);
                        tLIsite = OldValueSite;
                        Oldentity = _mapper.Map<TEntity>(tLIsite);
                        Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).AsNoTracking().FirstOrDefault();
                        var solarLibrarySpaceLibrary = _context.TLIsolarLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (solarLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + solarLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                var NewSite = new TLIsite
                                {
                                    SiteCode = Site.SiteCode,
                                    SiteName = Site.SiteName,
                                    LocationHieght = Site.LocationHieght,
                                    LocationType = Site.LocationType,
                                    Latitude = Site.Latitude,
                                    Longitude = Site.Longitude,
                                    siteStatusId = Site.siteStatusId,
                                    RentedSpace = Site.RentedSpace,
                                    SiteVisiteDate = Site.SiteVisiteDate,
                                    Zone = Site.Zone,
                                    SubArea = Site.SubArea,
                                    RegionCode = Site.RegionCode,
                                    STATUS_DATE = Site.STATUS_DATE,
                                    CREATE_DATE = Site.CREATE_DATE,
                                    ReservedSpace = space,
                                    AreaId = Site.AreaId,

                                };

                                TEntity Newentity = _mapper.Map<TEntity>(NewSite);
                                UpdateSiteWithHistory(UserId, Oldentity, Newentity);
                                _context.SaveChanges(); ;

                            }
                            else
                            {
                                return new Response<string>(false, null, null, "Not available space in site", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(false, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }

                return new Response<string>();
            }
            catch (Exception err)
            {
                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        // Update One Item
        public virtual void UpdateWithHistory(int? UserId, TEntity OldObject, TEntity NewObject)
        {
            //if (UserId == null)
            //    UserId = _context.TLIuser.FirstOrDefault(x => x.UserName == "AdminSy").Id;
            TEntity entity = _mapper.Map<TEntity>(NewObject);
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Edit.ToString().ToLower()).Id;
                
                int entityId = (int)entity.GetType().GetProperty("Id").GetValue(entity, null);

                TLItablesHistory PreviousHistory = _context.TLItablesHistory.OrderBy(x => x.Date).LastOrDefault(x => x.RecordId == entityId.ToString() &&
                    x.TablesNameId == EntityTableNameModel.Id);

                int? PreviousHistoryId = null;
                if (PreviousHistory != null)
                    PreviousHistoryId = PreviousHistory.Id;

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = PreviousHistoryId,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();

                List<PropertyInfo> Attributes = OldObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ? 
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? 
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDetails> ListOfHistoryDetailsToAdd = new List<TLIhistoryDetails>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object OldAttributeValue = Attribute.GetValue(OldObject, null);
                    object NewAttributeValue = Attribute.GetValue(NewObject, null);

                    if (((OldAttributeValue != null && NewAttributeValue != null) ? OldAttributeValue.ToString() == NewAttributeValue.ToString() : false) ||
                        (OldAttributeValue == null && NewAttributeValue == null))
                        continue;

                    TLIhistoryDetails HistoryDetails = new TLIhistoryDetails
                    {
                        TablesHistoryId = AddTablesHistory.Id,
                        AttName = Attribute.Name,
                        OldValue = OldAttributeValue != null ? OldAttributeValue.ToString() : null,
                        NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                        AttributeType = AttributeType.Static
                    };
                    ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                }
                _context.TLIhistoryDetails.AddRange(ListOfHistoryDetailsToAdd);
                _context.SaveChanges();
            }
            OldObject = NewObject;
            _context.Entry<TEntity>(OldObject).State = EntityState.Detached;
            dataTable.Update(OldObject);
            _context.SaveChanges();
        }

        public virtual void UpdateSiteWithHistory(int? UserId, TEntity OldObject, TEntity NewObject)
        {
            //if (UserId == null)
            //    UserId = _context.TLIuser.FirstOrDefault(x => x.UserName == "AdminSy").Id;
            TEntity entity = _mapper.Map<TEntity>(NewObject);
            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Edit.ToString().ToLower()).Id;

                string entityId = (string)entity.GetType().GetProperty("SiteCode").GetValue(entity, null);

                TLItablesHistory PreviousHistory = _context.TLItablesHistory.OrderBy(x => x.Date).LastOrDefault(x => x.RecordId == entityId &&
                    x.TablesNameId == EntityTableNameModel.Id);

                int? PreviousHistoryId = null;
                if (PreviousHistory != null)
                    PreviousHistoryId = PreviousHistory.Id;

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = PreviousHistoryId,
                    RecordId = entityId,
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();

                List<PropertyInfo> Attributes = OldObject.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType ?
                    (x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                        (x.PropertyType.GetGenericArguments()[0] == typeof(int) || x.PropertyType.GetGenericArguments()[0] == typeof(string) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(double) || x.PropertyType.GetGenericArguments()[0] == typeof(float) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(Single) || x.PropertyType.GetGenericArguments()[0] == typeof(bool) ||
                         x.PropertyType.GetGenericArguments()[0] == typeof(DateTime)) : false) :
                    (x.PropertyType == typeof(int) || x.PropertyType == typeof(double) || x.PropertyType == typeof(string) ||
                     x.PropertyType == typeof(bool) || x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(float) ||
                     x.PropertyType == typeof(Single))).ToList();

                List<TLIhistoryDetails> ListOfHistoryDetailsToAdd = new List<TLIhistoryDetails>();

                foreach (PropertyInfo Attribute in Attributes)
                {
                    object OldAttributeValue = Attribute.GetValue(OldObject, null);
                    object NewAttributeValue = Attribute.GetValue(NewObject, null);

                    if (((OldAttributeValue != null && NewAttributeValue != null) ? OldAttributeValue.ToString() == NewAttributeValue.ToString() : false) ||
                        (OldAttributeValue == null && NewAttributeValue == null))
                        continue;

                    TLIhistoryDetails HistoryDetails = new TLIhistoryDetails
                    {
                        TablesHistoryId = AddTablesHistory.Id,
                        AttName = Attribute.Name,
                        OldValue = OldAttributeValue != null ? OldAttributeValue.ToString() : null,
                        NewValue = NewAttributeValue != null ? NewAttributeValue.ToString() : null,
                        AttributeType = AttributeType.Static
                    };
                    ListOfHistoryDetailsToAdd.Add(HistoryDetails);
                }
                _context.TLIhistoryDetails.AddRange(ListOfHistoryDetailsToAdd);
                _context.SaveChanges();
            }
            OldObject = NewObject;
            _context.Entry<TEntity>(OldObject).State = EntityState.Detached;
            dataTable.Update(OldObject);
            _context.SaveChanges();
        }
      
        public virtual void RemoveItemWithHistory(int? UserId, TEntity Entity)
        {
            dataTable.Remove(Entity);
            _context.SaveChanges();

            if (UserId != null)
            {
                TLItablesNames EntityTableNameModel = _context.TLItablesNames.FirstOrDefault(x => x.TableName.ToLower() == Entity.GetType().Name.ToLower());

                int HistoryTypeId = _context.TLIhistoryType.FirstOrDefault(x =>
                    x.Name.ToLower() == Helpers.Constants.TLIhistoryType.Delete.ToString().ToLower()).Id;

                int entityId = (int)Entity.GetType().GetProperty("Id").GetValue(Entity, null);

                TLItablesHistory PreviousHistory = _context.TLItablesHistory.OrderBy(x => x.Id).LastOrDefault(x => x.RecordId == entityId.ToString() &&
                    x.TablesNameId == EntityTableNameModel.Id);

                int? PreviousHistoryId = null;
                if (PreviousHistory != null)
                    PreviousHistoryId = PreviousHistory.Id;

                TLItablesHistory AddTablesHistory = new TLItablesHistory
                {
                    Date = DateTime.Now,
                    HistoryTypeId = HistoryTypeId,
                    PreviousHistoryId = PreviousHistoryId,
                    RecordId = entityId.ToString(),
                    TablesNameId = EntityTableNameModel.Id,
                    UserId = UserId.Value
                };
                _context.TLItablesHistory.Add(AddTablesHistory);
                _context.SaveChanges();

            }
        }
        
        public virtual void RemoveRangeItemsWithHistory(int? UserId, IEnumerable<TEntity> Entities)
        {
            foreach (TEntity Entity in Entities)
            {
                RemoveItemWithHistory(UserId, Entity);
            }
            _context.SaveChanges();
        }
        public void RefreshView(string connectionString)
        {
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    string storedProcedureName = "refresh_all_mviews";
                    using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    {
                        procedureCommand.CommandType = CommandType.StoredProcedure;
                        procedureCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during refreshing view: {ex.Message}");
            }
        }
        #endregion
    }
}