using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AllLoadInstDTOs
{
    public class AllLoadInstViewModel
    {
        public int Id { get; set; }
        public int? mwBUId { get; set; }
        public string mwBUName { get; set; }
        public int? mwDishId { get; set; }
        public string mwDishName { get; set; }
        public int? mwODUId { get; set; }
        public string mwODUName { get; set; }
        public int? mwRFUId { get; set; }
        public string mwRFUName { get; set; }
        public int? mwOtherId { get; set; }
        public string mwOtherName { get; set; }
        public int? radioAntennaId { get; set; }
        public string radioAntennaName { get; set; }
        public int? radioRRUId { get; set; }
        public string radioRRUName { get; set; }
        public int? radioOtherId { get; set; }
        public string radioOtherName { get; set; }
        public int? powerId { get; set; }
        public string powerName { get; set; }
        public int? loadOtherId { get; set; }
        public string loadOtherName { get; set; }
        public int? TicketId { get; set; }
        public int? ItemStatusId { get; set; }
    }
}
