using System;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class MemberInfoExtensions
    {


        public static Type GetPropertyOrFieldType(this MemberInfo member)
        {
            if (member is null)
                throw new ArgumentNullException(nameof(member));

            return member is PropertyInfo prop ? prop.PropertyType
                : member is FieldInfo field ? field.FieldType
                : throw new InvalidOperationException($@"Member ""{member}"" is neither a property nor a field");
        }


    }
}
