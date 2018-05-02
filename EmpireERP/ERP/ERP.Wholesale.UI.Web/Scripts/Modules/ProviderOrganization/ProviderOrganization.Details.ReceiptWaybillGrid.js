var ProviderOrganization_Details_ReceiptWaybillGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridReceiptWaybill table.grid_table tr").each(function () {
                var id = $(this).find(".ReceiptWaybillId").text();
                $(this).find("a.Number").attr("href", "/ReceiptWaybill/Details?id=" + id + GetBackUrl());

                id = $(this).find(".ReceiptStorageId").text();
                $(this).find("a.ReceiptStorage").attr("href", "/Storage/Details?id=" + id + GetBackUrl());
            });
        });
    }
};