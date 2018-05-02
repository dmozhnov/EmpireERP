var Deal_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();

            $("#btnBack").live('click', function () {
                window.location = $('#BackURL').val();
            });

            // Вывод формы выбора клиента
            $("#ClientName").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClient",
                    success: function (result) {
                        $('#clientSelector').hide().html(result);
                        $.validator.unobtrusive.parse($("#clientSelector"));
                        ShowModal("clientSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                    }
                });
            });

            // Обработка выбора клиента
            $(".select_client").live("click", function () {
                var name = $(this).parent("td").parent("tr").find(".Name").text();
                var id = $(this).parent("td").parent("tr").find(".Id").text();

                $("#ClientId").val(id);
                $("#ClientName").text(name);

                HideModal();
            });
        });
    },

    OnSuccessDealEdit: function (ajaxContext) {
        window.location = "/Deal/Details?id=" + ajaxContext + GetBackUrl();
    },

    OnFailDealEdit: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDealEdit");
    }
};