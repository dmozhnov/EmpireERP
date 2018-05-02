var DealPaymentDocument_DealPaymentToClientDetails = {
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

            if (IsTrue($("#AllowToViewReturnedByDetails").val())) {
                var returnedById = $("#dealPaymentDocumentDetails #ReturnedById").val();
                $("#ReturnedByName").attr("href", "/User/Details?id=" + returnedById + "&backURL=" + currentUrl);
            }
            else {
                $("#ReturnedByName").addClass("disabled");
            }

            $("#changeReturnedBy").click(function () {
                $.ajax({
                    url: "/User/SelectUserByTeamByCombobox",
                    data: { teamId: $("#dealPaymentDocumentDetails #TeamId").val(), mode: "DealPaymentToClientReturnedByChange" },
                    success: function (result) {
                        $("#userSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#userSelector"));
                        ShowModal("userSelector");

                        BindUserSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageDealPaymentToClientDetails");
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
                        url: "/DealPayment/ChangeReturnedByInPaymentToClient",
                        data: { dealPaymentId: $("#dealPaymentDocumentDetails #PaymentId").val(), newReturnedById: userId },
                        success: function (result) {
                            $("#dealPaymentDocumentDetails #ReturnedByName").text(userName);
                            $("#dealPaymentDocumentDetails #ReturnedById").val(userId)

                            if (IsTrue($("#AllowToViewReturnedByDetails").val())) {
                                var returnedById = $("#dealPaymentDocumentDetails #ReturnedById").val();
                                $("#ReturnedByName").attr("href", "/User/Details?id=" + returnedById + "&returnedById=" + currentUrl);
                            }
                            else {
                                $("#ReturnedByName").addClass("disabled");
                            }

                            HideModal(function () { ShowSuccessMessage("Пользователь успешно сменен.", "messageDealPaymentToClientDetails"); });

                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageUserSelectByCombobox");
                        }
                    });



                });
            }

            $("#dealPaymentDocumentDetails #btnDeleteDealPaymentToClient").bind("click", function () {
                var paymentId = $("#dealPaymentDocumentDetails #PaymentId").val();
                OnDealPaymentToClientDeleteButtonClick(paymentId);
            });

        });
    }
};