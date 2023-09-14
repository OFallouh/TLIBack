using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// options for each item
    /// </summary>
    public class TLIactionItemOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIaction")]
        public int ActionId { get; set; }
        public TLIaction Action { get; set; }
        public string Name { get; set; }
        public bool Deleted {get; set;}
        public DateTime? DeleteDate { get; set; }
        public List<TLIstepActionItemOption> StepActinItemOptions { get; set; }
    }
}