var ReturnFromClientReason_ReturnFromClientReasonGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateReturnFromClientReason').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ReturnFromClientReason_ReturnFromClientReasonGrid.ShowReturnFromClientReasonDetailsForEdit(id);
            });

            $('#gridReturnFromClientReason .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ReturnFromClientReason_ReturnFromClientReasonGrid.ShowReturnFromClientReasonDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ReturnFromClientReason";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridReturnFromClientReason", function () {
                                ShowSuccessMessage("Удалено.", "messageReturnFromClientReasonList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientReasonList");
                        }
                    });
                }
            });
        });
    },

    ShowReturnFromClientReasonDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ReturnFromClientReason";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#returnFromClientReasonEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#returnFromClientReasonEdit"));
                ShowModal("returnFromClientReasonEdit");
                $("#returnFromClientReasonEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageReturnFromClientReasonList");
            }
        });
    }
};
