var AccountOrganization_StorageSelectList = {
    Init: function () {
        $(document).ready(function () {
            DisableButton("btnSaveLinkedStorage");

            $('#StorageId').change(function () {
                if ($('#StorageId').val() == "") {
                    DisableButton("btnSaveLinkedStorage");
                }
                else {
                    EnableButton("btnSaveLinkedStorage");
                }
            });
        });
    },

    OnFailStorageAdd: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageSelectStorage");
    }
};