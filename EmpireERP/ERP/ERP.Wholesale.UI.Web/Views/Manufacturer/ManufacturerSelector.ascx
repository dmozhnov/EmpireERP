<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Manufacturer.ManufacturerSelectorViewModel>" %>

<script type="text/javascript">
    Manufacturer_ManufacturerSelector.Init();
   
</script>

<div style="width: 740px; padding: 0px 10px 0px;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Manufacturer_Select") %></div>
    <br />
    <%: Html.HiddenFor(model => model.ProducerId) %>

    <%= Html.GridFilterHelper("filterManufacturer", Model.Filter, new List<string>() { "gridManufacturerSelectorList" }, true)%>
    
    <div id="messageAddNewManufacturer"></div>
    <% if (Model.AllowToCreate) { %> 
        <span class="selector_link" id="addNewManufacturerLink">Добавить фабрику-изготовителя и выбрать ее</span>
        <br /><br />
    <%} %>
    

    <div id="messageManufacturerSelectorListGrid"></div>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ManufacturerSelectorGrid", Model.ManufacturerGrid); %>
    </div>

    <div class="button_set">            
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>    
</div>
<div id="addNewManufacturer"></div>
