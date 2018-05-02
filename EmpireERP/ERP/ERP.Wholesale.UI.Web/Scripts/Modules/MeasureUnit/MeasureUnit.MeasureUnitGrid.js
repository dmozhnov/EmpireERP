var MeasureUnit_MeasureUnitGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridMeasureUnits table.grid_table tr").each(function (i, el) {
                $(this).find("a.FullName").attr("onclick", "return false;");
            });

            $('#btnCreateMeasureUnit').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                MeasureUnit_MeasureUnitGrid.ShowMeasureUnitDetailsForEdit(id);
            });

            $('#gridMeasureUnits .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                MeasureUnit_MeasureUnitGrid.ShowMeasureUnitDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/MeasureUnit/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridMeasureUnits", function () {
                                ShowSuccessMessage("Единица измерения удалена.", "messageMeasureUnitList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageMeasureUnitList");
                        }
                    });
                }
            });
        });
    },

    ShowMeasureUnitDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");

        $.ajax({
            type: "GET",
            url: "/MeasureUnit/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#measureUnitEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#measureUnitEdit"));
                ShowModal("measureUnitEdit");
                $("#measureUnitEdit #ShortName").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageMeasureUnitList");
            }
        });
    }
};
