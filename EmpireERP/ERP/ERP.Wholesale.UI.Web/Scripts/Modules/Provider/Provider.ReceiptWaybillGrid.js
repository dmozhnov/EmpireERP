var Provider_ReceiptWaybillGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            $("#gridReceiptWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".ReceiptWaybillId").text();
                $(this).find("a.Number").attr("href", "/ReceiptWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".ReceiptStorageId").text();
                $(this).find("a.ReceiptStorageName").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });
            
            // Кнопка "Создать новую накладную"
            $("#btnCreateReceiptWaybill").click(function () {
                window.location = "/ReceiptWaybill/Create?providerId=" + $("#MainDetails_Id").val() + GetBackUrl();
            });
        });
    }
};
