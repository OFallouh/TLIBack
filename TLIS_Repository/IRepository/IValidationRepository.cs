using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IValidationRepository:IRepositoryBase<TLIvalidation, ValidationViewModel,int>
    {
        bool CheckValidation(List<TLIvalidation> Validations, string DynamicAttName, object Value, out string ErrorMessage);
        //bool CheckDependencyValidation(object AddViewModel, TLIdependency validation, string TableName, object DynamicAttValue, string DynamicAttName, string SiteCode, OracleConnection con, out string ErrorMessage);
    };
}
