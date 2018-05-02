using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ERP.Utils
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Получить аттрибут для свойства
        /// </summary>
        /// <typeparam name="T">класс для свойства которого ищем значение аттрибута</typeparam>
        /// <typeparam name="A">тип аттрибута</typeparam>
        /// <param name="propertyExpression">Лямбда выражение определяющее свойство для которого ищем значение аттрибута</param>
        public static TAttribute GetPropertyAttribute<TModel, TAttribute>(Expression<Func<TModel, object>> propertyExpression) where TAttribute : Attribute
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            ValidationUtils.NotNull(memberInfo, "Не удалось найти указанное свойство.");

            return memberInfo.GetAttribute<TAttribute>();
        }

        /// <summary>
        /// Получить значение аттрибута DisplayName для свойства
        /// </summary>
        /// <typeparam name="T">класс для свойства которого ищем значение аттрибута</typeparam>
        /// <param name="propertyExpression">Лямбда выражение определяющее свойство для которого ищем значение аттрибута</param>
        public static string GetPropertyDisplayName<TModel>(Expression<Func<TModel, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            ValidationUtils.NotNull(memberInfo, "Не удалось найти указанное свойство.");
            
            var attr = memberInfo.GetAttribute<DisplayNameAttribute>();
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.DisplayName;
        }

        /// <summary>
        /// Получение информации о свойстве из linq выражения
        /// </summary>
        /// <param name="propertyExpression">linq выражение</param>
        /// <returns>информация о свойстве (null - если свойство не найдено)</returns>
        private static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            ValidationUtils.Assert(propertyExpression != null, "Укажите поле для которого искать аттрибут.");

            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }

        /// <summary>
        /// Получить аттрибут
        /// </summary>
        /// <typeparam name="T">Тип аттрибута</typeparam>
        /// <param name="member">Элемент для которого ищем аттрибут</param>
        /// <param name="isRequired">Аттрибут должен присутствовать обязательно</param>
        /// <returns>Аттрибут (null - если аттрибут не найден)</returns>
        private static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            ValidationUtils.Assert(!(attribute == null && isRequired), String.Format("Аттрибут «{0}» должен быть определен для поля «{1}».", typeof(T).Name, member.Name));

            return (T)attribute;
        }

        /// <summary>
        /// Получить название свойства
        /// </summary>
        /// <typeparam name="T">класс для свойства которого ищем название свойства</typeparam>
        /// <param name="propertyExpression">Лямбда выражение определяющее свойство для которого ищем название</param>
        public static string GetPropertyName<TModel>(Expression<Func<TModel, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            ValidationUtils.NotNull(memberInfo,"Не удалось найти указанное свойство.");

            return memberInfo.Name;
       }
    }
}
