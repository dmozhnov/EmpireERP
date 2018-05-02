var ProductionOrderCustomsDeclaration_ProductionOrderCustomsDeclarationGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridProductionOrderCustomsDeclaration table.grid_table tr").each(function () {
                var productionOrderId = $(this).find(".ProductionOrderId").text();
                $(this).find("a.ProductionOrderName").attr("href", "/ProductionOrder/Details?id=" + productionOrderId + GetBackUrl());                
            });

            $("#gridProductionOrderCustomsDeclaration .linkCustomsDeclarationEdit").click(function () {
                var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                var customsDeclarationId = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/EditProductionOrderCustomsDeclaration",
                    data: { productionOrderId: productionOrderId, customsDeclarationId: customsDeclarationId },
                    success: function (result) {
                        $("#productionOrderCustomsDeclarationEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderCustomsDeclarationEdit"));
                        ShowModal("productionOrderCustomsDeclarationEdit");
                        $("#productionOrderCustomsDeclarationEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderCustomsDeclarationList");
                    }
                });
            });            

            $("#gridProductionOrderCustomsDeclaration .linkCustomsDeclarationDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var productionOrderId = $(this).parent("td").parent("tr").find(".ProductionOrderId").text();
                    var customsDeclarationId = $(this).parent("td").parent("tr").find(".Id").text();
                    
                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ProductionOrder/DeleteProductionOrderCustomsDeclaration/",
                        data: { productionOrderId: productionOrderId, customsDeclarationId: customsDeclarationId },
                        success: function (result) {
                            RefreshGrid("gridProductionOrderCustomsDeclaration", function () {
                                ShowSuccessMessage("Таможенный лист удален.", "messageProductionOrderCustomsDeclarationList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderCustomsDeclarationList");
                        }
                    });
                }
            });

        });

    },

    OnSuccessProductionOrderCustomsDeclarationEdit: function (ajaxContext) {
        RefreshGrid("gridProductionOrderCustomsDeclaration", function () {           
            HideModal(function () {
                ShowSuccessMessage("Сохранено.", "messageProductionOrderCustomsDeclarationList");
            });           
        });
    },

    OnFailProductionOrderCustomsDeclarationEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProductionOrderCustomsDeclarationEdit");
    },
};