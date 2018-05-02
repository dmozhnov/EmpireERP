var Deal_Details_QuotaGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridDealSales table.grid_table tr").each(function (i, el) {
                var expenditureId = $(this).find(".ExpenditureWaybillId").text();
                $(this).find("a.Number").attr("href", "/ExpenditureWaybill/Details?Id=" + expenditureId + "&backURL=" + currentUrl);

                var storageId = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?Id=" + storageId + "&backURL=" + currentUrl);
            });

            $('#btnAddQuota').click(function () {
                var dealId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/DealQuota/SelectDealQuota",
                    data: { dealId: dealId, mode: "Deal" },
                    success: function (result) {
                        $('#dealQuotaSelect').hide().html(result);
                        $.validator.unobtrusive.parse($("#dealQuotaSelect"));
                        ShowModal("dealQuotaSelect");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, 'messageDealQuotaList');
                    }
                });
            });

            $('#btnAddAllQuotas').click(function () {
                var dealId = $("#Id").val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "POST",
                    url: "/Deal/AddAllQuotas",
                    data: { dealId: dealId },
                    success: function (result) {
                        RefreshGrid("gridDealQuota", function () {
                            Deal_Details.RefreshPermissions(result.Indicators.Permissions);
                            ShowSuccessMessage("Добавлено квот: " + result.AddedCount, "messageDealQuotaList");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, 'messageDealQuotaList');
                    }
                });
            });

            $("#gridDealQuota .delete_link").bind("click", function () {
                if (confirm("Вы уверены?")) {
                    var dealId = $("#Id").val();
                    var quotaId = $(this).parent("td").parent("tr").find(".quotaId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Deal/RemoveQuota",
                        data: { dealId: dealId, quotaId: quotaId },
                        success: function (result) {
                            RefreshGrid("gridDealQuota", function () {
                                Deal_Details.RefreshPermissions(result.Permissions);
                                ShowSuccessMessage("Квота удалена.", "messageDealQuotaList"); 
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealQuotaList");
                        }
                    });
                }
            });
        });
    }
}; 