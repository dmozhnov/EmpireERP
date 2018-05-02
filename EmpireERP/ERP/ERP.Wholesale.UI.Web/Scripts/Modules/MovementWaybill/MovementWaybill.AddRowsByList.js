var MovementWaybill_AddRowsByList = {
    Init: function () {
        $(document).ready(function () {
            Article_AddRowsByList.Init();

            Waybill_ForWaybillRowsAdditionByListGrid.actionName = "/MovementWaybill/AddRowSimply/";
            Waybill_ForWaybillRowsAdditionByListGrid.gridId = "gridMovementWaybillRows";
            Waybill_ForWaybillRowsAdditionByListGrid.messageId = "messageMovementWaybillRowList"  
        });
    }
};
