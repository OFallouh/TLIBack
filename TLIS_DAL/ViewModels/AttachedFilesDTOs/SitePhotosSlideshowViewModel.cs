using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttachedFilesDTOs
{
    public class SitePhotosSlideshowViewModel
    {
        
        public string Name { get; set; }
        public string Path { get; set; }
        public int RecordId { get; set; }
        public float fileSize { get; set; }
        public bool UnAttached { get; set; }
        public int tablesNamesId { get; set; }
        public string tablesNames_Name { get; set; }
        public int documenttypeId { get; set; }
        public string documenttype_Name { get; set; }

    }
}
