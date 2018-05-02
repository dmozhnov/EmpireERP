var Bank_List_ForeignBankGrid = {
    Init: function () {
        $(document).ready(function () {
            $(".editForeignBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();

                $.ajax({
                    type: "GET",
                    url: "/Bank/EditForeignBank/",
                    data: { id: bankId },
                    success: function (result) {
                        $("#bankForeignBankEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#bankForeignBankEdit"));
                        ShowModal("bankForeignBankEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBank");
                    }
                });
            });

            $(".deleteForeignBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();
                
                if (confirm("Вы действительно хотите удалить банк?")) {
                    StartGridProgress($(this).closest(".grid"));
                    
                    $.ajax({
                        type: "GET",
                        url: "/Bank/DeleteForeignBank/",
                        data: { id: bankId },
                        success: function (result) {
                            RefreshGrid("gridForeignBank", function () {
                                ShowSuccessMessage("Банк удален.", "messageForeignBank");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBank");
                        }
                    });
                }
            });


            $("#btnCreateForeignBank").click(function () {
                var bankId = $(this).parent("td").parent("tr").find(".Id").html();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Bank/AddForeignBank/",
                    data: { id: bankId },
                    success: function (result) {
                        $("#bankForeignBankEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#bankForeignBankEdit"));
                        ShowModal("bankForeignBankEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageForeignBank");
                    }
                });
            });
        });
    },

    OnSuccessForeignBankEdit: function (ajaxContext) {
        RefreshGrid("gridForeignBank", function () {
            ShowSuccessMessage("Банк сохранен.", "messageForeignBank");
            HideModal();
        });
    }
};