var ReceiptWaybill_List = {
    Init: function () {
        $(document).ready(function () {
            $("#btnCreateReceiptWaybill").live("click", function () {
                window.location = "/ReceiptWaybill/Create?" + GetBackUrl(true);
            });
        });
    }
}; 