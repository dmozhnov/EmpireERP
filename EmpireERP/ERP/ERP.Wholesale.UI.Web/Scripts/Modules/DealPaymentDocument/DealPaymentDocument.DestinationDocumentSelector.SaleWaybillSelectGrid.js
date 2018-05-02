var DealPaymentDocument_DestinationDocumentSelector_SaleWaybillSelectGrid = {
    Init: function () {
        $(document).ready(function () {
            // Формируем ссылки на детали сделок и накладных реализации в гриде
            $("#gridSaleWaybillSelect table.grid_table tr").each(function () {
                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + GetBackUrl());

                var saleWaybillId = $(this).find(".Id").text();
                var controllerName = $(this).find(".ControllerName").text();
                $(this).find("a.SaleWaybillName").attr("href", "/" + controllerName + "/Details?id=" + saleWaybillId + GetBackUrl());
            });

            DealPaymentDocument_DestinationDocumentSelector_SelectGrid.Init("gridSaleWaybillSelect");
        });
    }
};