var Report0005_Settings = {
    Init: function () {
        //установим комбобокс типа отчета
        $(function () {
            $("#rbReportSourceType_1").attr("checked", "checked");
        });

        $("#btnBack").live("click", function () {
            window.location = $('#BackURL').val();
        });

        $("#btnRestoreDefaults").live("click", function () {
            window.location = window.location;
        });

        $('#btnRender').live('click', function () {

            if (!$('#form0').validate().form()) {
                return false;
            }

            if (IsFalse($("#multipleSelectorStorages").CheckSelectedEntitiesCount("Не выбрано ни одного места хранения.",
            "Выберите все места хранения или не больше ", "messageReport0005Settings"))) {
                scroll(0, 205);
                return;
            }
           
            if (!Report0005_Settings.ValidateDate($("#StartDate").val(), $("#EndDate").val(), "messageReport0005Settings")) {
                return;
            }

            var Url = "/Report/Report0005?" +
                            $("#multipleSelectorStorages").FormSelectedEntitiesUrlParametersString("AllStorages", "StorageIDs") +
                            "&ArticleId=" + $("#ArticleId").val() +
                            "&ReportSourceType=" + $('input[name="ReportSourceType"]:checked').val() +
                            "&StartDate=" + $("#StartDate").val() +
                            "&EndDate=" + $("#EndDate").val() +
                            "&IncomingWaybillTypeId=" + $("#IncomingWaybillTypeId").val() +
                            "&IncomingWaybillId=" + $('#IncomingWaybillId').val();

            window.open(Url);
        });

        $('input[name="ReportSourceType"]').live('click', function () {
            var value = $(this).val();

            if (value == 1 || value == 2) {
                $('#ReportSourceType_Period').show();
                $('#ReportSourceType_Waybill').hide();
            }

            if (value == 3) {
                $('#ReportSourceType_Period').hide();
                $('#ReportSourceType_Waybill').show();
            }
        });

        // открытие формы выбора товара
        $("span#ArticleName").live("click", function () {
            $.ajax({
                type: "GET",
                url: "/Article/SelectArticle",
                success: function (result) {
                    $('#articleSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#articleSelector"));
                    ShowModal("articleSelector");

                    Report0005_Settings.BindArticleSelection();
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0005Settings");
                }
            });
        });

        $('#IncomingWaybillTypeId').live('change', function () {
            var type = $(this).val();

            if (type != "") {
                $(this).ValidationValid();
            }

            if (type != "" && $("#ArticleId").val() != '0') {
                $("span#IncomingWaybillName").removeClass('link').addClass('select_link');
            }
            else {
                $("span#IncomingWaybillName").removeClass('select_link').addClass('link');
            }
        });

        // открытие формы выбора входящей накладной
        $("span#IncomingWaybillName").live("click", function () {
            var articleId = $('#ArticleId').val();

            if (articleId == "0") {
                $('#ArticleId').ValidationError("Выберите товар");

                return false;
            }

            var waybillType = $('#IncomingWaybillTypeId').val();

            if (waybillType == "") {
                $('#IncomingWaybillTypeId').ValidationError("Выберите тип входящей накладной.");

                return false;
            }

            var methodPath;
            var gridName;
            var selectLinkClass;

            switch (waybillType) {
                case "1":
                    methodPath = "/ReceiptWaybill/SelectWaybill"
                    gridName = "gridSelectReceiptWaybill";
                    selectLinkClass = "receipt_waybill_select_link";
                    break;
                case "2":
                    methodPath = "/MovementWaybill/SelectWaybill"
                    gridName = "gridSelectMovementWaybill";
                    selectLinkClass = "movement_waybill_select_link";
                    break;
                case "3":
                    methodPath = "/ChangeOwnerWaybill/SelectWaybill"
                    gridName = "gridSelectChangeOwnerWaybill";
                    selectLinkClass = "changeowner_waybill_select_link";
                    break;
                case "4":
                    methodPath = "/ReturnFromClientWaybill/SelectWaybill"
                    gridName = "gridSelectReturnFromClientWaybill";
                    selectLinkClass = "returnfromclient_waybill_select_link";
                    break;
            }

            $.ajax({
                type: "GET",
                url: methodPath,
                data: { articleId: articleId },
                success: function (result) {
                    $('#waybillSelector').hide().html(result);
                    $.validator.unobtrusive.parse($("#waybillSelector"));
                    ShowModal("waybillSelector");

                    Report0005_Settings.BindWaybillSelection(gridName, selectLinkClass);
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageReport0005Settings");
                }
            });
        });
    },

    ValidateDate: function (startDate, endDate, idMessage) {
        // проверка дат
        var starDateObj = stringToDate(startDate);
        var endDateObj = stringToDate(endDate);
        
        if (!isValidDate(starDateObj)) {
            scroll(0, 205);
            ShowErrorMessage("Неверная дата начала.", idMessage);

            return false;
        }

        if (!isValidDate(endDateObj)) {
            scroll(0, 205);
            ShowErrorMessage("Неверная дата конца.", idMessage);

            return false;
        }
        
        if (starDateObj > endDateObj) {
            scroll(0, 205);
            ShowErrorMessage("Дата начала периода для отчета должна быть меньше даты конца.", idMessage);

            return false;
        }

        var today = new Date();
        if (endDateObj > today) {
            scroll(0, 205);
            ShowErrorMessage("Дата окончания периода для отчета должна быть меньше или равна текущей дате.", idMessage);

            return false;
        }
        
        return true;
    },

    BindArticleSelection: function () {
        // выбор товара из списка
        var selectLink = $("#gridSelectArticle .article_select_link");
        selectLink.die("click");
        selectLink.live("click", function () {
            $("#ArticleName").text($(this).parent("td").parent("tr").find(".articleFullName").text());
            var articleId = $(this).parent("td").parent("tr").find(".articleId").text();
            $("#ArticleId").val(articleId);
            HideModal();
            $('#ArticleId').ValidationValid();
        });
    },

    BindWaybillSelection: function (gridName, selectLinkClass) {
        // выбор товара из списка
        $("#" + gridName + " ." + selectLinkClass).die("click");
        $("#" + gridName + " ." + selectLinkClass).live("click", function () {
            $("#IncomingWaybillName").text($(this).parent("td").parent("tr").find(".name").text());
            var waybillId = $(this).parent("td").parent("tr").find(".Id").text();
            $("#IncomingWaybillId").val(waybillId);
            HideModal();
            $("#IncomingWaybillId").ValidationValid();
        });
    }
};