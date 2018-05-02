var ChangeOwnerWaybill_AddRowsByList = {
    Init: function () {
        $(document).ready(function () {
            Article_AddRowsByList.Init();

            Waybill_ForWaybillRowsAdditionByListGrid.actionName = "/ChangeOwnerWaybill/AddRowSimply/";
            Waybill_ForWaybillRowsAdditionByListGrid.gridId = "gridChangeOwnerWaybillRow";
            Waybill_ForWaybillRowsAdditionByListGrid.messageId = "messageChangeOwnerWaybillRowList"            
        });
    }
};
