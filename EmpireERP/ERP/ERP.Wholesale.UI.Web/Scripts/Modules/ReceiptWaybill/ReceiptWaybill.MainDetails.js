var ReceiptWaybill_MainDetails = {
    Init: function () {
        $(document).ready(function () {

            SetEntityDetailsLink('AllowToViewCreatedByDetails', 'CreatedByName', 'User', 'CreatedById');

            SetEntityDetailsLink('AllowToViewAcceptedByDetails', 'AcceptedByName', 'User', 'AcceptedById');

            SetEntityDetailsLink('AllowToViewReceiptedByDetails', 'ReceiptedByName', 'User', 'ReceiptedById');

            SetEntityDetailsLink('AllowToViewApprovedByDetails', 'ApprovedByName', 'User', 'ApprovedById');

            SetEntityDetailsLink('AllowToViewStorageDetails', 'StorageName', 'Storage', 'StorageId');

            SetEntityDetailsLink(null, 'AccountOrganizationName', 'AccountOrganization', 'AccountOrganizationId');

            SetEntityDetailsLink('AllowToViewProviderDetails', 'ProviderName', 'Provider', 'ProviderId');

            SetEntityDetailsLink('AllowToViewProducerDetails', 'ProducerName', 'Producer', 'ProducerId');

            SetEntityDetailsLink('AllowToViewProductionOrderDetails', 'ProductionOrderName', 'ProductionOrder', 'ProductionOrderId');

            SetEntityDetailsLink('AllowToViewCuratorDetails', 'CuratorName', 'User', 'CuratorId');
        });
    }
}; 