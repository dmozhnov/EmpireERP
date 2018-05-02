var ChangeOwnerWaybill_List_AcceptedGrid = {
    Init: function () {
        $(document).ready(function () {

            var currentUrl = $("#currentUrl").val();
            $("#gridChangeOwnerWaybillAcceptedWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/ChangeOwnerWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".SenderId").text();
                $(this).find("a.SenderName").attr("href", "/AccountOrganization/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".RecipientId").text();
                $(this).find("a.RecipientName").attr("href", "/AccountOrganization/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".StorageId").text();
                $(this).find("a.StorageName").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });
        });
    }
};