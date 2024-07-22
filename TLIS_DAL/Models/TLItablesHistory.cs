using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLItablesHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIhistoryType HistoryType { get; set; }
        [Required]
        public int HistoryTypeId { get; set; }
        public TLIuser User { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RecordId { get; set; }
        //[Required]
        //public string TableName { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public TLItablesNames TablesName { get; set; }
        public int TablesNameId { get; set; }
        public int? BaseRecordId { get; set; }
        public int? BaseTabeslNameId { get; set; }
        public TLItablesHistory PreviousHistory { get; set; }
        public int? PreviousHistoryId { get; set; }
        public IEnumerable<TLIhistoryDetails> historyDetails { get; set; }
    }
}
