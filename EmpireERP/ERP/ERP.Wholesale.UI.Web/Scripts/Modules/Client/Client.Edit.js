var Client_Edit = {
    Init: function () {
        $(document).ready(function () {
            $("#Name").focus();
        });

        $('#btnCancel').live('click', function () {
            window.location = $('#BackURL').val();
        });

        $('#AddRegion').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ClientRegion/Create",
                success: function (result) {
                    $('#regionEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#regionEdit"));
                    ShowModal("regionEdit");
                    $("#regionEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
                }
            });
        });

        $('#AddServiceProgram').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ClientServiceProgram/Create",
                success: function (result) {
                    $('#clientServiceProgramEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#clientServiceProgramEdit"));
                    ShowModal("clientServiceProgramEdit");
                    $("#clientServiceProgramEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
                }
            });
        });

        $('#AddType').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/ClientType/Create",
                success: function (result) {
                    $('#clientTypeEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#clientTypeEdit"));
                    ShowModal("clientTypeEdit");
                    $("#clientTypeEdit #Name").focus();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
                }
            });
        });
    },

    OnSuccessClientTypeSave: function (ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/Client/GetClientTypeList",
            success: function (result) {
                $('#TypeId').fillSelect(result);
                $('#TypeId').val(ajaxContext.Id);
                ShowSuccessMessage("Тип клиента добавлен.", "messageClientEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
            }
        });

        HideModal();
    },

    OnSuccessClientServiceProgramEdit: function (ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/Client/GetClientServiceProgramList",
            success: function (result) {
                $('#ServiceProgramId').fillSelect(result);
                $('#ServiceProgramId').val(ajaxContext.Id);
                ShowSuccessMessage("Программа обслуживания клиента добавлена.", "messageClientEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
            }
        });

        HideModal();
    },

    OnSuccessClientRegionSave: function (ajaxContext) {
        $.ajax({
            type: "POST",
            url: "/Client/GetClientRegionList",
            success: function (result) {
                $('#RegionId').fillSelect(result);
                $('#RegionId').val(ajaxContext.Id);
                ShowSuccessMessage("Регион клиента добавлен.", "messageClientEdit");
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, "messageClientEdit");
            }
        });

        HideModal();
    },

    OnFailClientSave: function (ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageClientEdit");
    },

    OnSuccessClientSave: function (ajaxContext) {
        window.location = "/Client/Details?id=" + ajaxContext.Id + "&backURL=" + ajaxContext.BackURL;
    }
};