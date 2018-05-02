using System.Collections.Generic;
using System.Linq;
using System;

namespace ERP.Utils
{
    /// <summary>
    /// Динамический массив
    /// </summary>
    /// <remarks>
    /// При объявлении в памяти создается только экземпляр класса. Коллекции заполняются по мере обращения к ним. 
    /// Единственный минус - в качестве класса для дженерика подходит только класс имеющий конструктор без параметров. 
    /// </remarks>    
    public class DynamicDictionary<TT, T> : Dictionary<TT, T> where T : new()
    {
        public new T this[TT index]
        {
            get
            {
                if (!ContainsKey(index))
                {
                    Add(index, new T());
                }

                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }  
    }

    public static class DynamicDictionaryExtension
    {

        /// <summary>
        /// Создает словарь DynamicDictionary из объекта IEnumerable в соответствии с заданной функцией селектора ключа и значения.
        /// </summary>
        /// <typeparam name="TSource">Тип элементов последовательности source.</typeparam>
        /// <typeparam name="TKey">Тип ключа, возвращаемого функцией keySelector.</typeparam>
        /// <typeparam name="TElement">Тип ключа, возвращаемого функцией elementSelector.</typeparam>
        /// <param name="source">Объект IEnumerable, на основе которого создается словарь DynamicDictionary.</param>
        /// <param name="keySelector">Функция, извлекающая ключ из каждого элемента.</param>
        /// <param name="elementSelector">Функция, извлекающая значение из каждого элемента.</param>
        /// <param name="aggregateSelector">Функция, объединяющая предыдущее записанное в словаре значение с новым в случае дублирования ключа.</param>
        /// <returns></returns>
        public static DynamicDictionary<TKey, TElement> ToDynamicDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
            Func<TElement, TSource, TElement> aggregateSelector = null) where TElement : new()
        {
            var result = new DynamicDictionary<TKey, TElement>();

            foreach (var element in source)
            {
                var key = keySelector(element);
                
                if (result.ContainsKey(key) && aggregateSelector != null)
                {
                    result[key] = aggregateSelector(result[key], element);
                }
                else
                {
                    result[keySelector(element)] = elementSelector(element);
                }
            }   

            return result;
        }

        /// <summary>
        /// Транспонирует матрицу из двух DynamicDictionary. Например, если на вход подать DynamicDictionary&lt;int, DynamicDictionary&lt;short, decimal&gt;&gt;, то на выходе будет DynamicDictionary&lt;short, DynamicDictionary&lt;int, decimal&gt;&gt;.
        /// </summary>
        /// <typeparam name="TMasterKey"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DynamicDictionary<TKey, DynamicDictionary<TMasterKey, TElement>> Transpose<TMasterKey, TKey, TElement>(this DynamicDictionary<TMasterKey, DynamicDictionary<TKey, TElement>> source) where TElement: new()
        {
            var result = new DynamicDictionary<TKey, DynamicDictionary<TMasterKey, TElement>>();
            
            var flattenDictionary = source.SelectMany(x => x.Value.Select(y => new { MasterKey = x.Key, Key = y.Key, Value = y.Value } ));
            
            result = flattenDictionary.ToDynamicDictionary<dynamic, TKey, DynamicDictionary<TMasterKey, TElement>>(x => x.Key, 
                x => { var a = new DynamicDictionary<TMasterKey, TElement>(); a.Add(x.MasterKey, x.Value); return a; },
                (x, y) => { x.Add(y.MasterKey, y.Value); return x; });            

            return result;            
        }
    }
}
