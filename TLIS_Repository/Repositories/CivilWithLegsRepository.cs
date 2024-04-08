using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using static TLIS_Repository.Helpers.Constants;

namespace TLIS_Repository.Repositories
{
    public class CivilWithLegsRepository : RepositoryBase<TLIcivilWithLegs,CivilWithLegsViewModel,int> , ICivilWithLegsRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public CivilWithLegsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Response<float> Checkspaceload(int allcivilinstId, string TableName,float SpaceInstallation, float CenterHigh, int libraryId,float HBA)
        {
            try
            {
                float EquivalentSpace = 0;
                var allCivilInst = _context.TLIallCivilInst.Where(x => x.Id == allcivilinstId).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).ToList();
                foreach (var item in allCivilInst)
                {
                    if (item.civilWithLegsId != null)
                    {
                        TLIcivilWithLegs TLIcivilWithLegs = item.civilWithLegs;
                        if (LoadSubType.TLImwBU.ToString() == TableName)
                        {
                            var mwBULibrary = _context.TLImwBULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLImwRFU.ToString() == TableName)
                        {
                            var mwRFULibrary = _context.TLImwRFULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLImwODU.ToString() == TableName)
                        {
                            var mwODULibrary = _context.TLImwODULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLImwDish.ToString() == TableName)
                        {
                            var mwDishLibrary = _context.TLImwDishLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation + (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLImwOther.ToString() == TableName)
                        {
                            var mwOtherLibrary = _context.TLImwOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLIradioAntenna.ToString() == TableName)
                        {
                            var radioAntennaLibrary = _context.TLIradioAntennaLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation + (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLIradioRRU.ToString() == TableName)
                        {
                            var radioRRULibrary = _context.TLIradioRRULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLIradioOther.ToString() == TableName)
                        {
                            var radioOtherLibrary = _context.TLIradioOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLIpower.ToString() == TableName)
                        {
                            var powerLibrary = _context.TLIpowerLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        else if (LoadSubType.TLIloadOther.ToString() == TableName)
                        {
                            var loadOtherLibrary = _context.TLIloadOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithLegs.HeightImplemented);
                            TLIcivilWithLegs.CurrentLoads = TLIcivilWithLegs.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithLegs.Update(TLIcivilWithLegs);
                        }
                        _context.SaveChanges();
                    }
                    else if (item.civilWithoutLegId != null)
                    {
                        TLIcivilWithoutLeg TLIcivilWithoutLeg = item.civilWithoutLeg;

                        if (LoadSubType.TLImwBU.ToString() == TableName)
                        {
                            var mwBULibrary = _context.TLImwBULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh /(float)  TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLImwRFU.ToString() == TableName)
                        {
                            var mwRFULibrary = _context.TLImwRFULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLImwODU.ToString() == TableName)
                        {
                            var mwODULibrary = _context.TLImwODULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLImwDish.ToString() == TableName)
                        {
                            var mwDishLibrary = _context.TLImwDishLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLImwOther.ToString() == TableName)
                        {
                            var mwOtherLibrary = _context.TLImwOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLIradioAntenna.ToString() == TableName)
                        {
                            var radioAntennaLibrary = _context.TLIradioAntennaLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLIradioRRU.ToString() == TableName)
                        {
                            var radioRRULibrary = _context.TLIradioRRULibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLIradioOther.ToString() == TableName)
                        {
                            var radioOtherLibrary = _context.TLIradioOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLIpower.ToString() == TableName)
                        {
                            var powerLibrary = _context.TLIpowerLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh / (float)TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        else if (LoadSubType.TLIloadOther.ToString() == TableName)
                        {
                            var loadOtherLibrary = _context.TLIloadOtherLibrary.Where(x => x.Id == libraryId).FirstOrDefault();
                            EquivalentSpace = SpaceInstallation * (CenterHigh /(float) TLIcivilWithoutLeg.HeightImplemented);
                            TLIcivilWithoutLeg.CurrentLoads = TLIcivilWithoutLeg.CurrentLoads + EquivalentSpace;
                            _context.TLIcivilWithoutLeg.Update(TLIcivilWithoutLeg);
                        }
                        _context.SaveChanges();
                    }
                }
                    return new Response<float>(true, EquivalentSpace, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<float>(true, 0, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public bool BuildDynamicQuery(List<FilterObjectList> filters, IDictionary<string, object> item)
        {
            bool x = true;
            if (filters != null && filters.Count > 0)
            {
                foreach (var filter in filters)
                {
                    object value;
                    if (item.TryGetValue(filter.key, out value))
                    {
                        if (value != null)
                        {
                            bool isDate = DateTime.TryParseExact(value.ToString(), "dd-MMM-yy hh.mm.ss.fffffff tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Dateres);
                            bool isInt = int.TryParse(value.ToString(), out int Intres);
                            if (filter.value.Count > 1)
                            {
                                if (!filter.value.Any(x => x.ToString().ToLower() == value.ToString().ToLower()))
                                {
                                    x = false;
                                    break;
                                }
                            }
                            else if (filter.value.Count == 1)
                            {
                                bool isIntF = int.TryParse(filter.value[0].ToString(), out int FIntres);
                                if ((isInt || isIntF) && Intres != FIntres)
                                {
                                    x = false;
                                    break;
                                }
                                bool isDateF = DateTime.TryParseExact(filter.value[0].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime FDateres);
                                if ((isDate || isDateF)
                                    && Dateres != FDateres)
                                {
                                    x = false;
                                    break;
                                }
                                else if (!isDate && !isInt && !value.ToString().ToLower().StartsWith(filter.value[0].ToString().ToLower()))
                                {
                                    x = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            x = false;
                            break;
                        }
                    }
                }
            }
            return x;
        }
        public IDictionary<string, object> BuildDynamicSelect(object obj, Dictionary<string, string>? dynamic, List<string> propertyNamesStatic, List<string> propertyNamesDynamic)
        {
            Dictionary<string, object> item = new Dictionary<string, object>();
            Type type = obj.GetType();
            foreach (var propertyName in propertyNamesStatic)
            {
                string name = propertyName;
                var property = type.GetProperty(name);

                // Check if the property exists
                if (property == null)
                {
                    name = name.ToUpper();
                    property = type.GetProperty(name);
                    if (property == null)
                    {
                        throw new ArgumentException($"Property {name} not found in {nameof(obj)}");
                    }
                }
                PropertyInfo propertyInfo = type.GetProperty(name);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(obj);
                 
                        item.Add(name, propertyInfo.GetValue(obj));
                    
                }
            }
            foreach (var propertyName in propertyNamesDynamic)
            {

                var datatype = _context.CIVIL_WITHLEG_LIBRARY_VIEW.FirstOrDefault(x => x.Key == propertyName);
                if (datatype != null)
                {
                    if (datatype.dataType.ToString().ToLower() == "bool")
                    {
                        var types = dynamic.GetValueOrDefault(propertyName);
                        if (types == "1")
                        {
                            item.Add(propertyName, true);
                        }
                        if (types == "0")
                        {
                            item.Add(propertyName, false);
                        }
                    }
                    else
                    {
                        item.Add(propertyName, dynamic.GetValueOrDefault(propertyName));
                    }
                }
               
            }
            return item;
        }
        public Response<float> CheckloadsOnCivil(int allcivilinstId,int ? loadid ,float Azimuth, float CenterHigh)
        {
            try
            {
                var LoadInst = _context.TLIcivilLoads.Where(x => x.allCivilInstId == allcivilinstId && x.allLoadInstId != null).Include(x => x.allLoadInst).Select(x => x.allLoadInst).ToList();

                foreach (var item in LoadInst)
                {
                    if (item.mwBUId != null)
                    {
                        var mwBU = _context.TLImwBU.Where(x => x.Azimuth == Azimuth && x.CenterHigh == CenterHigh && x.Id!=loadid)
                            .Select(x => new {x.Azimuth, x.CenterHigh }).ToList();

                        if (mwBU.Count() > 0)
                        {
                            return new Response<float>(true, 0, null, $"No  space on the civil at the same height and azimuth as the two entrances ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (item.mwDishId != null)
                    {
                        var h = _context.TLImwDish.Where(x => x.Azimuth == Azimuth && x.CenterHigh == CenterHigh && x.Id!=loadid)
                            .Select(x => new { x.Azimuth, x.CenterHigh }).ToList();

                        if (h.Count() > 0)
                        {
                            return new Response<float>(true, 0, null, $"No space on the civil at the same height and azimuth as the two entrances ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }

                    else if (item.radioAntennaId != null)
                    {
                        var h = _context.TLIradioAntenna.Where(x => x.Azimuth == Azimuth && x.CenterHigh == CenterHigh && x.Id != loadid)
                             .Select(x => new { x.Azimuth, x.CenterHigh }).ToList();

                        if (h.Count() > 0)
                        {
                            return new Response<float>(true, 0, null, $"No space on the Civil at the same height and azimuth as the two entrances ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (item.radioRRUId != null)
                    {
                        var h = _context.TLIRadioRRU.Where(x => x.Azimuth == Azimuth && x.CenterHigh == CenterHigh && x.Id != loadid)
                             .Select(x => new { x.Azimuth, x.CenterHigh }).ToList();

                        if (h.Count() > 0)
                        {
                            return new Response<float>(true, 0, null, $"No space on the civil at the same height and azimuth as the two entrances ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }

                }
                return new Response<float>(true, 0, null, "Success", (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<float>(true, 0, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<float> CheckAvailableSpaceOnCivil(int allcivilinstId)
        {
            try
            {
                double Availablespace = 0;
                var allCivilInst = _context.TLIallCivilInst.Where(x => x.Id == allcivilinstId).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).ToList();
                foreach (var item in allCivilInst)
                {
                    if (item.civilWithLegsId != null)
                    {
                        TLIcivilWithLegs TLIcivilWithLegs = item.civilWithLegs;
                        var civilWithLegs = _context.TLIcivilWithLegs.Where(x => x.Id == item.civilWithLegsId).Select(x => x.CivilWithLegsLib).FirstOrDefault();
                        if (TLIcivilWithLegs.CurrentLoads == null)
                        {
                            TLIcivilWithLegs.CurrentLoads = 0;
                        }
                        if (TLIcivilWithLegs.IsEnforeced == true)
                        {
                            Availablespace = TLIcivilWithLegs.SupportMaxLoadAfterInforcement - TLIcivilWithLegs.CurrentLoads;
                            if (Availablespace == 0 || Availablespace < 0)
                            {
                                return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }

                        else if (TLIcivilWithLegs.Support_Limited_Load != 0)
                        {
                            Availablespace = TLIcivilWithLegs.Support_Limited_Load - TLIcivilWithLegs.CurrentLoads;
                            if (Availablespace == 0 || Availablespace < 0)
                            {
                                return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            Availablespace = civilWithLegs.Manufactured_Max_Load - TLIcivilWithLegs.CurrentLoads;
                            if (Availablespace == 0 || Availablespace < 0)
                            {
                                return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                    }
                    else if (item.civilWithoutLegId != null)
                    {
                        TLIcivilWithoutLeg TLIcivilWithoutLeg = item.civilWithoutLeg;
                        var civilWithoutLeg = _context.TLIcivilWithoutLeg.Where(x => x.Id == item.civilWithoutLegId).Select(x => x.CivilWithoutlegsLib).FirstOrDefault();
                        if (TLIcivilWithoutLeg.CurrentLoads == null)
                        {
                            TLIcivilWithoutLeg.CurrentLoads = 0;
                        }
                        if (TLIcivilWithoutLeg.Support_Limited_Load != 0)
                        {
                            Availablespace = TLIcivilWithoutLeg.Support_Limited_Load - TLIcivilWithoutLeg.CurrentLoads;
                            if (Availablespace == 0 || Availablespace < 0)
                            {
                                return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            Availablespace = civilWithoutLeg.Manufactured_Max_Load - TLIcivilWithoutLeg.CurrentLoads;
                            if (Availablespace == 0 || Availablespace < 0)
                            {
                                return new Response<float>(true, 0, null, $"No available space on the civill ", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }


                    }
                }
                return new Response<float>(true, 0, null, "Success", (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<float>(true, 0, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }


        }
        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
           
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();

            //List<TLIowner> Owners = _context.TLIowner.Where(x => !x.Deleted && !x.Disable).ToList();
            List<TLIowner> Owners = _context.TLIowner.ToList();

            List<DropDownListFilters> OwnerLists = _mapper.Map<List<DropDownListFilters>>(Owners);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("OwnerId", OwnerLists));

            //  List<TLIguyLineType> GuyLineTypes = _context.TLIguyLineType.Where(x => !x.Deleted && !x.Disable).ToList();
              List<TLIguyLineType> GuyLineTypes = _context.TLIguyLineType.ToList();

            List<DropDownListFilters> GuyLineTypeLists = _mapper.Map<List<DropDownListFilters>>(GuyLineTypes);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("GuylineTypeId", GuyLineTypeLists));

            //  List<TLIsupportTypeImplemented> SupportTypesImplemented = _context.TLIsupportTypeImplemented.Where(x => !x.Deleted && !x.Disable).ToList();
             List<TLIsupportTypeImplemented> SupportTypesImplemented = _context.TLIsupportTypeImplemented.ToList();

            List<DropDownListFilters> SupportTypesImplementedLists = _mapper.Map<List<DropDownListFilters>>(SupportTypesImplemented);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("SupportTypeImplementedId", SupportTypesImplementedLists));

            // List<TLIbaseCivilWithLegsType> BaseCivilWithLegsTypes = _context.TLIbaseCivilWithLegsType.Where(x => !x.Deleted && !x.Disable).ToList();
             List<TLIbaseCivilWithLegsType> BaseCivilWithLegsTypes = _context.TLIbaseCivilWithLegsType.ToList();

            List<DropDownListFilters> BaseCivilWithLegsTypeLists = _mapper.Map<List<DropDownListFilters>>(BaseCivilWithLegsTypes);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("BaseCivilWithLegTypeId", BaseCivilWithLegsTypeLists));

            //List<TLIcivilWithLegLibrary> CivilWithLegsLibraries = _context.TLIcivilWithLegLibrary.Where(x => !x.Deleted && x.Active).ToList();
            List<TLIcivilWithLegLibrary> CivilWithLegsLibraries = _context.TLIcivilWithLegLibrary.ToList();

            List<DropDownListFilters> CivilWithLegsLibraryLists = _mapper.Map<List<DropDownListFilters>>(CivilWithLegsLibraries);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("CivilWithLegsLibId", CivilWithLegsLibraryLists));

            // List<TLIenforcmentCategory> enforcmentCategory = _context.TLIenforcmentCategory.Where(x => !x.Deleted && !x.Disable).ToList();
             List<TLIenforcmentCategory> enforcmentCategory = _context.TLIenforcmentCategory.ToList();

            List<DropDownListFilters> enforcmentCategoryLists = _mapper.Map<List<DropDownListFilters>>(enforcmentCategory);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("enforcmentCategoryId", enforcmentCategoryLists));

            // Ahmad's Add
            // List<TLIlocationType> locationType = _context.TLIlocationType.Where(x => !x.Deleted && !x.Disable).ToList();
             List<TLIlocationType> locationType = _context.TLIlocationType.ToList();

            List<DropDownListFilters> locationTypeLists = _mapper.Map<List<DropDownListFilters>>(locationType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("locationTypeId", locationTypeLists));

            // List<TLIbaseType> baseType = _context.TLIbaseType.Where(x => !x.Deleted && !x.Disable).ToList();
             List<TLIbaseType> baseType = _context.TLIbaseType.ToList();

            List<DropDownListFilters> baseTypeLists = _mapper.Map<List<DropDownListFilters>>(baseType);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("baseTypeId", baseTypeLists));


            List<DicMod> StructureMods = new List<DicMod>();
            Dictionary<int, string> StructurTypeDic = new Dictionary<int, string>() {

                {0,StructureTypeCompatibleWithDesign.Yes.ToString() },
                {1,StructureTypeCompatibleWithDesign.No.ToString() }

            } ;

            foreach(var x in StructurTypeDic)
            {
                DicMod dicMod = new DicMod();
                dicMod.Id = x.Key;
                dicMod.Value = x.Value;
                dicMod.Deleted = false;
                dicMod.Disable = false;
                StructureMods.Add(dicMod);
            }

            List<DropDownListFilters> StructureLists = _mapper.Map<List<DropDownListFilters>>(StructureMods);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("StructureType", StructureLists));

            List<DicMod> SectionLegMods = new List<DicMod>();
            Dictionary<int, string> SectionLegDic = new Dictionary<int, string>() {

                {0,SectionsLegTypeCompatibleWithDesign.Yes.ToString() },
                {1,SectionsLegTypeCompatibleWithDesign.No.ToString() }

            };

            foreach (var x in SectionLegDic)
            {
                DicMod dicMod = new DicMod();
                dicMod.Id = x.Key;
                dicMod.Value = x.Value;
                dicMod.Deleted = false;
                dicMod.Disable = false;
                SectionLegMods.Add(dicMod);
            }

            List<DropDownListFilters> SectionTypeLists = _mapper.Map<List<DropDownListFilters>>(SectionLegMods);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("SectionsLegType", SectionTypeLists));
            return RelatedTables;
        }
    }
}
