var MovementWaybill_Details_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            SetEntityDetailsLink(null, 'SenderName', 'AccountOrganization', 'SenderId');

            SetEntityDetailsLink(null, 'RecipientName', 'AccountOrganization', 'RecipientId');

            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewShippedByDetails', 'ShippedByName', 'User', 'ShippedById');

            SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');

            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');

            SetEntityDetailsLink('AllowToViewSenderStorageDetails', 'SenderStorageName', 'Storage', 'SenderStorageId');

            SetEntityDetailsLink('AllowToViewRecipientStorageDetails', 'RecipientStorageName', 'Storage', 'RecipientStorageId');
        });
    }
};