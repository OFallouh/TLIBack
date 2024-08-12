using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_DAL.ViewModels.BaseTypeDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.EnforcmentCategoryDTOs;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
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
        public Response<List<RecalculatSpace>> RecalculatSpace(int CivilId, string CivilType)
        {
            try
            {
                List<RecalculatSpace> recalculatSpaces = new List<RecalculatSpace>();
                if (CivilType == "TLIcivilWithLegs")
                {
                    float EquivalentSpace = 0;
                    float CenterHigh = 0;
                    var AllCivilInst = _context.TLIallCivilInst.Include(
                        x => x.civilWithLegs).Include(x => x.civilLoads).Include(x => x.civilWithoutLeg).Include(x => x.civilNonSteel)
                        .FirstOrDefault(x => x.civilWithLegsId == CivilId && x.Draft == false);
                    if (AllCivilInst != null)
                    {
                        List<TLIcivilLoads> AllLoadOnCivil = _context.TLIcivilLoads.Where(x => x.allCivilInstId == AllCivilInst.Id && x.ReservedSpace == true &&
                        x.Dismantle == false && x.allLoadInstId != null).Include( x => x.allLoadInst).Include(x => x.allLoadInst.mwBU)
                        .Include(x => x.allLoadInst.mwRFU).Include(x => x.allLoadInst.mwODU).Include(x => x.allLoadInst.mwOther)
                        .Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.radioAntenna).Include(x => x.allLoadInst.radioRRU)
                        .Include(x => x.allLoadInst.radioOther).Include(x => x.allLoadInst.power).Include(x => x.allLoadInst.loadOther).ToList();
                        AllCivilInst.civilWithLegs.CurrentLoads = 0;
                        if (AllLoadOnCivil.Count == 0)
                        {
                            AllCivilInst.civilWithLegs.CurrentLoads = 0;
                            _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                            _context.SaveChanges();
                        }
                        foreach (var item in AllLoadOnCivil)
                        {
                            if (item.allLoadInst != null)
                            {
                                if (item.allLoadInst.mwBUId != null)
                                {
                                    var LibraryInfo = _context.TLImwBULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwBU.MwBULibraryId);
                                    if (LibraryInfo != null)
                                    {
                                        if (item.allLoadInst.mwBU.Azimuth == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Azimuth",
                                                LoadType = "MWBU",
                                                LoadName = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwBU.Height == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Height",
                                                LoadType = "MWBU",
                                                LoadName = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwBU.SpaceInstallation == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "SpaceInstallation",
                                                LoadType = "MWBU",
                                                LoadModel = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);

                                        }
                                        if (item.allLoadInst.mwBU.CenterHigh == 0)
                                        {

                                            if (item.allLoadInst.mwBU.HBA == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "HBA",
                                                    LoadType = "MWBU",
                                                    LoadName = item.allLoadInst.mwBU.Name,
                                                    Type = "Installation",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            if (LibraryInfo.Length == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "Length",
                                                    LoadType = "MWBU",
                                                    LoadModel = LibraryInfo.Model,
                                                    Type = "Library",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            CenterHigh = item.allLoadInst.mwBU.HBA + LibraryInfo.Length / 2;
                                        }
                                        else
                                        {
                                            CenterHigh = item.allLoadInst.mwBU.CenterHigh;
                                        }
                                    }
                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwBU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.mwDish != null)
                                {
                                    var LibraryInfo = _context.TLImwDishLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwDish.MwDishLibraryId);
                                    if (item.allLoadInst.mwDish.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "MWBU",
                                            LoadName = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwDish.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "MWBU",
                                            LoadName = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwDish.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.mwDish.HBA_Surface == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA_Surface",
                                                LoadType = "MWDish",
                                                LoadModel = item.allLoadInst.mwDish.DishName,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "MWDish",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.mwDish.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.mwDish.CenterHigh;
                                    }
                                    if (item.allLoadInst.mwDish.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "MWDish",
                                            LoadModel = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwDish.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.mwOtherId != null)
                                {
                                    var LibraryInfo = _context.TLImwOtherLibrary.Where(x => x.Id == item.allLoadInst.mwOther.mwOtherLibraryId).FirstOrDefault();
                                    if (item.allLoadInst.mwOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "MWOther",
                                            LoadName = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "MWOther",
                                            LoadName = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwOther.Spaceinstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Spaceinstallation",
                                            LoadType = "MWOther",
                                            LoadModel = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.mwOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.mwOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "MWOther",
                                                LoadName = item.allLoadInst.mwOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "MWOther",
                                                LoadModel = item.allLoadInst.mwOther.Name,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.mwOther.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.mwOther.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwOther.Spaceinstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.mwODU != null)
                                {
                                    if (item.allLoadInst.mwODU.OduInstallationTypeId == 2)
                                    {
                                        var LibraryInfo = _context.TLImwODULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwODU.MwODULibraryId);
                                        if (item.allLoadInst.mwODU.Azimuth == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Azimuth",
                                                LoadType = "MWODU",
                                                LoadName = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwODU.Height == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Height",
                                                LoadType = "MWODU",
                                                LoadName = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwODU.SpaceInstallation == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "SpaceInstallation",
                                                LoadType = "MWODU",
                                                LoadModel = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);

                                        }
                                        if (item.allLoadInst.mwODU.CenterHigh == 0)
                                        {
                                            if (item.allLoadInst.mwODU.HBA == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "HBA",
                                                    LoadType = "MWODU",
                                                    LoadName = item.allLoadInst.mwODU.Name,
                                                    Type = "Installation",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            if (LibraryInfo.Height == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "Length",
                                                    LoadType = "MWODU",
                                                    LoadModel = item.allLoadInst.mwODU.Name,
                                                    Type = "Library",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            CenterHigh = item.allLoadInst.mwODU.HBA + LibraryInfo.Height / 2;
                                        }
                                        else
                                        {
                                            CenterHigh = item.allLoadInst.mwODU.CenterHigh;
                                        }

                                        if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HeightBase",
                                                LoadType = "civilWithLegs",
                                                LoadName = AllCivilInst.civilWithLegs.Name,
                                                Type = "Installation",

                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        EquivalentSpace = item.allLoadInst.mwODU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                        AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                        _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                    }
                                }
                                else if (item.allLoadInst.radioAntennaId != null)
                                {
                                    var LibraryInfo = _context.TLIradioAntennaLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioAntenna.radioAntennaLibraryId);
                                    if (item.allLoadInst.radioAntenna.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "RadioAntenna",
                                            LoadName = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioAntenna.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "RadioAntenna",
                                            LoadName = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioAntenna.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "RadioAntenna",
                                            LoadModel = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioAntenna.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioAntenna.HBASurface == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBASurface",
                                                LoadType = "RadioAntenna",
                                                LoadModel = item.allLoadInst.radioAntenna.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioAntenna",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioAntenna.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioAntenna.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioAntenna.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.radioRRUId != null)
                                {
                                    var LibraryInfo = _context.TLIradioRRULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioRRU.radioRRULibraryId);
                                    if (item.allLoadInst.radioRRU.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "radioRRU",
                                            LoadName = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioRRU.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "radioRRU",
                                            LoadName = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioRRU.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "radioRRU",
                                            LoadModel = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioRRU.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioRRU.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadName = item.allLoadInst.radioRRU.Name,
                                                LoadType = "c",
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioRRU",
                                                LoadModel = LibraryInfo.Model,
                                                ReservedSpaceInCivil = item.ReservedSpace,
                                                Type = "Library"
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioRRU.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioRRU.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioRRU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.radioOtherId != null)
                                {
                                    var LibraryInfo = _context.TLIradioOtherLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioOther.radioOtherLibraryId);
                                    if (item.allLoadInst.radioOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "RadioOther",
                                            LoadName = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "RadioOther",
                                            LoadName = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioOther.Spaceinstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Spaceinstallation",
                                            LoadType = "RadioOther",
                                            LoadModel = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "RadioOther",
                                                LoadName = item.allLoadInst.radioOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioOther",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioOther.CenterHigh;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioOther.CenterHigh;
                                    }
                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioOther.Spaceinstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.powerId != null)
                                {
                                    var LibraryInfo = _context.TLIpowerLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.power.powerLibraryId);
                                    if (item.allLoadInst.power.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "power",
                                            LoadName = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.power.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "power",
                                            LoadName = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.power.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "power",
                                            LoadModel = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.power.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.power.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "power",
                                                LoadName = item.allLoadInst.power.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "power",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.power.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.power.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.power.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                else if (item.allLoadInst.loadOtherId != null)
                                {
                                    var LibraryInfo = _context.TLIloadOtherLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.loadOther.loadOtherLibraryId);
                                    if (item.allLoadInst.loadOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "loadOther",
                                            LoadName = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "loadOther",
                                            LoadName = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "loadOther",
                                            LoadModel = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.loadOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "loadOther",
                                                LoadName = item.allLoadInst.loadOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "loadOther",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.loadOther.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.loadOther.CenterHigh;
                                    }
                                    if (AllCivilInst.civilWithLegs.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithLegs",
                                            LoadName = AllCivilInst.civilWithLegs.Name,
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.loadOther.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithLegs.HeightBase);
                                    AllCivilInst.civilWithLegs.CurrentLoads = AllCivilInst.civilWithLegs.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                                }
                                if (recalculatSpaces == null)
                                {
                                    _context.SaveChanges();
                                    return new Response<List<RecalculatSpace>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                                }
                            }
                        }
                    }
                }
                else if (CivilType == "TLIcivilWithoutLeg")
                {
                    float EquivalentSpace = 0;
                    float CenterHigh = 0;
                    var AllCivilInst = _context.TLIallCivilInst.Include(x => x.civilWithLegs)
                        .Include(x => x.civilWithoutLeg).Include(x => x.civilNonSteel)
                    .FirstOrDefault(x => x.civilWithoutLegId == CivilId && x.Draft == false);
                    if (AllCivilInst != null)
                    {
                        List<TLIcivilLoads> AllLoadOnCivil = _context.TLIcivilLoads.Where(x => x.allCivilInstId == AllCivilInst.Id && x.ReservedSpace == true &&
                          x.Dismantle == false && x.allLoadInstId != null).Include(x => x.allLoadInst).Include(x => x.allLoadInst.mwBU)
                          .Include(x => x.allLoadInst.mwRFU).Include(x => x.allLoadInst.mwODU).Include(x => x.allLoadInst.mwOther)
                          .Include(x => x.allLoadInst.mwDish).Include(x => x.allLoadInst.radioAntenna).Include(x => x.allLoadInst.radioRRU)
                          .Include(x => x.allLoadInst.radioOther).Include(x => x.allLoadInst.power).Include(x => x.allLoadInst.loadOther).ToList();
                        AllCivilInst.civilWithLegs.CurrentLoads = 0;
                        if (AllLoadOnCivil.Count == 0)
                        {
                            AllCivilInst.civilWithLegs.CurrentLoads = 0;
                            _context.TLIcivilWithLegs.Update(AllCivilInst.civilWithLegs);
                            _context.SaveChanges();
                        }
                        foreach (var item in AllLoadOnCivil)
                        {
                            if (item.allLoadInst != null)
                            {
                                if (item.allLoadInst.mwBUId != null)
                                {
                                    var LibraryInfo = _context.TLImwBULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwBU.MwBULibraryId);
                                    if (LibraryInfo != null)
                                    {
                                        if (item.allLoadInst.mwBU.Azimuth == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Azimuth",
                                                LoadType = "MWBU",
                                                LoadName = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwBU.Height == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Height",
                                                LoadType = "MWBU",
                                                LoadName = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwBU.SpaceInstallation == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "SpaceInstallation",
                                                LoadType = "MWBU",
                                                LoadModel = item.allLoadInst.mwBU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);

                                        }
                                        if (item.allLoadInst.mwBU.CenterHigh == 0)
                                        {

                                            if (item.allLoadInst.mwBU.HBA == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "HBA",
                                                    LoadType = "MWBU",
                                                    LoadName = item.allLoadInst.mwBU.Name,
                                                    Type = "Installation",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            if (LibraryInfo.Length == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "Length",
                                                    LoadType = "MWBU",
                                                    LoadModel = LibraryInfo.Model,
                                                    Type = "Library",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            CenterHigh = item.allLoadInst.mwBU.HBA + LibraryInfo.Length / 2;
                                        }
                                        else
                                        {
                                            CenterHigh = item.allLoadInst.mwBU.CenterHigh;
                                        }
                                    }
                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwBU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.mwDish != null)
                                {
                                    var LibraryInfo = _context.TLImwDishLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwDish.MwDishLibraryId);
                                    if (item.allLoadInst.mwDish.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "MWBU",
                                            LoadName = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwDish.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "MWBU",
                                            LoadName = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwDish.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.mwDish.HBA_Surface == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA_Surface",
                                                LoadType = "MWDish",
                                                LoadModel = item.allLoadInst.mwDish.DishName,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "MWDish",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.mwDish.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.mwDish.CenterHigh;
                                    }
                                    if (item.allLoadInst.mwDish.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "MWDish",
                                            LoadModel = item.allLoadInst.mwDish.DishName,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwDish.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.mwOtherId != null)
                                {
                                    var LibraryInfo = _context.TLImwOtherLibrary.Where(x => x.Id == item.allLoadInst.mwOther.mwOtherLibraryId).FirstOrDefault();
                                    if (item.allLoadInst.mwOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "MWOther",
                                            LoadName = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "MWOther",
                                            LoadName = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.mwOther.Spaceinstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Spaceinstallation",
                                            LoadType = "MWOther",
                                            LoadModel = item.allLoadInst.mwOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.mwOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.mwOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "MWOther",
                                                LoadName = item.allLoadInst.mwOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "MWOther",
                                                LoadModel = item.allLoadInst.mwOther.Name,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.mwOther.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.mwOther.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.mwOther.Spaceinstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.mwODU != null)
                                {
                                    if (item.allLoadInst.mwODU.OduInstallationTypeId == 2)
                                    {
                                        var LibraryInfo = _context.TLImwODULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.mwODU.MwODULibraryId);
                                        if (item.allLoadInst.mwODU.Azimuth == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Azimuth",
                                                LoadType = "MWODU",
                                                LoadName = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwODU.Height == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Height",
                                                LoadType = "MWODU",
                                                LoadName = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (item.allLoadInst.mwODU.SpaceInstallation == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "SpaceInstallation",
                                                LoadType = "MWODU",
                                                LoadModel = item.allLoadInst.mwODU.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace

                                            };
                                            recalculatSpaces.Add(recalculat);

                                        }
                                        if (item.allLoadInst.mwODU.CenterHigh == 0)
                                        {
                                            if (item.allLoadInst.mwODU.HBA == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "HBA",
                                                    LoadType = "MWODU",
                                                    LoadName = item.allLoadInst.mwODU.Name,
                                                    Type = "Installation",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            if (LibraryInfo.Height == 0)
                                            {
                                                RecalculatSpace recalculat = new RecalculatSpace()
                                                {
                                                    AttributeName = "Length",
                                                    LoadType = "MWODU",
                                                    LoadModel = item.allLoadInst.mwODU.Name,
                                                    Type = "Library",
                                                    ReservedSpaceInCivil = item.ReservedSpace
                                                };
                                                recalculatSpaces.Add(recalculat);
                                            }
                                            CenterHigh = item.allLoadInst.mwODU.HBA + LibraryInfo.Height / 2;
                                        }
                                        else
                                        {
                                            CenterHigh = item.allLoadInst.mwODU.CenterHigh;
                                        }

                                        if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HeightBase",
                                                LoadType = "civilWithoutLeg",
                                                LoadName = AllCivilInst.civilWithoutLeg.Name,
                                                Type = "Installation",

                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        EquivalentSpace = item.allLoadInst.mwODU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                        AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                        _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                    }
                                }
                                else if (item.allLoadInst.radioAntennaId != null)
                                {
                                    var LibraryInfo = _context.TLIradioAntennaLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioAntenna.radioAntennaLibraryId);
                                    if (item.allLoadInst.radioAntenna.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "RadioAntenna",
                                            LoadName = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioAntenna.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "RadioAntenna",
                                            LoadName = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioAntenna.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "RadioAntenna",
                                            LoadModel = item.allLoadInst.radioAntenna.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioAntenna.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioAntenna.HBASurface == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBASurface",
                                                LoadType = "RadioAntenna",
                                                LoadModel = item.allLoadInst.radioAntenna.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioAntenna",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioAntenna.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioAntenna.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioAntenna.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.radioRRUId != null)
                                {
                                    var LibraryInfo = _context.TLIradioRRULibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioRRU.radioRRULibraryId);
                                    if (item.allLoadInst.radioRRU.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "radioRRU",
                                            LoadName = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioRRU.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "radioRRU",
                                            LoadName = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioRRU.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "radioRRU",
                                            LoadModel = item.allLoadInst.radioRRU.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioRRU.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioRRU.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadName = item.allLoadInst.radioRRU.Name,
                                                LoadType = "c",
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioRRU",
                                                LoadModel = LibraryInfo.Model,
                                                ReservedSpaceInCivil = item.ReservedSpace,
                                                Type = "Library"
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioRRU.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioRRU.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioRRU.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.radioOtherId != null)
                                {
                                    var LibraryInfo = _context.TLIradioOtherLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.radioOther.radioOtherLibraryId);
                                    if (item.allLoadInst.radioOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "RadioOther",
                                            LoadName = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "RadioOther",
                                            LoadName = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.radioOther.Spaceinstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Spaceinstallation",
                                            LoadType = "RadioOther",
                                            LoadModel = item.allLoadInst.radioOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.radioOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.radioOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "RadioOther",
                                                LoadName = item.allLoadInst.radioOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "RadioOther",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.radioOther.CenterHigh;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.radioOther.CenterHigh;
                                    }
                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.radioOther.Spaceinstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.powerId != null)
                                {
                                    var LibraryInfo = _context.TLIpowerLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.power.powerLibraryId);
                                    if (item.allLoadInst.power.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "power",
                                            LoadName = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.power.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "power",
                                            LoadName = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.power.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "power",
                                            LoadModel = item.allLoadInst.power.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);

                                    }
                                    if (item.allLoadInst.power.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.power.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "power",
                                                LoadName = item.allLoadInst.power.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "power",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.power.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.power.CenterHigh;
                                    }

                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                            Type = "Installation",

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.power.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                else if (item.allLoadInst.loadOtherId != null)
                                {
                                    var LibraryInfo = _context.TLIloadOtherLibrary.FirstOrDefault(x => x.Id == item.allLoadInst.loadOther.loadOtherLibraryId);
                                    if (item.allLoadInst.loadOther.Azimuth == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Azimuth",
                                            LoadType = "loadOther",
                                            LoadName = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "Height",
                                            LoadType = "loadOther",
                                            LoadName = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.SpaceInstallation == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "SpaceInstallation",
                                            LoadType = "loadOther",
                                            LoadModel = item.allLoadInst.loadOther.Name,
                                            Type = "Installation",
                                            ReservedSpaceInCivil = item.ReservedSpace

                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    if (item.allLoadInst.loadOther.CenterHigh == 0)
                                    {
                                        if (item.allLoadInst.loadOther.HBA == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "HBA",
                                                LoadType = "loadOther",
                                                LoadName = item.allLoadInst.loadOther.Name,
                                                Type = "Installation",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        if (LibraryInfo.Length == 0)
                                        {
                                            RecalculatSpace recalculat = new RecalculatSpace()
                                            {
                                                AttributeName = "Length",
                                                LoadType = "loadOther",
                                                LoadModel = LibraryInfo.Model,
                                                Type = "Library",
                                                ReservedSpaceInCivil = item.ReservedSpace
                                            };
                                            recalculatSpaces.Add(recalculat);
                                        }
                                        CenterHigh = item.allLoadInst.loadOther.HBA + LibraryInfo.Length / 2;
                                    }
                                    else
                                    {
                                        CenterHigh = item.allLoadInst.loadOther.CenterHigh;
                                    }
                                    if (AllCivilInst.civilWithoutLeg.HeightBase == 0)
                                    {
                                        RecalculatSpace recalculat = new RecalculatSpace()
                                        {
                                            AttributeName = "HeightBase",
                                            LoadType = "civilWithoutLeg",
                                            LoadName = AllCivilInst.civilWithoutLeg.Name,
                                        };
                                        recalculatSpaces.Add(recalculat);
                                    }
                                    EquivalentSpace = item.allLoadInst.loadOther.SpaceInstallation * (CenterHigh / (float)AllCivilInst.civilWithoutLeg.HeightBase);
                                    AllCivilInst.civilWithoutLeg.CurrentLoads = AllCivilInst.civilWithoutLeg.CurrentLoads + EquivalentSpace;
                                    _context.TLIcivilWithoutLeg.Update(AllCivilInst.civilWithoutLeg);
                                }
                                if (recalculatSpaces == null)
                                {
                                    _context.SaveChanges();
                                    return new Response<List<RecalculatSpace>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                                }
                            }
                        }
                    }
                }
                _context.SaveChanges();
                return new Response<List<RecalculatSpace>>(true, recalculatSpaces, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception ex)
            {

                return new Response<List<RecalculatSpace>>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<bool> FilterAzimuthAndHeight(string? SiteCode, int? FirstLegId, int? SecondLegId, int? CivilwithLegId, int? CivilWithoutLegId, int? CivilNonSteelId, int? FirstSideArmId, int? SecondSideArmId,
        float Azimuth, float Height, int switchValue)
        {
            List<INSTALLATION_PLACE> Result = new List<INSTALLATION_PLACE>();

            var Check = _context.INSTALLATION_PLACE.Where(x =>
                x.SITECODE.ToLower() == SiteCode.ToLower() &&
                x.WITHLEG_ID == CivilwithLegId &&
                x.WITHOUTLEG_ID == CivilWithoutLegId &&
                x.NONSTEEL_ID == CivilNonSteelId
            );
            var cc = Check.ToList();
            if (FirstLegId != null)
            {
                Check = Check.Where(x => (x.FIRST_LEG_ID != null && x.FIRST_LEG_ID == FirstLegId) || (x.SECOND_LEG_ID != null && x.SECOND_LEG_ID == FirstLegId));
                if (SecondLegId != null)
                {
                    Check = Check.Where(x => (x.FIRST_LEG_ID != null && x.FIRST_LEG_ID == SecondLegId) || (x.SECOND_LEG_ID != null && x.SECOND_LEG_ID == SecondLegId));
                }
            }
            if (FirstSideArmId != null)
            {
                Check = Check.Where(x => (x.FIRST_SIDEARM_ID != null && x.FIRST_SIDEARM_ID == FirstSideArmId) || (x.SECOND_SIDEARM_ID != null && x.SECOND_SIDEARM_ID == FirstSideArmId));
                if (SecondSideArmId != null)
                {
                    Check = Check.Where(x => (x.FIRST_SIDEARM_ID != null && x.FIRST_SIDEARM_ID == SecondSideArmId) || (x.SECOND_SIDEARM_ID != null && x.SECOND_SIDEARM_ID == SecondSideArmId));
                }
            }
            switch (switchValue)
            {
                case 1:
                    Result = Check.Where(x =>
                        ((x.STATUS_NUMBER == 1 || x.STATUS_NUMBER == 4) && x.AZIMUTH == Azimuth && x.HEIGHT == Height) ||
                        (x.STATUS_NUMBER == 5 && x.HEIGHT == Height)
                    ).ToList();
                    break;
                case 2:
                case 3:
                    Result = Check.Where(x =>
                        (x.STATUS_NUMBER == 2 || x.STATUS_NUMBER == 3) && x.AZIMUTH == Azimuth && x.HEIGHT == Height
                    ).ToList();
                    break;
                case 4:
                    Result = Check.Where(x =>
                      ((x.STATUS_NUMBER == 1 || x.STATUS_NUMBER == 4) && x.AZIMUTH == Azimuth && x.HEIGHT == Height) ||
                      (x.STATUS_NUMBER == 5 && x.HEIGHT == Height)
                  ).ToList();
                    break;
                case 5:
                    Result = Check.Where(x =>
                        ((x.STATUS_NUMBER == 1 || x.STATUS_NUMBER == 4 || x.STATUS_NUMBER == 5) && x.HEIGHT == Height)).ToList();
                    break;
                default:

                    break;
            }


            if (Result.Count > 0)
            {
                return new Response<bool>(true, false, null, "Cannot install the load at the same azimuth and height because another load exists at the same angle.", (int)Helpers.Constants.ApiReturnCode.fail);
            }
            else
            {
                return new Response<bool>(true, true, null, "success", (int)Helpers.Constants.ApiReturnCode.success);
            }

            return new Response<bool>(true, true, null, "No conflicting load found.", (int)Helpers.Constants.ApiReturnCode.success);
        }

        public Response<bool> EditFilterAzimuthAndHeight(int? MWDishID, int? MWODUID, int? MWRFUID, int? MWBUID, int? MWOTHERID, int? RadioAntennaID
            , int? RadioRRUID, int? RadioOtherID, int? LOADOTHERID, int? PowerID,int?SideArmId,string LoadName,
            string? SiteCode, int? FirstLegId, int? SecondLegId, int? CivilwithLegId, int? CivilWithoutLegId, int? CivilNonSteelId, int? FirstSideArmId, int? SecondSideArmId,
        float Azimuth, float Height, int switchValue)
        {
            List<INSTALLATION_PLACE> Result = new List<INSTALLATION_PLACE>();
            IQueryable<INSTALLATION_PLACE> Check = null;
            if (LoadName == "TLImwBU")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.MWBU_ID != MWBUID
                    
                );
            }
            if (LoadName == "TLImwODU")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.MWODU_ID != MWODUID

                );
            }
            if (LoadName == "TLImwRFU")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.MWRFU_ID != MWRFUID

                );
            }
            if (LoadName == "TLImwDish")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.MWDISH_ID != MWDishID

                );
            }
            if (LoadName == "TLImwOther")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.MWOTHER_ID != MWOTHERID

                );
            }
            if (LoadName == "TLIradioOther")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.RADIO_OTHER_ID != RadioOtherID

                );
            }
            if (LoadName == "TLIradioRRU")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.RADIO_RRU_ID != RadioRRUID

                );
            }
            if (LoadName == "TLIradioAntenna")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.RADIO_ANTENNA_ID != RadioAntennaID

                );
            }
            if (LoadName == "TLIpower")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.POWER_ID != PowerID

                );
            }
            if (LoadName == "TLIloadOther")
            {
                 Check = _context.INSTALLATION_PLACE.Where(x =>
                    x.SITECODE.ToLower() == SiteCode.ToLower() &&
                    x.WITHLEG_ID == CivilwithLegId &&
                    x.WITHOUTLEG_ID == CivilWithoutLegId &&
                    x.NONSTEEL_ID == CivilNonSteelId && x.LOAD_OTHER_ID != LOADOTHERID

                );
            }
            if (LoadName == "TLIsideArm")
            {
                Check = _context.INSTALLATION_PLACE.Where(x =>
                   x.SITECODE.ToLower() == SiteCode.ToLower() &&
                   x.WITHLEG_ID == CivilwithLegId &&
                   x.WITHOUTLEG_ID == CivilWithoutLegId &&
                   x.NONSTEEL_ID == CivilNonSteelId && x.FIRST_SIDEARM_ID != SideArmId &&( x.STATUS_NUMBER==4 || x.STATUS_NUMBER == 5)

               );
            }
            var cc = Check.ToList();
            if (FirstLegId != null)
            {
                Check = Check.Where(x => (x.FIRST_LEG_ID != null && x.FIRST_LEG_ID == FirstLegId) || (x.SECOND_LEG_ID != null && x.SECOND_LEG_ID == FirstLegId));
                if (SecondLegId != null)
                {
                    Check = Check.Where(x => (x.FIRST_LEG_ID != null && x.FIRST_LEG_ID == SecondLegId) || (x.SECOND_LEG_ID != null && x.SECOND_LEG_ID == SecondLegId));
                }
            }
            if (FirstSideArmId != null)
            {
                Check = Check.Where(x => (x.FIRST_SIDEARM_ID != null && x.FIRST_SIDEARM_ID == FirstSideArmId) || (x.SECOND_SIDEARM_ID != null && x.SECOND_SIDEARM_ID == FirstSideArmId));
                if (SecondSideArmId != null)
                {
                    Check = Check.Where(x => (x.FIRST_SIDEARM_ID != null && x.FIRST_SIDEARM_ID == SecondSideArmId) || (x.SECOND_SIDEARM_ID != null && x.SECOND_SIDEARM_ID == SecondSideArmId));
                }
            }
            switch (switchValue)
            {
                case 1:
                    Result = Check.Where(x =>
                        ((x.STATUS_NUMBER == 1 || x.STATUS_NUMBER == 4) && x.AZIMUTH == Azimuth && x.HEIGHT == Height) ||
                        (x.STATUS_NUMBER == 5 && x.HEIGHT == Height)
                    ).ToList();
                    break;
                case 2:
                case 3:
                    Result = Check.Where(x =>
                        (x.STATUS_NUMBER == 2 || x.STATUS_NUMBER == 3) && x.AZIMUTH == Azimuth && x.HEIGHT == Height
                    ).ToList();
                    break;
                case 4:
                case 5:
                    Result = Check.Where(x =>
                        ((x.STATUS_NUMBER == 1 || x.STATUS_NUMBER == 4) && x.AZIMUTH == Azimuth && x.HEIGHT == Height) ||
                        (x.STATUS_NUMBER == 5 && x.HEIGHT == Height)
                    ).ToList();
                    break;
                default:

                    break;
            }


            if (Result.Count > 0)
            {
                return new Response<bool>(true, false, null, "Cannot install the load at the same azimuth and height because another load exists at the same angle.", (int)Helpers.Constants.ApiReturnCode.fail);
            }
            else
            {
                return new Response<bool>(true, true, null, "success", (int)Helpers.Constants.ApiReturnCode.success);
            }

            return new Response<bool>(true, true, null, "No conflicting load found.", (int)Helpers.Constants.ApiReturnCode.success);
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
        public IDictionary<string, object> BuildDynamicSelect(object obj, Dictionary<string, string>? dynamic, List<string> propertyNamesStatic, Dictionary<string, string> propertyNamesDynamic)
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
                    if (name.ToLower() == "baseplateshape")
                    {
                        var values = propertyInfo.GetValue(obj);
                        if (values.ToString() == "0")
                        {
                            values = "Circular";
                            item.Add(name, values);
                        }
                        else if (values.ToString() == "1")
                        {
                            values = "Rectangular";
                            item.Add(name, values);
                        }
                        else if(values.ToString() == "2")
                        {
                            values = "Square";
                            item.Add(name, values);

                        }
                        else if (values.ToString() == "3")
                        {
                            values = "NotMeasurable";
                            item.Add(name, values);
                        }
                    }
                    else if (name.ToLower() == "laddersteps")
                    {
                        var values = propertyInfo.GetValue(obj);
                        if (values != null)
                        {
                            if (values.ToString() == "0")
                            {
                                values = "Ladder";
                                item.Add(name, values);
                            }
                            else if (values.ToString() == "1")
                            {
                                values = "Steps";
                                item.Add(name, values);
                            }
                            
                        }
                        else
                        {
                            item.Add(name, propertyInfo.GetValue(obj));
                        }
                    }
                    else if (name.ToLower() == "equipmentslocation")
                    {
                        var values = propertyInfo.GetValue(obj);
                        if (values != null)
                        {
                            if (values.ToString() == "0")
                            {
                                values = "Body";
                                item.Add(name, values);
                            }
                            else if (values.ToString() == "1")
                            {
                                values = "Platform";
                                item.Add(name, values);
                            }
                            else if (values.ToString() == "2")
                            {
                                values = "Together";
                                item.Add(name, values);
                            }

                        }
                        else
                        {
                            item.Add(name, propertyInfo.GetValue(obj));
                        }
                    }
                    else if (name.ToLower() == "IntegratedWith")
                    {
                        var values = propertyInfo.GetValue(obj);
                        if (values != null)
                        {
                            if (values.ToString() == "0")
                            {
                                values = "Solar";
                                item.Add(name, values);
                            }
                            else if (values.ToString() == "1")
                            {
                                values = "Wind";
                                item.Add(name, values);
                            }
                           

                        }
                        else
                        {
                            item.Add(name, propertyInfo.GetValue(obj));
                        }
                    }
                    else
                    {
                        var value = propertyInfo.GetValue(obj);

                        item.Add(name, propertyInfo.GetValue(obj));
                    }
                    
                }
            }
            foreach (var propertyName in propertyNamesDynamic.Keys)
            {
                string datatype = propertyNamesDynamic[propertyName];

                if (datatype.ToLower() == "bool")
                {
                    var types = dynamic?.GetValueOrDefault(propertyName);
                    if (types == "1")
                    {
                        item.Add(propertyName, true);
                    }
                    else if (types == "0")
                    {
                        item.Add(propertyName, false);
                    }
                    else
                    {
                        item.Add(propertyName, false);
                    }
                }
                else if (datatype.ToLower() == "datetime")
                {
                    var value = dynamic?.GetValueOrDefault(propertyName);
                    if (value != null)
                    {
                        DateTime dateObject = DateTime.ParseExact(value, "dd-MMM-yy hh.mm.ss.fffffff tt", System.Globalization.CultureInfo.InvariantCulture);
                        item.Add(propertyName, dateObject);
                    }
                    else
                    {
                        item.Add(propertyName, value);
                    }
                    
                }
                else if (datatype.ToLower() == "double")
                {
                    var value = dynamic?.GetValueOrDefault(propertyName);
                    if (value != null)
                    {
                        var dateObject = Convert.ToDouble(value);
                        item.Add(propertyName, dateObject);
                    }
                    else
                    {
                        item.Add(propertyName, value);
                    }

                }

                else
                {
                    var value = dynamic?.GetValueOrDefault(propertyName);
                    item.Add(propertyName, value);
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
        public Response<float> CheckAvailableSpaceOnCivil(int AllCivilInst)
        {
            try
            {
                double Availablespace = 0;
                
                
                    //if (AllCivilInst.civilWithLegsId != null)
                    //{
                    //   if (AllCivilInst.civilWithLegs.CurrentLoads == null)
                    //    {
                    //    AllCivilInst.civilWithLegs.CurrentLoads = 0;
                    //    }
                    //    if (AllCivilInst.civilWithLegs.IsEnforeced == true)
                    //    {
                    //        Availablespace = AllCivilInst.civilWithLegs.SupportMaxLoadAfterInforcement - AllCivilInst.civilWithLegs.CurrentLoads;
                    //        if (Availablespace == 0 || Availablespace < 0)
                    //        {
                    //            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                    //        }
                    //    }

                    //    else if (AllCivilInst.civilWithLegs.Support_Limited_Load != 0)
                    //    {
                    //        Availablespace = AllCivilInst.civilWithLegs.Support_Limited_Load - AllCivilInst.civilWithLegs.CurrentLoads;
                    //        if (Availablespace == 0 || Availablespace < 0)
                    //        {
                    //            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Availablespace = AllCivilInst.civilWithLegs.CivilWithLegsLib.Manufactured_Max_Load - AllCivilInst.civilWithLegs.CurrentLoads;
                    //        if (Availablespace == 0 || Availablespace < 0)
                    //        {
                    //            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                    //        }
                    //    }
                    //}
                    //else if (AllCivilInst.civilWithoutLegId != null)
                    //{
                    //    if (AllCivilInst.civilWithoutLeg.CurrentLoads == null)
                    //    {
                    //    AllCivilInst.civilWithoutLeg.CurrentLoads = 0;
                    //    }
                    //    if (AllCivilInst.civilWithoutLeg.Support_Limited_Load != 0)
                    //    {
                    //        Availablespace = AllCivilInst.civilWithoutLeg.Support_Limited_Load - AllCivilInst.civilWithoutLeg.CurrentLoads;
                    //        if (Availablespace == 0 || Availablespace < 0)
                    //        {
                    //            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Availablespace = AllCivilInst.civilWithoutLeg.CivilWithoutlegsLib.Manufactured_Max_Load - AllCivilInst.civilWithoutLeg.CurrentLoads;
                    //        if (Availablespace == 0 || Availablespace < 0)
                    //        {
                    //            return new Response<float>(true, 0, null, $"No available space on the civill ", (int)Helpers.Constants.ApiReturnCode.fail);
                    //        }
                    //    }


                    //}
                
                return new Response<float>(true, 0, null, "Success", (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<float>(true, 0, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }


        }
        public Response<float> CheckAvailableSpaceOnCivils(TLIallCivilInst AllCivilInst)
        {
            try
            {
                double Availablespace = 0;


                if (AllCivilInst.civilWithLegsId != null)
                {
                    if (AllCivilInst.civilWithLegs.CurrentLoads == null)
                    {
                        AllCivilInst.civilWithLegs.CurrentLoads = 0;
                    }
                    if (AllCivilInst.civilWithLegs.IsEnforeced == true)
                    {
                        Availablespace = AllCivilInst.civilWithLegs.SupportMaxLoadAfterInforcement - AllCivilInst.civilWithLegs.CurrentLoads;
                        if (Availablespace < 0)
                        {
                            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }

                    else if (AllCivilInst.civilWithLegs.Support_Limited_Load != 0)
                    {
                        Availablespace = AllCivilInst.civilWithLegs.Support_Limited_Load - AllCivilInst.civilWithLegs.CurrentLoads;
                        if ( Availablespace <= 0)
                        {
                            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        Availablespace = AllCivilInst.civilWithLegs.CivilWithLegsLib.Manufactured_Max_Load - AllCivilInst.civilWithLegs.CurrentLoads;
                        if (Availablespace < 0)
                        {
                            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
                else if (AllCivilInst.civilWithoutLegId != null)
                {
                    if (AllCivilInst.civilWithoutLeg.CurrentLoads == null)
                    {
                        AllCivilInst.civilWithoutLeg.CurrentLoads = 0;
                    }
                    if (AllCivilInst.civilWithoutLeg.Support_Limited_Load != 0)
                    {
                        Availablespace = AllCivilInst.civilWithoutLeg.Support_Limited_Load - AllCivilInst.civilWithoutLeg.CurrentLoads;
                        if (Availablespace < 0)
                        {
                            return new Response<float>(true, 0, null, $"No available space on the civil ", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        Availablespace = AllCivilInst.civilWithoutLeg.CivilWithoutlegsLib.Manufactured_Max_Load - AllCivilInst.civilWithoutLeg.CurrentLoads;
                        if (Availablespace < 0)
                        {
                            return new Response<float>(true, 0, null, $"No available space on the civill ", (int)Helpers.Constants.ApiReturnCode.fail);
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
