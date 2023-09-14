using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.MW_PortDTOs;

namespace TLIS_Service.IService
{
    public interface IMW_PortService
    {
        Task<Response<IEnumerable<MW_PortViewModel>>> GetPorts();
        Task<Response<MW_PortViewModel>> GetPortById(int Id);
        Task<Response<AddMW_PortViewModel>> Create(AddMW_PortViewModel Port);
        Task<Response<EditMW_PortViewModel>> Update(EditMW_PortViewModel Port);
    }
}
