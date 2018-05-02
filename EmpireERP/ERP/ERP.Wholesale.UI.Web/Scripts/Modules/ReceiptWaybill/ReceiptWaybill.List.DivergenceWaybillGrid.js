var ReceiptWaybill_List_DivergenceWaybillGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridDivergenceWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/ReceiptWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                var providerId = $(this).find(".providerId").text();
                $(this).find("a.ProviderName").attr("href", "/Provider/Details?id=" + providerId + "&backURL=" + currentUrl);

                var accountOrganizationId = $(this).find(".accountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + accountOrganizationId + "&backURL=" + currentUrl);
            });           
        });
    }
}; 