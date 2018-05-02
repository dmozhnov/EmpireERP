var Deal_Details_SalesGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridDealSales table.grid_table tr").each(function (i, el) {
                var expenditureId = $(this).find(".ExpenditureWaybillId").text();
                $(this).find("a.Number").attr("href", "/ExpenditureWaybill/Details?Id=" + expenditureId + "&backURL=" + currentUrl);

                var storageId = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?Id=" + storageId + "&backURL=" + currentUrl);
            });

            $("#btnCreateExpenditureWaybill").click(function () {
                var dealId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/ExpenditureWaybill/CheckPosibilityToCreateExpenditureWaybill",
                    data: { id: dealId },
                    success: function (result) {
                        window.location = "/ExpenditureWaybill/Create?Dealid=" + $("#Id").val() + GetBackUrl();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageSalesGrid");
                    }
                });
            });
        });
    }
}; 