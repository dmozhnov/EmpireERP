var Team_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();
        });

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });
    },

    OnSuccessTeamSave: function (ajaxContext) {
        window.location = "/Team/Details?id=" + ajaxContext + "&backURL=/Team/List";
    },

    OnFailTeamSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageTeamEdit");
    },

    OnBeginTeamSave: function () {
        StartButtonProgress($("#btnSaveTeam"));
    }
};