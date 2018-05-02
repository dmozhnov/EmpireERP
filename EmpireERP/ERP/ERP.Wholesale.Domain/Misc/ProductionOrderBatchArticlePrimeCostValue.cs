namespace ERP.Wholesale.Domain.Misc
{
    /// <summary>
    /// Информация о рассчитанной себестоимости заказа или партии заказа по трем позициям:
    /// по текущим показателям (это могут быть плановые показатели, факт или оплаты из 2 - 3 полей), по факту и по оплатам
    /// </summary>
    public class ProductionOrderBatchArticlePrimeCostValue
    {
        #region Свойства

        /// <summary>
        /// Текущий показатель
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Фактический показатель
        /// </summary>
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Показатель, вычисленный по оплатам
        /// </summary>
        public decimal PaymentValue { get; set; }

        #endregion
    }
}
