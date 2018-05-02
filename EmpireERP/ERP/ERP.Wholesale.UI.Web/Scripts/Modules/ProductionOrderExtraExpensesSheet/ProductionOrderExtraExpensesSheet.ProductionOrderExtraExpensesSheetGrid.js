var ProductionOrderExtraExpensesSheet_ProductionOrderExtraExpensesSheetGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderExtraExpensesSheet table.grid_table tr").each(function () {
                var productionOrderId = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + GetBackUrl());
            });

            $("#gridProductionOrderExtraExpensesSheet .linkExtraExpensesSheetEdit").click(function () {
                var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                var extraExpensesSheetId = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditProductionOrderExtraExpensesSheet",
                    data: { productionOrderId: productionOrderId, extraExpensesSheetId: extraExpensesSheetId },
                    success: function (result) {
                        $("#productionOrderExtraExpensesSheetEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderExtraExpensesSheetEdit"));
                        ShowModal("productionOrderExtraExpensesSheetEdit");
                        $("#productionOrderExtraExpensesSheetEdit #ExtraExpensesContractorName").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderExtraExpensesSheetList");
                    }
                });
            });

            $("#gridProductionOrderExtraExpensesSheet .linkExtraExpensesSheetDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                    var extraExpensesSheetId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteProductionOrderExtraExpensesSheet/",
                        data: { productionOrderId: productionOrderId, extraExpensesSheetId: extraExpensesSheetId },
                        success: function (result) {
                            RefreshGrid("gridProductionOrderExtraExpensesSheet", function () {
                                ShowSuccessMessage("Лист дополнительных расходов удален.", "messageProductionOrderExtraExpensesSheetList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderExtraExpensesSheetList");
                        }
                    });
                }
            });

        });

    },

    OnSuccessProductionOrderExtraExpensesSheetEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderExtraExpensesSheet", function () {
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderExtraExpensesSheetList");
            });
        });
    },

    OnFailProductionOrderExtraExpensesSheetEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderExtraExpensesSheetEdit");
    },

     OnBeginProductionOrderExtraExpensesSheetEdit: function () {
        StartButtonProgress($('#btnProductionOrderExtraExpensesSheetEdit'));
    }
};