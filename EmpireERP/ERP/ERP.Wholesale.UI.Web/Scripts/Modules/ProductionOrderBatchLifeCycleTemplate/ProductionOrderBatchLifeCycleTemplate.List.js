var ProductionOrderBatchLifeCycleTemplate_List = {
    Init: function () {
        $(document).ready(function () {
            $("#btnCreateProductionOrderBatchLifeCycleTemplate").live("click", function () {
                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ProductionOrderBatchLifeCycleTemplate/Create",
                    success: function (result) {
                        $('#productionOrderBatchLifeCycleTemplateEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#productionOrderBatchLifeCycleTemplateEdit"));
                        ShowModal("productionOrderBatchLifeCycleTemplateEdit");
                        $("#productionOrderBatchLifeCycleTemplateEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProductionOrderBatchLifeCycleTemplateList");
                    }
                });
            });
        });
    },

    OnSuccessProductionOrderBatchLifeCycleTemplateEdit: function (ajaxContext) {
        HideModal(function () {
            window.location = "/ProductionOrderBatchLifeCycleTemplate/Details?id=" + ajaxContext.MainDetails.Id + GetBackUrl();
        });
    }

};