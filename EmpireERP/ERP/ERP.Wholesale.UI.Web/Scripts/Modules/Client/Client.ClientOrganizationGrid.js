var Client_ClientOrganizationGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridClientOrganization table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".ClientOrganizationId").text();
                $(this).find("a.ShortName").attr("href", "/ClientOrganization/Details?id=" + id + GetBackUrl());
            });

            $('#btnAddOrganization').click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/Client/SelectClientOrganization/",
                    data: { clientId: $("#Id").val(), mode: "excludeclient" },
                    success: function (result) {
                        $("#contractorOrganizationSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                        ShowModal("contractorOrganizationSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageClientOrganizationList");
                    }
                });
            });
        });
    }
};