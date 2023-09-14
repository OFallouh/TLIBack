using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.ViewModels.OrderStatusDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.IntegrationDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.PartDTOs;
using TLIS_DAL.ViewModels.TicketTargetDTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class TicketService : ITicketService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IServiceCollection _services;
        private IMapper _mapper;
        List<int> userGroups = new List<int>();
        List<int> userActors = new List<int>();
        List<MailboxAddress> MailList = new List<MailboxAddress>();
        List<int> mailRecipient = new List<int>();
        public TicketService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //-------------------------------------------  Ticket

        /// <summary>
        /// return all Tickets that their deleted flag is false
        /// </summary>
        public Response<List<ListTicketViewModel>> GetAllTickets(ParameterPagination parameterPagination, List<FilterObjectList> filter)
        {
            int count = 0;
            try
            {
                var entity = _unitOfWork.TicketRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var model = _mapper.Map<List<ListTicketViewModel>>(entity);
                foreach (var item in model)
                {

                }
                return new Response<List<ListTicketViewModel>>(true, model, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListTicketViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }       
        /// <summary>
        /// Add a new Ticket
        /// </summary>
        public Response<ListTicketViewModel> AddTicket(IConfiguration configuration, AddTicketViewModel ticket,int? CreatorId, int? IntegrationId)
        {
            try
            {
                this._configuration = configuration;
                TLIticket entity = new TLIticket();
                entity.WorkFlowId = ticket.WorkFlowId;
                entity.TypeId = ticket.TypeId;
                entity.SiteCode = ticket.SiteCode;
                entity.CreatorId = CreatorId;
                entity.IntegrationId = IntegrationId;

                // save to DB
                _unitOfWork.TicketRepository.Add(entity);
                _unitOfWork.SaveChanges();


                int nextActionId;
                


                //TLIticketAction request = new TLIticketAction();
                //request.TicketId = entity.Id;

                // create first 
                List<TLIstepAction> all = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == entity.WorkFlowId).ToList();
                if (entity.TypeId!=null) // first request is based on type id
                {
                    //TLIworkFlowType type = _unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.entity.TypeId == Id && x.Deleted == false).SingleOrDefault();
                    TLIworkFlowType type = _unitOfWork.WorkFlowTypeRepository.GetByID((int) entity.TypeId);
                    //request.StepActionId = (int)type.nextStepActionId;
                    nextActionId = (int)type.nextStepActionId;
                }
                else // first request is the first step action (smallest seqence in this workflow)
                {
                    int minSequence = all.Min(x => x.sequence);
                    TLIstepAction firstAction = all.Where(x => x.sequence == minSequence).FirstOrDefault();
                    //request.StepActionId = (int)firstAction.Id;
                    nextActionId = (int)firstAction.Id;
                }
                //_unitOfWork.TicketActionRepository.Add(request);
                //_unitOfWork.SaveChanges();
                setNextRequest(entity.Id, nextActionId);
                /*
                // create task for this request
                TLIagenda agenda = new TLIagenda();
                agenda.TicketActionId = request.Id;
                //agenda.dateCreated = DateTime.Now;
                agenda.CreationDate = DateTime.Now;
                agenda.period = 1;
                _unitOfWork.AgendaRepository.Add(agenda);
                _unitOfWork.SaveChanges();
                if (agenda.AgendaGroups == null)
                {
                    agenda.AgendaGroups = new List<TLIagendaGroup>();
                }
                // Assign this request to groups
                // union of site, parts, files
                // site
                var groups = _unitOfWork.StepActionGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId);
                foreach(var g in groups)
                {
                    TLIagendaGroup group = new TLIagendaGroup();
                    group.ActorId = g.ActorId;
                    group.GroupId = g.GroupId;
                    group.IntegrationId = g.IntegrationId;
                    group.UserId = g.UserId;
                    group.AgendaId = agenda.Id;
                    agenda.AgendaGroups.Add(group);
                }
                // files
                var groups2 = _unitOfWork.StepActionFileGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId);
                foreach(var g in groups2)
                {
                    TLIagendaGroup group = new TLIagendaGroup();
                    group.ActorId = g.ActorId;
                    group.GroupId = g.GroupId;
                    group.IntegrationId = g.IntegrationId;
                    group.UserId = g.UserId;
                    group.AgendaId = agenda.Id;
                    agenda.AgendaGroups.Add(group);
                }
                //parts
                var parts = _unitOfWork.StepActionPartRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId);
                foreach (var p in parts)
                {
                    var groups3 = _unitOfWork.StepActionPartGroupRepository.GetAllAsQueryable().Where(x => x.StepActionPartId == p.Id);
                    foreach (var g in groups3)
                    {
                        TLIagendaGroup group = new TLIagendaGroup();
                        group.ActorId = g.ActorId;
                        group.GroupId = g.GroupId;
                        group.IntegrationId = g.IntegrationId;
                        group.UserId = g.UserId;
                        group.AgendaId = agenda.Id;
                        agenda.AgendaGroups.Add(group);
                    }
                }
                _unitOfWork.SaveChanges();
                ExecuteSystemRequest(request);
                //*/

                var model = _mapper.Map<ListTicketViewModel>(entity);
                return new Response<ListTicketViewModel>(true, model, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ListTicketViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public void getParentGroups(int groupId)
        {
            if (!userGroups.Contains(groupId))
            {
                //*  
                // group isn't herediting from parent group so this code has been hashed
                var parentGroup = _unitOfWork.GroupRepository.GetByID(groupId);
                if (parentGroup != null && parentGroup.ParentId != null)
                {
                    getParentGroups((int)parentGroup.ParentId);
                }
                //*/
                userGroups.Add(groupId);
            }
        }
        public int? getGroupActor(int groupId)
        {
            var group = _unitOfWork.GroupRepository.GetAllAsQueryable().Where(v => v.Id == groupId).SingleOrDefault();
            if (group != null)
            {
                if (group.ActorId != null)
                {
                    return group.ActorId;
                }
                else
                {
                    if (group.ParentId != null)
                    {
                        return getGroupActor((int)group.ParentId);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public static int getUserId(int? userId)
        {
            // userid from session
            if (userId != null) return (int)userId;
            return 83;
        }

        /*
        public Response<ListTicketActionViewModel> ExecuteRequeste()
        {

        }
        //*/
        public void fillUserGroupsActors(int UserID)
        {
            // get user groups
            userGroups.Clear();
            var groupEntities = _unitOfWork.GroupUserRepository.GetAllAsQueryable().Where(v => v.userId == UserID).ToList();
            foreach (var groupEntity in groupEntities)
            {
                getParentGroups(groupEntity.groupId);
            }
            // get user actors
            userActors.Clear();
            foreach (var group in userGroups)
            {
                int? actor = getGroupActor(group);
                if (actor != null && !userActors.Contains((int)actor))
                {
                    userActors.Add((int)actor);
                }
            }

        }

        public Response<List<PendingRequestViewModel>> GetPendingRequestes(int? UserID)
        {
            UserID = getUserId(UserID);
            if (UserID != null)
            {
                fillUserGroupsActors((int)UserID);
            }
            var ticketActionEnitities = _unitOfWork.TicketActionRepository.GetAllAsQueryable().Where(w => w.ExecutionDate == null).ToList();
            int x = ticketActionEnitities.Count();
            for(int i = 0; i < x; i++)
            {
                var ticketActionEntity = ticketActionEnitities.ElementAt(i);
                if (ticketActionEntity.AssignedToId == UserID) // assigned to this user
                {
                    continue; // keep this ticket Action included in pending pending requestes
                }
                var agenda = _unitOfWork.AgendaRepository.GetAllAsQueryable().Where(y => y.TicketActionId == ticketActionEntity.Id).ToList();
                bool notAssigned = false;
                foreach (var a in agenda)
                {
                    bool break1 = false;
                    var agendaGroupsEntities = _unitOfWork.AgendaGroupRepository.GetAllAsQueryable().Where(z => z.AgendaId == a.Id).ToList();
                    foreach(var agendaGroup in agendaGroupsEntities)
                    {
                        if (agendaGroup.UserId != null && agendaGroup.UserId == UserID) // current userId
                        {
                            break1 = true;
                            notAssigned = true;
                            break;
                        }
                        if (agendaGroup.GroupId != null && userGroups.Contains((int)agendaGroup.GroupId))
                        {
                            break1 = true;
                            notAssigned = true;
                            break;
                        }
                        if (agendaGroup.ActorId != null && userActors.Contains((int)agendaGroup.ActorId))
                        {
                            break1 = true;
                            notAssigned = true;
                            break;
                        }
                    }
                    if (break1)
                    {
                        break;
                    }
                }
                if (!notAssigned) // not assigned request to me
                {
                    // remove current request from my pending requests
                    ticketActionEnitities.RemoveAt(i);
                    x--;
                }
            }
            List<PendingRequestViewModel> res = new List<PendingRequestViewModel>();
            foreach (var ticketAction in ticketActionEnitities)
            {
                PendingRequestViewModel pr = new PendingRequestViewModel();
                pr.Id = ticketAction.Id;
                pr.AssignedToId = ticketAction.AssignedToId;
                var ticketEntity = _unitOfWork.TicketRepository.GetByID(ticketAction.TicketId);
                if (ticketEntity.CreatorId != null)
                {
                    var userEntity = _unitOfWork.UserRepository.GetByID((int)ticketEntity.CreatorId);
                    pr.Requester = userEntity.FirstName + " " + userEntity.LastName;
                }
                var workflowEntity = _unitOfWork.WorkFlowRepository.GetByID(ticketEntity.WorkFlowId);
                pr.WorkFlowName = workflowEntity.Name;
                if (ticketEntity.TypeId != null)
                {
                    var workflowTypeEntity = _unitOfWork.WorkFlowTypeRepository.GetByID((int)ticketEntity.TypeId);
                    pr.WorkFlowType = workflowTypeEntity.Name;
                }
                pr.Id = ticketAction.Id;
                var siteEntity = _unitOfWork.SiteRepository.GetByID(ticketEntity.SiteCode);
                if (siteEntity != null)
                {
                    pr.Sitecode = siteEntity.SiteCode;
                    pr.SiteName = siteEntity.SiteName;
                }
                var stepActionEntity = _unitOfWork.StepActionRepository.GetByID(ticketAction.StepActionId);
                pr.NeededAction = stepActionEntity.label;
                pr.TicketCreationDate = ticketEntity.CreationDate;
                var agendaEntity = _unitOfWork.AgendaRepository.GetAllAsQueryable().Where(a => a.TicketActionId == ticketAction.Id).FirstOrDefault();
                if (agendaEntity != null)
                {
                    pr.Custody = new StepActionGroupsViewModel();
                    pr.RequestCreationDate = agendaEntity.CreationDate;
                    var agendaGroupEntity = _unitOfWork.AgendaGroupRepository.GetAllAsQueryable().Where(q => q.AgendaId == agendaEntity.Id).ToList();
                    foreach (var act in agendaGroupEntity)
                    {

                        if (act.UserId != null)
                        {
                            if (pr.Custody.users == null)
                            {
                                pr.Custody.users = new List<UserViewModel>();
                            }
                            pr.Custody.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)act.UserId)));
                        }
                        if (act.GroupId != null)
                        {
                            if (pr.Custody.groups == null)
                            {
                                pr.Custody.groups = new List<GroupViewModel>();
                            }
                            pr.Custody.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)act.GroupId)));
                        }
                        if (act.ActorId != null)
                        {
                            if (pr.Custody.actors == null)
                            {
                                pr.Custody.actors = new List<ActorViewModel>();
                            }
                            pr.Custody.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)act.ActorId)));
                        }
                        if (act.IntegrationId != null)
                        {
                            if (pr.Custody.integration == null)
                            {
                                pr.Custody.integration = new List<IntegrationViewModel>();
                            }
                            pr.Custody.integration.Add(_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId)));
                        }
                    }
                }
                if (ticketEntity.StatusId != null)
                {
                    var orderstatusEntity = _unitOfWork.OrderStatusListRepository.GetByID((int)ticketEntity.StatusId);
                    pr.TicketStatus = _mapper.Map<OrderStatusViewModel>(orderstatusEntity);
                }
                res.Add(pr);
            }
            // _mapper.Map<List<TicketActionViewModel>>(ticketActionEnitities)
            foreach(var ticketAction in ticketActionEnitities)
            {

            }
            return new Response<List<PendingRequestViewModel>>(true,res, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
        }

        public void ExecuteSystemRequest(TLIticketAction request, TLIagenda agenda) {
            // system Execution
            TLIstepAction stepAction = _unitOfWork.StepActionRepository.GetByID(request.StepActionId);
            List<int> nextAction;
            switch (stepAction.type)
            {
                case ActionType.Email:
                    mailRecipient.Clear();
                    //Get sender account thgen sender mail
                    if (stepAction.StepActionMailFromId == null)
                    {
                        throw new Exception("from mail couldnot be null");
                    }
                    var from = _unitOfWork.StepActionMailFromRepository.GetByID((int)stepAction.StepActionMailFromId);
                    if (from == null)
                    {
                        throw new Exception("from mail couldnot be unexist item");
                    }
                    string email = "hazem.maatok@intdatasys.com"; // sender mail as static (coomented code get sender mail from DB)
                    string name = "";
                    /*
                    if (from.ActorId != null)
                    {
                        // get email settings
                    }
                    if (from.GroupId != null)
                    {
                        // get email settings
                        MailList.Clear();
                        GetGroupMail((int)from.GroupId);
                        if (MailList.Count() > 0)
                        {
                            email = MailList[0];
                            var grp = _unitOfWork.GroupRepository.GetByID((int)from.GroupId);
                            if (grp != null)
                            {
                                name = grp.Name;
                            }

                        }
                    }
                    if (from.UserId != null)
                    {
                        // get email settings
                        var user = _unitOfWork.UserRepository.GetByID((int)from.UserId);
                        if (user == null)
                        {
                            throw new Exception("from mail couldnot be unvalid user");
                        }
                        email = user.Email;
                        name = user.FirstName + " " + user.LastName;
                    }
                    if (from.ActorId == null && from.GroupId == null && from.UserId == null)
                    {
                        throw new Exception("from mail couldnot be contains nothing");
                    }
                    if (email == null)
                    {
                        throw new Exception("from mail couldnot be empty");
                    }
                    //*/
                    string sender_user = "";
                    string sender_pass = "";
                    string sender_server = "";
                    int sender_port = 25;
                    bool sender_TLS = false;
                    try
                    {
                        sender_user = _configuration["SMTP:" + email+ ":user"];
                        sender_pass = _configuration["SMTP:" + email+ ":pass"];
                        sender_server = _configuration["SMTP:" + email+ ":server"];
                        sender_port = Int32.Parse(_configuration["SMTP:" + email + ":port"]);
                        sender_TLS = bool.Parse(_configuration["SMTP:" + email + ":TLS"]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("from mail couldnot be unconfiger on appsettings.json", ex);
                    }


                    MimeMessage message = new MimeMessage();

                    MailboxAddress fromMail = new MailboxAddress(name, sender_user);
                    message.From.Add(fromMail);

                    // get recipient mails
                    var mailsTo = _unitOfWork.StepActionMailToRepository.GetAllAsQueryable().Where(x => x.StepActionId == stepAction.Id).ToList();
                    if(mailsTo==null || mailsTo.Count == 0)
                    {
                        throw new Exception("to mails list couldnot be empty");
                    }
                    foreach ( var recipient in mailsTo)
                    {
                        if (recipient.ActorId != null)
                        {
                            // get email settings
                        }
                        if (recipient.GroupId != null)
                        {
                            // get email settings
                            MailList.Clear();
                            GetGroupMail((int)recipient.GroupId);
                            foreach (var mail in MailList)
                            {
                                if (mail != null)
                                {
                                    message.To.Add(mail);
                                }
                            }
                        }
                        if (recipient.UserId != null)
                        {
                            var mail = getMailAddress((int)recipient.UserId);
                            if (mail != null)
                            {
                                message.To.Add(mail);
                            }
                        }
                        if (recipient.ActorId == null && recipient.GroupId == null && recipient.UserId == null)
                        {
                            throw new Exception("to mail couldnot be contains nothing");
                        }
                    }

                    message.Subject = stepAction.MailSubject;

                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = stepAction.MailBody;
                    bodyBuilder.TextBody = stepAction.MailBody;

                    message.Body = bodyBuilder.ToMessageBody();

                    SmtpClient client = new SmtpClient();
                    client.ServerCertificateValidationCallback = RemoteServerCertificateValidationCallback;
                    ServicePointManager.ServerCertificateValidationCallback = RemoteServerCertificateValidationCallback;
                    client.Connect(sender_server, sender_port, sender_TLS);
                    client.Authenticate(sender_user, sender_pass);
                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();
                    // get the next step action id
                    nextAction = getNextStepActionId(stepAction, null, null);
                    break;
                case ActionType.CheckAvailableSpace:
                    bool CheckAvailableSpace = true;// based on formula
                    //get option id
                    int? optionId = null;
                    int? requestOptionId = null;
                    var act = _mapper.Map<List<ListConditionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.CheckAvailableSpace && x.Deleted == false))).FirstOrDefault();
                    if (act != null)
                    {
                        var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                        foreach (var a in ActionOptions)
                        {
                            if (CheckAvailableSpace) // not contains not
                            {
                                if (!a.Name.Contains("Not"))
                                {
                                    optionId = a.Id;
                                }
                            }
                            else // contains not
                            {
                                if (a.Name.Contains("Not"))
                                {
                                    optionId = a.Id;
                                }

                            }
                        }
                        if (optionId != null)
                        {
                            var option = _unitOfWork.StepActionOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId && x.ActionOptionId == optionId).FirstOrDefault();
                            requestOptionId = option.Id;

                        }
                    }
                    // get the next step action id
                    nextAction = getNextStepActionId(stepAction, null, requestOptionId);
                    break;
                case ActionType.TicketStatus:
                    //request.TicketId
                    var ticket = _unitOfWork.TicketRepository.GetByID(request.TicketId);
                    if (ticket != null)
                    {
                        ticket.StatusId = stepAction.OrderStatusId;
                    }
                    _unitOfWork.TicketRepository.Update(ticket);
                    _unitOfWork.SaveChanges();
                    nextAction = getNextStepActionId(stepAction, null, null);
                    break;
                case ActionType.AppyCalculation:
                    // calculate spaces based on formula and save the result
                    nextAction = getNextStepActionId(stepAction, null, null);
                    break;
                default:
                    return;
                    break;
            }
            // set ticket action as executed
            agenda.ExecuterId = null;
            agenda.ExecutionDate = DateTime.Now;
            _unitOfWork.AgendaRepository.Update(agenda);
            request.ExecutionDate = agenda.ExecutionDate;
            request.ExecuterId = agenda.ExecuterId;
            _unitOfWork.TicketActionRepository.Update(request);
            _unitOfWork.SaveChanges();
            // insert the next action

            if (nextAction != null && nextAction.Count > 0)
            {
                foreach (int next in nextAction)
                {
                    setNextRequest(request.TicketId, next);
                }
            }
            else // it was the final step action
            {
                finishedTicket(request.TicketId);
            }
        }

        public void setRequestAsExecuted(int requestId, int? UserId)
        {
            var agendGroup = _unitOfWork.AgendaRepository.GetAllAsQueryable().Where(x => x.TicketActionId == requestId).SingleOrDefault();
            if (agendGroup != null)
            {
                agendGroup.ExecuterId = UserId;
                agendGroup.ExecutionDate = DateTime.Now;
                _unitOfWork.AgendaRepository.Update(agendGroup);
                _unitOfWork.SaveChanges();
            }

        }

        public void finishedTicket(int ticketId)
        {
            var ticketActions= _unitOfWork.TicketActionRepository.GetAllAsQueryable().Where(x => x.TicketId == ticketId).ToList();
            foreach ( var act in ticketActions)
            {
                var agendGroup = _unitOfWork.AgendaRepository.GetAllAsQueryable().Where(x => x.TicketActionId == act.Id && x.ExecutionDate == null).SingleOrDefault();
                if (agendGroup!=null)
                {
                    return;
                }
            }
            var status = _unitOfWork.OrderStatusListRepository.GetAllAsQueryable().Where(x => x.IsFinish == true).ToList();
            if (status != null && status.Count > 0)
            {
                var ticket = _unitOfWork.TicketRepository.GetByID(ticketId);
                ticket.StatusId = status.ElementAt(0).Id;
                _unitOfWork.TicketRepository.Update(ticket);
                _unitOfWork.SaveChanges();
            }
        }

        public bool setNextRequest(int TicketId, int nextActionId)
        {
            try
            {
                TLIticketAction request = new TLIticketAction();
                request.TicketId = TicketId;
                request.StepActionId = nextActionId;
                _unitOfWork.TicketActionRepository.Add(request);
                _unitOfWork.SaveChanges();

                // create task for this request
                TLIagenda agenda = new TLIagenda();
                agenda.TicketActionId = request.Id;
                //agenda.dateCreated = DateTime.Now;
                agenda.CreationDate = DateTime.Now;
                agenda.period = 1;
                agenda.ExecuterId =null;
                _unitOfWork.AgendaRepository.Add(agenda);
                _unitOfWork.SaveChanges();
                if (agenda.AgendaGroups == null)
                {
                    agenda.AgendaGroups = new List<TLIagendaGroup>();
                }
                // Assign this request to groups
                // union of site, parts, files
                // site
                var groups = _unitOfWork.StepActionGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId);
                if (groups != null)
                    foreach (var g in groups)
                    {
                        TLIagendaGroup group = new TLIagendaGroup();
                        group.ActorId = g.ActorId;
                        group.GroupId = g.GroupId;
                        group.IntegrationId = g.IntegrationId;
                        group.UserId = g.UserId;
                        group.AgendaId = agenda.Id;
                        agenda.AgendaGroups.Add(group);
                    }
                // files
                var groups2 = _unitOfWork.StepActionFileGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId);
                if (groups2 != null)
                    foreach (var g in groups2)
                    {
                        TLIagendaGroup group = new TLIagendaGroup();
                        group.ActorId = g.ActorId;
                        group.GroupId = g.GroupId;
                        group.IntegrationId = g.IntegrationId;
                        group.UserId = g.UserId;
                        group.AgendaId = agenda.Id;
                        agenda.AgendaGroups.Add(group);
                    }
                //parts
                var parts = _unitOfWork.StepActionPartRepository.GetAllAsQueryable().Where(x => x.StepActionId == request.StepActionId).ToList();
                foreach (var p in parts)
                {
                    var groups3 = _unitOfWork.StepActionPartGroupRepository.GetAllAsQueryable().Where(x => x.StepActionPartId == p.Id).ToList();
                    if (groups3 != null)
                        foreach (var g in groups3)
                        {
                            TLIagendaGroup group = new TLIagendaGroup();
                            group.ActorId = g.ActorId;
                            group.GroupId = g.GroupId;
                            group.IntegrationId = g.IntegrationId;
                            group.UserId = g.UserId;
                            group.AgendaId = agenda.Id;
                            agenda.AgendaGroups.Add(group);
                        }
                }
                _unitOfWork.SaveChanges();
                ExecuteSystemRequest(request, agenda);
            }
            catch (Exception err)
            {
                
                throw new Exception("setNextRequest: there is an error", err);
            }
            return false;
        }

        public List<int> getNextStepActionId(TLIstepAction stepAction, int? itemOption, int? actionOption)
        {
            if (itemOption != null) // getting the next step action id is based on chosen item option 
            {
                TLIstepActionItemOption option = _unitOfWork.StepActionItemOptionRepository.GetByID((int)itemOption);
                if (option != null && option.NextStepActions != null )
                {
                    return _mapper.Map<List<int>>(option.NextStepActions);
                }
            }
            if (actionOption != null) // getting the next step action id is based on selected action option
            {
                var option = _unitOfWork.StepActionOptionRepository.GetByID((int)actionOption);
                if (option != null && option.NextStepActions != null)
                {
                    return _mapper.Map<List<int>>(option.NextStepActions);
                }
            }
            if (stepAction != null)
            {
                if (stepAction.NextStepActions != null) // predefined next action is the next action
                {
                    return _mapper.Map<List<int>>(stepAction.NextStepActions);
                }
                else // next action in sequence is the next action (in case of there is a next action in sequence)
                {
                    var listNextStepActions = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == stepAction.WorkflowId && x.sequence > stepAction.sequence).ToList();
                    if (listNextStepActions != null && listNextStepActions.Count > 0)
                    {
                        int minSeq = listNextStepActions.Min(x => x.sequence);
                        var nextStepAction = listNextStepActions.Where(x => x.sequence == minSeq).ToList();
                        if (nextStepAction != null)
                        {
                            List<int> nextActions = new List<int>();
                            foreach(var next in nextStepAction)
                            {
                                nextActions.Add(next.Id);
                            }
                            return nextActions;
                        }
                    }
                }
            }
            return null; // there is no next action so it is the final action
        }


        public static bool RemoteServerCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public MailboxAddress getMailAddress(int userId)
        {
            if (!mailRecipient.Contains(userId))
            {
                var user = _unitOfWork.UserRepository.GetByID(userId);
                if (user != null)
                {
                    mailRecipient.Add(userId);
                    return new MailboxAddress(user.FirstName + " " + user.LastName, user.Email);
                }
            }
            return null;
        }

    public void GetGroupMail(int GroupId)
        {
            var children = _unitOfWork.GroupRepository.GetAllAsQueryable().Where(x => x.ParentId == GroupId).ToList();
            if (children != null)
            {
                foreach (var child in children)
                {
                    GetGroupMail(child.Id);
                }
            }
            var users = _unitOfWork.GroupUserRepository.GetAllAsQueryable().Where(x => x.groupId == GroupId).ToList();
            if (users != null)
            {
                foreach (var user in users)
                {
                    var mail = getMailAddress(user.userId);
                    if (mail != null)
                    {
                        MailList.Add(mail);
                    }
                }
            }

        }


        public Response<TicketActinDetailsViewModel> GetTicketActionById(int Id, int? UserID)
        {
            TicketActinDetailsViewModel res = new TicketActinDetailsViewModel();
            var ticketAction = _unitOfWork.TicketActionRepository.GetByID(Id);
            if (ticketAction != null)
            {
                var ticket = _unitOfWork.TicketRepository.GetByID(ticketAction.TicketId);
                WorkflowService wfService = new WorkflowService(_unitOfWork, _services,_mapper);
                var sa = wfService.StepActionWithNames(wfService.GetOneStepAction(ticketAction.StepActionId));
                res.Id = ticketAction.Id;
                res.AllowNote = sa.AllowNote;
                res.AllowUploadFile = sa.AllowUploadFile;
                res.IncomItemStatus = sa.IncomItemStatus;
                res.InputMode = sa.InputMode;
                res.label = sa.label;
                res.NoteIsMandatory = sa.NoteIsMandatory;
                res.Operation = sa.Operation;
                res.OutputMode = sa.OutputMode;
                res.SiteCode = ticket.SiteCode;
                if (res.Terget == null)
                {
                    res.Terget = new List<TicketTargetItemViewModel>();
                }
                //*
                var targets = _unitOfWork.TicketTargetRepository.GetAllAsQueryable().Where(x => x.TicketId == ticket.Id).ToList();
                if (targets != null)
                {
                    foreach (var target in targets)
                    {
                        TicketTargetItemViewModel t = new TicketTargetItemViewModel();
                        t.TargetTable = target.TargetTable;
                        t.TableId = target.TableId;
                        res.Terget.Add(t);
                    }
                }
                //*/
                if (res.PreviouseNotes == null)
                {
                    res.PreviouseNotes = new List<string>();
                }
                //*
                var notes = _unitOfWork.TicketOptionNoteRepository.GetAllAsQueryable().Where(x => x.TicketId == ticket.Id).OrderBy(x => x.Id).ToList();
                if (notes != null)
                {
                    foreach (var note in notes)
                    {
                        res.PreviouseNotes.Add(note.Note);
                    }
                }
                //*/
                if (sa.StepActionItemOption != null)
                {
                    if (res.StepActionItemOption == null)
                    {
                        res.StepActionItemOption = new List<ListPartViewModel>();
                    }
                    foreach (var io in sa.StepActionItemOption)
                    {
                        /*
                        var itemOption = new ListPartViewModel();
                        itemOption.Id = io.Id;
                        itemOption.Name = io.ActionItemOptionName;
                        res.StepActionItemOption.Add(itemOption);
                        //*/
                    }
                }
                if (sa.StepActionOption != null)
                {
                    if (res.StepActionOption == null)
                    {
                        res.StepActionOption = new List<ListPartViewModel>();
                    }
                    foreach (var io in sa.StepActionOption)
                    {
                        var itemOption = new ListPartViewModel();
                        itemOption.Id = io.Id;
                        itemOption.Name = io.ActionOptionName;
                        res.StepActionOption.Add(itemOption);
                    }
                }
                if (sa.StepActionPart != null)
                {
                    if (res.StepActionPart == null)
                    {
                        res.StepActionPart = new List<ListPartViewModel>();
                    }
                    foreach (var io in sa.StepActionPart)
                    {
                        /*
                        var itemOption = new ListPartViewModel();
                        itemOption.Id = io.Id;
                        itemOption.Name = io.PartName;
                        res.StepActionOption.Add(itemOption);
                        //*/
                    }
                }
                res.TicketId = ticketAction.TicketId;
                if (ticket.StatusId != null)
                {
                    var orderStatus = _unitOfWork.OrderStatusListRepository.GetByID((int)ticket.StatusId);
                    res.TicketStatus = orderStatus.Name;
                }
                res.type = sa.type;
                res.UploadFileIsMandatory = sa.UploadFileIsMandatory;
                if (res.AllowUploadFile)
                {
                    // check if this user is not blong to groups who allowed to upload files 
                    bool userNotBelongToFileGroups = true;
                    if (UserID != null) {
                        fillUserGroupsActors((int)UserID);
                    }
                    if (userNotBelongToFileGroups)
                    {
                        res.AllowUploadFile = false;
                        res.UploadFileIsMandatory = false;
                    }
                }
                return new Response<TicketActinDetailsViewModel>(true, res, null, "there is no ticker action with this id: " + Id, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            return new Response<TicketActinDetailsViewModel>(true, null, null, "there is no ticker action with this id: " + Id, (int)Helpers.Constants.ApiReturnCode.fail);
        }


        public Response<TicketActinDetailsViewModel> ExecuteTicktRequeste(IConfiguration _configuration,TicketActinDetailsViewModel request, int ? userID)
        {
            int nextActionId = 0;
            this._configuration = _configuration;
            var TicketAction = _unitOfWork.TicketActionRepository.GetByID(request.Id);
            if (TicketAction == null)
            {
                return GetTicketActionById(request.Id, userID);

            }
            var StepAction = _unitOfWork.StepActionRepository.GetByID(TicketAction.StepActionId);
            if (StepAction == null)
            {
                return GetTicketActionById(request.Id, userID);
            }
            List<TLInextStepAction> NextStepActions = null;
            switch (StepAction.type)
            {
                case ActionType.InsertData:
                    if (!_unitOfWork.WorkflowHistoryRepository.IsExecutedAction(request.Id))
                    {
                        return GetTicketActionById(request.Id, userID);
                    }
                    break;
                case ActionType.UpdateData:
                    if (!_unitOfWork.WorkflowHistoryRepository.IsExecutedAction(request.Id))
                    {
                        return GetTicketActionById(request.Id, userID);
                    }
                    break;
                case ActionType.Correction:
                    if (!_unitOfWork.WorkflowHistoryRepository.IsExecutedAction(request.Id))
                    {
                        return GetTicketActionById(request.Id, userID);
                    }
                    break;
                case ActionType.TelecomValidation:
                    if (NextStepActions == null)
                    {
                        NextStepActions = new List<TLInextStepAction>();
                    }
                    if(true)
                    {
                        NextStepActions.Clear();
                        //*
                        // get all item status so item options so next actions
                        var items = _unitOfWork.WorkflowHistoryRepository.GetAllAsQueryable().Where(x => x.TicketActionId == request.Id).ToList();
                        List<int> itemStatus = new List<int>();
                        foreach (var item in items)
                        {
                            if (!itemStatus.Contains(item.ItemStatusId))
                            {
                                itemStatus.Add(item.ItemStatusId);
                            }
                        }
                        var StepActionItemOption = _unitOfWork.StepActionItemOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == StepAction.Id).ToList(); // && itemStatus.Contains(x.ActionItemOptionId)
                                                                                                                                                                        // check which of Item Options contains item status as output
                        List<int> next = new List<int>();
                        foreach (var stepActionItemOption in StepActionItemOption) //request.StepActionItemOption
                        {
                            var StepActionItemStatus = _unitOfWork.StepActionItemStatusRepository.GetAllAsQueryable().Where(x => x.StepActionItemOptionId == stepActionItemOption.Id && itemStatus.Contains(x.OutgoingItemStatusId)).ToList(); // && itemStatus.Contains(x.ActionItemOptionId)
                            if (StepActionItemStatus != null && StepActionItemStatus.Count > 0)
                            {
                                next.Add(stepActionItemOption.Id);
                            }
                        }
                        NextStepActions = _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => next.Contains((int)x.StepActionItemOptionId)).ToList();
                    }

                    break;
                case ActionType.CivilValidation:
                    if (StepAction.NextStepActions == null)
                    {
                        NextStepActions = new List<TLInextStepAction>();
                    }
                    if(true)
                    {
                        NextStepActions.Clear();
                        //*
                        // get all item status so item options so next actions
                        var items = _unitOfWork.WorkflowHistoryRepository.GetAllAsQueryable().Where(x => x.TicketActionId == request.Id).ToList();
                        List<int> itemStatus = new List<int>();
                        foreach (var item in items)
                        {
                            if (!itemStatus.Contains(item.ItemStatusId))
                            {
                                itemStatus.Add(item.ItemStatusId);
                            }
                        }
                        var StepActionItemOption = _unitOfWork.StepActionItemOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == StepAction.Id).ToList(); // && itemStatus.Contains(x.ActionItemOptionId)
                                                                                                                                                                        // check which of Item Options contains item status as output
                        List<int> next = new List<int>();
                        foreach (var stepActionItemOption in StepActionItemOption) //request.StepActionItemOption
                        {
                            var StepActionItemStatus = _unitOfWork.StepActionItemStatusRepository.GetAllAsQueryable().Where(x => x.StepActionItemOptionId == stepActionItemOption.Id && itemStatus.Contains(x.OutgoingItemStatusId)).ToList(); // && itemStatus.Contains(x.ActionItemOptionId)
                            if (StepActionItemStatus != null && StepActionItemStatus.Count > 0)
                            {
                                next.Add(stepActionItemOption.Id);
                            }
                        }
                        NextStepActions = _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => next.Contains((int)x.StepActionItemOptionId)).ToList();
                    }
                    break;
                case ActionType.SelectTargetSupport:
                    if (request.Terget != null && request.Terget.Count > 0)
                    {
                        foreach(var target in request.Terget)
                        {
                            TLIticketTarget t = new TLIticketTarget();
                            t.TicketId = TicketAction.TicketId;
                            t.TargetTable = target.TargetTable;
                            t.TableId = target.TableId;
                            _unitOfWork.TicketTargetRepository.Add(t);
                        }
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return GetTicketActionById(request.Id, userID);
                    }
                    break;
                case ActionType.CivilDecision:
                case ActionType.Condition:
                case ActionType.ProposalApproved:
                    if (request.SelectedOption != null)
                    {
                        TLIstepActionOption option = _unitOfWork.StepActionOptionRepository.GetByID((int)request.SelectedOption);
                        if (option != null)
                        {
                            //*
                            //change item status based on option
                            //*/
                            // change ticket status based on option
                            if (option.OrderStatusId != null)
                            {
                                var ticket = _unitOfWork.TicketRepository.GetByID(TicketAction.TicketId);
                                ticket.StatusId = option.OrderStatusId;
                                _unitOfWork.TicketRepository.Update(ticket);
                                _unitOfWork.SaveChanges();
                            }
                            // set next actions
                            NextStepActions = _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => x.StepActionOptionId == option.Id).ToList();
                            // set Note
                            if (request.ExecuterNote != null && request.ExecuterNote.Length > 0)
                            {
                                TLIticketOptionNote note = new TLIticketOptionNote();
                                note.Note = request.ExecuterNote;
                                note.StepActionOptionId = option.Id;
                                note.TicketActionId = request.Id;
                                note.TicketId = TicketAction.TicketId;
                                _unitOfWork.TicketOptionNoteRepository.Add(note);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else
                        {
                            return GetTicketActionById(request.Id, userID);
                        }
                    }
                    else
                    {
                        return GetTicketActionById(request.Id, userID);
                    }
                    break;
                case ActionType.StudyResult:
                    if (request.ExecuterNote != null && request.ExecuterNote.Length > 0)
                    {
                        TLIticketOptionNote note = new TLIticketOptionNote();
                        note.Note = request.ExecuterNote;
                        note.TicketActionId = request.Id;
                        note.TicketId = TicketAction.TicketId;
                        _unitOfWork.TicketOptionNoteRepository.Add(note);
                        _unitOfWork.SaveChanges();
                    }
                    break;
                /*

                case ActionType.InsertData:
                    break;
                    //*/
                default:
                    return GetTicketActionById(request.Id, userID);
                    break;

            }


            /*
            // set ticket action as executed
            agenda.ExecuterId = null;
            agenda.ExecutionDate = DateTime.Now;
            _unitOfWork.AgendaRepository.Update(agenda);
            request.ExecutionDate = agenda.ExecutionDate;
            request.ExecuterId = agenda.ExecuterId;
            _unitOfWork.TicketActionRepository.Update(request);
            _unitOfWork.SaveChanges();
            // insert the next action
            
            if (nextAction != null)
            {
                setNextRequest(request.TicketId, (int)nextAction);
            }
            else // it was the final step action
            {
                finishedTicket(request.TicketId);
            }

            //*/
            // set current ticket action as done
            var agenda = _unitOfWork.AgendaRepository.GetWhereFirst(x => x.TicketActionId==request.Id);
            agenda.ExecuterId = getUserId(userID);
            agenda.ExecutionDate = DateTime.Now;
            _unitOfWork.AgendaRepository.Update(agenda);
            TicketAction.ExecutionDate = agenda.ExecutionDate;
            TicketAction.ExecuterId = agenda.ExecuterId;
            _unitOfWork.TicketActionRepository.Update(TicketAction);
            _unitOfWork.SaveChanges();
            if (NextStepActions != null && NextStepActions.Count > 0)
            {
                foreach (var next in NextStepActions)
                {
                    nextActionId = next.NextStepActionId;
                    setNextRequest(TicketAction.TicketId, nextActionId);

                }
                var ta = _unitOfWork.TicketActionRepository.GetWhereFirst(x => x.TicketId == TicketAction.TicketId && x.StepActionId == nextActionId);
            
                return GetTicketActionById(ta.Id, userID);
            }
            else
            if (StepAction.NextStepActions != null && StepAction.NextStepActions.Count > 0)
            {
                foreach (var next in StepAction.NextStepActions)
                {
                    nextActionId = next.NextStepActionId;
                    setNextRequest(TicketAction.TicketId, nextActionId);

                }
                var ta = _unitOfWork.TicketActionRepository.GetWhereFirst(x => x.TicketId == TicketAction.TicketId && x.StepActionId == nextActionId);
            
                return GetTicketActionById(ta.Id, userID);
            }
            else
            {
                var entity = _unitOfWork.TicketRepository.GetByID(TicketAction.TicketId);
                if (entity != null)
                {
                    List<TLIstepAction> all = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == entity.WorkFlowId && x.sequence > StepAction.sequence).ToList();
                    if (all.Count > 0)
                    {
                        int minSequence = all.Min(x => x.sequence);
                        List<TLIstepAction> nextAction = all.Where(x => x.sequence == minSequence).ToList();
                        if (nextAction != null && nextAction.Count>0)
                        {
                            TLIticketAction ta=null;
                            foreach(var na  in nextAction)
                            {
                                nextActionId = (int)na.Id;
                                setNextRequest(request.TicketId, nextActionId);
                                ta = _unitOfWork.TicketActionRepository.GetWhereFirst(x => x.TicketId == request.TicketId && x.StepActionId == nextActionId);
                            }
                            if (ta != null)
                            {
                                return GetTicketActionById(ta.Id, userID);
                            }

                        }
                    }
                }

            }

            // in case of finished workflow
            finishedTicket(request.TicketId);
            return null;



        }



        //-------------------------------------------  End of Ticket


    }
}
