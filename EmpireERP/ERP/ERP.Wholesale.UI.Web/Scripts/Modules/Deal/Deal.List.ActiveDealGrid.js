var Deal_List_ActiveDealGrid = {
    Init: function () {
        $(document).ready(function () {
            $("#gridActiveDeal table.grid_table tr").each(function () {
                var id = $(this).find(".Id").text();
                $(this).find("a.Name").attr("href", "/Deal/Details?id=" + id + GetBackUrl());

                var curatorId = $(this).find(".CuratorId").text();
                $(this).find("a.CuratorName").attr("href", "/User/Details?id=" + curatorId + GetBackUrl());

                var clientId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());
            });

            $("#btnCreateDeal").click(function () {
                window.location = "/Deal/Create?backURL=" + $("#currentUrl").val();
            });
        });
    }
};