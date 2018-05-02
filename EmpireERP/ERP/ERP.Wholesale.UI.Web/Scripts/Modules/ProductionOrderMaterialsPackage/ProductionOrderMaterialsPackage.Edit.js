var ProductionOrderMaterialsPackage_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#btnBack").click(function () {
                window.location = $("#BackURL").val();
            });

            $("#ProductionOrder").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrder/SelectProductionOrderForMaterialsPackageAdding",
                    success: function (result) {
                        $("#selectProductionOrder").hide().html(result);
                        $.validator.unobtrusive.parse($("#selectProductionOrder"));
                        ShowModal("selectProductionOrder");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageMaterialsPackageEdit");
                    }
                });
            });

            $(".select").live("click", function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                var name = $(this).parent("td").parent("tr").find(".Name").text();

                $("#ProductionOrderId").val(id);
                $("#ProductionOrder").text(name);

                HideModal();
            });

        });
    },

    OnBeginMaterialsPackageEdit: function () {
        StartButtonProgress($("#btnMaterialsPackageSave"));
    },

    OnSuccessMaterialsPackageEdit: function (ajaxContext) {
        var isNew = $("#Id").val().length == 0; 
        if (isNew) {
            window.location = "/ProductionOrderMaterialsPackage/Details?id=" + ajaxContext.Id + "&backURL=" + $("#BackURL").val();
        } else {
            window.location = $("#BackURL").val();
        }
    },

    OnFailMaterialsPackageEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageMaterialsPackageEdit");
    }
};