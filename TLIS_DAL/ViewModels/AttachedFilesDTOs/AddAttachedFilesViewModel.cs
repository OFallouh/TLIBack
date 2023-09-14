using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttachedFilesDTOs
{
    public class AddAttachedFilesViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int RecordId { get; set; }
        public int tablesNamesId { get; set; }
        public bool IsImg { get; set; }
    }
}
