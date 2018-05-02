using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderSelectorViewModel
    {
        public string Title { get; set; }
        
        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид заказов
        /// </summary>
        public GridData Grid { get; set; }
    }
}
