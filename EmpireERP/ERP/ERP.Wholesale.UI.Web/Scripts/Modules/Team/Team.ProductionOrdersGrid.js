var Team_ProductionOrdersGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridProductionOrders table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnAddProductionOrder").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/SelectProductionOrderByTeam",
                    data: { teamId: $("#Id").val() },
                    success: function (result) {
                        $("#productionOrderSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderSelector"));
                        ShowModal("productionOrderSelector");

                        BindProductionOrderSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderList");
                    }
                });
            });

            $("#gridProductionOrders .remove_productionOrder").click(function () {
                if (confirm("Вы уверены?")) {
                    var orderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Team/RemoveProductionOrder",
                        data: { teamId: $("#Id").val(), orderId: orderId },
                        success: function (result) {
                            RefreshGrid("gridProductionOrders", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Заказ на производство исключен из области видимости команды.", "messageProductionOrderEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderList");
                        }
                    });
                }
            });
        });

        function BindProductionOrderSelection() {
            $("#productionOrderSelector .select").die('click');
            $("#productionOrderSelector .select").live('click', function () {
                var orderId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "POST",
                    url: "/Team/AddProductionOrder",
                    data: { teamId: $("#Id").val(), orderId: orderId },
                    success: function (result) {
                        HideModal(function () {
                            RefreshGrid("gridProductionOrders", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Заказ на производство добавлен в область видимости команды.", "messageProductionOrderList");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderSelectGrid");
                    }
                });
            });
        }
    }
};