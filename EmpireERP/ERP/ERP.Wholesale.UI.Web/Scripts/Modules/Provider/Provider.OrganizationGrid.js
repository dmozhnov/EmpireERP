var Provider_OrganizationGrid = {
    Init: function () {
        $(document).ready(function () {

            var currentUrl = $("#currentUrl").val();
            $("#gridProviderOrganization table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.ShortName").attr("href", "/ProviderOrganization/Details?id=" + id + "&backURL=" + currentUrl);
            });

            // Удалить организацию поставщика
            $("#gridProviderOrganization .linkProviderOrganizationDelete").click(function () {
                if (confirm('Вы уверены?')) {
                    var providerId = $('#MainDetails_Id').val();
                    var providerOrganizationId = $(this).parent("td").parent("tr").find(".Id").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Provider/RemoveProviderOrganization/",
                        data: { providerId: providerId, providerOrganizationId: providerOrganizationId },
                        success: function (result) {
                            RefreshGrid("gridProviderOrganization", function () {
                                RefreshMainDetails(result);
                                ShowSuccessMessage("Организация удалена из списка.", "messageProviderOrganizationList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderOrganizationList");
                        }
                    });
                }
            });

            // Вывод модальной формы "Добавление связанной организации"
            $("#btnAddOrganization").click(function () {
                var providerId = $('#MainDetails_Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Provider/SelectContractorOrganization",
                    data: { providerId: providerId, mode: "excludeprovider" },
                    success: function (result) {
                        $("#contractorOrganizationSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#contractorOrganizationSelector"));
                        ShowModal("contractorOrganizationSelector");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderOrganizationList");
                    }
                });
            });
        });
    }
};