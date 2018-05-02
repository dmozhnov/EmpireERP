var DealPaymentDocument_DealPaymentFromClientDetails = {
    Init: function () {
        $(function () {
            var currentUrl = $("#currentUrl").val();
            
            if (IsTrue($("#AllowToViewDealDetails").val())) {
                var dealId = $("#dealPaymentDocumentDetails #DealId").val();
                $("#DealName").attr("href", "/Deal/Details?id=" + dealId + "&backURL=" + currentUrl);
            }
            else {
                $("#DealName").addClass("disabled");
            }
            if (IsTrue($("#AllowToViewTeamDetails").val())) {
                var teamId = $("#dealPaymentDocumentDetails #TeamId").val();
                $("#TeamName").attr("href", "/Team/Details?id=" + teamId + "&backURL=" + currentUrl);
            }
            else {
                $("#TeamName").addClass("disabled");
            }

            if (IsTrue($("#AllowToViewTakenByDetails").val())) {
                var takenById = $("#dealPaymentDocumentDetails #TakenById").val();
                $("#TakenByName").attr("href", "/User/Details?id=" + takenById + "&backURL=" + currentUrl);
            }
            else {
                $("#TakenByName").addClass("disabled");
            }

            $("#changeTakenBy").click(function () {
                $.ajax({
                    url: "/User/SelectUserByTeamByCombobox",
                    data: { teamId: $("#dealPaymentDocumentDetails #TeamId").val(), mode: "DealPaymentToClientTakenByChange" },
                    success: function (result) {
                        $("#userSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#userSelector"));
                        ShowModal("userSelector");

                        BindUserSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentFromClientDetails");
                    }
                });
            });

            function BindUserSelection() {
                $('#userSelectByComboboxForm').die("submit");
                $('#userSelectByComboboxForm').live("submit", function () {
                    var userId = $("#userSelector #UserId").val();
                    var userName = $("#userSelector #UserId option:selected").text();

                    StartButtonProgress($("#btnSelectUser"));

                    $.ajax({
                        type: "POST",
                        url: "/DealPayment/ChangeTakenByInPaymentFromClient",
                        data: { dealPaymentId: $("#dealPaymentDocumentDetails #PaymentId").val(), newTakenById: userId },
                        success: function (result) {
                            $("#dealPaymentDocumentDetails #TakenByName").text(userName);
                            $("#dealPaymentDocumentDetails #TakenById").val(userId)

                            if (IsTrue($("#AllowToViewTakenByDetails").val())) {
                                var takenById = $("#dealPaymentDocumentDetails #TakenById").val();
                                $("#TakenByName").attr("href", "/User/Details?id=" + takenById + "&backURL=" + currentUrl);
                            }
                            else {
                                $("#TakenByName").addClass("disabled");
                            }

                            HideModal(function () { ShowSuccessMessage("Пользователь успешно сменен.", "messageDealPaymentFromClientDetails"); });

                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserSelectByCombobox");
                        }
                    });



                });
            }

            $("#dealPaymentDocumentDetails #btnDeleteDealPaymentFromClient").bind("click", function () {
                var paymentId = $("#dealPaymentDocumentDetails #PaymentId").val();
                OnDealPaymentFromClientDeleteButtonClick(paymentId);
            });
        });
    }
};