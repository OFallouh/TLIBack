using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Helper.Enums
{
    public class EnumDescription
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

public static class EnumExtensions
{
        public static (int Id, string Description) GetEnumDetails<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString()).FirstOrDefault();
            var descriptionAttribute = memberInfo?.GetCustomAttribute<DescriptionAttribute>();

            var id = Convert.ToInt32(enumValue);
            var description = descriptionAttribute?.Description ?? enumValue.ToString();

            return (id, description);
        }
    }


}
