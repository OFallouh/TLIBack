using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    /// <summary>
    /// this option detarim the interface and backend processing
    /// </summary>
    /// ///
    public enum ActionType
    {
        API,                           //0
        Email,                         //1
        TicketStatus,                  //2
        UploadFile,                    //3
        InsertData,                    //4
        UpdateData,                    //5
        AppyCalculation,               //6
        Condition,                     //7
        SelectTargetSupport,           //8
        CheckAvailableSpace,           //9
        TelecomValidation,             //10
        CivilDecision,                 //11
        ProposalApproved,              //12
        StudyResult,                   //13
        CivilValidation,               //14
        Correction,
        Close
    }

    /// <summary>
    /// through this option we choose if this action allow employee to send a proposal
    /// </summary>
    public enum StepProposal
    {

        /// <summary>
        /// with this option, employee couldn't send  any proposal
        /// </summary>
        proposalNotAllowed,
        /// <summary>
        /// with this option, employee could send proposal
        /// </summary>
        allowProposal
    }



    /// <summary>
    /// template of request
    /// </summary>
    public class TLIaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public ActionType Type { get; set; }
        public StepProposal Proposal { get; set; }
        public List<TLIstepAction> StepActions { get; set; }
        public List<TLIactionOption> ActionOptions { get; set; }
        public List<TLIactionItemOption> ActionItemOptions { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }

    }
}