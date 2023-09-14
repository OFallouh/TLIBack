using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IAttributeActivatedRepository : IRepositoryBase<TLIattributeActivated, AttributeActivatedViewModel, int>
    {
        IEnumerable<BaseAttView> GetAttributeActivated(string Type, Object Library = null, int? CategoryId = null, params string[] ExceptAtrributes);
        List<BaseInstAttView> GetInstAttributeActivatedForCivilWithoutLeg(int? CategoryId, object Installation = null, params string[] ExceptAtrributes);
        IEnumerable<BaseInstAttView> GetInstAttributeActivated(string Type, Object Installation = null, params string[] ExceptAtrributes);
    }
}
