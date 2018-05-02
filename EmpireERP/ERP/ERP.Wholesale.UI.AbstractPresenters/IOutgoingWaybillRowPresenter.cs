using System;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.ViewModels.OutgoingWaybillRow;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IOutgoingWaybillRowPresenter
    {
        GridData ShowSourceWaybillGrid(GridState state, UserInfo currentUser);
        OutgoingWaybillRowViewModel GetSourceWaybill(WaybillType waybillType, Guid waybillRowId, string articleName, string batchName, UserInfo currentUser);

        GridData ShowAvailableToReserveWaybillRowsGrid(GridState state, UserInfo currentUser);
        OutgoingWaybillRowViewModel GetAvailableToReserveWaybillRows(string selectedSources, WaybillType waybillType, int articleId,
            int senderOrganizationId, short senderStorageId, UserInfo currentUser, Guid? waybillRowId = null);
    }
}
