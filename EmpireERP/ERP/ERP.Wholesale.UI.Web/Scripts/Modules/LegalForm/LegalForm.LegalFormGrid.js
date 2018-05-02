var LegalForm_LegalFormGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateLegalForm').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                LegalForm_LegalFormGrid.ShowLegalFormDetailsForEdit(id);
            });

            $('#gridLegalForm .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                LegalForm_LegalFormGrid.ShowLegalFormDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "LegalForm";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridLegalForm", function () {
                                ShowSuccessMessage("Удалено.", "messageLegalFormList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageLegalFormList");
                        }
                    });
                }
            });
        });
    },

    ShowLegalFormDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "LegalForm";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#legalFormEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#legalFormEdit"));
                ShowModal("legalFormEdit");
                $("#legalFormEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageLegalFormList");
            }
        });
    }
};
