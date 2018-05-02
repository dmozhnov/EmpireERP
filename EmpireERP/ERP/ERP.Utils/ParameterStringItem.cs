using System.Collections.Generic;

namespace ERP.Utils
{
    public class ParameterStringItem
    {
        #region Поля

        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Тип операции
        /// </summary>
        public enum OperationType { OneOf, NotOneOf, Eq, NotEq, Like, Or, IsNull, IsNotNull, Gt, Ge, Lt, Le };

        /// <summary>
        /// Операция
        /// </summary>
        public OperationType Operation { get; set; }

        /// <summary>
        /// Значения
        /// </summary>
        public object Value { get; set; }

        #endregion

        #region Конструктор

        public ParameterStringItem(string key, OperationType operation, IEnumerable<string> values)
            : this(key, operation)
        {
            (Value as List<string>).AddRange(values);
        }

        public ParameterStringItem(string key, OperationType operation, string value)
            : this(key, operation)
        {
            Value = value;
        }

        public ParameterStringItem(string key, OperationType operation)
        {
            Key = key;
            Operation = operation;
            if (operation == OperationType.IsNull || operation == OperationType.IsNotNull)
            {
                Value = null;
            }
            else
            {
                Value = new List<string>();
            }
        }

        /// <summary>
        /// Параметры объединяются через Or
        /// </summary>
        /// <param name="key"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ParameterStringItem(string key, ParameterStringItem left, ParameterStringItem right)
        {
            Key = key;
            Operation = OperationType.Or;
            Value = new List<ParameterStringItem>() 
            {
                left,
                right
            };
        }

        public ParameterStringItem(string str)
        {
            Operation = OperationType.Eq;
            string[] value = str.Split('=');
            Key = value[0];
            var vals = value[1].Split(',');
            if (vals.Length > 1)
            {
                var val = new List<string>();
                foreach (var p in vals)
                {
                    if (p.Length > 0)
                        val.Add(p);
                }
                if (val.Count > 1)
                {
                    Operation = OperationType.OneOf;
                }
                else
                {
                    Operation = OperationType.Eq;
                }
                Value = val;
            }
            else
                Value = value[1];

        }

        #endregion

        #region Методы

        public override string ToString()
        {
            string result = "";
            if (Value is IList<string>)
            {
                foreach (var val in Value as IList<string>)
                {
                    if (result.Length > 0)
                        result += ',' + val;
                    else
                        result = val;
                }

                result = Key + '=' + result + ';';
            }
            else
                result = Key + '=' + (Value as string) + ';';

            return result;
        }

        #endregion
    }
}
