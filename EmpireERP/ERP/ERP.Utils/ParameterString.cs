using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Utils
{
    public class ParameterString
    {
        #region Поля

        private Dictionary<string, ParameterStringItem> Params;

        #endregion

        #region Конструкторы

        public ParameterString(string parameters)
        {
            Params = new Dictionary<string, ParameterStringItem>();
            AddParametersFromString(parameters);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Парсинг строки и добавление параметров
        /// </summary>
        /// <param name="parameters"></param>
        public void AddParametersFromString(string parameters)
        {
            if (parameters != null && parameters.Length > 1)
            {
                string[] p = parameters.Split(';');
                foreach (var parametr in p)
                {
                    if (parametr.Length > 1)
                    {
                        var item = new ParameterStringItem(parametr);
                        Params.Add(item.Key, item);
                    }
                }
            }
        }

        /// <summary>
        /// Доступ к элементу (read-only)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ParameterStringItem this[string key]
        {
            get
            {
                ParameterStringItem result = null;
                if (Params.ContainsKey(key))
                    result = Params[key];

                return result;
            }
        }

        /// <summary>
        /// Имеющиеся параметры (read-only)
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return Params.Keys;
            }
        }

        /// <summary>
        /// Приведение к строке
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (var key in Params.Keys)
            {
                result.Append(Params[key].ToString());
            }

            return result.ToString();
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="item"></param>
        public ParameterStringItem Add(ParameterStringItem item)
        {
            Params.Add(item.Key, item);

            return item;
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="operation">Операция</param>
        /// <returns>Параметр строки</returns>
        public ParameterStringItem Add(string key, ParameterStringItem.OperationType operation)
        {
            var item = new ParameterStringItem(key, operation);
            Params.Add(key, item);

            return item;
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="operation">Операция</param>
        /// <returns>Параметр строки</returns>
        public ParameterStringItem Add(string key, ParameterStringItem.OperationType operation, string value)
        {
            var item = Add(key, operation);
            (item.Value as IList<string>).Add(value);

            return item;
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="key">Ключ параметра</param>
        /// <param name="operation">Операция</param>
        /// <returns>Параметр строки</returns>
        public ParameterStringItem Add(string key, ParameterStringItem.OperationType operation, IEnumerable<string> value)
        {
            var item = Add(key, operation);
            (item.Value as List<string>).AddRange(value);

            return item;
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Левый операнд</param>
        /// <returns>Параметр строки</returns>
        public ParameterStringItem Add(ParameterStringItem.OperationType operation, ParameterStringItem left, ParameterStringItem right)
        {
            if (operation != ParameterStringItem.OperationType.Or)
            {
                throw new Exception("Данная операция не реализована.");
            }

            var key = Guid.NewGuid().ToString();
            var item = new ParameterStringItem(key, left, right);
            Params.Add(key, item);

            return item;
        }

        /// <summary>
        /// Удаление параметра. Если указанного ключа в коллекции нет, то ничего не произойдет (не будет выброшено исключения).
        /// </summary>
        /// <param name="key">Ключ параметра.</param>
        public void Delete(string key)
        {
            if (Keys.Contains(key))
            {
                Params.Remove(key);
            }
        }

        /// <summary>
        /// Содержит ли по указанному ключу значение, не равное пустой строке.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <returns>True, если по указанному ключу содержится значение, не равное пустой строке.</returns>
        public bool ContainsNonEmptyString(string key)
        {
            return Params.ContainsKey(key) && Params[key].Value as string != "";
        }

        /// <summary>
        /// Слияние двух коллекций значений
        /// </summary>
        /// <param name="ps"></param>
        /// <remarks>
        /// Правила слияния:
        /// 1 и 1,2 => 1
        /// 1 и 2,3 => 0
        /// "" и 1,2 => 1,2
        /// </remarks>
        public void MergeWith(ParameterString ps)
        {
            bool mergeKey = false;
            foreach (var key2 in ps.Keys)
            {
                mergeKey = false;
                foreach (var key1 in Keys)
                {
                    if (key1 == key2)
                    {
                        var vals1 = Params[key1].Value is string ? new List<string>() { Params[key1].Value as string } : Params[key1].Value as IList<string>;
                        var vals2 = ps[key2].Value is string ? new List<string>() { ps[key2].Value as string } : ps[key2].Value as IList<string>;

                        // удаляем пустые значения
                        vals1 = vals1.Where(x => x != "").ToList();
                        vals2 = vals2.Where(x => x != "").ToList();

                        IList<string> merge = new List<string>();
                        if (vals1.Count == 0 && vals2.Count != 0)
                        {
                            merge = vals2;
                        }
                        else if (vals2.Count == 0 && vals1.Count != 0)
                        {
                            merge = vals1;
                        }
                        else if (vals2.Count != 0 && vals1.Count != 0)
                        {
                            merge = IntersectionList(vals1, vals2);
                            if (merge.Count == 0)
                            {
                                merge = new List<string>() { "0" };
                            }
                        }

                        Params[key1].Value = merge;
                        mergeKey = true;
                        break;
                    }
                }
                if (mergeKey == false)
                {
                    var val = ps[key2].Value is string ? new List<string>() { ps[key2].Value as string } : ps[key2].Value as IList<string>;
                    if (val.Count() == 1)
                    {
                        if (val[0].Length == 0)
                        {
                            Params.Add(key2, new ParameterStringItem(key2, ParameterStringItem.OperationType.Eq, (string)null));
                        }
                        else
                        {
                            Params.Add(key2, new ParameterStringItem(key2, ParameterStringItem.OperationType.Eq, val[0]));
                        }
                    }
                    else
                    {
                        Params.Add(key2, new ParameterStringItem(key2, ParameterStringItem.OperationType.OneOf, val));
                    }
                }
            }
        }

        private IList<string> IntersectionList(IList<string> vals1, IList<string> vals2)
        {
            List<string> result = new List<string>();

            foreach (var str in vals1)
            {
                if (vals2.Contains(str))
                {
                    result.Add(str);
                }
            }

            return result;
        }

        private IList<string> UnionList(IList<string> vals1, IList<string> vals2)
        {
            List<string> result = new List<string>();

            foreach (var str in vals1)
            {
                if (!result.Contains(str))
                {
                    result.Add(str);
                }
            }
            foreach (var str in vals2)
            {
                if (!result.Contains(str))
                {
                    result.Add(str);
                }
            }

            return result;
        }

        #endregion
    }
}