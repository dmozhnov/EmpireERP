var Team_Details = {
    Init: function () {
        $('#btnEdit').live("click", function () {
            var id = $('#Id').val();
            window.location = "/Team/Edit?id=" + id + GetBackUrl();
        });

        $('#btnDelete').live("click", function () {
            if (confirm('Вы уверены?')) {
                var id = $('#Id').val();

                $.ajax({
                    type: "POST",
                    url: "/Team/Delete/",
                    data: { teamId: id },
                    success: function () {
                        //ShowSuccessMessage(".", "messageWriteoffWaybillDetails");
                        window.location = "/Team/List";
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageTeamEdit");
                    }
                });
            }
        });

        $("#btnBackTo").live('click', function () {
            window.location = $('#BackURL').val();
        });
    }
};