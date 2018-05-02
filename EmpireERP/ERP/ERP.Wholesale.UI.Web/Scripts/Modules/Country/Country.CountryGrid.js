var Country_CountryGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateCountry').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                Country_CountryGrid.ShowCountryDetailsForEdit(id);
            });

            $('#gridCountry .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                Country_CountryGrid.ShowCountryDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "Country";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridCountry", function () {
                                ShowSuccessMessage("Удалено.", "messageCountryList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageCountryList");
                        }
                    });
                }
            });
        });
    },

    ShowCountryDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "Country";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#countryEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#countryEdit"));
                ShowModal("countryEdit");
                $("#countryEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageCountryList");
            }
        });
    }
};
