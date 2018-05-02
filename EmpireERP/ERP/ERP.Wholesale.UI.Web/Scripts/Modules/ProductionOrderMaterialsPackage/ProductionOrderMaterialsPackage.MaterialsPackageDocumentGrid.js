var ProductionOrderMaterialsPackage_MaterialsPackageDocumentGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridMaterialsPackageDocument table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/ProductionOrderMaterialsPackage/DownloadProductionOrderMaterialsPackageDocument?id=" + id);
            });

            $("#btnAddMaterial").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderMaterialsPackage/ProductionOrderMaterialsPackageDocumentCreate",
                    data: { id: $("#Id").val() },
                    success: function (result) {
                        $("#productionOrderMaterialsPackageDocumentEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderMaterialsPackageDocumentEdit"));
                        ShowModal("productionOrderMaterialsPackageDocumentEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderMaterialsPackageDocument");
                    }
                });
            });

            $(".edit").click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderMaterialsPackage/ProductionOrderMaterialsPackageDocumentEdit",
                    data: { id: id },
                    success: function (result) {
                        $("#productionOrderMaterialsPackageDocumentEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderMaterialsPackageDocumentEdit"));
                        ShowModal("productionOrderMaterialsPackageDocumentEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderMaterialsPackageDocument");
                    }
                });
            });

            $(".delete").click(function () {
                if (confirm("Вы действительно хотите удалить документ?")) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "GET",
                        url: "/ProductionOrderMaterialsPackage/ProductionOrderMaterialsPackageDocumentDelete",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridMaterialsPackageDocument", function () {
                                RefreshMainDetails(result);
                                ShowSuccessMessage("Документ удален.", "messageProductionOrderMaterialsPackageDocument");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderMaterialsPackageDocument");
                        }
                    });
                }
            });
        });
    }
};