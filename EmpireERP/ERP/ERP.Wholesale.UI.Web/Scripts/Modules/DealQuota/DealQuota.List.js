var DealQuota_List = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#btnCreate").live("click", function () {
                StartButtonProgress($(this));

                $.ajax({
                    type: "GET",
                    url: "/DealQuota/Create",
                    success: function (result) {
                        $("#dealQuotaEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealQuotaEdit"));
                        ShowModal("dealQuotaEdit");
                        $("#dealQuotaEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageActiveDealQuotaList");
                    }
                });
            });

            $("#gridActiveDealQuota .edit_link, #gridActiveDealQuota .details_link, #gridInactiveDealQuota .edit_link, #gridInactiveDealQuota .details_link").live("click", function () {
                var quotaId = $(this).parent("td").parent("tr").find(".quotaId").text();
                $.ajax({
                    type: "GET",
                    url: "/DealQuota/Edit",
                    data: { id: quotaId },
                    success: function (result) {
                        $("#dealQuotaEdit").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealQuotaEdit"));
                        ShowModal("dealQuotaEdit");
                        $("#dealQuotaEdit #Name").focus();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealQuotaList");
                    }
                });
            });

            $("#gridActiveDealQuota .delete_link").live("click", function () {
                if (confirm("Вы уверены?")) {
                    var quotaId = $(this).parent("td").parent("tr").find(".quotaId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealQuota/Delete",
                        data: { id: quotaId },
                        success: function (result) {
                            RefreshGrid("gridActiveDealQuota", function () { ShowSuccessMessage("Квота удалена.", "messageActiveDealQuotaList"); });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageActiveDealQuotaList");
                        }
                    });
                }
            });

            $("#gridInactiveDealQuota .delete_link").live("click", function () {
                if (confirm("Вы уверены?")) {
                    var quotaId = $(this).parent("td").parent("tr").find(".quotaId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/DealQuota/Delete",
                        data: { id: quotaId },
                        success: function (result) {
                            RefreshGrid("gridInactiveDealQuota", function () { ShowSuccessMessage("Квота удалена.", "messageInactiveDealQuotaList"); });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageInactiveDealQuotaList");
                        }
                    });
                }
            });
        });
    },

    OnSuccessSaveDealQuota: function (ajaxContext) {
        if (ajaxContext.IsActive) {
            RefreshGrid("gridActiveDealQuota", function () {
                RefreshGrid("gridInactiveDealQuota", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageActiveDealQuotaList");
                    });
                });
            });
        }
        else {
            RefreshGrid("gridActiveDealQuota", function () {
                RefreshGrid("gridInactiveDealQuota", function () {
                    HideModal(function () {
                        ShowSuccessMessage("Сохранено.", "messageInactiveDealQuotaList");
                    });
                });
            });
        }
    }
}; 
