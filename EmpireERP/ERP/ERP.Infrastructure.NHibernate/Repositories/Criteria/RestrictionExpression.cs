using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ERP.Infrastructure.Repositories.Criteria;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class RestrictionExpression<T> : BaseExpression, ISubExpression, IExpression
    {
        /// <summary>
        /// Значение выражения ограничения
        /// </summary>
        private Expression<Func<T, bool>> restrictionExpression = null;

        /// <summary>
        /// имя поля
        /// </summary>
        private string fieldName;

        /// <summary>
        /// Второе поле операнд
        /// </summary>
        private string secondFieldName;

        /// <summary>
        /// Значение
        /// </summary>
        private object fieldValue;

        /// <summary>
        /// Условие сравнения поля и значения
        /// </summary>
        private CriteriaCond fieldCond;

        #region Конструкторы

        /// <summary>
        /// Ввыражение ограничения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        public RestrictionExpression(Expression<Func<T, bool>> expr)
        {
            restrictionExpression = expr;
        }

        /// <summary>
        /// Выражение ограничения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        public RestrictionExpression(string name, CriteriaCond cond, object value)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception("Не задано имя поля для операции where.");

            fieldName = name;
            fieldValue = value;
            fieldCond = cond;
        }

        public RestrictionExpression(string name, CriteriaCond cond, string secondName)
        {
            if (String.IsNullOrEmpty(name))
                throw new Exception("Не задано имя поля первого операнда для операции where.");

            if (String.IsNullOrEmpty(secondName))
                throw new Exception("Не задано имя поля второго операнда для операции where.");

            fieldName = name;
            fieldCond = cond;
            secondFieldName = secondName;
        }

        #endregion

        #region BaseExpression

        /// <summary>
        /// Компилирование выражения под ORM
        /// </summary>
        /// <param name="iCriteria"></param>
        /// <returns></returns>
        void IExpression.Compile(ref ICriteria iCriteria)
        {
            object result;
            if (restrictionExpression != null)
            {
                // Ограничение задано лямбда выражением
                result = CompileRestrictionFromLambda();
            }
            else
            {
                //Ограничение задано именем поля и значением
                result = CompileSimpleRestriction(String.Empty);
            }

            //Обрабатываем результат
            if (!(result is bool))
            {
                iCriteria.Add(result as global::NHibernate.Criterion.ICriterion);
            }
            else
            {
                if (!Convert.ToBoolean(result))
                    iCriteria = null;
            }
        }

        /// <summary>
        /// Компиляция выражения в ICriterion NHibernate
        /// </summary>
        /// <returns></returns>
        global::NHibernate.Criterion.ICriterion IExpression.Compile(ref ICriteria iCriteria, string alias)
        {
            object result;
            if (restrictionExpression != null)
            {
                // Ограничение задано лямбда выражением
                result = CompileRestrictionFromLambda();
            }
            else
            {
                //Ограничение задано именем поля и значением
                result = CompileSimpleRestriction(alias);
            }

            //Обрабатываем результат
            if (!(result is bool))
            {
               return result as global::NHibernate.Criterion.ICriterion;
            }
            else
            {
                if (!Convert.ToBoolean(result))
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Генерация ограничения по лямбда выражению
        /// </summary>
        /// <param name="iCriteria"></param>
        private object CompileRestrictionFromLambda()
        {
            var result = ParseExpression(restrictionExpression.Body, null);
            var crit = result as global::NHibernate.Criterion.ICriterion;

            if (crit == null)
            {
                return result;
            }
            
            return crit;
        }

        /// <summary>
        /// Генерация ограничения по данным поля
        /// </summary>
        /// <param name="iCriteria"></param>
        private object CompileSimpleRestriction(string alias)
        {
            Type fieldType = GetTypeField(fieldName, typeof(T));    //Получаем тип поля
            object value;
            string field = "", secondField = "";

            if (fieldType != Type.Missing as Type)  // Проверяем, найдено ли поле
            {
                //Да, найдено
                value = ConvertToType(fieldValue, fieldType);   //Приводим значение
                field = (String.IsNullOrEmpty(alias) ? "" : alias + '.') + fieldName;   // формируем название поля, добавляя псевдоним
            }
            else
            {
                //Нет, не найдено
                value = fieldValue; //Используем значение "как есть"
                field = fieldName;  //Оставляем название поля "как есть", т.к. оно предположительно содержит псевдоним
            }
            if (value == null && !string.IsNullOrEmpty(secondFieldName))
            {
                var secondFieldType = GetTypeField(secondFieldName, typeof(T));    //Получаем тип поля
                if (secondFieldType != Type.Missing as Type)  // Проверяем, найдено ли поле
                {
                    //Да, найдено
                    secondField = (String.IsNullOrEmpty(alias) ? "" : alias + '.') + secondFieldName;   // формируем название поля, добавляя псевдоним
                }
                else
                {
                    //Нет, не найдено
                    secondField = secondFieldName;  //Оставляем название поля "как есть", т.к. оно предположительно содержит псевдоним
                }
            }

            global::NHibernate.Criterion.ICriterion exp = null;
            
            switch (fieldCond)
            {
                case CriteriaCond.Eq:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Eq(field, value);
                    }
                    else if (String.IsNullOrEmpty(secondFieldName))
                    {
                        exp = global::NHibernate.Criterion.Expression.IsNull(field);
                    }
                    else
                    {
                        exp = global::NHibernate.Criterion.Expression.EqProperty(field, secondField);
                    }
                    break;
                
                case CriteriaCond.NotEq:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Not(global::NHibernate.Criterion.Expression.Eq(field, value));
                    }
                    else if (String.IsNullOrEmpty(secondFieldName))
                    {
                        exp = global::NHibernate.Criterion.Expression.IsNotNull(field);
                    }
                    else
                    {
                        exp = global::NHibernate.Criterion.Expression.NotEqProperty(field, secondField);
                    }
                    break;
                
                case CriteriaCond.Gt:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Gt(field, value);
                    }
                    else
                    {
                        return true;
                    }
                    break;
                
                case CriteriaCond.Ge:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Ge(field, value);
                    }
                    else
                    {
                        return true;
                    }
                    break;
               
                case CriteriaCond.Lt:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Lt(field, value);
                    }
                    else
                    {
                        return true;
                    }
                    break;
                
                case CriteriaCond.Le:
                    if (value != null)
                    {
                        exp = global::NHibernate.Criterion.Expression.Le(field, value);
                    }
                    else
                    {
                        return true;
                    }
                    break;
            }

            return exp;
        }

        /// <summary>
        /// Компилирование выражения под ORM
        /// </summary>
        /// <param name="iCriteria"></param>
        /// <returns></returns>
        void ISubExpression.Compile(ref global::NHibernate.Criterion.DetachedCriteria iCriteria)
        {
            object result;

            if (restrictionExpression != null)
            {
                // Ограничение задано лямбда выражением
                result = CompileRestrictionFromLambda();
            }
            else
            {
                //Ограничение задано именем поля и значением
                result = CompileSimpleRestriction(String.Empty);
            }

            //Обрабатываем результат
            if (!(result is bool))
            {
                iCriteria.Add(result as global::NHibernate.Criterion.ICriterion);
            }
            else
            {
                if (!Convert.ToBoolean(result))
                    iCriteria = null;
            }
        }

        /// <summary>
        /// Разбор лямбда выражения
        /// </summary>
        /// <param name="expr">Лямбда выражение</param>
        /// <param name="parameterOfExpression">Имя параметра лямбда выражения</param>
        /// <returns></returns>
        protected object ParseExpression(Expression expr, MemberInfo members)
        {
            object left = null, right = null;
            object result = null;

            #region Подготовка к обработке бинарного выражения

            //приведение к бинарному выражению
            var binExpr = expr as BinaryExpression;
            if (binExpr != null)
            {
                left = ParseExpression(binExpr.Left, members);
                right = ParseExpression(binExpr.Right, members);

                var leftMember = left as MemberInfo;
                var rightMember = right as MemberInfo;

                //вычисляем значение левого операнда
                if (left is MemberInfo)
                {
                    if (leftMember.Value != null)
                        left = GetMemberValue(leftMember);
                    else
                    {
                        if (leftMember.Name.Length == 0)
                            left = null;
                    }
                }

                //вычисляем значение правого операнда
                if (right is MemberInfo)
                {
                    if (rightMember.Value != null)
                        right = GetMemberValue(rightMember);
                    else
                    {
                        if (rightMember.Name.Length == 0)
                            right = null;
                    }
                }

                //проверяем порядок операндов
                if (right is MemberInfo && !(left is MemberInfo))
                {
                    //Порядок неверный, меняем местами
                    var tmp = right;
                    right = left;
                    left = tmp;
                }

                if (left is MemberInfo && !(right is MemberInfo)) //Если левый операнд параметер и правый значение, ...
                {
                    //то приводим правый операнд к типу левого параметра
                    right = ConvertToFieldType((left as MemberInfo).Name, typeof(T), right);  //приводим типы данных
                }
            }

            #endregion

            //обработка узла дерева
            switch (expr.NodeType)
            {
                #region Операции сравнения

                case ExpressionType.GreaterThan:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.GtProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                        if (right != null)
                            result = global::NHibernate.Criterion.Expression.Gt((left as MemberInfo).Name, right);
                        else
                            result = true;
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        object[] argGreaterThan = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argGreaterThan));
                    }
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.GeProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                        if (right != null)
                            result = global::NHibernate.Criterion.Expression.Ge((left as MemberInfo).Name, right);
                        else
                            result = true;
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        object[] argGreaterThan = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argGreaterThan));
                    }
                    break;

                case ExpressionType.LessThan:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.LtProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                        if (right != null)
                            result = global::NHibernate.Criterion.Expression.Lt((left as MemberInfo).Name, right);
                        else
                            result = true;
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        object[] argLessThan = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argLessThan));
                    }
                    break;
                case ExpressionType.LessThanOrEqual:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.LeProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                        if (right != null)
                            result = global::NHibernate.Criterion.Expression.Le((left as MemberInfo).Name, right);
                        else
                            result = true;
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        object[] argLessThan = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argLessThan));
                    }
                    break;

                case ExpressionType.Equal:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.EqProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                            if (right != null)  //Значение null?
                                //Нет
                                result = global::NHibernate.Criterion.Expression.Eq((left as MemberInfo).Name, right);
                            else
                                //Да
                                result = global::NHibernate.Criterion.Expression.IsNull((left as MemberInfo).Name);
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        if (binExpr.Method != null)
                        {
                            object[] argEqual = new object[] { left, right };
                            result = new MemberInfo("", binExpr.Method.Invoke(null, argEqual));
                        }
                        else
                        {
                            if (left == null && right == null)
                            {
                                result = true;
                            }
                            else if (left == null || right == null)
                            {
                                result = false;
                            }
                        }
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (left is MemberInfo && right is MemberInfo)  //Оба операнда параметры
                    {
                        result = global::NHibernate.Criterion.Expression.NotEqProperty((left as MemberInfo).Name, (right as MemberInfo).Name);
                    }
                    if (left is MemberInfo && !(right is MemberInfo))   //Только левый параметр
                    {
                        if (right != null)  //Значение null?
                            //Нет
                            result = global::NHibernate.Criterion.Expression.Not(global::NHibernate.Criterion.Expression.Eq((left as MemberInfo).Name, right));
                        else
                            //Да
                            result = global::NHibernate.Criterion.Expression.IsNotNull((left as MemberInfo).Name);
                    }
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба параметра константы
                    {
                        object[] argNotEqual = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argNotEqual));
                    }
                    break;

                #endregion

                #region Арифметические операции

                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Divide:
                case ExpressionType.Multiply:
                    if (!(left is MemberInfo) && !(right is MemberInfo))    //Оба операнда константы
                    {
                        object[] argAdd = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argAdd));
                    }
                    else
                    {
                        throw new Exception(String.Format("Операция {0} поддерживается только для констант. Выражение: {1}", expr.NodeType.ToString(), expr.ToString()));
                    }
                    break;

                #endregion

                #region Логические операции (or, and)

                case ExpressionType.OrElse:
                    result = ExecuteOperationOR(expr, left, right);
                    break;
                case ExpressionType.AndAlso:
                    result = ExecuteOperationAND(expr, left, right);
                    break;
                case ExpressionType.Not:
                    var unaryExpr = expr as UnaryExpression;
                    object argNot = null;

                    result = ParseExpression(unaryExpr.Operand, members);   //Вычисляем аргумент
                    if (result is MemberInfo)
                    {
                        var operandNot = result as MemberInfo;
                        if (operandNot.Value == null)   //Не параметер ли?
                        {
                            //Параметер, ошибка
                            throw new Exception("Отрицание не применимо к параметру.");
                        }
                        if (operandNot.Name.Length > 0)    //Обращение к члену?
                        {
                            //Да
                            operandNot.Value = GetMemberValue(members); //Вычисляем значение члена
                        }
                        argNot = operandNot.Value;   //Сохраняем значение аргумента
                    }
                    result = new MemberInfo("", !(bool)argNot);  //Вычисляем отрицание
                    break;
                #endregion

                #region Разбор дерева выражений

                case ExpressionType.MemberAccess:
                    result = ExecuteMemberAccess(expr, members);
                    break;
                case ExpressionType.Constant:
                    var exp8 = expr as ConstantExpression;
                    result = new MemberInfo("", exp8.Value);
                    break;
                case ExpressionType.Parameter:
                    var exp9 = expr as ParameterExpression;
                    
                    result = new MemberInfo("", null);   //Возвращаем пустую строку, опуская тем самым имя параметра
                    break;
                case ExpressionType.Convert:
                    var exp10 = expr as UnaryExpression;

                    result = ParseExpression(exp10.Operand, members);
                    var convertValueInfo = result as MemberInfo;

                    if (convertValueInfo.Value != null)
                    {
                        convertValueInfo.Name = "";
                        convertValueInfo.Value = ConvertToType(convertValueInfo.Value, exp10.Type);
                    }
                    break;
                case ExpressionType.Call:
                    result = ExecuteCall(expr, members);
                    break;

                #endregion

                default:
                    throw new Exception(String.Format("Операция {0} не поддерживается. Выражение: {1}", expr.NodeType.ToString(), expr.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Обработка логичской операции ИЛИ
        /// </summary>
        /// <param name="expr">Узел дерева</param>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns></returns>
        private object ExecuteOperationOR(Expression expr, object left, object right)
        {
            object result = null;
            var binExpr = expr as BinaryExpression;

            if (left is global::NHibernate.Criterion.ICriterion && right is global::NHibernate.Criterion.ICriterion)
            {
                result = global::NHibernate.Criterion.Expression.Or((global::NHibernate.Criterion.ICriterion)left, (global::NHibernate.Criterion.ICriterion)right);
            }
            else
            {
                if (left is global::NHibernate.Criterion.ICriterion && !(right is global::NHibernate.Criterion.ICriterion)) //Критерий и bool?
                {
                    //Да
                    if ((bool)right)    //истинно ли второе выражение?
                    {
                        //Да
                        result = right;  //оставляем только левый операнд
                    }
                    else
                    {
                        //Нет
                        result = left; //Возвращаем FALSE
                    }
                }
                else
                {
                    //нет
                    if (!(left is global::NHibernate.Criterion.ICriterion) && right is global::NHibernate.Criterion.ICriterion) //bool и критерий?
                    {
                        //Да
                        if ((bool)left)    //истинно ли первое выражение?
                        {
                            //Да
                            result = left;  //оставляем только правый операнд
                        }
                        else
                        {
                            //Нет
                            result = right; //Возвращаем FALSE
                        }
                    }
                    else
                    {
                        //Нет, просто два bool
                        object[] argAndAlso = new object[] { left, right };
                        result = new MemberInfo("", binExpr.Method.Invoke(null, argAndAlso));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Обработка логичской операции И
        /// </summary>
        /// <param name="expr">Узел дерева</param>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns></returns>
        private object ExecuteOperationAND(Expression expr, object left, object right)
        {
            object result = null;
            var binExpr = expr as BinaryExpression;

            if (left is global::NHibernate.Criterion.ICriterion && right is global::NHibernate.Criterion.ICriterion)
            {
                result = global::NHibernate.Criterion.Expression.And((global::NHibernate.Criterion.ICriterion)left, (global::NHibernate.Criterion.ICriterion)right);
            }
            else
            {
                if (left is global::NHibernate.Criterion.ICriterion && !(right is global::NHibernate.Criterion.ICriterion)) //Критерий и bool?
                {
                    //Да
                    if ((bool)right)    //истинно ли второе выражение?
                    {
                        //Да
                        result = left;  //оставляем только левый операнд
                    }
                    else
                    {
                        //Нет
                        result = right; //Возвращаем FALSE
                    }
                }
                else
                {
                    //нет
                    if (!(left is global::NHibernate.Criterion.ICriterion) && right is global::NHibernate.Criterion.ICriterion) //bool и критерий?
                    {
                        //Да
                        if ((bool)left)    //истинно ли первое выражение?
                        {
                            //Да
                            result = right;  //оставляем только правый операнд
                        }
                        else
                        {
                            //Нет
                            result = left; //Возвращаем FALSE
                        }
                    }
                    else
                    {
                        //Нет, просто два bool
                        object[] argAndAlso = new object[] { left, right };
                        if (binExpr.Method != null)
                            result = new MemberInfo("", binExpr.Method.Invoke(null, argAndAlso));
                        else
                            result = (bool)left && (bool)right;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Обрабатываем обращение к члену
        /// </summary>
        /// <param name="expr">Узел дерева</param>
        /// <param name="members">Описание обращения к члену</param>
        /// <returns></returns>
        private object ExecuteMemberAccess(Expression expr, MemberInfo members)
        {
            object result = null;
            MemberInfo memberInfo = null;
            var exp7 = expr as MemberExpression;

            if (exp7.Expression != null)    //Проверяем наличие выражения
            {
                if (members == null)
                {
                    //****Первый член*****
                    memberInfo = new MemberInfo();
                    result = ParseExpression(exp7.Expression, memberInfo);    //Получаем левый операнд

                    memberInfo = result as MemberInfo;

                    //Формируем цепочку обращений к членам
                    if (memberInfo.Name.Length > 0)
                        memberInfo.Name += '.' + exp7.Member.Name;
                    else
                        memberInfo.Name = exp7.Member.Name;

                    //Формируем результат
                    if (memberInfo.Value != null)
                    {
                        memberInfo.Value = GetMemberValue(memberInfo);  //Вычисляе значения
                        memberInfo.Name = "";   //Сбрасываем цепочку членов
                    }

                    result = memberInfo;
                }
                else
                {
                    //***Промежуточный член****
                    result = ParseExpression(exp7.Expression, members);    //Получаем левый операнд

                    //Обрабатываем результат
                    if (result is MemberInfo)
                    {
                        members = result as MemberInfo;
                    }
                    else
                    {
                        //вернулся результат последнего члена
                        members.Value = result;
                    }

                    //Формируем цепочку обращений к членам
                    if (members.Name.Length > 0)
                        members.Name += '.' + exp7.Member.Name;
                    else
                    {
                        members.Name = exp7.Member.Name;
                    }

                    result = members;
                }
            }
            else
            {
                //Выражения нет. 
                //Вызов свойства статического класса
                if (exp7.Member.MemberType == System.Reflection.MemberTypes.Property)
                {
                    var res = new MemberInfo();
                    res.Value = exp7.Member.DeclaringType.GetProperty(exp7.Member.Name).GetValue(null,null);
                    result = res;
                }
                //Вызов поля статического класса
                if (exp7.Member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    var res = new MemberInfo();
                    res.Value = exp7.Member.DeclaringType.GetField(exp7.Member.Name).GetValue(null);
                    result = res;
                }
            }

            return result;
        }

        /// <summary>
        /// Обрабатываем вызов
        /// </summary>
        /// <param name="expr">Узел дерева</param>
        /// <param name="members">Описание обращения к члену</param>
        /// <returns></returns>
        private object ExecuteCall(Expression expr, MemberInfo members)
        {
            object result = null;

            var exp11 = expr as MethodCallExpression;
            List<object> args = new List<object>(); //список аргументов вызовов
            object argValue;
            //Цикл вычисления аргументов вызова метода
            foreach (var argExpr in exp11.Arguments)
            {
                var valueArgs = ParseExpression(argExpr, members);
                if (valueArgs is MemberInfo)
                    argValue = GetMemberValue(valueArgs as MemberInfo);
                else
                    argValue = valueArgs;
                args.Add(argValue);
            }
            object obj = null;

            //Вычисляем объект
            if (exp11.Object != null)
            {
                obj = ParseExpression(exp11.Object, members);

                var memberInfoObj = obj as MemberInfo;
                if (memberInfoObj.Value == null)    //Вызывается ли член у параметра?
                {
                    //Да, генерируем исключение
                    throw new Exception(String.Format("Вызов метода параметра «{0}» не поддерживается.", exp11.Method.Name));
                }
                obj = memberInfoObj.Value;
            }

            result = exp11.Method.Invoke(obj, args.ToArray<object>());  //Вызываем метод

            result = new MemberInfo("", result);

            return result;
        }

        #endregion
    }
}
