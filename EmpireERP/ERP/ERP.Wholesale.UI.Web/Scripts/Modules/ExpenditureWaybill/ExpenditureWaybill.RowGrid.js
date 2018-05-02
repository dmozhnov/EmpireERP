var ExpenditureWaybill_RowGrid = {
    Init: function () {
        $(document).ready(function () {

            // добавление позиции
            $("#btnCreateExpenditureWaybillRow").click(function () {
                var expenditureWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/ExpenditureWaybill/AddRow",
                    data: { expenditureWaybillId: expenditureWaybillId },
                    success: function (result) {
                        $('#expenditureWaybillRowEdit').hide().html(result);
                        ShowModal("expenditureWaybillRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowList");
                    }
                });
            });

            // редактирование / просмотр позиции
            $("#gridExpenditureWaybillRows .edit_link, #gridExpenditureWaybillRows .details_link").click(function () {

                var expenditureWaybillId = $('#Id').val();
                var expenditureWaybillRowId = $(this).parent("td").parent("tr").find(".expenditureWaybillRowId").text();

                $.ajax({
                    type: "GET",
                    url: "/ExpenditureWaybill/EditRow",
                    data: { expenditureWaybillId: expenditureWaybillId, expenditureWaybillRowId: expenditureWaybillRowId },
                    success: function (result) {
                        $('#expenditureWaybillRowEdit').hide().html(result);
                        ShowModal("expenditureWaybillRowEdit");

                        if ($("#expenditureWaybillRowEdit #ManualSourcesInfo").val() == "") {
                            $("#BatchLink").show();
                        }
                        else {
                            $("#ManualSourcesLink").show();
                        }

                        $("#expenditureWaybillRowEdit #SellingCount").focus();
                        ExpenditureWaybill_Shared.CheckSaveButtonAvailability();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowList");
                    }
                });
            });

            //просмотр источников позиции накладной
            $("#gridExpenditureWaybillRows .source_link").click(function () {
                var WaybillRowId = $(this).parent("td").parent("tr").find(".expenditureWaybillRowId").text();
                var articleName = $(this).parent("td").parent("tr").find(".ArticleName").text();
                var batchName = $(this).parent("td").parent("tr").find(".Batch").text();

                $.ajax({
                    type: "POST",
                    url: "/OutgoingWaybillRow/GetSourceWaybill/",
                    data: { type: "ExpenditureWaybill", id: WaybillRowId, articleName: articleName, batchName: batchName },
                    success: function (result) {
                        $("#expenditureWaybillSourceLink").hide().html(result);
                        ShowModal("expenditureWaybillSourceLink");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowList");
                    }
                });
            });

            // удаление позиции накладной
            $("#gridExpenditureWaybillRows .delete_link").click(function () {
                if (confirm('Вы уверены?')) {
                    var expenditureWaybillId = $('#Id').val();
                    var expenditureWaybillRowId = $(this).parent("td").parent("tr").find(".expenditureWaybillRowId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/ExpenditureWaybill/DeleteRow/",
                        data: { expenditureWaybillId: expenditureWaybillId, expenditureWaybillRowId: expenditureWaybillRowId },
                        success: function (result) {
                            // грид для формы добавления товаров списком
                            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                                RefreshGrid("gridExpenditureWaybillRows", function () {
                                    RefreshGrid("gridArticleGroups", function () {
                                        ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                                        ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                                        ShowSuccessMessage("Позиция удалена.", "messageExpenditureWaybillRowList");
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillRowList");
                        }
                    });
                }
            });

        });
    }
};