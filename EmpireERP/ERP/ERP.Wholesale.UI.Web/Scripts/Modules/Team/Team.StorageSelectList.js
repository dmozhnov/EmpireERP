var Team_StorageSelectList = {
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
        ShowErrorMessage(ajaxContext.responseText, "messageStorageSelectList");
    },

    OnBeginStorageAdd: function () {
        StartButtonProgress($("#btnSaveLinkedStorage"));
    },
    
    OnSuccessStorageAdd: function (ajaxContext) {
        HideModal(function () {
            RefreshGrid("gridStorages", function () {
                Team_MainDetails.RefreshMainDetails(ajaxContext.MainDetails);
                ShowSuccessMessage("Место хранения добавлено.", "messageStorageList");
            });
        });
    }
};