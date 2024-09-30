using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class getLayersAndAttributesStaticAndDynamic
    {
        public List<layers> layers { get; set; }

        public IEnumerable<BaseInstAttViews> Operation { get; set; } = new List<BaseInstAttViews>();
    }
    public class layers
    {
   
        public string Label { get; set; }
        public InstalltionAttribute InstalltionAttributes { get; set; }
        public LibraryAttribute LibraryAttributes { get; set; }
      
    }
    public class InstalltionAttribute
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public item Items { get; set; }
    
    }
    public class LibraryAttribute
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public item Items { get; set; }

    }
    public class item
    {
        public IEnumerable<BaseInstAttViewDynamic> dyanmicAttributes { get; set; } = new List<BaseInstAttViewDynamic>();
        public IEnumerable<BaseInstAttViews> staticAttributes { get; set; } = new List<BaseInstAttViews>();
    }

}
