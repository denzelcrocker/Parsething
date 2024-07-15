using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parsething.Classes
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public class EnumItem
        {
            public RejectionReason Value { get; set; }
            public string Description { get; set; }
        }

        public static List<EnumItem> GetEnumValuesAndDescriptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new EnumItem { Value = (RejectionReason)(object)e, Description = EnumHelper.GetEnumDescription((Enum)(object)e) })
                       .ToList();
        }
    }
}
