using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class AttributeActivatedRepository : RepositoryBase<TLIattributeActivated, AttributeActivatedViewModel, int>, IAttributeActivatedRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public AttributeActivatedRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<BaseAttView> GetAttributeActivated(string Type, object Library = null, int? CategoryId = null, params string[] ExceptAtrributes)
        {
            if (CategoryId != null)
            {
                List<TLIattActivatedCategory> Excepted = _context.TLIattActivatedCategory.Where(x => ExceptAtrributes.Contains(x.attributeActivated.Key) && x.attributeActivated.Tabel == Type && x.civilWithoutLegCategoryId == CategoryId).ToList();
                List<TLIattActivatedCategory> AttActivatedCategoryStatus = _context.TLIattActivatedCategory
                    .Where(x => (x.civilWithoutLegCategoryId != null ? (x.civilWithoutLegCategoryId.Value == CategoryId.Value) : false) && x.attributeActivated.Tabel == Type && x.enable &&
                        x.attributeActivated.Key.ToLower() != "id" && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted")
                    .Include(a => a.attributeActivated)
                    .AsEnumerable().Except(Excepted).ToList();

                List<BaseAttView> BaseAttsView = new List<BaseAttView>();
                object value = null;
                foreach (var AttributeActivated in AttActivatedCategoryStatus)
                {
                    value = null;
                    if (Library != null)
                    {
                        value = Library.GetType().GetProperty(AttributeActivated.attributeActivated.Key).GetValue(Library);
                    }
                    BaseAttsView.Add(new BaseAttView { Key = AttributeActivated.attributeActivated.Key, Label = AttributeActivated.attributeActivated.Label, Desc = AttributeActivated.attributeActivated.Description, Value = value, enable = AttributeActivated.attributeActivated.enable, AutoFill = AttributeActivated.attributeActivated.AutoFill, Manage = AttributeActivated.attributeActivated.Manage, Required = AttributeActivated.attributeActivated.Required, DataType = AttributeActivated.attributeActivated.DataType });
                }

                foreach (BaseAttView item in BaseAttsView)
                {
                    TLIattActivatedCategory Test = AttActivatedCategoryStatus.FirstOrDefault(x =>
                        x.civilWithoutLegCategoryId == CategoryId && x.attributeActivated.Key.ToLower() == item.Key.ToLower());

                    item.Required = Test.Required;
                    item.enable = Test.enable;
                    item.Label = Test.Label;

                    if (item.DataType.ToLower() != "list")
                        item.Desc = Test.Description;
                }
                return BaseAttsView;
            }
            else
            {
                List<TLIattributeActivated> Excepted = _context.TLIattributeActivated.Where(x =>
                    ExceptAtrributes.Contains(x.Key) && x.Tabel == Type && x.enable).ToList();

                List<TLIattributeActivated> AttributesActivated = _context.TLIattributeActivated.Where(x =>
                    x.enable && x.Tabel == Type && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" &&
                    x.Key.ToLower() != "deleted").ToList().Except(Excepted).ToList();

                List<BaseAttView> BaseAttsView = new List<BaseAttView>();
                object value = null;
                foreach (var AttributeActivated in AttributesActivated)
                {
                    value = null;
                    if (Library != null && !AttributeActivated.Label.ToLower().Contains("_name"))
                    {
                        value = Library.GetType().GetProperty(AttributeActivated.Key).GetValue(Library);
                    }
                    BaseAttsView.Add(new BaseAttView { Key = AttributeActivated.Key, Label = AttributeActivated.Label, Desc = AttributeActivated.Description, Value = value, enable = AttributeActivated.enable, AutoFill = AttributeActivated.AutoFill, Manage = AttributeActivated.Manage, Required = AttributeActivated.Required, DataType = AttributeActivated.DataType });
                }
                return BaseAttsView;
            }
        }
        public List<BaseInstAttView> GetInstAttributeActivatedForCivilWithoutLeg(int? CategoryId, object Installation = null, params string[] ExceptAtrributes)
        {
            List<TLIattActivatedCategory> Excepted = _context.TLIattActivatedCategory.Include(x => x.attributeActivated)
                .Where(x => ExceptAtrributes.Contains(x.attributeActivated.Key) && x.civilWithoutLegCategoryId == CategoryId).ToList();

            List<TLIattActivatedCategory> AttActivatedCategories = _context.TLIattActivatedCategory.Include(x => x.attributeActivated)
                .Where(x => x.enable && x.civilWithoutLegCategoryId.Value == CategoryId.Value && !x.IsLibrary &&
                    x.attributeActivated.Tabel.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower() &&
                    x.attributeActivated.Key.ToLower() != "id" && x.attributeActivated.Key.ToLower() != "active" &&
                    x.attributeActivated.Key.ToLower() != "deleted").ToList().Except(Excepted).ToList();

            List<TLIattributeActivated> AttributesActivated = _context.TLIattributeActivated
                .Where(x => AttActivatedCategories.FirstOrDefault(y => y.attributeActivatedId == x.Id) != null).ToList();

            List<BaseInstAttView> BaseAttsView = new List<BaseInstAttView>();
            if (Installation == null)
            {
                foreach (var AttributeActivated in AttributesActivated)
                {
                    TLIattActivatedCategory AttActivatedCategory = AttActivatedCategories
                        .FirstOrDefault(x => x.attributeActivatedId == AttributeActivated.Id);

                    BaseAttsView.Add(new BaseInstAttView
                    {
                        Key = AttributeActivated.Key,
                        Label = AttActivatedCategory.Label,
                        Desc = AttActivatedCategory.Description,
                        enable = AttActivatedCategory.enable,
                        AutoFill = AttributeActivated.AutoFill,
                        Manage = AttributeActivated.Manage,
                        Required = AttActivatedCategory.Required,
                        DataType = AttributeActivated.DataType
                    });
                }
            }
            else
            {
                foreach (var AttributeActivated in AttributesActivated)
                {
                    TLIattActivatedCategory AttActivatedCategory = AttActivatedCategories
                        .FirstOrDefault(x => x.attributeActivatedId == AttributeActivated.Id);

                    object value = Installation.GetType().GetProperty(AttributeActivated.Key).GetValue(Installation);
                    if (AttributeActivated.Key == "equipmentsLocation" || AttributeActivated.Key == "reinforced" || AttributeActivated.Key == "availabilityOfWorkPlatforms" || AttributeActivated.Key == "ladderSteps")
                    {
                        value = value.ToString();
                    }
                    BaseAttsView.Add(new BaseInstAttView
                    {
                        Key = AttributeActivated.Key,
                        Label = AttActivatedCategory.Label,
                        Desc = AttActivatedCategory.Description,
                        Value = value,
                        enable = AttActivatedCategory.enable,
                        AutoFill = AttributeActivated.AutoFill,
                        Manage = AttributeActivated.Manage,
                        Required = AttActivatedCategory.Required,
                        DataType = AttributeActivated.DataType
                    });
                }
            }
            return BaseAttsView;
        }


        public IEnumerable<BaseInstAttView> GetInstAttributeActivated(string Type, object Installation = null, params string[] ExceptAtrributes)
        {
            List<TLIattributeActivated> Excepted = _context.TLIattributeActivated.Where(x =>
                ExceptAtrributes.Contains(x.Key)).ToList();

            List<TLIattributeActivated> AttributesActivated = _context.TLIattributeActivated.Where(x =>
                x.Tabel == Type && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" &&
                x.Key.ToLower() != "deleted" && x.enable).ToList().Except(Excepted).ToList();

            List<BaseInstAttView> BaseAttsView = new List<BaseInstAttView>();
            if (Installation == null)
            {
                foreach (var AttributeActivated in AttributesActivated)
                {
                    if (AttributeActivated.Key.ToLower() == "reservedspace")
                    {
                        BaseAttsView.Add(new BaseInstAttView { Key = AttributeActivated.Key, Value = false, Label = AttributeActivated.Label, Desc = AttributeActivated.Description, enable = AttributeActivated.enable, AutoFill = AttributeActivated.AutoFill, Manage = AttributeActivated.Manage, Required = AttributeActivated.Required, DataType = AttributeActivated.DataType });
                    }
                    else
                    {
                        BaseAttsView.Add(new BaseInstAttView { Key = AttributeActivated.Key, Label = AttributeActivated.Label, Desc = AttributeActivated.Description, enable = AttributeActivated.enable, AutoFill = AttributeActivated.AutoFill, Manage = AttributeActivated.Manage, Required = AttributeActivated.Required, DataType = AttributeActivated.DataType });
                    }

                }
            }
            else
            {
                foreach (var AttributeActivated in AttributesActivated)
                {
                    var value = Installation.GetType().GetProperty(AttributeActivated.Key).GetValue(Installation);
                    BaseAttsView.Add(new BaseInstAttView { Key = AttributeActivated.Key, Label = AttributeActivated.Label, Desc = AttributeActivated.Description, Value = value, enable = AttributeActivated.enable, AutoFill = AttributeActivated.AutoFill, Manage = AttributeActivated.Manage, Required = AttributeActivated.Required, DataType = AttributeActivated.DataType });
                }
            }
            return BaseAttsView;
        }
    }
}
