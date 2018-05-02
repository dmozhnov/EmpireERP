var Deal_Details_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            if ($("#ClientOrganizationId").val() != undefined && IsTrue($("#AllowToViewClientOrganizationDetails").val())) {
                $("#ClientOrganizationName").attr("href", "/ClientOrganization/Details?id=" + $("#ClientOrganizationId").val() + GetBackUrl());
            }
            else {
                $("#ClientOrganizationName").addClass("disabled");
            }

            if ($("#AccountOrganizationId").val() != undefined) {
                $("#AccountOrganizationName").attr("href", "/AccountOrganization/Details?id=" + $("#AccountOrganizationId").val() + GetBackUrl());
            }
            else {
                $("#AccountOrganizationName").addClass("disabled");
            }

            if (IsTrue($("#AllowToViewCuratorDetails").val())) {
                var userId = $("#CuratorId").val();
                $("#CuratorName").attr("href", "/User/Details?id=" + userId + GetBackUrl());
            }
            else {
                $("#CuratorName").addClass("disabled");
            }

            if (IsTrue($("#AllowToViewClientDetails").val())) {
                var clientId = $("#ClientId").val();
                $("#ClientName").attr("href", "/Client/Details?id=" + clientId + GetBackUrl());
            }
            else {
                $("#ClientName").addClass("disabled");
            }

            $("#linkChangeStage").click(function () {
                var dealId = $("#Id").val();
                $.ajax({
                    type: "GET",
                    url: "/Deal/ChangeStage",
                    data: { dealId: dealId },
                    success: function (result) {
                        $("#dealChangeStage").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealChangeStage"));
                        ShowModal("dealChangeStage");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                    }
                });
            });

            $("#linkChangeContract").click(function () {
                var dealId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Deal/CheckPossibilityToSetContract",
                    data: { dealId: dealId },
                    success: function (result) {


                        var oldContractId = $("#ClientContractId").val();

                        $.ajax({
                            type: "GET",
                            url: "/ClientContract/IsUsedBySingleDeal",
                            data: { dealId: dealId, clientContractId: oldContractId },
                            success: function (result) {
                                if (IsTrue(result)) {
                                    var oldContractName = $("#ClientContractName").text();
                                    var message = "Так как договор «" + oldContractName +
                                    "» больше не будет использоваться ни одной сделкой, то он будет удален.\n\nВы уверены, что хотите сменить договор?";

                                    if (!confirm(message)) {
                                        StopLinkProgress();
                                        return false;
                                    }
                                }

                                $.ajax({
                                    type: "GET",
                                    url: "/ClientContract/Select",
                                    data: { dealId: dealId },
                                    success: function (result) {
                                        $("#clientContractSelector").hide().html(result);
                                        $.validator.unobtrusive.parse($("#clientContractSelector"));
                                        ShowModal("clientContractSelector");
                                    },
                                    error: function (XMLHttpRequest, textStatus, thrownError) {
                                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                                    }
                                });
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                            }
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                    }
                });
            });

            $("#linkAddContract").click(function () {
                var dealId = $("#Id").val();

                $.ajax({
                    type: "GET",
                    url: "/Deal/CheckPossibilityToSetContract",
                    data: { dealId: dealId },
                    success: function (result) {

                        $.ajax({
                            type: "GET",
                            url: "/ClientContract/Select",
                            data: { dealId: dealId },
                            success: function (result) {
                                $("#clientContractSelector").hide().html(result);
                                $.validator.unobtrusive.parse($("#clientContractSelector"));
                                ShowModal("clientContractSelector");
                            },
                            error: function (XMLHttpRequest, textStatus, thrownError) {
                                ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                            }
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealEdit");
                    }
                });
            });
        });
    }
};

