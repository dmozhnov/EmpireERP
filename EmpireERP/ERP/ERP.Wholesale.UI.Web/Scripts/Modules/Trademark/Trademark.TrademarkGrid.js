var Trademark_TrademarkGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $('#btnCreateTrademark').click(function () {
                StartButtonProgress($(this));
                var id = 0;
                Trademark_TrademarkGrid.ShowTrademarkDetailsForEdit(id);
            });

            $('#gridTrademark .edit_link').click(function () {
                var id = $(this).parent("td").parent("tr").find(".Id").text();
                Trademark_TrademarkGrid.ShowTrademarkDetailsForEdit(id);
            });

            $('.delete_link').click(function () {
                if (confirm('Вы уверены?')) {
                    var id = $(this).parent("td").parent("tr").find(".Id").text();
                    var controllerName = "Trademark";

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/" + controllerName + "/Delete/",
                        data: { id: id },
                        success: function (result) {
                            RefreshGrid("gridTrademark", function () {
                                ShowSuccessMessage("Удалено.", "messageTrademarkList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageTrademarkList");
                        }
                    });
                }
            });
        });
    },

    ShowTrademarkDetailsForEdit: function (id) {
        var method = (id == 0 ? "Create" : "Edit");
        var controllerName = "Trademark";

        $.ajax({
            type: "GET",
            url: "/" + controllerName + "/" + method + "/",
            data: { id: id },
            success: function (result) {
                $("#trademarkEdit").hide().html(result);
                $.validator.unobtrusive.parse($("#trademarkEdit"));
                ShowModal("trademarkEdit");
                $("#trademarkEdit #Name").focus();
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageTrademarkList");
            }
        });
    }
};
