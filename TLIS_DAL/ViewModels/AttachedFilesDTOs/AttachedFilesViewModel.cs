using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttachedFilesDTOs
{
    public class AttachedFilesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int RecordId { get; set; }
        public int tablesNamesId { get; set; }
        public string TablesName { get; set; }
        public bool IsImg { get; set; }
        public float fileSize { get; set; }
        public bool UnAttached { get; set; }

    }
}
