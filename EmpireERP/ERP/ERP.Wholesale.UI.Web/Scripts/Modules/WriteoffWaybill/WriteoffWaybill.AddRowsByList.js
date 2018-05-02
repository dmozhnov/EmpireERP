var WriteoffWaybill_AddRowsByList = {
    Init: function () {
        $(document).ready(function () {
            Article_AddRowsByList.Init();

            Waybill_ForWaybillRowsAdditionByListGrid.actionName = "/WriteoffWaybill/AddRowSimply/";
            Waybill_ForWaybillRowsAdditionByListGrid.gridId = "gridWriteoffWaybillRows";
            Waybill_ForWaybillRowsAdditionByListGrid.messageId = "messageWriteoffWaybillRowList"  
        });
    }
};
