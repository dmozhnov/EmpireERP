using System.Collections.Generic;
using System.Linq;
using ERP.UI.ViewModels.Grid;

namespace ERP.UI.Utils
{
    public static class GridUtils
    {
        /// <summary>
        /// Получение текущей страницы грида
        /// </summary>
        /// <typeparam name="T">Тип данных, выводимых в грид</typeparam>
        /// <param name="collection">Коллекция значений, выводимых в грид</param>
        /// <param name="state">Состояние грида</param>
        /// <returns>Коллекция, содержащая данные текущей страницы</returns>
        public static IEnumerable<T> GetEntityRange<T>(IEnumerable<T> collection, GridState state) where T : class
        {
            state.TotalRow = collection.Count();
            state.CheckAndCorrectCurrentPage();

            int count;
            if (state.CurrentPage * state.PageSize < state.TotalRow)
                count = state.CurrentPage.Value * state.PageSize - (state.CurrentPage.Value - 1) * state.PageSize;
            else
                count = state.TotalRow - (state.CurrentPage.Value - 1) * state.PageSize;

            return collection.Skip((state.CurrentPage.Value - 1) * state.PageSize).Take(count);
        }
    }
}
