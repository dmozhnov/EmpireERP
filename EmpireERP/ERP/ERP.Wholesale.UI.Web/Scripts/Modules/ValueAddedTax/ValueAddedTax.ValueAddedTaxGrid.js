var ValueAddedTax_ValueAddedTaxGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateValueAddedTax').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                ValueAddedTax_ValueAddedTaxGrid.ShowValueAddedTaxDetailsForEdit(id);
            });

            $('#gridValueAddedTax .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                ValueAddedTax_ValueAddedTaxGrid.ShowValueAddedTaxDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "ValueAddedTax";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridValueAddedTax", function () {
                                ShowSuccessMessage("Удалено.", "messageValueAddedTaxList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageValueAddedTaxList");
                        }
                    });
                }
            });
        });
    },

    ShowValueAddedTaxDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "ValueAddedTax";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#valueAddedTaxEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#valueAddedTaxEdit"));
                ShowModal("valueAddedTaxEdit");
                $("#valueAddedTaxEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageValueAddedTaxList");
            }
        });
    }
};
