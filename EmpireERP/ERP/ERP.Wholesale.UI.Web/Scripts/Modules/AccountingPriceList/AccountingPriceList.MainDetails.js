var AccountingPriceList_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            
            if (IsTrue($("#AllowToViewReceiptWaybillDetails").val())) {
                var waybillId = $("#ReasonReceiptWaybillId").val();
                $("#ReasonDescription").attr("href", "/ReceiptWaybill/Details?id=" + waybillId + "&backURL=" + currentUrl);
            }
            else {
                $("#ReasonDescription").addClass("disabled");
            }

            if (IsTrue($("#AllowToViewCuratorDetails").val())) {
                var userId = $("#CuratorId").val();
                $("#CuratorName").attr("href", "/User/Details?id=" + userId + "&backURL=" + currentUrl);
            }
            else {
                $("#CuratorName").addClass("disabled");
            }
        });
    }
}; 