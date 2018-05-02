using System;
using System.Reflection;
using ERP.Utils;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public abstract class BaseExpression
    {
        #region Данные 

        /// <summary>
        /// Класс, описывающий обращение к члену
        /// </summary>
        protected class MemberInfo
        {
            /// <summary>
            /// Строка обращения с члену
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Значение
            /// </summary>
            public object Value { get; set; }


            public MemberInfo(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public MemberInfo()
            {
                Name = "";
                Value = null;
            }
        }

        /// <summary>
        /// Тип операции
        /// </summary>
        protected enum OperationType { Unary, Binary, Parameter, Const, Convert };

        #endregion

        #region Методы

        /// <summary>
        /// Приведение значения к типу данных поля
        /// </summary>
        /// <param name="field">Поле объекта</param>
        /// <param name="objectType">Тип данных</param>
        /// <returns></returns>
        protected object ConvertToFieldType(string field, Type objectType, object value)
        {
            Type type = GetTypeField(field, objectType);

            return ConvertToType(value, type);
        }

        /// <summary>
        /// Приведение значения к типу
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="type">Тип, к которому надо привести</param>
        /// <returns></returns>
        protected object ConvertToType(object value, Type type)
        {
            object result = null;

            if (value == null) return null;
            if (value.GetType() == type) return value;  //Если типы совпадают, то возвращаем объект без изменений

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                result = Convert.ChangeType(value, type.GetGenericArguments()[0]);
            }
            else
            {
                if (type.IsEnum)
                {
                    var val = Convert.ToInt32(value);
                    result = Enum.ToObject(type, val);
                }
                else
                {
                    #region Блок кода для приведения Proxy к типу сущности
                    
                    //Получаем методы Is<>() и As<>()
                    var isPropInfo = type.GetMethod("Is");
                    var asPropInfo = type.GetMethod("As");

                    if (isPropInfo != null && asPropInfo != null)   //Если методы найдены, то...
                    {
                        var canConvert = isPropInfo.MakeGenericMethod(type).Invoke(value, System.Type.EmptyTypes);  //проверяем, возможно ли приведение.
                        if (Convert.ToBoolean(canConvert))  //Если да...
                        {
                            result = asPropInfo.MakeGenericMethod(type).Invoke(value, System.Type.EmptyTypes);   // выполняем приведение
                        }
                        // иначе пытаемся сделать стандартное приведение.
                    }

                    #endregion

                    if (result == null) //Проверяем, было ли сделано приведение ранее.
                    {
                        if (type == typeof(Guid))
                        {
                            result = ValidationUtils.TryGetGuid(value as string);
                        }
                        else
                        {
                            //Нет, приведение не сделано
                            result = Convert.ChangeType(value, type);   //Приводим объект к типу с помощью стандартных механизмов
                        }
                    }
                }
            }

            return result;
        }
       
        /// <summary>
        /// Получение типа данных поля
        /// </summary>
        /// <param name="field">Поле</param>
        /// <param name="objectType">Тип данных</param>
        /// <returns></returns>
        protected Type GetTypeField(string field, Type objectType)
        {
            string[] members = field.Split('.');
            Type result = objectType;

            //Проверяем, является ли тип дженериком и поддерживает ли интерфейсы коллекций
            if (result.IsGenericType &&
               (result.GetInterface("IEnumerable") != null || result.GetInterface("IList") != null))
            {
                // Да. Обрабатываем как коллекцию
                var genericTypes = result.GetGenericArguments(); //Получаем массив типов дженерика
                result = genericTypes[0]; // Т.к. дженерик коллекции имеет всего один параметер, то он и является типом поля
            }

            //Цикл получения типов вложенных полей
            foreach (var member in members)
            {
                var propInfo = result.GetProperty(member);
                if (propInfo == null)   //Проверяем, найдено ли свойство
                {
                    // Нет, не найдено. Предполагаем, что в названии поля используется псевдоним
                    return Type.Missing as Type;    // Возвращаем признак использования псевдонима
                }
                result = propInfo.PropertyType;
                //Проверяем, является ли тип дженериком и поддерживает ли интерфейсы коллекций
                if (result.IsGenericType &&
                   (result.GetInterface("IEnumerable") != null || result.GetInterface("IList") != null))
                {
                    // Да. Обрабатываем как коллекцию
                    var genericTypes = result.GetGenericArguments(); //Получаем массив типов дженерика
                    result = genericTypes[0]; // Т.к. дженерик коллекции имеет всего один параметер, то он и является типом поля
                }
            }

            return result;
        }

        /// <summary>
        /// Получение значения члена
        /// </summary>
        /// <param name="memberInfo">Объект, описывающий обращение к члену</param>
        /// <returns></returns>
        protected object GetMemberValue(MemberInfo memberInfo)
        {
            var members = memberInfo.Name.Split('.');
            object value = memberInfo.Value;
            Type type = value.GetType();

            if (memberInfo.Name.Length > 0)
            {
                foreach (var member in members)
                {
                    if (type == null)
                    {
                        throw new Exception(String.Format("Невозможно вычислить значение члена {0}, т.к. слева от {1} значение null.", memberInfo.Name, member));
                    }

                    var mInfo = type.GetMember(member);
                    if (mInfo == null || mInfo.Length == 0)
                    {
                        throw new Exception(String.Format("Не найден член {0} типа данных {1}", member, type.ToString()));
                    }

                    switch (mInfo[0].MemberType)
                    {
                        case MemberTypes.Field:
                            var fieldInfo = type.GetField(member);
                            
                            if (fieldInfo == null)
                            {
                                throw new Exception(String.Format("Не найдено поле {0} из обращения {1}", member, memberInfo.Name));
                            }

                            value = fieldInfo.GetValue(value);
                            break;
                        case MemberTypes.Property:
                            var propertyInfo = type.GetProperty(member);
                            
                            if (propertyInfo == null)
                            {
                                throw new Exception(String.Format("Не найдено свойство {0} из обращения {1}", member, memberInfo.Name));
                            }

                            value = propertyInfo.GetValue(value, null);
                            break;
                    }
                    if (value != null)
                        type = value.GetType();
                    else
                        type = null;
                }
            }

            return value;
        }

        #endregion
    }
}
