using System;
using System.Reflection;

namespace Mimp.SeeSharper.Reflection
{
    public static class MemberInfoExtensions
    {


        /// <summary>
        /// Return the property or field type of the member.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">If member isn't a property or a field</exception>
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
