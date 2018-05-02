var Trademark_TrademarkSelector = {
    Init: function () {
        $(document).ready(function () {
            $("#createTrademark").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/Trademark/Create/",
                    success: function (result) {
                        $('#Edit').hide().html(result);
                        $.validator.unobtrusive.parse($("#Edit"));
                        ShowModal("Edit");
                        $("#Edit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageSelectTrademark");
                    }
                });
            });
        });
    }
};