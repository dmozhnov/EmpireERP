var ExpenditureWaybill_Details_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            
            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewShippedByDetails', 'ShippedByName', 'User', 'ShippedById');
            
            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');

            SetEntityDetailsLink('AllowToViewClientDetails', 'ClientName', 'Client', 'ClientId');

            SetEntityDetailsLink('AllowToViewDealDetails', 'DealName', 'Deal', 'DealId');

            SetEntityDetailsLink('AllowToViewSenderStorageDetails', 'SenderStorageName', 'Storage', 'SenderStorageId');

            SetEntityDetailsLink('AllowToViewTeamDetails', 'TeamName', 'Team', 'TeamId');

            SetEntityDetailsLink(null, 'AccountOrganizationName', 'AccountOrganization', 'AccountOrganizationId');

            $("#linkChangeDealQuota").click(function () {
                var dealId = $("#DealId").val();

                $.ajax({
                    type: "GET",
                    url: "/DealQuota/SelectDealQuota",
                    data: { dealId: dealId, mode: "Sale" },
                    success: function (result) {
                        $("#dealQuotaSelector").hide().html(result);
                        $.validator.unobtrusive.parse($("#dealQuotaSelector"));
                        ShowModal("dealQuotaSelector");

                        BindDealQuotaSelection();
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageExpenditureWaybillEdit");
                    }
                });
            });

            // обработка выбора квоты
            function BindDealQuotaSelection() {
                $("#gridDealQuotaSelect .dealQuota_select_link").die("click");
                $("#gridDealQuotaSelect .dealQuota_select_link").live("click", function () {
                    var dealQuotaId = $(this).findCell(".quotaId").text();
                    var expenditureWaybillId = $("#Id").val();
                    $.ajax({
                        type: "POST",
                        url: "/ExpenditureWaybill/SetDealQuota",
                        data: { expenditureWaybillId: expenditureWaybillId, dealQuotaId: dealQuotaId },
                        success: function (result) {
                            var oldPaymentType = $("#PaymentType").text();
                            ExpenditureWaybill_Details.RefreshMainDetails(result.MainDetails);
                            ExpenditureWaybill_Details.RefreshPermissions(result.Permissions);
                            HideModal(function () {
                                var message = "Квота изменена.";
                                if(oldPaymentType != result.MainDetails.PaymentType)
                                {
                                    message += " Форма взаиморасчетов изменена на «" + result.MainDetails.PaymentType + "».";
                                }
                                ShowSuccessMessage(message, "messageExpenditureWaybillEdit");
                            });
                        },
                        error: function (XMLHttpRequest, textStatus, thrownError) {
                            ShowErrorMessage(XMLHttpRequest.responseText, "messageDealQuotaSelectorListGrid");
                        }
                    });
                });
            }
        });
    },
};