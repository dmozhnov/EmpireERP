var Team_DealsGrid = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            $("#gridDeals table.grid_table tr").each(function (i, el) {
                var userId = $(this).find(".ClientId").text();
                $(this).find("a.ClientName").attr("href", "/Client/Details?id=" + userId + "&backURL=" + currentUrl);

                var dealId = $(this).find(".DealId").text();
                $(this).find("a.DealName").attr("href", "/Deal/Details?id=" + dealId + "&backURL=" + currentUrl);
            });

            $("#btnAddDeal").click(function () {
                StartButtonProgress($(this));
                $.ajax({
                    url: "/Deal/SelectDealByTeam",
                    data: { teamId: $("#Id").val() },
                    success: function (result) {
                        $("#dealSelector").hide().html(result);
                        ShowModal("dealSelector");

                        BindDealSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealList");
                    }
                });
            });

            $("#gridDeals .remove_deal").click(function () {
                if (confirm("Вы уверены?")) {
                    var dealId = $(this).parent("td").parent("tr").find(".DealId").text();

                    StartGridProgress($(this).closest(".grid"));
                    $.ajax({
                        type: "POST",
                        url: "/Team/RemoveDeal",
                        data: { teamId: $("#Id").val(), dealId: dealId },
                        success: function (result) {
                            RefreshGrid("gridDeals", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Сделка исключена из области видимости команды.", "messageDealList");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealList");
                        }
                    });
                }
            });
        });

        function BindDealSelection() {
            $("#gridSelectDeal .select_deal").die("click");
            $("#gridSelectDeal .select_deal").live('click', function () {
                var dealId = $(this).parent("td").parent("tr").find(".Id").text();

                $.ajax({
                    type: "POST",
                    url: "/Team/AddDeal",
                    data: { teamId: $("#Id").val(), dealId: dealId },
                    success: function (result) {
                        HideModal(function () {
                            RefreshGrid("gridDeals", function () {
                                Team_MainDetails.RefreshMainDetails(result.MainDetails);
                                ShowSuccessMessage("Сделка добавлена в область видимости команды.", "messageDealList");
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        HideModal(function () {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealList");
                        });
                    }
                });
            });
        }
    }
};