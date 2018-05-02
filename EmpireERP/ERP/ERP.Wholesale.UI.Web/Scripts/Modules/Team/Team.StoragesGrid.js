var Team_StoragesGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridStorages table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#btnAddStorage").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    url: "/Team/StoragesList",
                    data: { teamId: $("#Id").val() },
                    success: function (result) {
                        $("#storageSelectList").hide().html(result);
                        ShowModal("storageSelectList");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageList");
                    }
                });
            });

            $("#gridStorages .remove_storage").click(function () {
                if (confirm("Вы уверены?")) {
                    var storageId = $(this).parent("td").parent("tr").find(".StorageId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Team/RemoveStorage",
                        data: { teamId: $("#Id").val(), storageId: storageId },
                        success: function (result) {
                            RefreshGrid("gridStorages", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Место хранения исключено из области видимости команды.", "messageStorageList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageStorageList");
                        }
                    });
                }
            });
        });
    }
};