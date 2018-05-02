<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.BaseDictionary.BaseDictionarySelectViewModel>" %>

<script type="text/javascript">
    Trademark_TrademarkSelector.Init();
</script>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Trademark_Select") %></div>
    <br />

    <%= Html.GridFilterHelper("filterTrademark", Model.Filter, new List<string> { "gridTrademark" }, true) %>
    <div id="messageSelectTrademark"></div>

    <div style="padding:0 0 10px 30px;">
        <% if(Model.AllowToCreate){ %>
            <span id="createTrademark" class="link">Создать торговую марку и выбрать ее</span>
        <%} %>
    </div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("TrademarkSelectGrid", Model.Grid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>

<div id="Edit"></div>