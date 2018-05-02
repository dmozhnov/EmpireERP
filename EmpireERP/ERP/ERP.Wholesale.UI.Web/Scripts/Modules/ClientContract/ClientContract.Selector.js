var ClientContract_Selector = {
    Init: function () {
        $(document).ready(function () {
            $("#linkCreateClientContract").click(function () {
                var dealId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Deal/CreateContract",
                    data: { dealId : dealId },
                    success: function (result) {
                        $('#clientContractEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#clientContractEdit"));
                        ShowModal("clientContractEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientContractSelectList");
                    }
                });
            });
        });
    }
};