using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.wf;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class SumbitTaskByTLI
    {
        public ActionViewDto Result { get; set; }
        public int Id { get; set; }
        public object Exception { get; set; }
        public int Status { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCompletedSuccessfully { get; set; }
        public int CreationOptions { get; set; }
        public object AsyncState { get; set; }
        public bool IsFaulted { get; set; }
    }
    public class ActionViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Colspan { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int AssignToUserId { get; set; }
        public Task_Status_Enum Status { get; set; }
        public bool IsDraft { get; set; }
        public string Value { get; set; }
        public MetaLinkViewDto MetaLink { get; set; }
        public List<FormElementViewDto> FormElements { get; set; }
        public List<ReturnedValue> ReturnedValue { get; set; }

    }
    public enum Task_Status_Enum
    {
        Open,
        End,
        OnProgress,
        SubmitedByTLI
    }
    public class ReturnedValue
    {
        public string Label { get; set; }
        public string TaskValue { get; set; }
    }
    public class MetaLinkViewDto
    {
        public int Id { get; set; }
        public string Api { get; set; }
        public string URL { get; set; }
        public string URLActive { get; set; }
        public string Permission { get; set; }

    }
    public class FormElementViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ColSpan { get; set; }
        public string Label { get; set; }
        public string? Hint { get; set; }
        public List<string> Value { get; set; }
        public bool IsReturn { get; set; }
        public List<FormElementValidatorViewDto> VALIDATORS { get; set; }
        public List<FormElementChoicesViewDto> CHOICES { get; set; }
    }
    public class FormElementValidatorViewDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int? Value { get; set; }
        public string ErrorMessage { get; set; }
        public int FormElementId { get; set; }
        public string FormElement_Name { get; set; }
        
    }
    public class FormElementChoicesViewDto
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int FormElementId { get; set; }
        public string FormElement_Name { get; set; }
    }
}
