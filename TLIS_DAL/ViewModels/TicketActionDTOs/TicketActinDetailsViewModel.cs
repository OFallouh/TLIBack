using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PartDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.TicketTargetDTOs;

namespace TLIS_DAL.ViewModels.TicketActionDTOs
{
    public class TicketActinDetailsViewModel
    {

        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SiteCode { get; set; }
        public ActionType type { get; set; }
        public string label { get; set; }
        public stepActionOutputMode OutputMode { get; set; }
        public stepActionInputMode InputMode { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool AllowNote { get; set; }
        public bool NoteIsMandatory { get; set; }
        public stepActionOperation? Operation { get; set; }
        public List<ListPartViewModel> StepActionPart { get; set; }
        public List<ListPartViewModel> StepActionOption { get; set; }
        public List<ListPartViewModel> StepActionItemOption { get; set; }
        public string TicketStatus { get; set; }
        public List<ListStepActionItemStatusWiNameViewModel> IncomItemStatus { get; set; }
        public List<TicketTargetItemViewModel> Terget { get; set; }
        public int? SelectedOption { get; set; }
        public string ExecuterNote { get; set; }
        public List<string> PreviouseNotes { get; set; }
    }
}
