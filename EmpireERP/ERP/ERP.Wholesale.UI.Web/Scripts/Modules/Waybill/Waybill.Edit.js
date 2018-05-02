var Waybill_Edit = {
    Init: function () {
        $(document).ready(function () {
            $(".select_user").live("click", function () {
                var userId = $(this).parent("td").parent("tr").find(".Id").text();
                var userName = $(this).parent("td").parent("tr").find(".Name").text();

                $("#CuratorId").val(userId);
                $("#CuratorName").text(userName);

                HideModal();
            });
        })
    },

    ShowCuratorSelectorForm: function (waybillTypeId, storageIds, dealId, link, errorMessageContainer) {
        if (link != null) {
            StartLinkProgress(link);
        }

        $.ajax({
            type: "GET",
            url: "/User/SelectCurator",
            data: { waybillTypeId: waybillTypeId, storageIds: storageIds, dealId: dealId },
            success: function (result) {
                $("#curatorSelector").hide().html(result);
                $.validator.unobtrusive.parse($("#curatorSelector"));
                ShowModal("curatorSelector");
                if (link != null) {
                    StopLinkProgress(link);
                }
            },
            error: function (XMLHttpRequest, textStatus, thrownError) {
                ShowErrorMessage(XMLHttpRequest.responseText, errorMessageContainer);
            }
        });
    }
};