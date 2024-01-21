using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.AllCivilInstDTOs
{
    public class CivilNonSteelView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SITECODE { get; set; }
        public string INPUTVALUE { get; set; }
        public string Key { get; set; }
        public double CurrentLoads { get; set; }
        public float SpaceInstallation { get; set; }
        public int CIVILNONSTEELLIBRARY { get; set; }
        public int? OWNER { get; set; }
        public int? SUPPORTTYPEIMPLEMENTED { get; set; }
        public int? LOCATIONTYPE { get; set; }
        public float locationHeight { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float CivilSupportCurrentLoad { get; set; }
        public float H2Height { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public float Support_Limited_Load { get; set; }

    }
}
