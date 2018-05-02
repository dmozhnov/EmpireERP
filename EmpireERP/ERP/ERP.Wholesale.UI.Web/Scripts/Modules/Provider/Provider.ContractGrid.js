var Provider_ContractGrid = {
    Init: function () {
        $(document).ready(function () {

            $("#gridProviderContract table.grid_table tr").each(function () {
                var id = $(this).find(".ContractorId").text();
                $(this).find("a.Provider").attr("href", "/Provider/Details?id=" + id + GetBackUrl());

                id = $(this).find(".AccountOrganizationId").text();
                $(this).find("a.AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + id + GetBackUrl());

                id = $(this).find(".ProviderOrganizationId").text();
                $(this).find("a.ProviderOrganizationName").attr("href", "/ProviderOrganization/Details?id=" + id + GetBackUrl());
            });

            // Вывод модальной формы "Добавление договора"
            $("#btnAddContract").click(function () {
                var providerId = $('#MainDetails_Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/Provider/CreateContract",
                    data: { providerId: providerId },
                    success: function (result) {
                        $("#providerContractEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#providerContractEdit"));
                        ShowModal("providerContractEdit");
                        $("#providerContractEdit #Number").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageContractList");
                    }
                });
            });

            // Удалить договор
            $("#gridProviderContract .linkProviderContractDelete").click(function () {
                if (confirm('Вы действительно хотите удалить договор?')) {
                    var providerId = $('#MainDetails_Id').val();
                    var contractId = $(this).parent("td").parent("tr").find(".providerContractId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Provider/DeleteContract/",
                        data: { providerId: providerId, contractId: contractId },
                        success: function (result) {
                            RefreshGrid("gridProviderContract", function () {
                                RefreshMainDetails(result);
                                ShowSuccessMessage("Договор удален.", "messageContractList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageContractList");
                        }
                    });
                }
            });

            // Редактировать договор
            $("#gridProviderContract .linkProviderContractEdit").click(function () {
                var providerId = $('#MainDetails_Id').val();
                var contractId = $(this).parent("td").parent("tr").find(".providerContractId").text();
                $.ajax({
                    type: "GET",
                    url: "/Provider/EditContract",
                    data: { providerId: providerId, contractId: contractId },
                    success: function (result) {
                        $('#providerContractEdit').hide().html(result);
                        $.validator.unobtrusive.parse($("#providerContractEdit"));
                        ShowModal("providerContractEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageContractList");
                    }
                });
            });
        });
    }
};