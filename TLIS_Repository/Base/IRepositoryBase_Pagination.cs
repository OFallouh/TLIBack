using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
   public interface IRepositoryBase<TEntity,TModel,TSearchModel, TKey>:IRepositoryBase<TEntity, TModel, TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(ParameterPagination parameterPagination, TSearchModel modelItem);
    }
}
