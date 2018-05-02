var ProductionOrderTransportSheet_ProductionOrderTransportSheetGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderTransportSheet table.grid_table tr").each(function () {
                var productionOrderId = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + GetBackUrl());                
            });


            $("#gridProductionOrderTransportSheet .linkTransportSheetEdit").click(function () {
                var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                var transportSheetId = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditProductionOrderTransportSheet",
                    data: { productionOrderId: productionOrderId, transportSheetId: transportSheetId },
                    success: function (result) {
                        $("#productionOrderTransportSheetEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderTransportSheetEdit"));
                        ShowModal("productionOrderTransportSheetEdit");
                        $("#productionOrderTransportSheetEdit #ForwarderName").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderTransportSheetList");
                    }
                });
            });

            $("#gridProductionOrderTransportSheet .linkTransportSheetDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                    var transportSheetId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteProductionOrderTransportSheet/",
                        data: { productionOrderId: productionOrderId, transportSheetId: transportSheetId },
                        success: function (result) {
                            RefreshGrid("gridProductionOrderTransportSheet", function () {
                                ShowSuccessMessage("Транспортный лист удален.", "messageProductionOrderTransportSheetList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderTransportSheetList");
                        }
                    });
                }
            });
        });
    },

    OnSuccessProductionOrderTransportSheetEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderTransportSheet", function () {
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderTransportSheetList");
            });
        });
    },

    OnFailProductionOrderTransportSheetEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderTransportSheetEdit");
    }
};