var AccountingPriceList_Shared = {
        RefreshMainDetails:function(details) {
            $('#priceListDetailsPurchaseCostSum').text(details.PurchaseCostSum);
            $('#priceListDetailsOldAccountingPriceSum').text(details.OldAccountingPriceSum);
            $('#priceListDetailsNewAccountingPriceSum').text(details.NewAccountingPriceSum);
            $('#priceListDetailsAccountingPriceDifPercent').text(details.AccountingPriceDifPercent);
            $('#priceListDetailsAccountingPriceDifSum').text(details.AccountingPriceDifSum);
            $('#priceListDetailsPurchaseMarkupPercent').text(details.PurchaseMarkupPercent);
            $('#priceListDetailsPurchaseMarkupSum').text(details.PurchaseMarkupSum);
            $('#priceListDetailsRowCount').text(details.RowCount);
            $('#priceListDetailsDistribution').text(details.DistributionName);

            UpdateButtonAvailability("btnAccept", details.AllowToAccept);
            UpdateElementVisibility("btnAccept", details.AllowToAccept);
     }
};