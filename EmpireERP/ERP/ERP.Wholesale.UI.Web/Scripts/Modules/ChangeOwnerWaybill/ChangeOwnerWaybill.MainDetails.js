var ChangeOwnerWaybill_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            SetEntityDetailsLink(null, 'SenderName', 'AccountOrganization', 'SenderId');

            SetEntityDetailsLink(null, 'mainDetailsRecipientLink', 'AccountOrganization', 'RecipientId');

            SetEntityDetailsLink(null, 'RecipientStorageName', 'Storage', 'StorageId');

            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewChangedOwnerByDetails', 'ChangedOwnerByName', 'User', 'ChangedOwnerById');

            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');
        });
    }
};