using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        IUnitOfWorkService _unitOfWork;
        private readonly IConfiguration _configuration;
        private int? getUserId()
        {
            /*
            int userId = 0;
            if (int.TryParse(HttpContext.Session.GetString("UserId"), out userId))
            {
                return userId;
            }
            return null;
            //*/
            return 83; // user id is 83
        }
        public TicketController(IUnitOfWorkService unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        //--------------------------------- Ticket 
        [HttpPost("GetAllTickets")]
        [ProducesResponseType(200, Type = typeof(List<ListTicketViewModel>))]
        public IActionResult GetAllTickets([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWork.TicketService.GetAllTickets(parameterPagination, filters);
            return Ok(response);
        }

        

        //--------------------------------- Ticket 
        [HttpGet("GetPendingRequestes")]
        [ProducesResponseType(200, Type = typeof(List<PendingRequestViewModel>))]
        public IActionResult GetPendingRequestes()
        {
            var response = _unitOfWork.TicketService.GetPendingRequestes(getUserId());
            return Ok(response);
        }

        [HttpGet("GetTicketActionById/{Id}")]
        [ProducesResponseType(200, Type = typeof(TicketActinDetailsViewModel))]
        public IActionResult GetTicketActionById(int Id)
        {
            var response = _unitOfWork.TicketService.GetTicketActionById(Id, getUserId());
            return Ok(response);
        }

        

        [HttpPost("AddTicket")]
        [ProducesResponseType(200, Type = typeof(ListTicketViewModel))]
        public IActionResult AddTicket(AddTicketViewModel ticket)
        {
            try
            {
                if (TryValidateModel(ticket, nameof(AddTicketViewModel)))
                {
                    var response = _unitOfWork.TicketService.AddTicket(_configuration, ticket, null, null);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<ListTicketViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<ListTicketViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }        

        [HttpPost("ExecuteTicktRequeste")]
        [ProducesResponseType(200, Type = typeof(TicketActinDetailsViewModel))]
        public IActionResult ExecuteTicktRequeste(TicketActinDetailsViewModel ticket)
        {
            try
            {
                if (TryValidateModel(ticket, nameof(TicketActinDetailsViewModel)))
                {
                    var response = _unitOfWork.TicketService.ExecuteTicktRequeste(_configuration, ticket, getUserId());
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<ListTicketViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<TicketActinDetailsViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        
        //--------------------------------- End of Ticket
    }

}