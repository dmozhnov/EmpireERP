var Storage_List = {
    Init: function () {
        $(document).ready(function () {
            $("#btnCreateStorage").live('click', function () {
                $.ajax({
                    type: "GET",
                    url: "/Storage/Create/",
                    success: function (result) {
                        $('#storageEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#storageEdit"));
                        ShowModal("storageEdit");
                        $("#storageEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageList");
                    }
                });
            });

            $("#gridStorage .delete_link").live("click", function () {
                if (confirm('Вы уверены?')) {
                    var storage_id = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Storage/Delete/",
                        data: { id: storage_id },
                        success: function (result) {
                            RefreshGrid("gridStorage", function () {
                                ShowSuccessMessage("Место хранения удалено.", "messageStorageList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageList");
                        }
                    });
                };
            });
        });
     },

    OnSuccessStorageSave: function () {
            HideModal();
            RefreshGrid("gridStorage", function () {
                ShowSuccessMessage("Место хранения добавлено.", "messageStorageList");
            });
        }
};