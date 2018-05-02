var WriteoffWaybill_List_WriteoffPendingGrid = {
    Init: function () {
        $(document).ready(function () {
            
            $("#gridWriteoffPending table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/WriteoffWaybill/Details?id=" + id + GetBackUrl());

                var senderOrganizationId = $(this).find(".SenderOrganizationId").text();
                $(this).find("a.SenderOrganizationName").attr("href", "/AccountOrganization/Details?id=" + senderOrganizationId + GetBackUrl());

                var senderStorageId = $(this).find(".SenderStorageId").text();
                $(this).find("a.SenderStorageName").attr("href", "/Storage/Details?id=" + senderStorageId + GetBackUrl());
            });

            $('#btnCreateWriteoffWaybill').click(function () {
                window.location = "/WriteoffWaybill/Create?" + GetBackUrl(true);
            });
        });
    }
};