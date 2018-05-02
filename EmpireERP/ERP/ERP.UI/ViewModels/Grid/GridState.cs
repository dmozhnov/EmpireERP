
using System.Diagnostics;
using System;
namespace ERP.UI.ViewModels.Grid
{
    /// <summary>
    /// Состояние грида
    /// </summary>
    public class GridState : ICloneable
    {
        #region Текущая страница
        
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int? CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                if (value != null)
                    currentPage = value.Value;
                else
                    currentPage = 1;
            }
        }
        private int currentPage; 
        
        public void CheckAndCorrectCurrentPage()
        {
            int max = (totalRow + pageSize - 1) / pageSize;

            if (max < 1)
                max = 1;

            if (currentPage < 1)
                currentPage = 1;

            if (currentPage > max)
                currentPage = max;
        }
        #endregion

        #region Размер страницы
        
        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value > 0)
                {
                    pageSize = value;
                }
            }
        }
        private int pageSize; 
        
        #endregion
        
        #region Дополнительные параметры таблицы
        
        /// <summary>
        /// Дополнительные параметры таблицы
        /// </summary>
        public string Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                if (value != null)
                    parameters = value;
                else
                    parameters = string.Empty;
            }
        }
        private string parameters; 
        
        #endregion

        #region Фильтр по данным
        
        /// <summary>
        /// Фильтр по данным
        /// </summary>
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (value != null)
                    filter = value;
                else
                    filter = string.Empty;
            }
        }
        private string filter;
        
        #endregion
        
        #region Общее количество строк
        
        /// <summary>
        /// Общее количество строк
        /// </summary>
        public int TotalRow
        {
            get
            {
                return totalRow;
            }
            set
            {
                if (value >= 0)
                {
                    totalRow = value;
                }
            }
        }
        private int totalRow;

        #endregion

        /// <summary>
        /// Сортировка данных грида
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        [DebuggerStepThrough]
        public GridState()
        {
            CurrentPage = 1;
            PageSize = 10;
            Parameters = string.Empty;
            Filter = string.Empty;
            Sort = string.Empty;
            TotalRow = 0;
        }

        public object Clone()
        {
            return new GridState()
            {
                CurrentPage = this.CurrentPage,
                Filter = this.Filter,
                PageSize = this.PageSize,
                Parameters = this.Parameters,
                Sort = this.Sort,
                TotalRow = this.TotalRow
            };
        }
    }
}