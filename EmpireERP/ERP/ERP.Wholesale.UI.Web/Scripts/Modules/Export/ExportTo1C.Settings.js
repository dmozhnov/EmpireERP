var ExportTo1C_Settings = {
    Init: function () {
        $(function () {

            $("#btnRestoreDefaults, #btnRestoreDefaults2").live("click", function () {
                window.location = window.location;
            });

            $("#multipleSelectorAccountOrganization").live("addElement", function (event, param) {
                for (i = 0; i < param.length; i++) {
                    HideElement("multipleSelectorCommissionaireOrganizations", param[i]);
                    HideElement("multipleSelectorConsignorOrganizations", param[i]);
                    HideElement("multipleSelectorReturnsFromCommissionaireOrganizations", param[i]);
                    HideElement("multipleSelectorReturnsAcceptedByCommissionaireOrganizations", param[i]);
                }
            });

            $("#multipleSelectorAccountOrganization").live("removeElement", function (event, param) {
                for (i = 0; i < param.length; i++) {
                    ShowElement("multipleSelectorCommissionaireOrganizations", param[i]);
                    ShowElement("multipleSelectorConsignorOrganizations", param[i]);
                    ShowElement("multipleSelectorReturnsFromCommissionaireOrganizations", param[i]);
                    ShowElement("multipleSelectorReturnsAcceptedByCommissionaireOrganizations", param[i]);
                }
            });

            $("#AddTransfersToCommission").prev().bind("change", function () {
                GetMultipleSelectorList("AddTransfersToCommission", "CommissionaireOrganizations");
            });

            $("#AddReturnsFromCommissionaires").prev().bind("change", function () {
                GetMultipleSelectorList("AddReturnsFromCommissionaires", "ReturnsFromCommissionaireOrganizations");
            });

            $("#AddReturnsAcceptedByCommissionaires").prev().bind("change", function () {
                GetMultipleSelectorList("AddReturnsAcceptedByCommissionaires", "ReturnsAcceptedByCommissionaireOrganizations");
            });

            $("#OperationTypeId").live("change", function () {
                $("#AccountOrganizations input.multiple_selector_remove_button").trigger("click");
                $("#CommissionaireOrganizations input.multiple_selector_remove_button").trigger("click");
                $("#ReturnsFromCommissionaireOrganizations input.multiple_selector_remove_button").trigger("click");
                $("#ReturnsAcceptedByCommissionaireOrganizations input.multiple_selector_remove_button").trigger("click");
                $("#ConsignorOrganizations input.multiple_selector_remove_button").trigger("click");

                ResetYesNoToggle($("#AddTransfersToCommission").prev());
                $("#CommissionaireOrganizations .OrganizationList").hide();

                ResetYesNoToggle($("#AddReturnsFromCommissionaires").prev());
                $("#ReturnsFromCommissionaireOrganizations .OrganizationList").hide();

                ResetYesNoToggle($("#AddReturnsAcceptedByCommissionaires").prev());
                $("#ReturnsAcceptedByCommissionaireOrganizations .OrganizationList").hide();

                var operationTypeId = $("#OperationTypeId").val();
                switch (operationTypeId.toString()) {
                    case "1":
                        $("#AccountOrganizations .group_title").text("Выберите организацию, реализацию которой нужно выгружать:");
                        $("#CommissionaireOrganizations").show();
                        $("#ReturnsFromCommissionaireOrganizations").hide();
                        $("#ReturnsAcceptedByCommissionaireOrganizations").hide();
                        $("#ConsignorOrganizations").hide();
                        break;
                    case "2":
                        $("#AccountOrganizations .group_title").text("Выберите организацию, перемещения внутри которой нужно выгрузить:");
                        $("#CommissionaireOrganizations").hide();
                        $("#ReturnsFromCommissionaireOrganizations").hide();
                        $("#ReturnsAcceptedByCommissionaireOrganizations").hide();
                        $("#ConsignorOrganizations").hide();
                        break;
                    case "3":
                        $("#AccountOrganizations .group_title").text("Выберите организацию, которая принимала возвраты:");
                        $("#CommissionaireOrganizations").hide();
                        $("#ReturnsFromCommissionaireOrganizations").show();
                        $("#ReturnsAcceptedByCommissionaireOrganizations").show();
                        $("#ConsignorOrganizations").hide();
                        break;
                    case "4":
                        $("#AccountOrganizations .group_title").text("Выберите организацию, на которую передается товар на комиссию:");
                        $("#CommissionaireOrganizations").hide();
                        $("#ReturnsFromCommissionaireOrganizations").hide();
                        $("#ReturnsAcceptedByCommissionaireOrganizations").hide();
                        if ($("#ConsignorOrganizations").html() == "" || $("#ConsignorOrganizations .OrganizationList").html() == null
                        || $("#ConsignorOrganizations").html() == undefined) {
                            $.ajax({
                                type: "GET",
                                url: "/ExportTo1C/GetConsignorOrganizationsList/",
                                success: function (result) {
                                    $("#ConsignorOrganizations").html(result);
                                },
                                error: function (XMLHttpRequest, textStatus, thrownError) {
                                    ShowErrorMessage(XMLHttpRequest.responseText, "messageExportTo1CSettings");
                                }
                            });
                        }
                        $("#ConsignorOrganizations").show();
                        break;
                    default:
                        $("#AccountOrganizations .group_title").text("Выберите организацию, для которой выгружать данные:");
                        $("#CommissionaireOrganizations").hide();
                        $("#ReturnsFromCommissionaireOrganizations").hide();
                        $("#ConsignorOrganizations").hide();
                }
            });


            $("#btnExport, #btnExport2").live("click", function () {
                if (ValidateExportParameters()) {

                    var url = "/ExportTo1C/ExportOperationsTo1C?" +
                    "startDate=" + $("#StartDate").val() +
                    "&endDate=" + $("#EndDate").val() +
                    "&OperationTypeId=" + $("#OperationTypeId").val() +
                    "&AddTransfersToCommission=" + $('#AddTransfersToCommission').val() +
                    "&AddReturnsFromCommissionaires=" + $('#AddReturnsFromCommissionaires').val() +
                    "&AddReturnsAcceptedByCommissionaires=" + $('#AddReturnsAcceptedByCommissionaires').val() +
                    "&" + $("#multipleSelectorAccountOrganization").FormSelectedEntitiesUrlParametersString("AllAccountOrganizations", "AccountOrganizationIDs") +
                    "&" + $("#multipleSelectorCommissionaireOrganizations").FormSelectedEntitiesUrlParametersString("AllCommissionaireOrganizations", "CommissionaireOrganizationsIDs") +
                    "&" + $("#multipleSelectorConsignorOrganizations").FormSelectedEntitiesUrlParametersString("AllConsignorOrganizations", "ConsignorOrganizationsIDs") +
                    "&" + $("#multipleSelectorReturnsFromCommissionaireOrganizations").FormSelectedEntitiesUrlParametersString("AllReturnsFromCommissionairesOrganizations", "ReturnsFromCommissionairesOrganizationsIDs") +
                    "&" + $("#multipleSelectorReturnsAcceptedByCommissionaireOrganizations").FormSelectedEntitiesUrlParametersString("AllReturnsAcceptedByCommissionairesOrganizations", "ReturnsAcceptedByCommissionairesOrganizationsIDs");

                    StartButtonProgress($(this));

                    $.fileDownload(url, {
                        successCallback: function (response) {
                            StopButtonProgress();
                            scroll(0, $("#messageExportTo1CSettings").offset().top - 10);
                            ShowSuccessMessage("Файл успешно сформирован.", "messageExportTo1CSettings");
                        },
                        failCallback: function (response) {
                            StopButtonProgress();
                            scroll(0, $("#messageExportTo1CSettings").offset().top - 10);
                            ShowErrorMessage("Произошла ошибка при экспорте данных: " + response, "messageExportTo1CSettings");
                        }
                    });
                }
            });

            function ValidateExportParameters() {
                var scroll_y = $("#messageExportTo1CSettings").offset().top - 10;

                var operationTypeId = $("#OperationTypeId").val();

                if (operationTypeId == "") {
                    scroll(0, scroll_y);
                    ShowErrorMessage("Не выбран тип операции.", "messageExportTo1CSettings");
                    return false;
                }

                if (!ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageExportTo1CSettings", true, scroll_y)) {
                    return false;
                }


                if (IsFalse($("#multipleSelectorAccountOrganization").CheckSelectedEntitiesCount("Не выбрана ни одна организация, для которой выгружаются данные.",
                "Выберите все организации или не больше ", "messageExportTo1CSettings"))) {
                    scroll(0, scroll_y);
                    return false;
                }

                if (operationTypeId == 1 && $("#AddTransfersToCommission").val() == "1") {
                    if (IsFalse($("#multipleSelectorCommissionaireOrganizations").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация комиссионер.",
                "Выберите все организации или не больше ", "messageExportTo1CSettings"))) {
                        scroll(0, scroll_y);
                        return false;
                    }
                }

                if (operationTypeId == 4) {
                    if (IsFalse($("#multipleSelectorConsignorOrganizations").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация, которая передает товар на комиссию.",
                "Выберите все организации или не больше ", "messageExportTo1CSettings"))) {
                        scroll(0, scroll_y);
                        return false;
                    }
                }

                if (operationTypeId == 3 && $("#AddReturnsFromCommissionaires").val() == "1") {
                    if (IsFalse($("#multipleSelectorReturnsFromCommissionaireOrganizations").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация-комиссионер возвраты от которой нужно выгрузить.",
                "Выберите все организации или не больше ", "messageExportTo1CSettings"))) {
                        scroll(0, scroll_y);
                        return false;
                    }
                }

                if (operationTypeId == 3 && $("#AddReturnsAcceptedByCommissionaires").val() == "1") {
                    if (IsFalse($("#multipleSelectorReturnsAcceptedByCommissionaireOrganizations").CheckSelectedEntitiesCount("Не выбрана ни одна собственная организация-комиссионер, возвраты от клиентов которой нужно выгрузить.",
                "Выберите все организации или не больше ", "messageExportTo1CSettings"))) {
                        scroll(0, scroll_y);
                        return false;
                    }
                }
                return true;
            }

            function GetMultipleSelectorList(addOptionId, selectorDivId) {
                if ($("#" + addOptionId).val() == "1") {
                    if ($("#" + selectorDivId + " .OrganizationList").html() == "" || $("#" + selectorDivId + " .OrganizationList").html() == null
                    || $("#" + selectorDivId + " .OrganizationList").html() == undefined) {
                        StartLinkProgress($("#" + selectorDivId + " .OrganizationList"));
                        $.ajax({
                            type: "GET",
                            url: "/ExportTo1C/Get" + selectorDivId + "List/",
                            success: function (result) {
                                $("#" + selectorDivId + " .OrganizationList").html(result);
                                $("#multipleSelectorAccountOrganization_selected").find('div.multiple_selector_item:visible').each(function (index, element) {
                                    HideElement("multipleSelector" + selectorDivId, $(element).attr("value"));
                                });
                                StopLinkProgress();
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageExportTo1CSettings");
                                StopLinkProgress();
                            }
                        });
                    }
                    $("#" + selectorDivId + " .OrganizationList").show();
                }
                else {
                    $("#" + selectorDivId + " .OrganizationList").hide();
                }
            }

            function HideElement(selectorId, elementValue) {
                var item = $("#" + selectorId + " div.multiple_selector_item[value=" + elementValue + "]");

                if ($(item).parent().attr("id") == selectorId + "_selected") $(item).trigger("click");

                $("#" + selectorId + " div.multiple_selector_item[value=" + elementValue + "]").hide();
            }

            function ShowElement(selectorId, elementValue) {
                $("#" + selectorId + " div.multiple_selector_item[value=" + elementValue + "]").show();
            }

            function ValidateDate(startDate, endDate, messageId, performScrolling, scroll_y) {
                var starDateObj = stringToDate(startDate);
                var endDateObj = stringToDate(endDate);

                if (!isValidDate(starDateObj)) {
                    if (performScrolling) {
                        scroll(0, scroll_y);
                    }
                    ShowErrorMessage("Неверная дата начала периода.", messageId);

                    return false;
                }

                if (!isValidDate(endDateObj)) {
                    if (performScrolling) {
                        scroll(0, scroll_y);
                    }
                    ShowErrorMessage("Неверная дата окончания периода.", messageId);

                    return false;
                }

                if (starDateObj > endDateObj) {
                    if (performScrolling) {
                        scroll(0, scroll_y);
                    }
                    ShowErrorMessage("Дата начала периода должна быть меньше даты окончания периода.", messageId);

                    return false;
                }

                var today = new Date();
                if (endDateObj > today) {
                    if (performScrolling) {
                        scroll(0, scroll_y);
                    }
                    ShowErrorMessage("Дата окончания периода должна быть меньше или равна текущей дате.", messageId);

                    return false;
                }

                return true;
            }
        });
    }
};
