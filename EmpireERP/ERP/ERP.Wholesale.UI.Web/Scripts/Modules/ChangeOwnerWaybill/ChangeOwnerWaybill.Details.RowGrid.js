var ChangeOwnerWaybill_Details_RowGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#btnAddChangeOwnerWaybillRow").click(function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/AddRow/",
                    data: { id: $("#Id").val() },
                    success: function (result) {
                        $('#changeOwnerWaybillRowForEdit').hide().html(result);
                        ShowModal("changeOwnerWaybillRowForEdit");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowList");
                    }
                });
            });

            $(".edit_link").click(function () {
                var rowId = $(this).parent("td").parent("tr").find(".Id").html();

                $.ajax({
                    type: "GET",
                    url: "/ChangeOwnerWaybill/EditRow/",
                    data: { id: $("#Id").val(), rowId: rowId },
                    success: function (result) {
                        $('#changeOwnerWaybillRowForEdit').hide().html(result);
                        ShowModal("changeOwnerWaybillRowForEdit");

                        if ($("#changeOwnerWaybillRowForEdit #ManualSourcesInfo").val() == "") {
                            $("#BatchLink").show();
                        }
                        else {
                            $("#ManualSourcesLink").show();
                        }

                        $("#changeOwnerWaybillRowForEdit #MovingCount").focus();
                        ChangeOwnerWaybill_Shared.CheckSaveButtonAvailability();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowList");
                    }
                });
            });

            //просмотр источников позиции накладной
            $(".source_link").click(function () {
                var WaybillRowId = $(this).parent("td").parent("tr").find(".Id").html();
                var articleName = $(this).parent("td").parent("tr").find(".ArticleName").text();
                var batchName = $(this).parent("td").parent("tr").find(".Batch").text();

                $.ajax({
                    type: "POST",
                    url: "/OutgoingWaybillRow/GetSourceWaybill/",
                    data: { type: "ChangeOwnerWaybill", id: WaybillRowId, articleName: articleName, batchName: batchName },
                    success: function (result) {
                        $("#changeOwnerWaybillSourceLink").hide().html(result);
                        ShowModal("changeOwnerWaybillSourceLink");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowList");
                    }
                });
            });

            $(".delete_link").click(function () {
                if (confirm("Вы уверены?")) {
                    var rowId = $(this).parent("td").parent("tr").find(".Id").html();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "GET",
                        url: "/ChangeOwnerWaybill/DeleteRow/",
                        data: { id: $("#Id").val(), rowId: rowId },
                        success: function (result) {
                            // грид для формы добавления товаров списком
                            RefreshGrid("gridArticlesForWaybillRowsAdditionByList", function () {
                                RefreshGrid("gridChangeOwnerWaybillRow", function () {
                                    RefreshGrid("gridArticleGroups", function () {
                                        ChangeOwnerWaybill_Details.RefreshMainDetails(result);
                                        ChangeOwnerWaybill_Details.RefreshPermissions(result);
                                        ShowSuccessMessage("Позиция удалена.", "messageChangeOwnerWaybillRowList");
                                    });              
                                });
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageChangeOwnerWaybillRowList");
                        }
                    });
                }
            });
        });
    }
};