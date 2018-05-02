var AccountOrganization_StorageGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridStorage table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });

            $("#gridStorage .delete_link").click(function () {
                if (confirm('Вы уверены?')) {
                    var storage_id = $(this).parent("td").parent("tr").find(".Id").text();
                    var accountOrganizationId = $("#AccountOrganizationId").val();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/AccountOrganization/DeleteStorage/",
                        data: { storageId: storage_id, accountOrganizationId: accountOrganizationId },
                        success: function () {
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
    }
};