var WriteoffReason_WriteoffReasonGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateWriteoffReason').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                WriteoffReason_WriteoffReasonGrid.ShowWriteoffReasonDetailsForEdit(id);
            });

            $('#gridWriteoffReason .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                WriteoffReason_WriteoffReasonGrid.ShowWriteoffReasonDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "WriteoffReason";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridWriteoffReason", function () {
                                ShowSuccessMessage("Удалено.", "messageWriteoffReasonList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffReasonList");
                        }
                    });
                }
            });
        });
    },

    ShowWriteoffReasonDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "WriteoffReason";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#writeoffReasonEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#writeoffReasonEdit"));
                ShowModal("writeoffReasonEdit");
                $("#writeoffReasonEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffReasonList");
            }
        });
    }
};
