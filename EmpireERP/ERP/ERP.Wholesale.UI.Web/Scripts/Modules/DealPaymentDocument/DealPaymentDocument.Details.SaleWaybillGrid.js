var DealPaymentDocument_Details_SaleWaybillGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridSaleWaybillDistribution table.grid_table tr").each(function (i, el) {
                var saleWaybillId = $(this).find(".SaleWaybillId").text();
                var controllerName = $(this).find(".ControllerName").text();
                $(this).find("a.SaleWaybillName").attr("href", "/" + controllerName + "/Details?id=" + saleWaybillId + GetBackUrl());
            });
        });
    }
};