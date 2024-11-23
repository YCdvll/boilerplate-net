using System.ComponentModel;
using System.Reflection;

namespace Project.Common.Services.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute) return attribute.Description;

        return value.ToString();
    }
}