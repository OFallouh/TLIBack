using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WF_API.Model

{
    public enum Relation_Option_Enum
    {
        greater,
        Less,
        greaterhanorequleto,
        lessthanorequleto,
        equals,
        notequals,
        StartWith,
        EndWith,
        EqualTo,
        Include,
        notInclude,
        Is,
        notIs,

    }
    public class T_WF_ACTION_OPTION
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public Relation_Option_Enum Relation { get; set; }
        public int FormElementId { get; set; }
        public virtual T_WF_FORM_ELEMENT FormElement { get; set; }
        public int NodeId { get; set; }
        public virtual T_WF_NODE Node { get; set; }
        public T_WF_ACTION_OPTION()
        {

        }
       

    }
}
