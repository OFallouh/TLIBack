using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class LogisticalRepository : RepositoryBase<TLIlogistical, LogisticalViewModel, int>, ILogistcalRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public LogisticalRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<BaseAttView> GetLogistical(string Part, string TableName, int RecordId)
        {
            List<BaseAttView> result = new List<BaseAttView>();

            int TableNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == TableName).Id;
            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogistaclTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogistaclTypes)
            {
                TLIlogisticalitem LogisticalItem = _context.TLIlogisticalitem.Include(x => x.logistical)
                    .FirstOrDefault(x => x.tablesNamesId == TableNameId && x.RecordId == RecordId &&
                        x.logistical.tablePartNameId == TablePartNameId && x.logistical.logisticalTypeId == LogisticalType.Id);

                if (LogisticalItem != null)
                {
                    result.Add(new BaseAttView
                    {
                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        enable = true,
                        DataType = "List",
                        AutoFill = false,
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Required = false,
                        Value = LogisticalItem.logistical.Name
                    });
                }
                else
                {
                    result.Add(new BaseAttView
                    {
                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        enable = true,
                        DataType = "List",
                        AutoFill = false,
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Required = false,
                        Value = "NA"
                    });
                }
            }

            return result;
        }
        public IEnumerable<BaseInstAttViews> GetLogisticals(string Part, string TableName, int RecordId)
        {
            List<BaseInstAttViews> result = new List<BaseInstAttViews>();

            int TableNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == TableName).Id;
            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogistaclTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogistaclTypes)
            {
                TLIlogisticalitem LogisticalItem = _context.TLIlogisticalitem.Include(x => x.logistical).ThenInclude(x=>x.logisticalType)
                    .FirstOrDefault(x => x.tablesNamesId == TableNameId && x.RecordId == RecordId &&
                        x.logistical.tablePartNameId == TablePartNameId && x.logistical.logisticalTypeId == LogisticalType.Id);
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                   .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id &&
                       x.Active && !x.Deleted).ToList());


                if (LogisticalItem != null)
                {
                    if (LogisticalItem.logistical.logisticalType.Name.ToLower() == "vendor")
                    {
                        result.Add(new BaseInstAttViews
                        {
                            Key = LogisticalType.Name,
                            Label = LogisticalType.Name,
                            enable = true,
                            DataType = "List",
                            AutoFill = false,
                            Desc = LogisticalType.Name,
                            Manage = false,
                            Required = true,
                            Value = _mapper.Map<LogisticalViewModel>(LogisticalItem.logistical),
                            Options = Logisticals
                        });

                    }
                    else
                    {
                        result.Add(new BaseInstAttViews
                        {
                            Key = LogisticalType.Name,
                            Label = LogisticalType.Name,
                            enable = true,
                            DataType = "List",
                            AutoFill = false,
                            Desc = LogisticalType.Name,
                            Manage = false,
                            Required = false,
                            Value = _mapper.Map<LogisticalViewModel>(LogisticalItem.logistical),
                            Options = Logisticals
                        });
                    }
                }
                else
                {
                    if (LogisticalType.Name.ToLower() == "vendor")
                    {
                        result.Add(new BaseInstAttViews
                        {
                            Key = LogisticalType.Name,
                            Label = LogisticalType.Name,
                            enable = true,
                            DataType = "List",
                            AutoFill = false,
                            Desc = LogisticalType.Name,
                            Manage = false,
                            Required = true,
                            Value = null,
                            Options = Logisticals
                        });

                    }
                    else
                    {

                        result.Add(new BaseInstAttViews
                        {
                            Key = LogisticalType.Name,
                            Label = LogisticalType.Name,
                            enable = true,
                            DataType = "List",
                            AutoFill = false,
                            Desc = LogisticalType.Name,
                            Manage = false,
                            Required = false,
                            Value = null,
                            Options = Logisticals

                        });
                    }
                }
            }

            return result;
        }
        public IEnumerable<BaseInstAttViews> GetLogisticalsNonSteel(string Part, string TableName, int RecordId)
        {
            List<BaseInstAttViews> result = new List<BaseInstAttViews>();

            int TableNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == TableName).Id;
            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogistaclTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogistaclTypes)
            {
                TLIlogisticalitem LogisticalItem = _context.TLIlogisticalitem.Include(x => x.logistical).
                    ThenInclude(x => x.logisticalType)
                    .FirstOrDefault(x => x.tablesNamesId == TableNameId && x.RecordId == RecordId &&
                        x.logistical.tablePartNameId == TablePartNameId && x.logistical.logisticalTypeId == LogisticalType.Id);
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                   .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id &&
                       x.Active && !x.Deleted).ToList());


                if (LogisticalItem != null)
                {

                    result.Add(new BaseInstAttViews
                    {
                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        enable = true,
                        DataType = "List",
                        AutoFill = false,
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Required = false,
                        Value = _mapper.Map<LogisticalViewModel>(LogisticalItem.logistical),
                        Options = Logisticals
                    });
                    
                }
                else
                {

                    result.Add(new BaseInstAttViews
                    {
                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        enable = true,
                        DataType = "List",
                        AutoFill = false,
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Required = false,
                        Value = null,
                        Options = Logisticals

                    });
                }
            }

            return result;
        }
        public IEnumerable<BaseAttView> GetLogistical(string Part)
        {
            List<BaseAttView> result = new List<BaseAttView>();

            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogisticalTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogisticalTypes)
            {
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                    .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id && 
                        x.Active && !x.Deleted).ToList());

                BaseAttView BaseAtt = new BaseAttView
                {
                    Key = LogisticalType.Name,
                    Label = LogisticalType.Name,
                    Required = false,
                    enable = true,
                    AutoFill = false,
                    DataType = "List",
                    Desc = LogisticalType.Name,
                    Manage = false,
                    Value = Logisticals
                };
                if (!result.Exists(x => x.Key == BaseAtt.Key))
                    result.Add(BaseAtt);
            }
            
            return result;
        }
        public IEnumerable<BaseInstAttViews> GetLogisticals(string Part)
        {
            List<BaseInstAttViews> result = new List<BaseInstAttViews>();

            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogisticalTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogisticalTypes)
            {
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                    .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id &&
                        x.Active && !x.Deleted).ToList());

                BaseInstAttViews BaseAtt = new BaseInstAttViews
                {
                    Key = LogisticalType.Name,
                    Label = LogisticalType.Name,
                    Required = false,
                    enable = true,
                    AutoFill = false,
                    DataType = "List",
                    Desc = LogisticalType.Name,
                    Manage = false,
                    Options = Logisticals
                };
                if (!result.Exists(x => x.Key == BaseAtt.Key))
                    result.Add(BaseAtt);
            }

            return result;
        }
        public IEnumerable<BaseInstAttViews> GetLogisticalLibrary(string Part)
        {
            List<BaseInstAttViews> result = new List<BaseInstAttViews>();

            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogisticalTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogisticalTypes)
            {
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                    .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id &&
                        x.Active && !x.Deleted).ToList());
                if (LogisticalType.Name.ToLower() == "vendor")
                {
                    BaseInstAttViews BaseAtt = new BaseInstAttViews
                    {

                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        Required = true,
                        enable = true,
                        AutoFill = false,
                        DataType = "List",
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Value = null,
                        Options = Logisticals
                    };
                    if (!result.Exists(x => x.Key == BaseAtt.Key))
                        result.Add(BaseAtt);
                }
                else
                {

                    BaseInstAttViews BaseAtt = new BaseInstAttViews
                    {

                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        Required = false,
                        enable = true,
                        AutoFill = false,
                        DataType = "List",
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Value = null,
                        Options = Logisticals
                    };
                    if (!result.Exists(x => x.Key == BaseAtt.Key))
                        result.Add(BaseAtt);
                }
            }

            return result;
        }
        public IEnumerable<BaseInstAttViews> GetLogisticalLibraryNonSteel(string Part)
        {
            List<BaseInstAttViews> result = new List<BaseInstAttViews>();

            int TablePartNameId = _context.TLItablePartName.FirstOrDefault(x => x.PartName.ToLower() == Part.ToLower()).Id;

            List<TLIlogisticalType> LogisticalTypes = _context.TLIlogisticalType.Where(x => !x.Deleted && !x.Disable).ToList();

            foreach (TLIlogisticalType LogisticalType in LogisticalTypes)
            {
                List<LogisticalViewModel> Logisticals = _mapper.Map<List<LogisticalViewModel>>(_context.TLIlogistical
                    .Where(x => x.tablePartNameId == TablePartNameId && x.logisticalTypeId == LogisticalType.Id &&
                        x.Active && !x.Deleted).ToList());
                

                    BaseInstAttViews BaseAtt = new BaseInstAttViews
                    {

                        Key = LogisticalType.Name,
                        Label = LogisticalType.Name,
                        Required = false,
                        enable = true,
                        AutoFill = false,
                        DataType = "List",
                        Desc = LogisticalType.Name,
                        Manage = false,
                        Value = null,
                        Options = Logisticals
                    };
                    if (!result.Exists(x => x.Key == BaseAtt.Key))
                       result.Add(BaseAtt);
                
            }

            return result;
        }
    }
}
