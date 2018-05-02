using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ERP.Infrastructure.NHibernate.SessionManager;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using NHibernate;

namespace ERP.Infrastructure.NHibernate.Repositories
{
    /// <summary>
    /// Базовый класс для репозиториев
    /// </summary>
    public abstract class BaseRepository
    {
        #region Данные

        protected int currentPage;
        protected int pageSize;
        protected string filter;
        protected string parameters;
        protected string sort;
        protected ParameterString parameterString;

        /// <summary>
        /// Текущая сессия NHibernate
        /// </summary>
        protected ISession CurrentSession
        {
            get { return NHibernateSessionManager.CurrentSession; }
        }

        #endregion

        #region Фильтр

        protected object CreateCriteria<T>(string propNames, object criteria) where T : class
        {
            object crit = criteria;
            var t = typeof(T);
            var props = propNames.Split('.');

            for (int i = 0; i < props.Length - 1; i++)
            {
                var prop = props[i];
                t = t.GetProperty(prop).PropertyType;

                if (t.IsGenericType)
                {
                    var r = t.GetGenericArguments();
                    t = r[0];
                }

                var alias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");   //Создаем псевдоним для вложенного критерия
                crit = crit.GetType().GetMethod("Restriction", new Type[] { typeof(string), typeof(string) }).MakeGenericMethod(t).Invoke(crit, new object[] { prop, alias });
            }

            return crit;
        }
        
        private Type GetProperty(Type baseType, string prop)
        {
            PropertyInfo propInfo = null;
            Type propType = baseType;
            var names = prop.Split('.');
            foreach (var name in names)
            {
                propInfo = propType.GetProperty(name);
                if (propInfo == null)
                    throw new Exception(String.Format("Свойство «{0}» не найдено у типа «{1}».", name, propType.ToString()));

                //Проверяем, является ли тип дженериком и поддерживает ли интерфейсы коллекций
                if (propInfo.PropertyType.IsGenericType &&
                   (propInfo.PropertyType.GetInterface("IEnumerable") != null || propInfo.PropertyType.GetInterface("IList") != null))
                {
                    // Да. Обрабатываем как коллекцию
                    var genericTypes = propInfo.PropertyType.GetGenericArguments(); //Получаем массив типов дженерика
                    propType = genericTypes[0]; // Т.к. дженерик коллекции имеет всего один параметер, то он и является типом поля
                } 
                else
                {
                    //Нет. Обрабатываем как обычное свойство
                    propType = propInfo.PropertyType;
                }
            }

            return propType;
        }
        
