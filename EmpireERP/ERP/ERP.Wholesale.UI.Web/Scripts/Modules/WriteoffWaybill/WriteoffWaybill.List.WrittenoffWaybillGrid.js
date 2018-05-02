var WriteoffWaybill_List_WrittenoffWaybillGrid = {
    Init: function () {
        $(document).ready(function () {
            
            $("#gridWrittenoff table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/WriteoffWaybill/Details?id=" + id + GetBackUrl());

                var senderOrganizationId = $(this).find(".SenderOrganizationId").text();
                $(this).find("a.SenderOrganizationName").attr("href", "/AccountOrganization/Details?id=" + senderOrganizationId + GetBackUrl());

                var senderStorageId = $(this).find(".SenderStorageId").text();
                $(this).find("a.SenderStorageName").attr("href", "/Storage/Details?id=" + senderStorageId + GetBackUrl());
            });
        });
    }
};