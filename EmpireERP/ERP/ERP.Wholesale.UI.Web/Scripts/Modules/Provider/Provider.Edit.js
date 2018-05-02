var Provider_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();

            $("#btnBack").click(function () {
                window.location = $("#BackURL").val();
            });

            $("#btnAddProviderType").click(function () {
                $.ajax({
                    type: "GET",
                    url: "/ProviderType/Create",
                    success: function (result) {
                        $("#providerTypeEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#providerTypeEdit"));
                        ShowModal("providerTypeEdit");
                        $("#providerTypeEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderEdit");
                    }
                });
            });

        }); // document ready        
    },

    OnSuccessProviderTypeEdit: function (ajaxContext) {
        $.ajax({
            type: "GET",
            url: "/Provider/GetProviderTypes",
            success: function (result) {
                $("#Type").fillSelect(result);
                $("#Type").attr("value", ajaxContext.Id);
                ShowSuccessMessage("Тип поставщика добавлен.", "messageProviderEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageProviderEdit");
            }
        });

        HideModal();
    },

    OnFailProviderSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageProviderEdit");
    },

    OnSuccessProviderSave: function (ajaxContext) {
        window.location = "/Provider/Details?id=" + ajaxContext + "&backURL=/Provider/List";
    }
};