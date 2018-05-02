var MeasureUnit_MeasureUnitSelector = {
    Init: function () {
        $(document).ready(function () {
            $("#createMeasureUnit").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/MeasureUnit/Create/",
                    success: function (result) {
                        $('#Edit').hide().html(result);
                        $.validator.unobtrusive.parse($("#Edit"));
                        ShowModal("Edit");
                        $("#Edit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageSelectMeasureUnit");
                    }
                });
            });
        });
    }
};