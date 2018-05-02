var ProductionOrderMaterialsPackage_Details = {
    Init: function () {
        $(document).ready(function () {
            //Формируем ссылку для заказа
            if (IsTrue($("#AllowToViewProductionOrder").val())) {
                $("#ProductionOrder").attr("href", "/ProductionOrder/Details?id=" + $("#ProductionOrderId").val() + "&backURL=" + $("#currentUrl").val());
            }
            else {
                $("#ProductionOrder").addClass("disabled");
            }

            $("#btnEdit").click(function () {
                window.location = "/ProductionOrderMaterialsPackage/Edit?id=" + $("#Id").val() + "&backURL=" + $("#currentUrl").val();
            });

            $("#btnBack").click(function () {
                window.location = $("#BackURL").val();
            });

            $("#btnDelete").click(function () {
                var id = $("#ProductionOrderId").val();
                if (confirm("Вы действительно хотите удалить пакет материалов?")) {
                    $.ajax({
                        type: "GET",
                        url: "/ProductionOrderMaterialsPackage/DeleteMaterialsPackage",
                        data: { id: $("#Id").val() },
                        success: function (result) {
                            window.location = $("#BackURL").val(); /// <reference path="../Deal/Deal.Details.SalesGrid.js" />

                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderMaterialsPackageDetails");
                        }
                    });
                }
            });
        });
    },

    OnSuccessProductionOrderMaterialsPackageDocumentCreate: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridMaterialsPackageDocument", function () {
                RefreshMainDetails(ajaxContext);
                ShowSuccessMessage("Документ сохранен.", "messageProductionOrderMaterialsPackageDocument");
            });
        });
    },

    RefreshMainDetails: function (model) {
        $("#PakageSize").text(model.PakageSize);
        $("#DocumentCount").text(model.DocumentCount);
        $("#LastChangeDate").text(model.LastChangeDate);
    },

    OnSuccessProductionOrderMaterialsPackageDocumentEdit: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridMaterialsPackageDocument", function () {
                ShowSuccessMessage("Документ сохранен.", "messageProductionOrderMaterialsPackageDocument");
            });
        });
    }
};