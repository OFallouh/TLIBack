using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.StaticAttributesHistory;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;



namespace TLIS_Repository.Repositories
{
    public class TablesHistoryRepository: RepositoryBase<TLItablesHistory, TablesHistoryViewModel, int>, ITablesHistoryRepository 
    {
        private readonly ApplicationDbContext _context;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;
        IMapper _mapper;
        public TablesHistoryRepository(ApplicationDbContext context, IMapper mapper) :base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
           
        }

        public void AddTableHistory(int RecordId, int TableNameId, int UserId, int HistoryTypeId, List<Tuple<string, string, string>> values = null)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLItablesHistory tableHistory = new TLItablesHistory();
                    //int UserId = http.Headers.UserAgent.
                    //var TablesHistory = _mapper.Map<TLItablesHistory>(addTablesHistory);
                    //var test = _httpContextAccessor.HttpContext.Request.Headers["cookie"].ToString();
                    //TablesHistory.UserId = Int32.Parse(test);
                    //var message = _session.GetString("Test");
                    tableHistory.RecordId = RecordId.ToString();
                    tableHistory.TablesNameId = TableNameId;
                    tableHistory.UserId = UserId;
                    tableHistory.HistoryTypeId = HistoryTypeId;
                    tableHistory.Date = DateTime.Now;
                    tableHistory.PreviousHistoryId = _context.TLItablesHistory.Where(x => (x.RecordId == RecordId.ToString() && x.TablesNameId == TableNameId)).OrderByDescending(t => t.Date).Select(x => x.Id).FirstOrDefault();
                    _context.TLItablesHistory.Add(tableHistory);
                    _context.SaveChanges();
                    if(HistoryTypeId == 2)
                    {
                        foreach(var value in values)
                        {
                            TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                            historyDetails.TablesHistoryId = tableHistory.Id;
                            historyDetails.AttName = value.Item1;
                            historyDetails.OldValue = value.Item2;
                            historyDetails.NewValue = value.Item3;
                            _context.TLIhistoryDetails.Add(historyDetails);
                            _context.SaveChanges();
                        }
                    }
                    transaction.Complete();
                }
                catch(Exception)
                {
                    
                }
            }    
        }

        public void AddTableHistory(AddTablesHistoryViewModel addModel, List<Tuple<string, string, string>> values = null)
        {
            //using (TransactionScope transaction = new TransactionScope())
           // {
                try
                {
                    TLItablesHistory tableHistory = new TLItablesHistory();
                    //int UserId = http.Headers.UserAgent.
                    //var TablesHistory = _mapper.Map<TLItablesHistory>(addTablesHistory);
                    //var test = _httpContextAccessor.HttpContext.Request.Headers["cookie"].ToString();
                    //TablesHistory.UserId = Int32.Parse(test);
                    //var message = _session.GetString("Test");
                    TLItablesHistory entity = _mapper.Map<TLItablesHistory>(addModel); 
                    entity.Date = DateTime.Now;
                    _context.TLItablesHistory.Add(entity);
                    _context.SaveChanges();
                   // tableHistory.Date = DateTime.Now;
                   // _context.TLItablesHistory.Add(tableHistory);
                    //_context.SaveChanges();
                   // if (entity.HistoryTypeId == 1)
                  //  {
                    if (values !=null )
                    {
                        foreach (var value in values)
                        {
                            TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                            historyDetails.TablesHistoryId = tableHistory.Id;
                            historyDetails.AttName = value.Item1;
                            historyDetails.OldValue = value.Item2;
                            historyDetails.NewValue = value.Item3;
                            _context.TLIhistoryDetails.Add(historyDetails);
                            _context.SaveChanges();
                        }
                    }
                       
                  //  }
                 //   transaction.Complete();
                }
                catch (Exception e)
                {

                }
           // }
        }

        public int AddTableHistory(AddTablesHistoryViewModel addModel, List<TLIhistoryDetails> details)
        {
            //using (TransactionScope transaction = new TransactionScope())
            // {
            int resultId = 0;
            try
            {
                
                TLItablesHistory entity = _mapper.Map<TLItablesHistory>(addModel);
                entity.Date = DateTime.Now;
                _context.TLItablesHistory.Add(entity);
               
                _context.SaveChanges();
                
                // var x = entity.Id;


               if (details!=null )
                    foreach (var value in details)
                    {
                        //TLIhistoryDetails newRec = new TLIhistoryDetails();
                        //newRec.OldValue = value.OldValue;
                        // newRec.NewValue = value.NewValue;
                        // newRec.AttName = value.AttName;
                        //newRec.TablesHistoryId = entity.Id;
                        value.AttributeType = AttributeType.Static;
                        value.TablesHistoryId = entity.Id;
                        _context.TLIhistoryDetails.Add(value);
                        _context.SaveChanges();
                    }

                //   transaction.Complete();
                resultId = entity.Id;
            }
            catch (Exception e)
            {
                
            }
            return resultId;
        }
       //public List<StaticAttsHistoryViewModel> GetDynamicAttributesHistory(string TableName, ParameterPagination parameterPagination)
       // {
            
       //     List<StaticAttsHistoryViewModel> List = (from tablesHistory in _context.TLItablesHistory
       //              join dynamicAtt in _context.TLIdynamicAtt on tablesHistory.RecordId equals dynamicAtt.Id 
                                                     
       //              join Details in _context.TLIhistoryDetails on tablesHistory.Id equals Details.TablesHistoryId into x
       //                  from Details in x. DefaultIfEmpty()
       //                  where dynamicAtt.tablesNames.TableName == TableName && tablesHistory.TablesName.TableName == TableName
                                                     
       //              select new StaticAttsHistoryViewModel
       //              {
       //                  Key = dynamicAtt.Key,
       //                  UpdatedInfo = Details != null ? Details.AttName : null,
       //                  User = tablesHistory.User.UserName,
       //                  Date = tablesHistory.Date,
       //                  Operation = tablesHistory.HistoryType.Name,
       //                  OldValue = Details != null ? Details.OldValue : null,
       //                  NewValue = Details != null ? Details.NewValue : null,
       //              }
                     
       //             ).ToList().Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
       //                        .Take(parameterPagination.PageSize).ToList();


       //     return List;
       // }
        //public List<StaticAttsHistoryViewModel> GetStaticAttributesHistory(string TableName, ParameterPagination parameterPagination)
        //{
        //     var list = (from Details in _context.TLIhistoryDetails
        //             join History in _context.TLItablesHistory on Details.TablesHistoryId equals History.Id
        //             join AttaActivated in _context.TLIattributeActivated on History.RecordId equals AttaActivated.Id
        //             where AttaActivated.Tabel == TableName //&& Details.AttributeType ==AttributeType.Static  //History.TablesName.TableName == "TLIattributeActivated" 
        //                 select new StaticAttsHistoryViewModel
        //             {
        //                 Key = AttaActivated.Key ,
        //                 UpdatedInfo = Details.AttName,
        //                 User = History.User.UserName,
        //                 Date = History.Date,
        //                 Operation = History.HistoryType.Name,
        //                 OldValue = Details.OldValue != null ? Details.OldValue : null,
        //                 NewValue = Details.NewValue != null? Details.NewValue : null,
        //             }
        //             ).ToList().Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
        //                       .Take(parameterPagination.PageSize);
        //    return list.ToList();
        //}
        //public List<HistoryViewModel> GetAttachedFileHistory( string TableName,int RecordId, ParameterPagination parameterPagination)//, List<FilterObjectList> filter)
        //{
        //    List<HistoryViewModel> List = new List<HistoryViewModel>();
        //  //  var expression = ExpressionUtils.BuildPredicate<object>(filter);
        //   // var test = _context.TLItablesHistory.Where(expression).ToList();
        //    List = (from tablesHistory in _context.TLItablesHistory
        //                join attachedFiles in _context.TLIattachedFiles on tablesHistory.RecordId equals attachedFiles.Id
        //                where (attachedFiles.tablesName.TableName == TableName && attachedFiles.RecordId == RecordId)
        //           // where(expression)
        //    select new HistoryViewModel
        //    {
        //       Key = attachedFiles.Name,
        //       User = tablesHistory.User.UserName,
        //       Date = tablesHistory.Date,
        //       Operation = tablesHistory.HistoryType.Name,
        //    }

        //            ).ToList().Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
        //                       .Take(parameterPagination.PageSize).ToList();
            
           
        //    return List;
        //}
        public EditHistoryDetails CheckUpdateObject(object originalObj, object updateObj)
        {
            EditHistoryDetails result = new EditHistoryDetails();
            result.original = originalObj;
            result.Details = new List<TLIhistoryDetails>();
            foreach (var property in updateObj.GetType().GetProperties())
            {

                var x = property.GetValue(updateObj);
                var y = property.GetValue(originalObj);

                if (x != null || y != null)
                {
                    if (x != null)
                    {
                        if (!x.Equals(y))

                        {
                            property.SetValue(result.original, x);
                            TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                            // historyDetails.AttType = "static";
                            historyDetails.AttName = property.Name;
                            if (y != null)
                            {
                                historyDetails.OldValue = y.ToString();
                            }
                            if (x != null)
                            {
                                historyDetails.NewValue = x.ToString();
                            }

                            result.Details.Add(historyDetails);
                            // _unitOfWork.HistoryDetailsRepository.Add(historyDetails);
                            // _unitOfWork.SaveChanges();
                            //property.SetValue(originalObj, updateObj.GetType().GetProperty(property.Name)
                            //.GetValue(originalObj, null));
                        }


                    }
                    else
                    {
                        property.SetValue(result.original, x);
                        TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                        // historyDetails.AttType = "static";
                        historyDetails.AttName = property.Name;
                        if (y != null)
                        {
                            historyDetails.OldValue = y.ToString();
                        }
                        if (x != null)
                        {
                            historyDetails.NewValue = x.ToString();
                        }
                        result.Details.Add(historyDetails);
                    }

                }
            }
            return result;
        }
        public int AddHistoryForEdit(int RecordId, int TableNameid, string HistoryType, List<TLIhistoryDetails> details)
        {
            var SyUser = _context.TLIuser.FirstOrDefault(x=>x.UserName== "AdminSy");
            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = RecordId;
            history.TablesNameId = TableNameid;//_unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.Id == TableNameid).Id;
            history.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == HistoryType).Select(x => x.Id).FirstOrDefault();
            history.UserId = SyUser.Id;
            int? TableHistoryId = null;
            var CheckTableHistory = _context.TLItablesHistory.Any(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId.ToString() && x.TablesNameId == TableNameid);
            if (CheckTableHistory)
            {
                var TableHistory = _context.TLItablesHistory.Where(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId.ToString() && x.TablesNameId == TableNameid).Select(s => s.Id).ToList().Max();//, x => new { x.Id }).ToList().Max(x => x.Id);
                if (TableHistory != null)
                    TableHistoryId = TableHistory;
                if (TableHistoryId != null)
                {
                    history.PreviousHistoryId = TableHistoryId;
                }
            }

            int HistoryId = AddTableHistory(history, details);
            _context.SaveChangesAsync();
            return HistoryId;
        }
        #region Add History
        public void AddHistory(int MW_lib_id, string historyType, string TableName)
        {

            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = MW_lib_id;
            history.TablesNameId = _context.TLItablesNames.Where(x => x.TableName == TableName).Select (x => x.Id).FirstOrDefault();
            history.HistoryTypeId = _context.TLIhistoryType.Where(x => x.Name == historyType).Select(x => x.Id).FirstOrDefault();
            history.UserId = 261;
           AddTableHistory(history);

        }
        #endregion
    }
}
