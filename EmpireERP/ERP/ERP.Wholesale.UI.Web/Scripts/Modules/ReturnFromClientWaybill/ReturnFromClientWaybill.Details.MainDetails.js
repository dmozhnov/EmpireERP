var ReturnFromClientWaybill_Details_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');

            SetEntityDetailsLink('AllowToViewRecipientStorageDetails', 'RecipientStorageName', 'Storage', 'RecipientStorageId');

            SetEntityDetailsLink('AllowToViewClientDetails', 'ClientName', 'Client', 'ClientId');

            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');

            SetEntityDetailsLink('AllowToViewDealDetails', 'DealName', 'Deal', 'DealId');

            SetEntityDetailsLink('AllowToViewTeamDetails', 'TeamName', 'Team', 'TeamId');

            SetEntityDetailsLink(null, 'RecipientName', 'AccountOrganization', 'RecipientId');
        });     
    }
};