        /// <summary>
        /// Получение дат из диапозона
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        private DateTime?[] GetDate(string dateRange)
        {
            DateTime?[] result = new DateTime?[2] { null, null };

            if (!String.IsNullOrEmpty(dateRange))
            {
                var dateRangeSplit = dateRange.Split('-');
                var fDate = dateRangeSplit[0].Trim();

                if (String.IsNullOrEmpty(fDate))
                {
                    result[0] = null;
                }
                else
                {
                    result[0] = ValidationUtils.TryGetDate(fDate);
                }

                if (dateRangeSplit.Length == 2)
                {
                    var sDate = dateRangeSplit[1].Trim();
                    if (String.IsNullOrEmpty(sDate))
                    {
                        result[1] = null;
                    }
                    else
                    {
                        result[1] = ValidationUtils.TryGetDate(sDate);
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Чтение данных из объекта состояния грида
        /// </summary>
        /// <param name="state">Объект состояния грида</param>
        protected void ReadState(object state)
        {
            Type stateType = state.GetType();
            currentPage = (int)stateType.GetProperty("CurrentPage").GetValue(state, null);
            pageSize = (int)stateType.GetProperty("PageSize").GetValue(state, null);
            filter = (string)stateType.GetProperty("Filter").GetValue(state, null);
            parameters = (string)stateType.GetProperty("Parameters").GetValue(state, null);
            sort = (string)stateType.GetProperty("Sort").GetValue(state, null);
        }

        /// <summary>
        /// Запись в объект состояния грида
        /// </summary>
        protected void WriteTotalRowCount(object state, int totalRowCount)
        {
            Type stateType = state.GetType();
            stateType.GetProperty("TotalRow").SetValue(state, totalRowCount, null);    //Записываем кол-во строк в объект
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>        
        protected IList<T> GetBaseFilteredList<T>(object state, bool ignoreDeletedRows = true, Func<ISubCriteria<T>, ISubCriteria<T>> cond = null) where T : class
        {
            Type stateType = state.GetType();
            pageSize = (int)stateType.GetProperty("PageSize").GetValue(state, null);
            filter = (string)stateType.GetProperty("Filter").GetValue(state, null);
            parameters = (string)stateType.GetProperty("Parameters").GetValue(state, null);
            sort = (string)stateType.GetProperty("Sort").GetValue(state, null);

            var multiCrit = MultiQuery();   //Мультизапрос
            var crit = Query<T>(ignoreDeletedRows: ignoreDeletedRows);  //Создаем критерий
            if (cond != null)
            {
                var sq = cond.Invoke(SubQuery<T>());
                crit.PropertyIn("Id", sq);
            }
            CreateFilter(crit);     // Фильтруем записи
            multiCrit.Add(crit.CountDistinct(true), "Count");    //Добавляем в мультикритерий запрос на кол-во выводимых строк

            SortByCriteria(crit, sort); //Сортируем выборку
            
            currentPage = (int)stateType.GetProperty("CurrentPage").GetValue(state, null);  //Получаем запрошенную страницу
            crit.SetFirstResult((currentPage - 1) * pageSize).SetMaxResults(pageSize);  //Выбираем эту страницу

            multiCrit.Add(crit, "Rows"); //Добавляем выборку строк в мультикритерий
            var result = multiCrit.List();  //Запрашиваем данные из БД

            int totalRowCount = (int)(result["Count"][0]);  //Получаем актуальное кол-во выводимых строк
            IList<T> rows = result["Rows"].Cast<T>().ToList<T>();  //Получаем строки запрошенной страницы

            WriteTotalRowCount(state, totalRowCount);   //Записываем общее кол-во строк

            state.GetType().GetMethod("CheckAndCorrectCurrentPage").Invoke(state, null);  //приводит текущую страницу к актуальному значению
            
            var newCurrentPage = (int)stateType.GetProperty("CurrentPage").GetValue(state, null);   //Получаем номер актуальной страницы
            if (newCurrentPage != currentPage)  //Если запрошенная не совпадает с актуальной, то ...
            {
                // получаем актуальную страницу
                crit.SetFirstResult((newCurrentPage - 1) * pageSize).SetMaxResults(pageSize);
                rows = crit.ToList<T>();
            }

            return rows;
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>        
        protected IList<T> GetBaseFilteredList<T>(object state, ParameterString parameterString, bool ignoreDeletedRows = true, Func<ISubCriteria<T>, ISubCriteria<T>> cond = null) where T : class
        {
            this.parameterString = parameterString;
            var result = GetBaseFilteredList<T>(state, ignoreDeletedRows, cond);
            this.parameterString = null;

            return result;
        }
        
        #endregion

        #region Сортировка

        protected virtual void SortByCriteria<T>(ERP.Infrastructure.Repositories.Criteria.ICriteria<T> criteria, string sortParams) where T : class
        {
            if (!String.IsNullOrEmpty(sortParams))
            {
                var fields = sortParams.Split(';');
                foreach (var row in fields)
                {
                    if (row.Length > 0)
                    {
                        var field = row.Split('=');
                        var path = field[0].Split('.').ToList();
                        var property = path[path.Count - 1];
                        var alias = RestrictByPath(path, criteria, "");
                        property = (alias.Length > 0 ? alias + "." : "") + property;

                        if (field[1].ToLower() == "asc")
                        {
                            criteria.OrderByAsc(property);
                        }
                        else
                        {
                            criteria.OrderByDesc(property);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ограничения по указанному пути (для того, чтобы NHibernate мог "резолвить" поля)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="crit"></param>
        /// <param name="lastAlias"></param>
        /// <returns></returns>
        private string RestrictByPath(IList<string> path, object crit, string lastAlias)
        {
            if (path.Count > 1)
            {
                var property = path[0];
                path.RemoveAt(0);
                var type = crit.GetType().GetGenericArguments()[0];
                var alias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");

                var method = crit.GetType()
                    .GetMethod("Restriction", new Type[] { typeof(string), typeof(string) })
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(new Type[] { type });
                crit = method.Invoke(crit, new object[] { property, alias });

                return RestrictByPath(path, crit, alias);
            }
            else
            {
                return lastAlias;
            }
        }

        #endregion

        #region Критерии

        /// <summary>
        /// Выполнение запроса с ограничениями
        /// </summary>
        public ERP.Infrastructure.Repositories.Criteria.ICriteria<T> Query<T>(bool ignoreDeletedRows = true, string alias = "") where T : class
        {
            return new ERP.Infrastructure.NHibernate.Repositories.Criteria.Criteria<T>(CurrentSession, alias, ignoreDeletedRows);
        }

        /// <summary>
        /// Подзапрос
        /// </summary>
        public ERP.Infrastructure.Repositories.Criteria.ISubCriteria<T> SubQuery<T>(bool ignoreDeletedRows = true) where T : class
        {
            return new ERP.Infrastructure.NHibernate.Repositories.Criteria.SubCriteria<T>();
        }

        /// <summary>
        /// Мультизапрос
        /// </summary>
        /// <returns></returns>
        private ERP.Infrastructure.Repositories.Criteria.IMultiCriteria MultiQuery()
        {
            return new ERP.Infrastructure.NHibernate.Repositories.Criteria.MultiCriteria(CurrentSession);
        }

        #endregion

        #region Новый фильтр

        /// <summary>
        /// Формирование фильтра в запросе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected virtual void CreateFilter<T>(ERP.Infrastructure.Repositories.Criteria.ICriteria<T> criteria) where T : class
        {
            CreateFilter<T>((object)criteria);
        }

        /// <summary>
        /// Формирование фильтра в подзапросе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subCriteria"></param>
        protected virtual void CreateFilter<T>(ERP.Infrastructure.Repositories.Criteria.ISubCriteria<T> subCriteria) where T : class
        {
            CreateFilter<T>((object)subCriteria);
        }
        
        private void CreateFilter<T>(object criteria) where T : class
        {
            ParameterString pStr = parameterString;
            if (pStr != null)
            {
                pStr.MergeWith(new ParameterString(filter));
            }
            else
            {
                pStr = new ParameterString(filter);
            }

            Type type = typeof(T);

            var tree = new Tree();
            tree.Parse(pStr, '_');

            foreach (var node in tree.Nodes)
            {
                CreateFilterForNode<T>(criteria, node, "");
            }
        }

        protected void CreateFilterForNode<T>(object criteria, TreeNode node, string path) where T : class
        {
            switch (node.Nodes.Count())
            {
                case 0:
                    CreateFilterForNodeWithNoChildNodes<T>(criteria, node, path);
                    break;
                case 1:

                    // обрабатываем вершину если она имеет значение
                    if (node.Value != null)
                    {
                        CreateFilterForNodeWithNoChildNodes<T>(criteria, node, path);
                    }
                
                    //Вершина промежуточная, передаем данные дальше.
                    var p = node.Name;
                    if (path.Length > 0)
                    {
                        p = path + '.' + node.Name;
                    }

                    CreateFilterForNode<T>(criteria, node.Nodes.ElementAt(0), p);
                    break;
                
                default:
                    //Потомков более одного - необходимо делать restriction.
                    var t = typeof(T);
                    if (path.Length > 0)
                    {
                        foreach (var val in path.Split('.'))
                        {
                            t = GetProperty(t, val);
                        }
                    }
                    t = GetProperty(t, node.Name);


                    //Делаем рестрикшн
                    var alias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");
                    var criterion = criteria.GetType().GetMethod("Restriction", new Type[] { typeof(string), typeof(string) }).MakeGenericMethod(t).
                        Invoke(criteria, new object[] { node.Name, alias });

                    // обрабатываем вершину если она имеет значение
                    if (node.Value != null)
                    {
                        CreateFilterForNodeWithNoChildNodes<T>(criteria, node, path);   
                    }

                    // Обрабатываем потомков
                    foreach (var val in node.Nodes)
                    {
                        this.GetType().GetMethod("CreateCriterionFilterForNode").MakeGenericMethod(t, typeof(T)).Invoke(this, new object[] { criterion, val, "" });
                    }                   
                    
                    break;
            }
        }

        protected void CreateFilterForNodeWithNoChildNodes<T>(object criteria, TreeNode node, string path) where T : class
        {
            Type fieldType;

            if (node.Value.Operation == ParameterStringItem.OperationType.Or)
            {
                CreateFilterConditionOR<T>(criteria, node.Value);
            }
            else
            {
                var fullPath = node.Name;
                if (path.Length > 0)
                    fullPath = path + '.' + node.Name;
                fieldType = GetProperty(typeof(T), fullPath);


                //Генерируем критерий
                if (node.Value != null /*&& node.Value.Value != null*/) //Проверяем нулевое ли значение
                {
                    //Строка содержит данные
                    if (fieldType != typeof(DateTime) && fieldType != typeof(DateTime?))
                    {
                        if (node.Value.Value != null)
                        {
                            if (fieldType == typeof(string))
                            {
                                if (node.Value.Operation == ParameterStringItem.OperationType.Eq)
                                    node.Value.Operation = ParameterStringItem.OperationType.Like;

                                CreateFilterCondition<T>(criteria, node.Value, false);
                            }
                            else
                            {
                                CreateFilterCondition<T>(criteria, node.Value, fieldType.IsClass);
                            }
                        }
                    }
                    else
                    {
                        var isDateRange = false;
                        var key = node.Value.Key.Replace('_', '.');
                        DateTime?[] dates = null;
                        var types = new Type[]
                                {
                                    typeof(string),
                                    typeof(CriteriaCond),
                                    typeof(object)
                                };
                        var methodInfo = criteria.GetType().GetMethod("Where", types);

                        if (node.Value.Value is string)
                        {
                            if (node.Value.Value != null)
                            {
                                var val= node.Value.Value as string;
                                isDateRange = val.Contains('-');
                                dates = GetDate(val);
                            }
                        }
                        else if (node.Value.Value is IList<string>)
                        {
                            var list = node.Value.Value as IList<string>;
                            if (!(list.Count == 1 && list[0] == null))
                            {
                                var val = (node.Value.Value as IList<string>)[0];
                                isDateRange = val.Contains('-');
                                dates = GetDate(val);
                            }
                        }


                        if (dates != null)
                        {
                            if (isDateRange)
                            {
                                methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Ge,dates[0] != null ? dates[0].Value.SetHoursMinutesAndSeconds(0, 0, 0) : dates[0] });
                                methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Le, dates[1] != null ? dates[1].Value.SetHoursMinutesAndSeconds(23, 59, 59) : dates[1] });
                            }
                            else
                            {
                                // Указана только одна дата
                                switch (node.Value.Operation)
                                {
                                    case ParameterStringItem.OperationType.Eq:
                                    case ParameterStringItem.OperationType.IsNull:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Eq, dates[0] });
                                        break;
                                    case ParameterStringItem.OperationType.NotEq:
                                    case ParameterStringItem.OperationType.IsNotNull:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.NotEq, dates[0] });
                                        break;
                                    case ParameterStringItem.OperationType.Ge:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Ge, dates[0] });
                                        break;
                                    case ParameterStringItem.OperationType.Gt:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Gt, dates[0] });
                                        break;
                                    case ParameterStringItem.OperationType.Le:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Le, dates[0] });
                                        break;
                                    case ParameterStringItem.OperationType.Lt:
                                        methodInfo.Invoke(criteria, new object[] { key, CriteriaCond.Lt, dates[0] });
                                        break;
                                    default:
                                        throw new Exception(String.Format("Задано неверное условие c датой «{0}»", key));
                                }
                                
                            }
                        }
                        else
                        {
                            CriteriaCond cond;
                            if (node.Value.Operation == ParameterStringItem.OperationType.IsNull)
                            {
                                cond = CriteriaCond.Eq;
                            }
                            else if (node.Value.Operation == ParameterStringItem.OperationType.IsNotNull)
                            {
                                cond = CriteriaCond.NotEq;
                            }
                            else
                            {
                                throw new Exception(String.Format("Задано неверное условие при сравнении даты «{0}» с null", key));
                            }
                            methodInfo.Invoke(criteria, new object[] { key, cond, null });
                        }
                    }
                }
            }
        }

        private void CreateFilterConditionOR<T>(object criteria, ParameterStringItem value) where T : class
        {
            Type type;
            var list = value.Value as List<ParameterStringItem>;

            type = GetProperty(typeof(T), list[0].Key);
            if (type != typeof(string))
            {
                CreateFilterCondition<T>(criteria, list[0], type.IsClass);
            }
            else
            {
                if (list[0].Operation == ParameterStringItem.OperationType.Eq)
                {
                    list[0].Operation = ParameterStringItem.OperationType.Like;
                }
                CreateFilterCondition<T>(criteria, list[0], false);
            }

            type = GetProperty(typeof(T), list[1].Key);
            if (type != typeof(string))
            {
                CreateFilterCondition<T>(criteria, list[1], type.IsClass);
            }
            else
            {
                if (list[1].Operation == ParameterStringItem.OperationType.Eq)
                {
                    list[1].Operation = ParameterStringItem.OperationType.Like;
                }
                CreateFilterCondition<T>(criteria, list[1], false);
            }
            

            criteria.GetType().GetMethod("Or", System.Type.EmptyTypes).Invoke(criteria, null);
        }

        private void CreateFilterCondition<T>(object criteria, ParameterStringItem value, bool isClass) where T : class
        {
            var key = value.Key.Replace("_", ".");

            var crit = CreateCriteria<T>(key, criteria);
            object val = "";
            if (value.Value is IList<string>)
            {
                //Проверяем наличие значений, если нет, то выходим
                if ((value.Value as IList<string>).Count == 0)
                    return;
                //Значения есть, обрабатываем
                val = (value.Value as IList<string>)[0];
            }
            else
            {
                val = value.Value;
            }

            var propKey = key.Substring(key.LastIndexOf('.') + 1);

            switch (value.Operation)
            {
                case ParameterStringItem.OperationType.Like:
                    if ((val as string).Length > 0)
                    {
                        crit = crit.GetType().GetMethod("Like", new Type[] { typeof(string), typeof(string) })
                            .Invoke(crit, new object[] { propKey, val });
                    }
                    break;
                case ParameterStringItem.OperationType.Eq:
                    if (val == null || ((val is Guid) || (val is string && (val as string).Length > 0)))
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.Eq, val });
                    }
                    break;
                case ParameterStringItem.OperationType.NotEq:
                    if (val == null || (val as string).Length > 0)
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.NotEq, val });
                    }
                    break;
                case ParameterStringItem.OperationType.OneOf:
                    crit = crit.GetType().GetMethod("OneOf", new Type[] { typeof(string), typeof(IEnumerable) })
                        .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), value.Value });
                    break;
                case ParameterStringItem.OperationType.NotOneOf:
                    crit = crit.GetType().GetMethod("NotOneOf", new Type[] { typeof(string), typeof(IEnumerable) })
                        .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), value.Value });
                    break;
                case ParameterStringItem.OperationType.Gt:
                    if (val == null || ((val is Guid) || (val is string && (val as string).Length > 0)))
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.Gt, val });
                    }
                    break;
                case ParameterStringItem.OperationType.Ge:
                    if (val == null || ((val is Guid) || (val is string && (val as string).Length > 0)))
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.Ge, val });
                    }
                    break;
                case ParameterStringItem.OperationType.Lt:
                    if (val == null || ((val is Guid) || (val is string && (val as string).Length > 0)))
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.Lt, val });
                    }
                    break;
                case ParameterStringItem.OperationType.Le:
                    if (val == null || ((val is Guid) || (val is string && (val as string).Length > 0)))
                    {
                        crit = crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { propKey + (isClass ? ".Id" : ""), CriteriaCond.Le, val });
                    }
                    break;
            }

        }

        public void CreateCriterionFilterForNode<T, TParent>(object criterion, TreeNode node, string path) where T : class where TParent: class 
        {
            switch (node.Nodes.Count())
            {
                case 0:
                    //Конечная вершина. Генерируем критерий
                     var fullPath = node.Name;
                    if (path.Length > 0)
                        fullPath = path + '.' + node.Name;
                    var fieldType = GetProperty(typeof(T), fullPath);

                    //Конечная вершина. Генерируем критерий
                    if (node.Value != null && node.Value.Value != null) //Проверяем нулевая ли строка
                    {

                        //Строка содержит данные
                        if (fieldType != typeof(DateTime))
                        {
                            if (fieldType == typeof(string))
                            {
                                if (node.Value.Operation == ParameterStringItem.OperationType.Eq)
                                    node.Value.Operation = ParameterStringItem.OperationType.Like;

                                CreateFilterCondition<T, TParent>(criterion, node.Value, fullPath, false);
                            }
                            else
                            {
                                CreateFilterCondition<T, TParent>(criterion, node.Value, fullPath, fieldType.IsClass);
                            }
                        }
                        else
                        {
                            var key = node.Value.Key.Replace('_', '.');
                            var dates = GetDate(node.Value.Value as string);

                            var types = new Type[]
                                {
                                    typeof(string),
                                    typeof(CriteriaCond),
                                    typeof(object)
                                };
                            var methodInfo = criterion.GetType().GetMethod("Where", types);
                            methodInfo.Invoke(criterion, new object[] { key, CriteriaCond.Ge, dates[0] });
                            methodInfo.Invoke(criterion, new object[] { key, CriteriaCond.Le, dates[1] });
                        }
                    }
                    break;
                case 1:
                    //Вершина промежуточная, передаем данные дальше.
                    var p2 = node.Name;
                    if (path.Length > 0)
                    {
                        p2 = path + '.' + node.Name;
                    }
                    CreateCriterionFilterForNode<T, TParent>(criterion, node.Nodes.ElementAt(0), p2);
                    break;
                default:
                    //Потомков более одного - необходимо делать restriction.
                    var t = typeof(T);
                    foreach (var val in path.Split('.'))
                    {
                        t = GetProperty(t, val);
                    }
                    t = GetProperty(t, node.Name);

                    var alias = "alias_" + Guid.NewGuid().ToString().Replace("-", "");
                    var crit = criterion.GetType().GetMethod("Restriction", new Type[] { typeof(string), typeof(string) }).MakeGenericMethod(t)
                        .Invoke(criterion, new object[] { node.Name, alias });
                    
                    var p3 = node.Name;
                    if (path.Length > 0)
                    {
                        p3 = path + '.' + node.Name;
                    }

                    foreach (var val in node.Nodes)
                    {
                        CreateCriterionFilterForNode<T, TParent>(crit, val, p3);                        
                    }
                    break;
            }
        }

        private void CreateFilterCondition<T, TParent>(object criterion, ParameterStringItem value, string key, bool isClass)
            where T : class
            where TParent : class
        {
            object crit = criterion;
            int last = key.LastIndexOf('.');
            if (last > 0)
            {
                crit = CreateCriteria<T>(key, criterion);
            }

            object val = "";
            if (value.Value is IList<string>)
            {
                val = (value.Value as IList<string>)[0];
            }
            else
            {
                val = value.Value;
            }

            switch (value.Operation)
            {
                case ParameterStringItem.OperationType.Like:
                    if ((val as string).Length > 0)
                    {
                        crit.GetType().GetMethod("Like", new Type[] { typeof(string), typeof(string) })
                            .Invoke(crit, new object[] { key.Substring(last+1), val });
                    }
                    break;
                case ParameterStringItem.OperationType.Eq:
                    if (val == null || (val as string).Length > 0)
                    {
                        crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { key.Substring(last + 1) + (isClass ? ".Id" : ""), CriteriaCond.Eq, val });
                    }
                    break;
                case ParameterStringItem.OperationType.NotEq:
                    if (val == null || (val as string).Length > 0)
                    {
                        crit.GetType().GetMethod("Where", new Type[] { typeof(string), typeof(CriteriaCond), typeof(object) })
                            .Invoke(crit, new object[] { key.Substring(last + 1) + (isClass ? ".Id" : ""), CriteriaCond.NotEq, val });
                    }
                    break;
                case ParameterStringItem.OperationType.OneOf:
                    crit.GetType().GetMethod("OneOf", new Type[] { typeof(string), typeof(IEnumerable) })
                        .Invoke(crit, new object[] { key.Substring(last + 1) + (isClass ? ".Id" : ""), value.Value });
                    break;
                case ParameterStringItem.OperationType.NotOneOf:
                    crit.GetType().GetMethod("NotOneOf", new Type[] { typeof(string), typeof(IEnumerable) })
                        .Invoke(crit, new object[] { key.Substring(last + 1) + (isClass ? ".Id" : ""), value.Value });
                    break;
            }

        }
        
        #endregion
        
    }
}