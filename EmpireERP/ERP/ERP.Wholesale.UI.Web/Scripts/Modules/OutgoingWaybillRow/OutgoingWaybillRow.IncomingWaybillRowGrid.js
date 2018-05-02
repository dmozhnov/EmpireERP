var OutgoingWaybillRow_IncomingWaybillRowGrid = {
    Init: function () {
        $(document).ready(function () {
            var hash = {};

            var selectedSourcesInfo = $("#SelectedSources").val().split(";");

            $.each(selectedSourcesInfo, function (i, val) { var fields = val.split("_"); var id = fields[0]; var count = fields[1]; hash[id] = count; });

            var selectedSourceIds = $.each(selectedSourcesInfo, function (i, val) { return val.split("_")[0] });

            var sourcesSelectedEarlier = $('.WaybillRowId').filter(function () {
                return hash[$(this).text()] != undefined;
            });
            $.each(sourcesSelectedEarlier, function (i, val) {
                var row = $(val).parent("td").parent("tr");
                var id = row.find(".WaybillRowId").text();

                row.find(".takingCount").val(hash[id]);
            });

            var selectedBatch = $("#SelectedBatchId").val();
            OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(selectedBatch);

            $("#gridIncomingWaybillRow table.grid_table tr").each(function () {

                var Type = $(this).find(".Type").text();

                if (Type != "") {//исключаем операции над строчками без данных
                    var Id = $(this).find(".Id").text();
                    var SenderStorageName = $(this).find(".SenderStorageName").text();
                    var SenderStorageId = $(this).find(".SenderStorageId").text();
                    var SenderName = $(this).find(".SenderName").text();
                    var SenderId = $(this).find(".SenderId").text();
                    var ContractorName = $(this).find(".ContractorName").text();
                    var ProviderId = $(this).find(".ProviderId").text();
                    var ProducerId = $(this).find(".ProducerId").text();
                    var ContractorOrganizationName = $(this).find(".ContractorOrganizationName").text();
                    var ProviderOrganizationId = $(this).find(".ProviderOrganizationId").text();
                    var ProducerOrganizationId = $(this).find(".ProducerOrganizationId").text();
                    var ClientName = $(this).find(".ClientName").text();
                    var ClientId = $(this).find(".ClientId").text();
                    var ExpenditureWaybillName = $(this).find(".ExpenditureWaybillName").text();
                    var ExpenditureWaybillId = $(this).find(".ExpenditureWaybillId").text();

                    switch (Type) {
                        case "1": //приход
                            if (ProviderId == "" && ProducerId == "") {
                                $(this).find("span.Characteristics").html("---");
                            }
                            else if (ProducerId == "") {
                                $(this).find("span.Characteristics").html('Поставщик: <a class="ProviderName">' + ContractorName + '</a> (<a class="ProviderOrganizationName">' + ContractorOrganizationName + '</a>)');
                            } else {
                                $(this).find("span.Characteristics").html('Производитель: <a class="ProducerName">' + ContractorName + '</a> (<span class="ProducerOrganizationName">' + ContractorOrganizationName + '</span>)');
                            }

                            $(this).find("a.WaybillName").attr("href", "/ReceiptWaybill/Details?id=" + Id + GetBackUrl());
                            $(this).find("a.ProviderName").attr("href", "/Provider/Details?id=" + ProviderId + GetBackUrl());
                            $(this).find("a.ProducerName").attr("href", "/Producer/Details?id=" + ProducerId + GetBackUrl());
                            $(this).find("a.ProviderOrganizationName").attr("href", "/ProviderOrganization/Details?id=" + ProviderOrganizationId + GetBackUrl());
                            $(this).find("a.ProducerOrganizationName").attr("href", "/ProducerOrganization/Details?id=" + ProducerOrganizationId + GetBackUrl());
                            break;

                        case "2": //перемещение
                        case "5": //смена собственника
                            if (Type == "2") {
                                $(this).find("a.WaybillName").attr("href", "/MovementWaybill/Details?id=" + Id + GetBackUrl());
                            }
                            else {
                                $(this).find("a.WaybillName").attr("href", "/ChangeOwnerWaybill/Details?id=" + Id + GetBackUrl());
                            }

                            if (SenderId == "") {
                                $(this).find("span.Characteristics").html('Отправитель: ' + SenderStorageName + ' (' + SenderName + ')');
                            }
                            else {
                                $(this).find("span.Characteristics").html('Отправитель: <a class="SenderStorageName">' + SenderStorageName + '</a> (<a class="SenderName">' + SenderName + '</a>)');
                            }

                            $(this).find("a.SenderStorageName").attr("href", "/Storage/Details?id=" + SenderStorageId + GetBackUrl());
                            $(this).find("a.SenderName").attr("href", "/AccountOrganization/Details?id=" + SenderId + GetBackUrl());

                            break;

                        case "6": //возврат товара от клиента

                            $(this).find("a.WaybillName").attr("href", "/ReturnFromClientWaybill/Details?id=" + Id + GetBackUrl());

                            var text;
                            if (ClientId == "") {
                                text = 'Клиент: ---; ';
                            }
                            else {
                                text = 'Клиент: <a class="ClientName">' + ClientName + '</a>; ';
                            }

                            if (ExpenditureWaybillId == "") {
                                text += 'Реализация: ' + ExpenditureWaybillName;
                            }
                            else {
                                text += 'Реализация: <a class="ExpenditureWaybillName">' + ExpenditureWaybillName + '</a>';
                            }

                            $(this).find("span.Characteristics").html(text);

                            $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + ClientId + GetBackUrl());
                            $(this).find("a.ExpenditureWaybillName").attr("href", "/ExpenditureWaybill/Details?id=" + ExpenditureWaybillId + GetBackUrl());

                            break;
                    }
                }
            });

            $(".takingCount").each(function (i) { $(this).data("prev", $(this).val()) }).bind("change keyup", function () {

                var selectedSourcesField = $("#SelectedSources");
                var row = $(this).parent("td").parent("tr");
                var sourceId = row.find(".WaybillRowId").text();
                var waybillType = $(this).findCell(".Type").text();
                selectedSourcesField.val(selectedSourcesField.val().replace(sourceId + "_" + $(this).data("prev") + "_" + waybillType + ";", ""));

                var takingCount = TryGetDecimal($(this).val());

                var count = row.find(".Count").text();
                if ($(this).val() != "" && (isNaN(takingCount) || takingCount > count || takingCount < 0 || !CheckValueScale(takingCount, $(this).findCell(".MeasureUnitScale").text(), 12)))
                { $(this).addClass("field-validation-error"); DisableButton("btnSaveSourcesSelection"); return false; }
                else { $(this).removeClass("field-validation-error"); }

                var batchId = row.find(".BatchId").text();

                var selectedBatchIdField = $("#SelectedBatchId");
                var selectedBatchNameField = $("#SelectedBatchName");

                if (takingCount > 0) {
                    selectedSourcesField.val(selectedSourcesField.val() + sourceId + "_" + takingCount + "_" + waybillType + ";");
                    EnableButton("btnSaveSourcesSelection");
                    $(this).data("prev", $(this).val())

                    if (selectedBatchIdField.val() == "") {
                        selectedBatchIdField.val(batchId);
                        selectedBatchNameField.val(row.find(".BatchName").text());
                        OutgoingWaybillRow_IncomingWaybillRowGrid.DisableRowsWithAnotherBatches(batchId);
                    }
                }
                else {
                    if (selectedSourcesField.val() == "") {
                        selectedBatchIdField.val("");
                        selectedBatchNameField.val("");
                        $(".takingCount").enableInput();
                        DisableButton("btnSaveSourcesSelection");
                    }
                }

                $(this).data("prev", $(this).val());

            });
        });         // document ready
    },

    DisableRowsWithAnotherBatches: function (batchId) {
        if (batchId != "") {
            var sourcesToDisable = $('.BatchId').filter(function () {
                return $(this).text() != batchId;
            });
            sourcesToDisable.findCell(".takingCount").val("").disableInput();
        }
    }
};
