using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WF_API.Model
{
    public enum Relation_Node_Enum
    {
       AND,
       OR,
    }
    public class T_WF_NODE
    {
        [Key]
        public int Id { get; set; }
        public Relation_Node_Enum? Relation { get; set; }
        public int LinkId { get; set; }
        public virtual T_WF_LINK Link { get; set; }
        public int? ParentId { get; set; }
        public virtual T_WF_NODE Parent { get; set; }
        public virtual ICollection<T_WF_ACTION_OPTION> ActionOptions { get; set; }
        public T_WF_NODE()
        { }
       
    }
}


