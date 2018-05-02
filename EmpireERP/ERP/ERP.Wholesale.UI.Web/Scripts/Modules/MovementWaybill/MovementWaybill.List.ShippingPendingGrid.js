var MovementWaybill_List_ShippingPendingGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();
            
			$('#btnCreateMovementWaybill').click(function () {
                window.location = "/MovementWaybill/Create?" + GetBackUrl(true);
            });

            $("#gridShippingPendingMovementWaybill table.grid_table tr").each(function (i, el) {
                var id = $(this).find(".Id").text();
                $(this).find("a.Number").attr("href", "/MovementWaybill/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".SenderStorageId").text();
                $(this).find("a.SenderStorageName").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);

                id = $(this).find(".RecipientStorageId").text();
                $(this).find("a.RecipientStorageName").attr("href", "/Storage/Details?id=" + id + "&backURL=" + currentUrl);
            });
        });
    }
};