using AutoMapper;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.wf;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using WF_API.Model;
using MailKit.Net.Smtp;
using static TLIS_Repository.Helpers.Constants;

namespace TLIS_Repository.Repositories
{
    public class SiteRepository : RepositoryBase<TLIsite, SiteViewModel, string>, ISiteRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        private readonly IConfiguration _configuration;
        public SiteRepository(ApplicationDbContext context, IConfiguration configuration, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            //var siteStatus = _context.TLIsiteStatus.ToList();
            //List<DropDownListFilters> siteStatusLists = _mapper.Map<List<DropDownListFilters>>(siteStatus);
            //RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("siteStatusId", siteStatusLists));
            List<TLIregion> Region = _context.TLIregion.ToList();
            List<DropDownListFilters> RegionLists = _mapper.Map<List<DropDownListFilters>>(Region);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("RegionCode", RegionLists));
            var Area = _context.TLIarea.ToList();
            List<DropDownListFilters> AreaLists = _mapper.Map<List<DropDownListFilters>>(Area);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("AreaCode", AreaLists));
            return RelatedTables;
        }

        public void UpdateReservedSpace(string SiteCode, float SpaceInstallation)
        {
            var Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).FirstOrDefault();
            Site.ReservedSpace += SpaceInstallation;
            _context.TLIsite.Update(Site);
            _context.SaveChanges();

        }
        //public Response<string> CheckSpace (string SiteCode , string TableName, int LibraryId)
        // {
        //     try
        //     {
        //         var civilsite = _context.TLIcivilSiteDate.Where(x => x.SiteCode == SiteCode && x.ReservedSpace == true).Include(x => x.allCivilInst).Select(x => x.allCivilInst).ToList();
        //         float Space = 0;
        //         foreach (var Item in civilsite)
        //         {
        //            if(Item.civilWithLegsId != null)
        //             {
        //                 var civilWithLegsSpace = _context.TLIcivilWithLegs.Where(x => x.Id == Item.civilWithLegsId).Select(x=>x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithLegsSpace + Space;
        //             }
        //            if(Item.civilWithoutLegId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIcivilWithoutLeg.Where(x => x.Id == Item.civilWithoutLegId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }

        //         }
        //         var OtherInst = _context.TLIotherInSite.Where(x => x.SiteCode == SiteCode && x.ReservedSpace == true).Include(x => x.allOtherInventoryInst).Select(x => x.allOtherInventoryInst).ToList();

        //         foreach (var Item in OtherInst)
        //         {
        //             if (Item.cabinetId != null)
        //             {
        //                 var civilWithLegsSpace = _context.TLIcabinet.Where(x => x.Id == Item.cabinetId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithLegsSpace + Space;
        //             }
        //             if (Item.solarId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIsolar.Where(x => x.Id == Item.solarId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }
        //             if (Item.generatorId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIgenerator.Where(x => x.Id == Item.generatorId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }
        //         }
        //           if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
        //         {
        //             var civilwithlegsSpaceLibrary = _context.TLIcivilWithLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + civilwithlegsSpaceLibrary;
        //         }
        //         if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
        //         {
        //             var civilWithoutLegSpaceLibrary = _context.TLIcivilWithoutLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + civilWithoutLegSpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIcabinet.ToString() == TableName)
        //         {
        //             var cabinetPowerLibrarySpaceLibrary = _context.TLIcabinetPowerLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + cabinetPowerLibrarySpaceLibrary;

        //         }
        //         if (OtherInventoryType.TLIcabinet.ToString() == TableName)
        //         {
        //             var cabinetSpaceLibrary = _context.TLIcabinetTelecomLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + cabinetSpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIgenerator.ToString() == TableName)
        //         {
        //             var generatorLibrarySpaceLibrary = _context.TLIgeneratorLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + generatorLibrarySpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIsolar.ToString() == TableName)
        //         {
        //             var solarLibrarySpaceLibrary = _context.TLIsolarLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + solarLibrarySpaceLibrary;
        //         }
        //         var siteSpace = _context.TLIsite.FirstOrDefault(x => x.SiteCode == SiteCode).RentedSpace;
        //         if (Space > siteSpace)
        //         {
        //             return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
        //         }                                              
        //         return new Response<string>();
        //     }

        //     catch (Exception err)
        //     {

        //         return new Response<string>(true, null, null,err.Message , (int)Helpers.Constants.ApiReturnCode.fail); 
        //     }
        // }
        public Response<string> CheckSpace(string SiteCode, string TableName, int LibraryId, float SpaceInstallation, string Cabinet)
        {
            try
            {
                var Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).FirstOrDefault();
                if (SpaceInstallation != 0)
                {
                    var space = Site.ReservedSpace + SpaceInstallation;
                    if (space <= Site.RentedSpace)
                    {
                        Site.ReservedSpace = space;
                        _context.TLIsite.Update(Site);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else
                {
                    if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                    {
                        var civilWithoutLegSpaceLibrary = _context.TLIcivilWithoutLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
                        if (civilWithoutLegSpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilWithoutLegSpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                    {
                        var civilWithLegSpaceLibrary = _context.TLIcivilWithLegLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilWithLegSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilWithLegSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                    {
                        var civilNonSteelLibrary = _context.TLIcivilNonSteelLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilNonSteelLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilNonSteelLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Power")
                    {
                        var cabinetPowerLibrarySpaceLibrary = _context.TLIcabinetPowerLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetPowerLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetPowerLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }

                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else if (cabinetPowerLibrarySpaceLibrary.Depth != 0 && cabinetPowerLibrarySpaceLibrary.Width != 0)
                        {
                            var lengh = cabinetPowerLibrarySpaceLibrary.Depth;
                            var Width = cabinetPowerLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Telecom")
                    {
                        var cabinetSpaceLibrary = _context.TLIcabinetTelecomLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else if (cabinetSpaceLibrary.Depth != 0 && cabinetSpaceLibrary.Width != 0)
                        {
                            var lengh = cabinetSpaceLibrary.Depth;
                            var Width = cabinetSpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIgenerator.ToString() == TableName)
                    {
                        var generatorLibrarySpaceLibrary = _context.TLIgeneratorLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (generatorLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + generatorLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }

                        else if (generatorLibrarySpaceLibrary.Length != 0 && generatorLibrarySpaceLibrary.Width != 0)
                        {
                            var lengh = generatorLibrarySpaceLibrary.Length;
                            var Width = generatorLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIsolar.ToString() == TableName)
                    {
                        var solarLibrarySpaceLibrary = _context.TLIsolarLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (solarLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + solarLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }

                return new Response<string>();
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public ApiResponse SubmitTaskByTLI(int taskId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var Task = _context.T_WF_TASKS.Include(x => x.PhaseAction).ThenInclude(x => x.Action).ThenInclude(x => x.FORMELEMENTS).Include(x => x.PhaseAction.Action.MetaLink).FirstOrDefault(x => x.Id == taskId && x.Status == Task_Status_Enum.Open);
                    if (Task != null)
                    {
                        if (Task.PhaseAction.Action.MetaLink.Api != null && !Task.PhaseAction.Action.MetaLink.Api.ToLower().StartsWith("get"))
                        {
                            if (Task.PhaseAction.Action.FORMELEMENTS.Count == 0)
                            {
                                Task.Status = Task_Status_Enum.End;
                                _context.T_WF_TASKS.Update(Task);
                                _context.SaveChanges();
                                var Delegation = _context.T_WF_DELEGATIONS.FirstOrDefault(x => x.TaskId == Task.Id);
                                if (Delegation != null)
                                {
                                    Delegation.EndDate = DateTime.Now;
                                    _context.T_WF_DELEGATIONS.Update(Delegation); _context.SaveChanges();
                                    _context.SaveChanges();
                                }

                                var PhaseAction = _context.T_WF_PHASE_ACTIONS.FirstOrDefault(x => x.Id == Task.PhaseActionId);
                                if (PhaseAction != null)
                                {
                                    var Actions = _context.T_WF_PHASE_ACTIONS.Where(x => x.PhaseId == PhaseAction.PhaseId).Select(x => x.Id).ToList();
                                    var Phases = _context.T_WF_PHASE_ACTIONS.Where(x => x.PhaseId == PhaseAction.PhaseId).Select(x => x.PhaseId).ToList();
                                    var TaskAction = _context.T_WF_TASKS.Where(x => Actions.Any(y => y == x.PhaseActionId) && x.TicketId == Task.TicketId).ToList();
                                    var TicketInfo = _context.T_WF_TICKETS.FirstOrDefault(x => x.Id == Task.TicketId);
                                    if (TicketInfo != null)
                                    {
                                        var ActionInfo = _context.T_WF_ACTIONS.FirstOrDefault(x => x.Id == PhaseAction.ActionId);
                                        if (ActionInfo != null)
                                        {
                                            var UserTo = GetEmailByUserId(TicketInfo.CreatedById);
                                            var UserFrom = GetNameByUserId(Task.UserId);
                                            if (UserTo != null && UserFrom != null)
                                            {
                                                SendSubmitTaskNotificationMailToAdmin(TicketInfo.Name, ActionInfo.Name, UserFrom, UserTo);
                                            }
                                        }

                                    }
                                    if (TaskAction.All(x => x.Status == Task_Status_Enum.End))
                                    {
                                        foreach (var item in TaskAction)
                                        {
                                            List<bool> results = new List<bool>();
                                            List<T_WF_LINK>? Links = _context.T_WF_LINKS.Include(x => x.WF_CONDITIONS).Include(x => x.WF_TASKS).ToList();
                                            List<T_WF_LINK>? nextphase = Links.Where(x => Phases.Any(y => y == x.CurrentPhaseId)).ToList();
                                            foreach (var itemLink in nextphase)
                                            {
                                                results = new List<bool>();
                                                List<bool> ConditionResult = new List<bool>();
                                                if (itemLink.WF_CONDITIONS != null)
                                                {
                                                    foreach (var itemcondition in itemLink.WF_CONDITIONS)
                                                    {
                                                        if (itemcondition.Condition.ToString().ToLower() == "any")
                                                        {
                                                            ConditionResult.Add(true);
                                                        }
                                                        else
                                                        {
                                                            var Taskcompare = TaskAction.FirstOrDefault(x => x.Condition == itemcondition.Condition && x.PhaseActionId == itemcondition.PhaseActionId);
                                                            if (Taskcompare != null)
                                                            {
                                                                ConditionResult.Add(true);

                                                            }
                                                            else
                                                            {
                                                                ConditionResult.Add(false);
                                                            }
                                                        }
                                                    }
                                                    if (ConditionResult.All(x => x == true))
                                                    {


                                                        List<T_WF_NODE>? childNode = _context.T_WF_NODES
                                                         .Where(x => x.LinkId == itemLink.Id).ToList();

                                                        var Tasks = _context.T_WF_TASKS.Where(x => Actions.Any(y => y == x.PhaseActionId) && x.TicketId == Task.TicketId).ToList();
                                                        var tasksIds = Tasks.Select(x => x.Id).ToList();
                                                        var TaskValue = _context.T_WF_TASK_VALUES.Where(x => tasksIds.Any(y => y == x.TaskId)).ToList();

                                                        var rootNode = childNode.FirstOrDefault(node => node.ParentId == null);
                                                        var nodes = _mapper.Map<NodesViewModel>(rootNode);
                                                        if (rootNode != null)
                                                        {
                                                            nodes.Children = GetChildrenNodes(rootNode.Id, itemLink.Id);

                                                            nodes.Options = _mapper.Map<List<OptionsBinding>>(_context.T_WF_ACTION_OPTIONS.Where(x => x.NodeId == rootNode.Id).ToList());
                                                            bool result = ProcessSingleNode(nodes, TaskValue);
                                                            results.Add(result);
                                                            if (result == true)
                                                            {
                                                                var NextPhase = _context.T_WF_PHASE_ACTIONS.Where(x => x.PhaseId == itemLink.NextPhaseId).ToList();
                                                                foreach (var itemtask in NextPhase)
                                                                {
                                                                    T_WF_TASK t_WF_TASK = new T_WF_TASK()
                                                                    {
                                                                        PhaseActionId = itemtask.Id,
                                                                        TicketId = Task.TicketId,
                                                                        Status = Task_Status_Enum.Open,
                                                                        StratDate = DateTime.Now,
                                                                        EndDate = null,
                                                                        UserId = null,
                                                                        Condition = null,
                                                                        LinkId = itemLink.Id,
                                                                    };

                                                                    _context.T_WF_TASKS.Add(t_WF_TASK);
                                                                    _context.SaveChanges();
                                                                    if (TicketInfo != null)
                                                                    {
                                                                        string TicketName = TicketInfo.Name;
                                                                        var ActionInfo = _context.T_WF_ACTIONS.FirstOrDefault(x => x.Id == itemtask.ActionId);
                                                                        if (ActionInfo != null)
                                                                        {
                                                                            string ActionName = ActionInfo.Name;
                                                                            var UserTo = GetEmailByUserId(itemtask.UserAssignToId);
                                                                            if (UserTo != null)
                                                                            {
                                                                                SendSubmitTaskNotificationMail(TicketName, ActionName, UserTo);
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                break;
                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                            if (!results.Any(x => x == true) || results.Count == 0)
                                            {
                                                T_WF_TICKET? ticket = _context.T_WF_TICKETS.FirstOrDefault(x => x.Id == Task.TicketId);
                                                if (ticket != null)
                                                {
                                                    ticket.Status = "Closed";
                                                    ticket.EndDate = DateTime.Now;
                                                    _context.T_WF_TICKETS.Update(ticket);
                                                    _context.SaveChanges();
                                                    var UserTo = GetEmailByUserId(ticket.CreatedById);
                                                    if (UserTo != null)
                                                    {
                                                        SendClosedTicketNotificationMailToAdmin(ticket.Name, UserTo);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        _context.SaveChanges();
                                        transaction.Complete();
                                        return new ApiResponse(true, null);

                                    }
                                    _context.SaveChanges();
                                    transaction.Complete();
                                }
                            }
                            return new ApiResponse(true, null);
                        }
                        else
                        {
                            if (Task.PhaseAction.Action.FORMELEMENTS.Count == 0)
                            {
                                Task.Status = Task_Status_Enum.SubmitedByTLI;
                                _context.T_WF_TASKS.Update(Task);
                                _context.SaveChanges();
                            }
                            return new ApiResponse(true, null);
                        }
                    }
                    
                    else
                    {
                        return new ApiResponse(false, "TheTaskId Is Not Found");
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        public ApiResponse SendSubmitTaskNotificationMail(string TicketName, string ActionName, string UserTo)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration.GetSection("MailSettings:From").Value));

                email.To.Add(MailboxAddress.Parse(UserTo));

                email.Subject = "NewTask";
                email.Date = DateTimeOffset.Now;
                var builder = new BodyBuilder();

                builder.HtmlBody = $"You have a new task <span style='color: darkblue;'>{ActionName}</span> in Ticket <span style='color: black;'>{TicketName}</span>, open your mytasks page to see more details about the task";
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                try
                {
                    smtp.Connect(_configuration.GetSection("MailSettings:Host").Value, int.Parse(_configuration.GetSection("MailSettings:Port").Value), SecureSocketOptions.None);
                    smtp.Authenticate(_configuration.GetSection("MailSettings:From").Value, _configuration.GetSection("MailSettings:Password").Value);
                    smtp.Send(email);
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return new ApiResponse(true, null);
            }

            catch (Exception er)
            {
                return new ApiResponse(false, er.Message);
            }
        }
        public ApiResponse SendSubmitTaskNotificationMailToAdmin(string TicketName, string ActionName, string UserFrom, string UserTo)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration.GetSection("MailSettings:From").Value));

                email.To.Add(MailboxAddress.Parse(UserTo));

                email.Subject = "SubmitTask";
                email.Date = DateTimeOffset.Now;
                var builder = new BodyBuilder();

                builder.HtmlBody = $"<span style='color: darkblue;'>{UserFrom}</span> submitted his task <span style='color: green;'>{ActionName}</span> in Ticket <span style='color: black;'>{TicketName}</span>, open your myticket page to see more details about the task";
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                try
                {
                    smtp.Connect(_configuration.GetSection("MailSettings:Host").Value, int.Parse(_configuration.GetSection("MailSettings:Port").Value), SecureSocketOptions.None);
                    smtp.Authenticate(_configuration.GetSection("MailSettings:From").Value, _configuration.GetSection("MailSettings:Password").Value);
                    smtp.Send(email);
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return new ApiResponse(true, null);
            }

            catch (Exception er)
            {
                return new ApiResponse(false, er.Message);
            }
        }
        public ApiResponse SendRejectTaskNotificationMailToAdmin(string TicketName, string ActionName, string UserFrom, string UserTo)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration.GetSection("MailSettings:From").Value));

                email.To.Add(MailboxAddress.Parse(UserTo));

                email.Subject = "RejectTask";
                email.Date = DateTimeOffset.Now;
                var builder = new BodyBuilder();

                builder.HtmlBody = $"<span style='color: darkred;'>{UserFrom}</span> rejected his task <span style='color: red;'>{ActionName}</span> in Ticket <span style='color: black;'>{TicketName}</span>, open your myticket page to see more details about the task";
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                try
                {
                    smtp.Connect(_configuration.GetSection("MailSettings:Host").Value, int.Parse(_configuration.GetSection("MailSettings:Port").Value), SecureSocketOptions.None);
                    smtp.Authenticate(_configuration.GetSection("MailSettings:From").Value, _configuration.GetSection("MailSettings:Password").Value);
                    smtp.Send(email);
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return new ApiResponse(true, null);
            }

            catch (Exception er)
            {
                return new ApiResponse(false, er.Message);
            }
        }
        public ApiResponse SendClosedTicketNotificationMailToAdmin(string TicketName, string UserTo)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration.GetSection("MailSettings:From").Value));

                email.To.Add(MailboxAddress.Parse(UserTo));

                email.Subject = "RejectTask";
                email.Date = DateTimeOffset.Now;
                var builder = new BodyBuilder();

                builder.HtmlBody = $"The ticket <span style='color: green;'>{TicketName}</span> has now closed, open your myticket page to see more details about the ticket";
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                try
                {
                    smtp.Connect(_configuration.GetSection("MailSettings:Host").Value, int.Parse(_configuration.GetSection("MailSettings:Port").Value), SecureSocketOptions.None);
                    smtp.Authenticate(_configuration.GetSection("MailSettings:From").Value, _configuration.GetSection("MailSettings:Password").Value);
                    smtp.Send(email);
                }
                finally
                {
                    smtp.Disconnect(true);
                }

                return new ApiResponse(true, null);
            }

            catch (Exception er)
            {
                return new ApiResponse(false, er.Message);
            }
        }
        List<NodesViewModel> GetChildrenNodes(int parentId, int linkId)
        {
            List<NodesViewModel> childrenNodes = new List<NodesViewModel>();

            var childNodes = _context.T_WF_NODES
                .Where(x => x.ParentId == parentId && x.LinkId == linkId)
                .ToList();


            foreach (var itemc in childNodes)
            {
                List<OptionsBinding> optionsBindings = new List<OptionsBinding>();
                List<string> Value = new List<string>();

                var options = _context.T_WF_ACTION_OPTIONS.Where(x => x.NodeId == itemc.Id).ToList();

                foreach (var item in options)
                {
                    Value = new List<string>();

                    OptionsBinding optionsBinding = new OptionsBinding()
                    {
                        FormElementId = item.FormElementId,
                        Relation = item.Relation,
                        Value = new List<string>()  // Initialize the Value property
                    };

                    Value.Add(item.Value);
                    optionsBinding.Value.AddRange(Value);
                    optionsBindings.Add(optionsBinding);
                }

                NodesViewModel nodesViewModel = new NodesViewModel()
                {
                    Id = itemc.Id,
                    ParentId = itemc.ParentId,
                    Relation = itemc.Relation,
                    Options = optionsBindings,
                    Children = GetChildrenNodes(itemc.Id, linkId)
                };
                childrenNodes.Add(nodesViewModel);
            }

            return childrenNodes;
        }
        public bool ProcessSingleNode(NodesViewModel node, List<T_WF_TASK_VALUE> tasksvalues)
        {
            List<bool> result = new List<bool>();
            if (node.Options != null)
            {

                result.Add(GetResultFromOptions(node.Options, tasksvalues, node.Relation));

            }
            if (node.Children != null)
            {
                foreach (var itemchild in node.Children)
                {
                    result.Add(ProcessSingleNode(itemchild, tasksvalues));
                }

            }
            if (node.Relation == Relation_Node_Enum.OR)
            {
                if (result.Any(x => x == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (node.Relation == Relation_Node_Enum.AND)
            {
                if (result.All(x => x == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        bool GetResultFromOptions(List<OptionsBinding> options, List<T_WF_TASK_VALUE> tasksvalues, Relation_Node_Enum? Relation)
        {
            List<bool> Results = new List<bool>(); ;

            foreach (var item in options)
            {

                var result = CompareActionOptionAndTaskValue(item, tasksvalues.Where(x => x.FormElementId == item.FormElementId).ToList());
                Results.Add(result);
            }

            if (Relation == Relation_Node_Enum.OR)
            {
                if (Results.Any(x => x == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Relation == Relation_Node_Enum.AND)
            {
                if (Results.All(x => x == true))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool CompareActionOptionAndTaskValue(OptionsBinding actionOption, List<T_WF_TASK_VALUE> tasksvalues)
        {
            Relation_Option_Enum relation = actionOption.Relation;
            var formelement = _context.T_WF_FORM_ELEMENTS.FirstOrDefault(x => x.Id == actionOption.FormElementId);
            if (formelement != null)
            {
                if (formelement.Name != "Select Menu")
                {
                    var taskvalue = tasksvalues.FirstOrDefault(x => x.FormElementId == actionOption.FormElementId);
                    if (taskvalue != null)
                    {
                        switch (relation)
                        {
                            case Relation_Option_Enum.greater:
                                return Convert.ToInt64(taskvalue.Value) > Convert.ToInt64(actionOption.Value);
                            case Relation_Option_Enum.Less:
                                return Convert.ToInt64(taskvalue.Value) < Convert.ToInt64(actionOption.Value);
                            case Relation_Option_Enum.greaterhanorequleto:
                                return Convert.ToInt64(taskvalue.Value) >= Convert.ToInt64(actionOption.Value);
                            case Relation_Option_Enum.lessthanorequleto:
                                return Convert.ToInt64(taskvalue.Value) <= Convert.ToInt64(actionOption.Value);
                            case Relation_Option_Enum.equals:
                                return taskvalue.Value == actionOption.Value.FirstOrDefault();
                            case Relation_Option_Enum.notequals:
                                return taskvalue.Value != actionOption.Value.FirstOrDefault();
                            case Relation_Option_Enum.StartWith:
                                return taskvalue.Value.StartsWith(actionOption.Value.FirstOrDefault());
                            case Relation_Option_Enum.EndWith:
                                return taskvalue.Value.EndsWith(actionOption.Value.FirstOrDefault());

                            default:
                                return false;
                        }
                    }
                }
                else
                {
                    var taskvalues = tasksvalues.Where(x => x.FormElementId == actionOption.FormElementId).ToList();

                    if (taskvalues.Any())
                    {
                        switch (relation)
                        {
                            case Relation_Option_Enum.Is:
                                return taskvalues.All(taskvalue => actionOption.Value.All(av => taskvalue.Value.Contains(av)));
                            case Relation_Option_Enum.notIs:
                                return taskvalues.All(taskvalue => actionOption.Value.All(av => !taskvalue.Value.Contains(av)));
                            case Relation_Option_Enum.Include:
                                return taskvalues.Any(taskvalue => actionOption.Value.All(av => taskvalue.Value.Contains(av)));
                            case Relation_Option_Enum.notInclude:
                                return taskvalues.Any(taskvalue => actionOption.Value.All(av => !taskvalue.Value.Contains(av)));
                            default:
                                return false;
                        }
                    }

                }
            }
            return true;
        }
        public string GetEmailByUserId(int? UserId)
        {
            try
            {
                var UserInfo = _context.TLIuser.FirstOrDefault(x => x.Id == UserId);
                if (UserInfo != null)
                {
                    return UserInfo.Email;
                }
                else
                {
                    return "This UserId Not Found";
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        public string GetNameByUserId(int? UserId)
        {
            try
            {
                var UserInfo = _context.TLIuser.FirstOrDefault(x => x.Id == UserId);
                if (UserInfo != null)
                {
                    return UserInfo.UserName;
                }
                else
                {
                    return "This UserId Not Found";
                }
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }
}
