var Bank_List_RussianBankGrid = {
    Init: function () {
        $(document).ready(function () {
            $(".editRussianBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();

                $.ajax({
                    type: "GET",
                    url: "/Bank/EditRussianBank/",
                    data: { id: bankId },
                    success: function (result) {
                        $("#bankRussianBankEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#bankRussianBankEdit"));
                        ShowModal("bankRussianBankEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBank");
                    }
                });
            });

            $(".deleteRussianBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();

                if (confirm("Вы действительно хотите удалить банк?")) {
                    StartGridProgress($(this).closest(".grid"));

                    $.ajax({
                        type: "GET",
                        url: "/Bank/DeleteRussianbank/",
                        data: { id: bankId },
                        success: function (result) {
                            RefreshGrid("gridRussianBank", function () {
                                ShowSuccessMessage("Банк удален.", "messageRussianBank");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBank");
                        }
                    });
                }
            });


            $("#btnCreateRussianBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Bank/AddRussianBank/",
                    data: { id: bankId },
                    success: function (result) {
                        $("#bankRussianBankEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#bankRussianBankEdit"));
                        ShowModal("bankRussianBankEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageRussianBank");
                    }
                });
            });
        });
    },

    OnSuccessRussianBankEdit: function (ajaxContext) {
        RefreshGrid("gridRussianBank", function () {
            ShowSuccessMessage("Банк сохранен.", "messageRussianBank");
            HideModal();
        });
    }
};