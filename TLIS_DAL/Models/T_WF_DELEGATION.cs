using System;
using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_DELEGATION
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public virtual T_WF_TASK Task { get; set; }
        public int AssignFromId { get; set; }
        public int AssignToId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
