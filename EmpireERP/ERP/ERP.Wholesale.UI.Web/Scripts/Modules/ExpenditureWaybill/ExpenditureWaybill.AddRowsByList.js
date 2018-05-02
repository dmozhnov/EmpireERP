var ExpenditureWaybill_AddRowsByList = {
    Init: function () {
        $(document).ready(function () {
            Article_AddRowsByList.Init();

            Waybill_ForWaybillRowsAdditionByListGrid.actionName = "/ExpenditureWaybill/AddRowSimply/";
            Waybill_ForWaybillRowsAdditionByListGrid.gridId = "gridExpenditureWaybillRows";
            Waybill_ForWaybillRowsAdditionByListGrid.messageId = "messageExpenditureWaybillRowList"  
        });
    }
};
