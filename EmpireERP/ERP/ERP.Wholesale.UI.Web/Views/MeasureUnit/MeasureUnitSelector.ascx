<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.MeasureUnit.MeasureUnitSelectViewModel>" %>

<script type="text/javascript">
    MeasureUnit_MeasureUnitSelector.Init();
</script>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_MeasureUnit_Select") %></div>    
    <br />

    <%= Html.GridFilterHelper("filterMeasureUnit", Model.Filter, new List<string> { "gridMeasureUnit" }, true) %>

    <div id="messageSelectMeasureUnit"></div>

    <% if(Model.AllowToCreateMeasureUnit){ %>
        <span id="createMeasureUnit" class="link" style="padding-left: 30px;">Создать единицу измерения и выбрать ее</span>
        <br />
        <br />
    <%} %>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("MeasureUnitSelectGrid", Model.Grid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>

<div id="Edit"></div>