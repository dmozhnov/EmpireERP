using System.Collections;
using System.Collections.Generic;

namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Ячейка операций
    /// </summary>
    public class GridActionCell : GridCell, IEnumerable<GridActionCell.Action>
    {
        /// <summary>
        /// Класс операции
        /// </summary>
        public class Action
        {
            /// <summary>
            /// Название операции
            /// </summary>
            public string ActionName { get; set; }

            /// <summary>
            /// Ключ операции
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Конструктор операции
            /// </summary>
            /// <param name="actionName">Название операции</param>
            /// <param name="key">Ключ операции</param>
            public Action(string actionName, string key)
            {
                ActionName = actionName;
                Key = key;
            }
        }

        /// <summary>
        /// Перечень операций
        /// </summary>
        private List<Action> Actions = new List<Action>();

        /// <summary>
        /// Кол-во действий
        /// </summary>
        public int ActionCount
        {
            get { return Actions.Count; }
        }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        /// <param name="actions">Операции</param>
        public GridActionCell(string parentColumn) : base(parentColumn) { }

        /// <summary>
        /// Конструктор ячейки
        /// </summary>
        /// <param name="parentColumn">Имя родительского столбца</param>
        /// <param name="actions">Операции</param>
        public GridActionCell(string parentColumn, params Action[] actions)
            : base(parentColumn)
        {
            Actions = new List<Action>();
            foreach (var action in actions)
                AddAction(action);
        }

        /// <summary>
        /// Добавление операции
        /// </summary>
        /// <param name="action">Операция</param>
        public void AddAction(Action action)
        {
            Actions.Add(action);
        }

        /// <summary>
        /// Добавление операции
        /// </summary>
        /// <param name="actionName">Название операции</param>
        /// <param name="key">Ключ операции</param>
        public void AddAction(string actionName, string key)
        {
            Actions.Add(new Action(actionName, key));
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator<Action> IEnumerable<Action>.GetEnumerator()
        {
            return Actions.GetEnumerator();
        }

        /// <summary>
        /// Итератор
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Actions.GetEnumerator();
        }
    }
}