using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ERP.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Отображает значение атрибута EnumDisplayName
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(Enum.GetName(type, value));
            var displayNameAttribute =
                (EnumDisplayNameAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(EnumDisplayNameAttribute));

            if (displayNameAttribute != null)
                return displayNameAttribute.DisplayName;

            return value.ToString();
        }


        /// <summary>
        /// Отображает значение атрибута EnumDescription
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type type = value.GetType();

            FieldInfo fieldInfo = type.GetField(Enum.GetName(type, value));
            var descriptionAttribute =
                (EnumDescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(EnumDescriptionAttribute));

            if (descriptionAttribute != null)
                return descriptionAttribute.DisplayName;

            return value.ToString();
        }


        /// <summary>
        ///  Преобразует значение перечислимого типа в число, а затем в строку
        /// </summary>
        /// <param name="value">Исходное значение перечислимого типа</param>
        /// <returns>Строку с текстовым представлением числового значения элемента enum</returns>
        public static string ValueToString(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type type = value.GetType();
            Type underlyingType = Enum.GetUnderlyingType(type);

            return Convert.ChangeType(value, underlyingType).ToString();
        }

        /// <summary>
        /// Определяет: содержится ли указанное значение перечисления в указанном списке значений перечислений
        /// </summary>
        /// <param name="value">Искомое значние</param>
        /// <param name="values">Список значений для поиска</param>
        /// <returns>Результат проверки</returns>
        public static bool ContainsIn(this Enum value, params Enum[] values)
        {
            if (!values.Any())
            {
                throw new Exception("Список выбора значений перечисления пуст.");
            }

            var type = value.GetType();

            foreach (var item in values)
            {
                if (item.GetType() != type)
                {
                    throw new Exception(String.Format("Значение «{0}» не принадлежит перечислению «{1}».", item, type.Name));
                }

                if (item.ValueToString() == value.ValueToString())
                {
                    return true;
                }
            }

            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : DisplayNameAttribute
    {
        public EnumDisplayNameAttribute(string displayName)
            : base(displayName)
        { }
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumDescriptionAttribute : DisplayNameAttribute
    {
        public EnumDescriptionAttribute(string displayName)
            : base(displayName)
        { }
    }
}
