var ClientOrganization_Details_SalesGrid = {
    Init: function () {
        $(document).ready(function () {
            
            $("#gridSaleWaybill table.grid_table tr").each(function (i, el) {
                var saleId = $(this).find(".SaleId").text();
                $(this).find("a.Number").attr("href", "/ExpenditureWaybill/Details?Id=" + saleId + GetBackUrl());

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?Id=" + dealId + GetBackUrl());

                var storageId = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?Id=" + storageId + GetBackUrl());
            });

            //Новая реализация товаров
            $("#btnCreateSaleWaybill").click(function () {
                window.location = "/ExpenditureWaybill/Create?clientOrganizationId=" + $("#Id").val() + GetBackUrl();
            });
        });
    }
};