var Storage_AccountOrganizationSelectList = {
    Init: function () {
        $(document).ready(function () {
            DisableButton("btnSaveAccountOrganization");

            $('#SelectedAccountOrganizationId').change(function () {
                if ($('#SelectedAccountOrganizationId').val() == "") {
                    DisableButton("btnSaveAccountOrganization");
                }
                else {
                    EnableButton("btnSaveAccountOrganization");
                }
            });
        });
    },

    OnFailStorageAccountOrganizationAdd: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageAccountOrganizationEdit");
    }
};