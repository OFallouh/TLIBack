using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;

namespace TLIS_Service.IService
{
    public interface ITicketService
    {
        Response<List<ListTicketViewModel>> GetAllTickets(ParameterPagination parameterPagination, List<FilterObjectList> filter);
        Response<ListTicketViewModel> AddTicket(IConfiguration _configuration, AddTicketViewModel ticket, int? CreatorId, int? IntegrationId);
        Response<List<PendingRequestViewModel>> GetPendingRequestes(int? userId);
        Response<TicketActinDetailsViewModel> GetTicketActionById(int Id, int? UserID);
        Response<TicketActinDetailsViewModel> ExecuteTicktRequeste(IConfiguration _configuration, TicketActinDetailsViewModel request, int? userID);
    }
}
