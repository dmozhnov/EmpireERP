var WriteoffWaybill_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');

            SetEntityDetailsLink('AllowToViewSenderStorageDetails', 'SenderStorageName', 'Storage', 'SenderStorageId');

            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewWrittenoffByDetails', 'WrittenoffByName', 'User', 'WrittenoffById');

            SetEntityDetailsLink(null, 'SenderName', 'AccountOrganization', 'SenderId');
        });
    }
}; 