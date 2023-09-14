using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.ViewModels.IntegrationBinding;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace TLIS_DAL.Models
{
    public class TLIexternalSys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string SysName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.")]
        public string Password { get; set; }
        [Required]
        public string IP { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public DateTime StartLife { get; set; }
        public DateTime EndLife { get; set; }
        public int LifeTime { get; set; }
        public virtual ICollection<TLIexternalSysPermissions> TLIexternalSysPermissions { get; set; }

        public TLIexternalSys() { }
       
        public TLIexternalSys(AddExternalSysBinding model, string PasswordHash)
        {
            SysName = model.SysName;
            UserName= model.UserName;
            Password= PasswordHash;
            IP = model.IP;
            IsDeleted = false;
            IsActive = true;
            LifeTime = model.LifeTime;
            StartLife = DateTime.Now;
            EndLife = DateTime.Now.AddDays(model.LifeTime);
        }

        public TLIexternalSys(EditExternalSysBinding model, string PasswordHash)
        {
            Id=model.Id;
            SysName = model.SysName;
            UserName = model.UserName;
            Password = PasswordHash;
            IP = model.IP;
            IsDeleted = false;
            LifeTime= model.LifeTime;
            StartLife= DateTime.Now;
            EndLife= DateTime.Now.AddDays(model.LifeTime);
        }


    }
}
