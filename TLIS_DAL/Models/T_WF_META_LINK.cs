using System.ComponentModel.DataAnnotations;

namespace WF_API.Model
{
    public class T_WF_META_LINK
    {
        [Key]
        public int Id { get; set; }
        public string URL { get; set; }
        public string URLActive { get; set; }
        public string Api { get; set; }
        public string Permission { get; set; }
        public int ActionId { get; set; }
        public virtual T_WF_ACTION Action { get; set; }

    }
}
