var Currency_ListGrid = {
    Init: function () {
        $(document).ready(function () {
            $(".edit, .details").click(function () {
                var currencyId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "GET",
                    url: "/Currency/Edit",
                    data: { currencyId: currencyId },
                    success: function (result) {
                        $("#currencyEdit").addClass("hidden");
                        $("#currencyEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyEdit"));
                        $("#currencyEdit").removeClass("hidden");
                        ShowModal("currencyEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyList");
                    }
                });
            });

            $("#btnCreateCurrency").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Currency/Create",
                    success: function (result) {
                        $('#currencyEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#currencyEdit"));
                        ShowModal("currencyEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyList");
                    }
                });
            });

            $(".delete").click(function () {
                if (confirm("Вы уверены?")) {
                    var currencyId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "GET",
                        url: "/Currency/Delete",
                        data: { currencyId: currencyId },
                        success: function (result) {
                            RefreshGrid("gridCurrency", function () {
                                ShowSuccessMessage("Валюта удалена.", "messageCurrencyList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageCurrencyList");
                        }
                    });
                }
            });
        }); // document.ready
    }
};