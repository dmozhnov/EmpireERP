var WriteoffWaybill_RowGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#btnAddWriteoffWaybillRow").click(function () {
                var writeoffWaybillId = $('#Id').val();

                StartButtonProgress($(this));
                $.ajax({
                    type: "GET",
                    url: "/WriteoffWaybill/AddRow",
                    data: { writeoffWaybillId: writeoffWaybillId },
                    success: function (result) {
                        $('#writeoffWaybillRowEdit').hide().html(result);
                        ShowModal("writeoffWaybillRowEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowList");
                    }
                });
            });

            // редактирование позиции накладной
            $("#gridWriteoffWaybillRows .edit_link, #gridWriteoffWaybillRows .details_link").click(function () {
                var writeoffWaybillId = $('#Id').val();
                var writeoffWaybillRowId = $(this).parent("td").parent("tr").find(".writeoffWaybillRowId").text();

                $.ajax({
                    type: "GET",
                    url: "/WriteoffWaybill/EditRow",
                    data: { writeoffWaybillId: writeoffWaybillId, writeoffWaybillRowId: writeoffWaybillRowId },
                    success: function (result) {
                        $('#writeoffWaybillRowEdit').hide().html(result);
                        ShowModal("writeoffWaybillRowEdit");

                        if ($("#writeoffWaybillRowEdit #ManualSourcesInfo").val() == "") {
                            $("#BatchLink").show();
                        }
                        else {
                            $("#ManualSourcesLink").show();
                        }

                        $("#writeoffWaybillRowEdit #WritingoffCount").focus();
                        WriteoffWaybill_Shared.CheckSaveButtonAvailability();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowList");
                    }
                });
            });

            //просмотр источников позиции накладной
            $("#gridWriteoffWaybillRows .source_link").click(function () {
                var waybillRowId = $(this).parent("td").parent("tr").find(".writeoffWaybillRowId").text();
                var articleName = $(this).parent("td").parent("tr").find(".ArticleName").text();
                var batchName = $(this).parent("td").parent("tr").find(".Batch").text();

                $.ajax({
                    type: "POST",
                    url: "/OutgoingWaybillRow/GetSourceWaybill/",
                    data: { type: "WriteoffWaybill", id: waybillRowId, articleName: articleName, batchName: batchName },
                    success: function (result) {
                        $("#writeoffWaybillSourceLink").hide().html(result);
                        ShowModal("writeoffWaybillSourceLink");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowList");
                    }
                });
            });

            // удаление позиции накладной
            $("#gridWriteoffWaybillRows .delete_link").click(function () {
                if (confirm('Вы уверены?')) {
                    var writeoffWaybillId = $('#Id').val();
                    var writeoffWaybillRowId = $(this).parent("td").parent("tr").find(".writeoffWaybillRowId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/WriteoffWaybill/DeleteRow/",
                        data: { writeoffWaybillId: writeoffWaybillId, writeoffWaybillRowId: writeoffWaybillRowId },
                        success: function (result) {
                            // грид для формы добавления товаров списком
                            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                                RefreshGrid("gridWriteoffWaybillRows", function () {
                                    RefreshGrid("gridArticleGroups", function () {
                                        WriteoffWaybill_Details.RefreshMainDetails(result);
                                        ShowSuccessMessage("Позиция удалена.", "messageWriteoffWaybillRowList");
                                    });
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageWriteoffWaybillRowList");
                        }
                    });
                }
            });
        });
    }
};



