<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.OutgoingWaybillRow.OutgoingWaybillRowViewModel>" %>

<script type="text/javascript">
    OutgoingWaybillRow_IncomingWaybillRow.Init();
</script>
<script src="../../Scripts/DatePicker.js" type="text/javascript"></script>

<div style="width: 800px; padding: 0px 10px 0;">
    <div class="modal_title">
        <%: Model.Title %></div>
    <br />
    <span style="margin-left: 30px; font-size: 16px;"><span class="greytext">Описание позиции: </span>
        <%: Model.ArticleName %>
        </span>
    <br />
    <br />

    <%:Html.HiddenFor(x => x.ArticleId) %>
    <%:Html.HiddenFor(x => x.SenderId) %>
    <%:Html.HiddenFor(x => x.SenderStorageId) %>
    <%:Html.HiddenFor(x => x.RecipientStorageId) %>
    <%:Html.HiddenFor(x => x.SelectedSources) %>
    <input type="hidden" id="SelectedBatchId" />
    <input type="hidden" id="SelectedBatchName" />


    <% if (Model.FilterData != null)
       { %> 
    <%= Html.GridFilterHelper("filterIncomingWaybillRow", Model.FilterData, new List<string> { "gridIncomingWaybillRow" }, true)%>
    <% } %>

    <div id="messageOutgoingWaybillRow"></div>

    <div id="grid_OutgoingWaybillRow" style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("~/Views/OutgoingWaybillRow/IncomingWaybillRowGrid.ascx", Model.IncomingWaybillRowGrid); %>
    </div>

    <div id="batchFilterSelector"></div>

    <div class="button_set">
        <%: Html.Button("btnSaveSourcesSelection", "Сохранить", isVisible:Model.AllowToSave, isEnabled:!String.IsNullOrEmpty(Model.SelectedSources)) %>        
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>
