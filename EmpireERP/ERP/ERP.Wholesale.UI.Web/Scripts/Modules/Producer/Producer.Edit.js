var Producer_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();
        });

        $("#btnBack").live("click", function () {
            window.location = $("#BackURL").val();
        });
    },

    OnSuccessProducerSave: function (ajaxContext) {
        if (IsDefaultOrEmpty($("#Id").val())) {
            window.location = "/Producer/Details?id=" + ajaxContext + GetBackUrlFromString($("#BackURL").val());
        } else {
            window.location = $("#BackURL").val();
        }
    },

    OnFailProducerSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProducerEdit");
    }
};