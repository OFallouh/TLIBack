namespace WF_API.Model
{
    public class T_WF_FORM_ELEMENT_CHOICE
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int FormElementId { get; set; }
        public virtual T_WF_FORM_ELEMENT FormElement { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
